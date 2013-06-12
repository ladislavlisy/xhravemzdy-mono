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
    public class InsuranceHealthConcept : PayrollConcept
    {
        static readonly uint TAG_AMOUNT_BASE = PayTagGateway.REF_INSURANCE_HEALTH_BASE.Code;

        public InsuranceHealthConcept(uint tagCode, IDictionary<string, object> values)
            : base(PayConceptGateway.REFCON_INSURANCE_HEALTH, tagCode)
        {
            InitValues(values);
        }

        public uint InterestCode { get; private set; }

        public uint MinimumAsses { get; private set; }

        public override void InitValues(IDictionary<string, object> values)
        {
            this.InterestCode = GetUIntOrZeroValue(values, "interest_code");
            this.MinimumAsses = GetUIntOrZeroValue(values, "minimum_asses");
        }

        public override PayrollConcept CloneWithValue(uint code, IDictionary<string, object> values)
        {
            PayrollConcept newConcept = (InsuranceHealthConcept)this.Clone();
            newConcept.InitCode(code);
            newConcept.InitValues(values);
            return newConcept;
        }

        public override PayrollTag[] PendingCodes()
        {
            return new PayrollTag[] {
                new InsuranceHealthBaseTag()
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
            decimal employerIncome = 0m;
            decimal employeeIncome = 0m;
            if (!Interest())
            {
                employerIncome = 0m;
                employeeIncome = 0m;
            }
            else
            {
                IncomeBaseResult resultIncome = (IncomeBaseResult)GetResultBy(results, TAG_AMOUNT_BASE);
                employerIncome = Math.Max(0m, resultIncome.EmployerBase);
                employeeIncome = Math.Max(0m, resultIncome.EmployeeBase);
            }
            decimal contPaymentValue = InsuranceContribution(period, employerIncome, employeeIncome);

            var resultValues = new Dictionary<string, object>() { { "payment", contPaymentValue } };
            return new PaymentDeductionResult(TagCode, Code, this, resultValues);
        }

        public decimal InsuranceContribution(PayrollPeriod period, decimal employerIncome, decimal employeeIncome)
        {
            decimal employerBase = Math.Max(employerIncome, employeeIncome);
            decimal employeeSelf = Math.Max(0m, decimal.Subtract(employeeIncome, employerIncome));
            decimal employeeBase = Math.Max(0m, decimal.Subtract(employerBase, employeeSelf));

            decimal healthFactor = HealthInsuranceFactor(period);

            decimal sumaPaymentValue = FixInsuranceRoundUp(
                BigMulti(employerBase, healthFactor));

            decimal emplPaymentValue = FixInsuranceRoundUp(
                BigMulti(employeeSelf, healthFactor) + BigDiv(BigMulti(employeeBase, healthFactor), 3));

            decimal contPaymentValue = emplPaymentValue;

            return contPaymentValue;
        }

        public bool Interest()
        {
            return InterestCode != 0;
        }

        public decimal HealthInsuranceFactor(PayrollPeriod period)
        {
            decimal factor = 0m;
            if (period.Year() < 1993)
            {
                factor = 0m;
            }
            else if (period.Year() < 2009)
            {
                factor = 13.5m;
            }
            else
            {
                factor = 13.5m;
            }
            return decimal.Divide(factor, 100);
        }

        #region ICloneable Members

        public override object Clone()
        {
            InsuranceHealthConcept other = (InsuranceHealthConcept)this.MemberwiseClone();
            return other;
        }

        #endregion
    }
}
