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
    public class IncomeNettoConcept : PayrollConcept
    {
        public IncomeNettoConcept(uint tagCode, IDictionary<string, object> values)
            : base(PayConceptGateway.REFCON_INCOME_NETTO, tagCode)
        {
            InitValues(values);
        }

        public override void InitValues(IDictionary<string, object> values)
        {
        }

        public override PayrollConcept CloneWithValue(uint code, IDictionary<string, object> values)
        {
            PayrollConcept newConcept = (IncomeNettoConcept)this.Clone();
            newConcept.InitCode(code);
            newConcept.InitValues(values);
            return newConcept;
        }

        public override PayrollTag[] PendingCodes()
        {
            return new PayrollTag[] {
                new TaxAdvanceFinalTag(),
                new TaxWithholdTag(),
                new TaxBonusChildTag(),
                new InsuranceHealthTag(),
                new InsuranceSocialTag()
            };
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
                if (tagConfigItem.IncomeNetto())
                {
                    return resultItem.Payment();
                }
                else
                if (tagConfigItem.DeductionNetto())
                {
                    return decimal.Negate(resultItem.Payment());
                }

            }
            return decimal.Zero;
        }

        #region ICloneable Members

        public override object Clone()
        {
            IncomeNettoConcept other = (IncomeNettoConcept)this.MemberwiseClone();
            return other;
        }

        #endregion
    }
}
