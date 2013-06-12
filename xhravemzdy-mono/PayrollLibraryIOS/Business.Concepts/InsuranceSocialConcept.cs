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
    public class InsuranceSocialConcept : PayrollConcept
    {
        static readonly uint TAG_AMOUNT_BASE = PayTagGateway.REF_INSURANCE_SOCIAL_BASE.Code;

        static readonly uint TAG_PENSION_CONT = PayTagGateway.REF_SAVINGS_PENSIONS.Code;

        public InsuranceSocialConcept(uint tagCode, IDictionary<string, object> values)
            : base(PayConceptGateway.REFCON_INSURANCE_SOCIAL, tagCode)
        {
            InitValues(values);
        }

        public uint InterestCode { get; private set; }

        public override void InitValues(IDictionary<string, object> values)
        {
            this.InterestCode = GetUIntOrZeroValue(values, "interest_code");
        }

        public override PayrollConcept CloneWithValue(uint code, IDictionary<string, object> values)
        {
            PayrollConcept newConcept = (InsuranceSocialConcept)this.Clone();
            newConcept.InitCode(code);
            newConcept.InitValues(values);
            return newConcept;
        }

        public override PayrollTag[] PendingCodes()
        {
            return new PayrollTag[] {
                new InsuranceSocialBaseTag(),
                new SavingsPensionsTag()
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
            bool pensionSaving = false;

            PaymentResult resultPension = (PaymentResult)GetResultBy(results, TAG_PENSION_CONT);

            decimal paymentIncome = 0m;
            
            if (!Interest())
            {
                paymentIncome = 0m;
            }
            else
            {
                IncomeBaseResult resultIncome = (IncomeBaseResult)GetResultBy(results, TAG_AMOUNT_BASE);

                pensionSaving = resultPension.Interest();
                paymentIncome = Math.Max(0m, resultIncome.EmployeeBase());
            }
            decimal contPaymentValue = InsuranceContribution(period, paymentIncome, pensionSaving);

            var resultValues = new Dictionary<string, object>() { { "payment", contPaymentValue } };
            return new PaymentDeductionResult(TagCode, Code, this, resultValues);
        }

        public decimal InsuranceContribution(PayrollPeriod period, decimal paymentIncome, bool pensionPill)
        {
            decimal socialFactor = SocialInsuranceFactor(period, pensionPill);

            decimal contPaymentValue = FixInsuranceRoundUp(
                BigMulti(paymentIncome, socialFactor));

            return contPaymentValue;
        }

        public bool Interest()
        {
            return InterestCode != 0;
        }

        public decimal SocialInsuranceFactor(PayrollPeriod period, bool pensPill)
        {
            decimal factor = 0m;
            if (period.Year() < 1993)
            {
                factor = 0m;
            }
            else if (period.Year() < 2009)
            {
                factor = 8.0m;
            }
            else if (period.Year() < 2013)
            {
                factor = 6.5m;
            }
            else
            {
                if (pensPill)
                {
                    factor = 3.5m;
                }
                else
                {
                    factor = 6.5m;
                }
            }
            return decimal.Divide(factor, 100);
        }

        #region ICloneable Members

        public override object Clone()
        {
            InsuranceSocialConcept other = (InsuranceSocialConcept)this.MemberwiseClone();
            return other;
        }

        #endregion
    }
}
