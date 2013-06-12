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
    public class IncomeGrossConcept : PayrollConcept
    {
        public IncomeGrossConcept(uint tagCode, IDictionary<string, object> values)
            : base(PayConceptGateway.REFCON_INCOME_GROSS, tagCode)
        {
            InitValues(values);
        }

        public override void InitValues(IDictionary<string, object> values)
        {
        }

        public override PayrollConcept CloneWithValue(uint code, IDictionary<string, object> values)
        {
            PayrollConcept newConcept = (IncomeGrossConcept)this.Clone();
            newConcept.InitCode(code);
            newConcept.InitValues(values);
            return newConcept;
        }

        public override uint CalcCategory()
        {
            return PayrollConcept.CALC_CATEGORY_FINAL;
        }

        public override PayrollResult Evaluate(PayrollPeriod period, PayTagGateway tagConfig, IDictionary<TagRefer, PayrollResult> results)
        {
            var resultIncome = ComputeResultValue(tagConfig, results);
            var resultValues = new Dictionary<string, object>() { { "amount", resultIncome } };
            return new AmountResult(TagCode, Code, this, resultValues);
        }

        private decimal ComputeResultValue(PayTagGateway tagConfig, IDictionary<TagRefer, PayrollResult> results)
        {
            decimal resultValue = results.Aggregate(decimal.Zero,
                (agr, termItem) => (decimal.Add(agr, InnerComputeResultValue(tagConfig, this.TagCode, termItem))));
            return resultValue;
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
                if (tagConfigItem.IncomeGross())
                {
                    return resultItem.Payment();
                }
            }
            return decimal.Zero;
        }

        #region ICloneable Members

        public override object Clone()
        {
            IncomeGrossConcept other = (IncomeGrossConcept)this.MemberwiseClone();
            return other;
        }

        #endregion
    }
}
