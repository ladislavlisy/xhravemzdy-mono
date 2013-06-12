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
    public class TaxEmployersHealthConcept : PayrollConcept
    {
        static readonly uint TAG_AMOUNT_BASE = PayTagGateway.REF_INSURANCE_HEALTH_BASE.Code;

        public TaxEmployersHealthConcept(uint tagCode, IDictionary<string, object> values)
            : base(PayConceptGateway.REFCON_TAX_EMPLOYERS_HEALTH, tagCode)
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
            PayrollConcept newConcept = (TaxEmployersHealthConcept)this.Clone();
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
            IncomeBaseResult insIncomeResult = (IncomeBaseResult)GetResultBy(results, TAG_AMOUNT_BASE);

            decimal employerBase = insIncomeResult.EmployerBase();
            decimal employeeBase = insIncomeResult.EmployeeBase();

            decimal paymentValue = ComputeResultValue(period, employerBase, employeeBase);

            var resultValues = new Dictionary<string, object>() { { "payment", paymentValue } };
            return new PaymentResult(TagCode, Code, this, resultValues);
        }

        private decimal ComputeResultValue(PayrollPeriod period, decimal employerBase, decimal employeeBase)
        {
            decimal employerIncome = 0m;
            decimal employeeIncome = 0m;
            if (!Interest())
            {
                employerBase = 0m;
                employeeBase = 0m;
            }
            else
            {
                employerIncome = Math.Max(0m, employerBase);
                employeeIncome = Math.Max(0m, employeeBase);
            }
            decimal paymentValue = InsurancePayment(period, employerIncome, employeeIncome);
            return paymentValue;
        }

        private decimal InsurancePayment(PayrollPeriod period, decimal employerIncome, decimal employeeIncome)
        {
            decimal employerBase = Math.Max(employerIncome, employeeIncome);
            decimal employeeSelf = Math.Max(0m, employeeIncome - employerIncome);
            decimal employeeBase = Math.Max(0m, employerBase - employeeSelf);

            decimal healthFactor = HealthInsuranceFactor(period);

            decimal sumaPaymentValue = FixInsuranceRoundUp(
                BigMulti(employerBase, healthFactor));
            decimal emplPaymentValue = FixInsuranceRoundUp(
                BigMulti(employeeSelf, healthFactor) 
                + BigDiv(BigMulti(employeeBase, healthFactor), 3m));
            decimal contPaymentValue = decimal.Subtract(sumaPaymentValue, emplPaymentValue);
            return contPaymentValue;
        }

        private decimal HealthInsuranceFactor(PayrollPeriod period)
        {
            uint year = period.Year();

            decimal factor = 0m;
            if (year < 2007)
            {
                factor = 0m;
            }
            else
            {
                factor = 13.5m;
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
            TaxEmployersHealthConcept other = (TaxEmployersHealthConcept)this.MemberwiseClone();
            return other;
        }

        #endregion
    }
}
