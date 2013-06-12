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
    public class TaxEmployersSocialConcept : PayrollConcept
    {
        static readonly uint TAG_AMOUNT_BASE = PayTagGateway.REF_INSURANCE_SOCIAL_BASE.Code;

        public TaxEmployersSocialConcept(uint tagCode, IDictionary<string, object> values)
            : base(PayConceptGateway.REFCON_TAX_EMPLOYERS_SOCIAL, tagCode)
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
            PayrollConcept newConcept = (TaxEmployersSocialConcept)this.Clone();
            newConcept.InitCode(code);
            newConcept.InitValues(values);
            return newConcept;
        }

        public override PayrollTag[] PendingCodes()
        {
            return new PayrollTag[] {
                new InsuranceSocialBaseTag()    
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
            IncomeBaseResult insIncomeResult = (IncomeBaseResult)GetResultBy(results, TAG_AMOUNT_BASE);

            decimal employerBase = insIncomeResult.EmployerBase();

            decimal paymentValue = ComputeResultValue(period, employerBase);

            var resultValues = new Dictionary<string, object>() { { "payment", paymentValue } };
            return new PaymentResult(TagCode, Code, this, resultValues);
        }

        private decimal ComputeResultValue(PayrollPeriod period, decimal employerBase)
        {
            decimal employerIncome = 0m;
            if (!Interest())
            {
                employerBase = 0m;
            }
            else
            {
                employerIncome = Math.Max(0m, employerBase);
            }
            decimal paymentValue = InsurancePayment(period, employerIncome);
            return paymentValue;
        }

        private decimal InsurancePayment(PayrollPeriod period, decimal employerIncome)
        {
            decimal employerBase = employerIncome;

            decimal socialFactor = SocialInsuranceFactor(period);

            decimal contPaymentValue = FixInsuranceRoundUp(
                BigMulti(employerBase, socialFactor));
            return contPaymentValue;
        }

        private decimal SocialInsuranceFactor(PayrollPeriod period)
        {
            uint year = period.Year();

            decimal factor = 0m;
            if (year < 2007)
            {
                factor = 0m;
            }
            else
            {
                factor = 25m;
            }
            return decimal.Divide(factor, 100m);
        }

        public bool Interest()
        {
            return InterestCode != 0;
        }

        #region ICloneable Members

        public override object Clone()
        {
            TaxEmployersSocialConcept other = (TaxEmployersSocialConcept)this.MemberwiseClone();
            return other;
        }

        #endregion
    }
}
