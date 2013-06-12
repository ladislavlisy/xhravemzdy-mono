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
    public class SalaryMonthlyConcept : PayrollConcept
    {
        static readonly uint TAG_TIMESHEET_PERIOD = PayTagGateway.REF_TIMESHEET_PERIOD.Code;
        static readonly uint TAG_HOURS_WORKING = PayTagGateway.REF_HOURS_WORKING.Code;
        static readonly uint TAG_HOURS_ABSENCE = PayTagGateway.REF_HOURS_ABSENCE.Code;

        public SalaryMonthlyConcept(uint tagCode, IDictionary<string, object> values)
            : base(PayConceptGateway.REFCON_SALARY_MONTHLY, tagCode)
        {
            InitValues(values);
        }

        public decimal AmountMonthly { get; private set; }

        public override void InitValues(IDictionary<string, object> values)
        {
            this.AmountMonthly = GetDecimalOrZero(values["amount_monthly"]);
        }

        public override PayrollConcept CloneWithValue(uint code, IDictionary<string, object> values)
        {
            PayrollConcept newConcept = (SalaryMonthlyConcept)this.Clone();
            newConcept.InitCode(code);
            newConcept.InitValues(values);
            return newConcept;
        }

        public override PayrollTag[] PendingCodes()
        {
            return new PayrollTag[] { 
                new HoursWorkingTag(),
                new HoursAbsenceTag()
            };
        }
        public override PayrollTag[] SummaryCodes()
        {
            return new PayrollTag[] { 
                new IncomeGrossTag(),
                new IncomeNettoTag(),
                new InsuranceSocialBaseTag(),
                new InsuranceHealthBaseTag(),
                new TaxIncomeBaseTag()  
            };
        }

        public override uint CalcCategory()
        {
            return PayrollConcept.CALC_CATEGORY_AMOUNT;
        }

        public override PayrollResult Evaluate(PayrollPeriod period, PayTagGateway tagConfig, IDictionary<TagRefer, PayrollResult> results)
        {
            decimal paymentValue = ComputeResultValue(tagConfig, results);

            var resultValues = new Dictionary<string, object>() { { "payment", paymentValue } };
            return new PaymentResult(TagCode, Code, this, resultValues);
        }

        private decimal ComputeResultValue(PayTagGateway tagConfig, IDictionary<TagRefer, PayrollResult> results)
        {
            TimesheetResult resultTimesheet = (TimesheetResult)GetResultBy(results, TAG_TIMESHEET_PERIOD);
            TermHoursResult resultWorking = (TermHoursResult)GetResultBy(results, TAG_HOURS_WORKING);
            TermHoursResult resultAbsence = (TermHoursResult)GetResultBy(results, TAG_HOURS_ABSENCE);

            decimal scheduleFactor = 1.0m;

            int timesheetHours = resultTimesheet.Hours();
            int workingHours = resultWorking.Hours;
            int absenceHours = resultAbsence.Hours;

            decimal paymentValue = ComputeResultPayment(AmountMonthly, scheduleFactor, timesheetHours, workingHours, absenceHours);

            return paymentValue;
        }

        private decimal ComputeResultPayment(decimal amountMonthly, decimal scheduleFactor, int timesheetHours, int workingHours, int absenceHours)
        {
            decimal amountFactor = FactorizeAmount(amountMonthly, scheduleFactor);

            decimal paymentValue = PaymentFromAmount(amountFactor, timesheetHours, workingHours, absenceHours);
            return paymentValue;
        }

        private decimal FactorizeAmount(decimal amount, decimal factor)
        {
            decimal amountFactor = BigMulti(amount, factor);
            return amountFactor;
        }

        private decimal PaymentFromAmount(decimal amount, int timesheetHours, int workingHours, int absenceHours)
        {
            int salariedHours = Math.Max(0, workingHours - absenceHours);
            decimal paymentValue = BigMultiAndDiv(salariedHours, amount, timesheetHours);
            return paymentValue;
        }

        #region ICloneable Members

        public override object Clone()
        {
            SalaryMonthlyConcept other = (SalaryMonthlyConcept)this.MemberwiseClone();
            other.AmountMonthly = this.AmountMonthly;
            return other;
        }

        #endregion
    }
}
