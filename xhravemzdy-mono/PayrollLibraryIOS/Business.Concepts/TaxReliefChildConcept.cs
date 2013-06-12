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
    public class TaxReliefChildConcept : PayrollConcept
    {
        static readonly uint TAX_ADVANCE = PayTagGateway.REF_TAX_ADVANCE.Code;
        static readonly uint TAX_RELIEF_PAYER = PayTagGateway.REF_TAX_RELIEF_PAYER.Code;
        static readonly uint TAX_CLAIM_BASE = PayTagGateway.REF_TAX_CLAIM_CHILD.Code;

        public TaxReliefChildConcept(uint tagCode, IDictionary<string, object> values)
            : base(PayConceptGateway.REFCON_TAX_RELIEF_CHILD, tagCode)
        {
            InitValues(values);
        }

        public override void InitValues(IDictionary<string, object> values)
        {
        }

        public override PayrollConcept CloneWithValue(uint code, IDictionary<string, object> values)
        {
            PayrollConcept newConcept = (TaxReliefChildConcept)this.Clone();
            newConcept.InitCode(code);
            newConcept.InitValues(values);
            return newConcept;
        }

        public override PayrollTag[] PendingCodes()
        {
            return new PayrollTag[] {
                new TaxAdvanceTag(),
                new TaxReliefPayerTag(),
                new TaxClaimChildTag()
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
            decimal reliefClaimValue = SumReliefBy(results, TAX_CLAIM_BASE);

            decimal advanceBaseValue = advanceBaseResult.Payment();
            decimal reliefPayerValue = reliefPayerResult.TaxRelief();

            decimal taxReliefValue = ComputeResultValue(advanceBaseValue,
                reliefPayerValue, reliefClaimValue);

            var resultValues = new Dictionary<string, object>() { { "tax_relief", taxReliefValue } };
            return new TaxReliefResult(TagCode, Code, this, resultValues);
        }

        private decimal ComputeResultValue(decimal advanceBaseValue, decimal reliefValue, decimal claimsValue)
        {
            decimal taxAfterRelief = decimal.Subtract(advanceBaseValue, reliefValue);
            decimal taxReliefValue = decimal.Subtract(claimsValue,
                Math.Max(0m, decimal.Subtract(claimsValue, taxAfterRelief)));
            return taxReliefValue;
        }

        private decimal SumReliefBy(IDictionary<TagRefer, PayrollResult> results, uint payTag)
        {
            var resultHash = results.Where((keyValue) => (keyValue.Key.Code == payTag))
                .ToDictionary(key => key.Key, value => value.Value);
            decimal resultSuma = resultHash.Aggregate(decimal.Zero,
                (agr, termItem) => (decimal.Add(agr, termItem.Value.TaxRelief())));
            return resultSuma;
        }

        #region ICloneable Members

        public override object Clone()
        {
            TaxReliefChildConcept other = (TaxReliefChildConcept)this.MemberwiseClone();
            return other;
        }

        #endregion
    }
}
