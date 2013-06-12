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
    public class TaxAdvanceFinalConcept : PayrollConcept
    {
        static readonly uint TAX_ADVANCE = PayTagGateway.REF_TAX_ADVANCE.Code;
        static readonly uint TAX_RELIEF_PAYER = PayTagGateway.REF_TAX_RELIEF_PAYER.Code;
        static readonly uint TAX_RELIEF_CHILD = PayTagGateway.REF_TAX_RELIEF_CHILD.Code;

        public TaxAdvanceFinalConcept(uint tagCode, IDictionary<string, object> values)
            : base(PayConceptGateway.REFCON_TAX_ADVANCE_FINAL, tagCode)
        {
            InitValues(values);
        }

        public override void InitValues(IDictionary<string, object> values)
        {
        }

        public override PayrollConcept CloneWithValue(uint code, IDictionary<string, object> values)
        {
            PayrollConcept newConcept = (TaxAdvanceFinalConcept)this.Clone();
            newConcept.InitCode(code);
            newConcept.InitValues(values);
            return newConcept;
        }

        public override PayrollTag[] PendingCodes()
        {
            return new PayrollTag[] {
                new TaxAdvanceTag(),
                new TaxReliefPayerTag(),
                new TaxReliefChildTag()
            };
        }

        public override PayrollTag[] SummaryCodes()
        {
            return new PayrollTag[] {
                new IncomeNettoTag()
            };
        }

        public override uint CalcCategory()
        {
            return PayrollConcept.CALC_CATEGORY_NETTO;
        }

        public override PayrollResult Evaluate(PayrollPeriod period, PayTagGateway tagConfig, IDictionary<TagRefer, PayrollResult> results)
        {
            PaymentResult advanceBaseResult = (PaymentResult)GetResultBy(results, TAX_ADVANCE);
            TaxReliefResult reliefPayerResult = (TaxReliefResult)GetResultBy(results, TAX_RELIEF_PAYER);
            TaxReliefResult reliefChildResult = (TaxReliefResult)GetResultBy(results, TAX_RELIEF_CHILD);

            decimal advanceBaseValue = advanceBaseResult.Payment();
            decimal reliefPayerValue = reliefPayerResult.TaxRelief();
            decimal reliefChildValue = reliefChildResult.TaxRelief();

            decimal taxAdvanceAfterA = ComputeResultValueA(advanceBaseValue, reliefPayerValue, reliefChildValue);
            decimal taxAdvanceAfterC = ComputeResultValueC(advanceBaseValue, reliefPayerValue, reliefChildValue);

            decimal taxAdvanceValue = taxAdvanceAfterC;
            
            var resultValues = new Dictionary<string, object>() { 
                { "after_reliefA", taxAdvanceAfterA }, 
                { "after_reliefC", taxAdvanceAfterC }, 
                { "payment", taxAdvanceValue } 
            };
            return new TaxAdvanceResult(TagCode, Code, this, resultValues);
        }

        private decimal ComputeResultValueA(decimal advanceBaseValue, decimal reliefPayerValue, decimal reliefChildValue)
        {
            return AdvanceAfterRelief(advanceBaseValue, reliefPayerValue, 0);
        }

        private decimal ComputeResultValueC(decimal advanceBaseValue, decimal reliefPayerValue, decimal reliefChildValue)
        {
            return AdvanceAfterRelief(advanceBaseValue, reliefPayerValue, reliefChildValue);
        }

        private decimal AdvanceAfterRelief(decimal advanceBaseValue, decimal reliefPayerValue, decimal reliefChildValue)
        {
            decimal afterRelief = Math.Max(0, advanceBaseValue - reliefPayerValue - reliefChildValue);
            return afterRelief;
        }

        #region ICloneable Members

        public override object Clone()
        {
            TaxAdvanceFinalConcept other = (TaxAdvanceFinalConcept)this.MemberwiseClone();
            return other;
        }

        #endregion
    }
}
