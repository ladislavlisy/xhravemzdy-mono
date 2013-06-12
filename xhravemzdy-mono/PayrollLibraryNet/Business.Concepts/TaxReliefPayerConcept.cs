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
    public class TaxReliefPayerConcept : PayrollConcept
    {
        static readonly uint TAX_ADVANCE = PayTagGateway.REF_TAX_ADVANCE.Code;
        static readonly uint TAX_CLAIM_PAYER = PayTagGateway.REF_TAX_CLAIM_PAYER.Code;
        static readonly uint TAX_CLAIM_DISAB = PayTagGateway.REF_TAX_CLAIM_DISABILITY.Code;
        static readonly uint TAX_CLAIM_STUDY = PayTagGateway.REF_TAX_CLAIM_STUDYING.Code;

        public TaxReliefPayerConcept(uint tagCode, IDictionary<string, object> values)
            : base(PayConceptGateway.REFCON_TAX_RELIEF_PAYER, tagCode)
        {
            InitValues(values);
        }

        public override void InitValues(IDictionary<string, object> values)
        {
        }

        public override PayrollConcept CloneWithValue(uint code, IDictionary<string, object> values)
        {
            PayrollConcept newConcept = (TaxReliefPayerConcept)this.Clone();
            newConcept.InitCode(code);
            newConcept.InitValues(values);
            return newConcept;
        }

        public override PayrollTag[] PendingCodes()
        {
            return new PayrollTag[] {
                new TaxAdvanceTag(),
                new TaxClaimPayerTag(),
                new TaxClaimDisabilityTag(),
                new TaxClaimStudyingTag()
            };
        }

        public override uint CalcCategory()
        {
            return PayrollConcept.CALC_CATEGORY_NETTO;
        }

        public override PayrollResult Evaluate(PayrollPeriod period, PayTagGateway tagConfig, IDictionary<TagRefer, PayrollResult> results)
        {
            PaymentResult advanceBaseResult = (PaymentResult)GetResultBy(results, TAX_ADVANCE);
            TaxReliefResult reliefClaimPayerResult = (TaxReliefResult)GetResultBy(results, TAX_CLAIM_PAYER);
            TaxReliefResult reliefClaimDisabResult = (TaxReliefResult)GetResultBy(results, TAX_CLAIM_DISAB);
            TaxReliefResult reliefClaimStudyResult = (TaxReliefResult)GetResultBy(results, TAX_CLAIM_STUDY);

            decimal advanceBaseValue = advanceBaseResult.Payment();
            decimal reliefPayerValue = reliefClaimPayerResult.TaxRelief();
            decimal reliefDisabValue = reliefClaimDisabResult.TaxRelief();
            decimal reliefStudyValue = reliefClaimStudyResult.TaxRelief();

            decimal taxReliefValue = reliefPayerValue + reliefDisabValue + reliefStudyValue;
            decimal taxClaimsValue = reliefPayerValue + reliefDisabValue + reliefStudyValue;

            decimal resultValue = ComputeResultValue(advanceBaseValue,
                taxReliefValue, taxClaimsValue);

            var resultValues = new Dictionary<string, object>() { { "tax_relief", resultValue } };
            return new TaxReliefResult(TagCode, Code, this, resultValues);
        }

        private decimal ComputeResultValue(decimal advanceBaseValue, decimal reliefValue, decimal claimsValue)
        {
            decimal taxAfterRelief = decimal.Subtract(advanceBaseValue, reliefValue);
            decimal taxReliefValue = decimal.Subtract(claimsValue,
                Math.Max(0m, decimal.Subtract(claimsValue, taxAfterRelief)));
            return taxReliefValue;
        }


        #region ICloneable Members

        public override object Clone()
        {
            TaxReliefPayerConcept other = (TaxReliefPayerConcept)this.MemberwiseClone();
            return other;
        }

        #endregion
    }
}
