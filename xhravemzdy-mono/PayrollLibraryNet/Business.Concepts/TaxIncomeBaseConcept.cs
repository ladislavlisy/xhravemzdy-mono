using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Core;
using PayrollLibrary.Business.PayTags;
using PayrollLibrary.Business.Results;

namespace PayrollLibrary.Business.Concepts
{
    public class TaxIncomeBaseConcept : PayrollConcept
    {
        public TaxIncomeBaseConcept(uint tagCode, IDictionary<string, object> values)
            : base(PayConceptGateway.REFCON_TAX_INCOME_BASE, tagCode)
        {
            InitValues(values);
        }

        public uint InterestCode { get; private set; }

        public uint DeclareCode { get; private set; }

        public override void InitValues(IDictionary<string, object> values)
        {
            this.InterestCode = GetUIntOrZeroValue(values, "interest_code");
            this.DeclareCode  = GetUIntOrZeroValue(values, "declare_code");
        }

        public override PayrollConcept CloneWithValue(uint code, IDictionary<string, object> values)
        {
            PayrollConcept newConcept = (TaxIncomeBaseConcept)this.Clone();
            newConcept.InitCode(code);
            newConcept.InitValues(values);
            return newConcept;
        }

        public override uint CalcCategory()
        {
            return PayrollConcept.CALC_CATEGORY_GROSS;
        }

        public override PayrollResult Evaluate(PayrollPeriod period, PayTagGateway tagConfig, IDictionary<TagRefer, PayrollResult> results)
        {
            var resultIncome = ComputeResultValue(tagConfig, results);

            var resultValues = new Dictionary<string, object>() { 
                { "income_base", resultIncome }, 
                { "interest_code", this.InterestCode }, 
                { "declare_code", this.DeclareCode } 
            };
            return new IncomeBaseResult(TagCode, Code, this, resultValues);
        }

        private decimal ComputeResultValue(PayTagGateway tagConfig, IDictionary<TagRefer, PayrollResult> results)
        {
            if (!Interest())
            {
                return 0m;
            }
            else
            {
                decimal resultValue = results.Aggregate(decimal.Zero,
                    (agr, termItem) => (decimal.Add(agr, InnerComputeResultValue(tagConfig, this.TagCode, termItem))));
                return resultValue;
            }
        }

        private decimal InnerComputeResultValue(PayTagGateway tagConfig, uint tagCode, KeyValuePair<TagRefer, PayrollResult> result)
        {
            TagRefer resultKey = result.Key;
            PayrollResult resultItem = result.Value;

            return SumTermFor(tagConfig, tagCode, resultKey, resultItem);
        }

        private decimal SumTermFor(PayTagGateway tagConfig, uint tagCode, TagRefer resultKey, PayrollResult resultItem)
        {
            var tagConfigItem = tagConfig.FindTag(resultKey.Code);
            if (resultItem.SummaryFor(tagCode))
            {
                if (tagConfigItem.TaxAdvance())
                {
                    return resultItem.Payment();
                }
            }
            return decimal.Zero;
        }

        public bool Interest()
        {
            return InterestCode != 0;
        }

        public bool Declared()
        {
            return DeclareCode != 0;
        }

        #region ICloneable Members

        public override object Clone()
        {
            TaxIncomeBaseConcept other = (TaxIncomeBaseConcept)this.MemberwiseClone();
            return other;
        }

        #endregion
    }
}
