using System;
using TechTalk.SpecFlow;
using PayrollLibrary.Business.Core;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Concepts;
using PayrollLibrary.Business.PayTags;
using System.Linq;
using System.Collections.Generic;
using PayrollLibrary.Business.Results;
using NUnit.Framework;

namespace PayrollLibrary.PayrollSpec
{
    [Binding]
    public class PayrollProcessSteps
    {
        PayrollProcess PayProcess { get; set; }

        PayrollPeriod PayPeriod { get; set; }

        PayTagGateway PayTags { get; set; }

        PayConceptGateway PayConcepts { get; set; }

        decimal CaptureMoney(string money)
        {
            return decimal.Parse(money);
        }

        int CaptureHours(string hours)
        {
            return Int32.Parse(hours);
        }

        uint CaptureCount(string count)
        {
            return UInt32.Parse(count);
        }

        uint CaptureBool(string flag)
        {
            if (flag=="YES")
                return 1;
            return 0;
        }

        uint CaptureTaxes(string flag)
        {
            if (flag=="DECLARE")
                return 3;
            else if (flag=="YES")
                return 1;
            return 0;
        }

        uint Capture3bool(string flag1, string flag2, string flag3)
        {
            uint flags = 0;
            if (flag1=="YES")
                flags += 1;
            if (flag2=="YES")
                flags += 10;
            if (flag3=="YES")
                flags += 100;
            return flags;
        }

        PayrollProcess my_payroll_process()
        {
            if (PayProcess == null)
            {
                PayProcess = init_payroll_process(PayPeriod);
            }
            return PayProcess;
        }

        PayrollProcess init_payroll_process(PayrollPeriod period)
        {
            PayTags = new PayTagGateway();
            PayConcepts = new PayConceptGateway();
            var payrollProcess = new PayrollProcess(PayTags, PayConcepts, period);
            return payrollProcess;
        }

        void calculate_payroll_process()
        {
            var empty_value = new Dictionary<string, object>() { };
            var gross_tag = PayProcess.AddTerm(PayTagGateway.REF_INCOME_GROSS, empty_value);
            var netto_tag = PayProcess.AddTerm(PayTagGateway.REF_INCOME_NETTO, empty_value);
            PayProcess.Evaluate(netto_tag);
        }

        private decimal GetResultIncomeBase(CodeNameRefer result_ref)
        {
            IDictionary<TagRefer, PayrollResult> results = PayProcess.GetResults();
                
            var result_select = results.Where( (x) => (x.Key.Code == result_ref.Code)).ToDictionary(key => key.Key, value => value.Value);
            var result_value = result_select.Aggregate(0m, (agr, item) => (agr + item.Value.IncomeBase()));
            return (decimal)result_value;
        }

        private decimal GetResultEmployeeBase(CodeNameRefer result_ref)
        {
            IDictionary<TagRefer, PayrollResult> results = PayProcess.GetResults();

            var result_select = results.Where((x) => (x.Key.Code == result_ref.Code)).ToDictionary(key => key.Key, value => value.Value);
            var result_value = result_select.Aggregate(0m, (agr, item) => (agr + item.Value.EmployeeBase()));
            return (decimal)result_value;
        }

        private decimal GetResultPayment(CodeNameRefer result_ref)
        {
            IDictionary<TagRefer, PayrollResult> results = PayProcess.GetResults();

            var result_select = results.Where((x) => (x.Key.Code == result_ref.Code)).ToDictionary(key => key.Key, value => value.Value);
            var result_value = result_select.Aggregate(0m, (agr, item) => (agr + item.Value.Payment()));
            return (decimal)result_value;
        }

        private decimal GetResultAfterReliefA(CodeNameRefer result_ref)
        {
            IDictionary<TagRefer, PayrollResult> results = PayProcess.GetResults();

            var result_select = results.Where((x) => (x.Key.Code == result_ref.Code)).ToDictionary(key => key.Key, value => value.Value);
            var result_value = result_select.Aggregate(0m, (agr, item) => (agr + item.Value.AfterReliefA()));
            return (decimal)result_value;
        }

        private decimal GetResultAfterReliefC(CodeNameRefer result_ref)
        {
            IDictionary<TagRefer, PayrollResult> results = PayProcess.GetResults();

            var result_select = results.Where((x) => (x.Key.Code == result_ref.Code)).ToDictionary(key => key.Key, value => value.Value);
            var result_value = result_select.Aggregate(0m, (agr, item) => (agr + item.Value.AfterReliefC()));
            return (decimal)result_value;
        }

        private decimal GetResultTaxRelief(CodeNameRefer result_ref)
        {
            IDictionary<TagRefer, PayrollResult> results = PayProcess.GetResults();

            var result_select = results.Where((x) => (x.Key.Code == result_ref.Code)).ToDictionary(key => key.Key, value => value.Value);
            var result_value = result_select.Aggregate(0m, (agr, item) => (agr + item.Value.TaxRelief()));
            return (decimal)result_value;
        }

        private decimal GetResultAmount(CodeNameRefer result_ref)
        {
            IDictionary<TagRefer, PayrollResult> results = PayProcess.GetResults();

            var result_select = results.Where((x) => (x.Key.Code == result_ref.Code)).ToDictionary(key => key.Key, value => value.Value);
            var result_value = result_select.Aggregate(0m, (agr, item) => (agr + item.Value.Amount()));
            return (decimal)result_value;
        }

        [Given(@"Payroll process for payroll period (\d+) (\d+)$")]
        public void GivenPayrollProcessForPayrollPeriod(string monthCode, string yearCode)
        {
            string monthParse = monthCode.TrimStart('0');
            string yearParse = yearCode.TrimStart('0');
            byte monthNumber = Byte.Parse(monthParse);
            uint yearNumber = UInt32.Parse(yearParse);
            PayPeriod = new PayrollPeriod(yearNumber, monthNumber);
            my_payroll_process();
        }
        
        [Given(@"Employee works in Weekly schedule (.*) hours")]
        public void GivenEmployeeWorksInWeeklyScheduleHours(int hoursWeekly)
        {
            var schedule_work_value = new Dictionary<string, object>() { { "hours_weekly", hoursWeekly } };
            var schedule_term_value = new Dictionary<string, object>() { { "date_from", null }, { "date_end", null } };

            PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_WORK, schedule_work_value);
            PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_TERM, schedule_term_value);
        }
        
        [Given(@"Employee has (.*) hours of absence")]
        public void GivenEmployeeHasHoursOfAbsence(int hoursAbsence)
        {
            var absence_hours_value = new Dictionary<string, object>() { { "hours", hoursAbsence } };

            PayProcess.AddTerm(PayTagGateway.REF_HOURS_ABSENCE, absence_hours_value);
        }
        
        [Given(@"Employee Salary is CZK (.*) monthly")]
        public void GivenEmployeeSalaryIsCZKMonthly(int salary)
        {
            var salary_amount_value = new Dictionary<string, object>() { { "amount_monthly", new decimal(salary) } };

            PayProcess.AddTerm(PayTagGateway.REF_SALARY_BASE, salary_amount_value);
        }

        [Given(@"Employee Contract is CZK (.*) monthly")]
        public void GivenEmployeeContractIsCZKMonthly(int contract)
        {
            var salary_amount_value = new Dictionary<string, object>() { { "amount_monthly", new decimal(contract) } };

            PayProcess.AddTerm(PayTagGateway.REF_SALARY_BASE, salary_amount_value);
        }

        [Given(@"(DECLARE|YES|NO) Employee is Regular Tax payer")]
        public void GivenYESEmployeeIsRegularTaxPayer(string flag)
        {
            uint taxes = CaptureTaxes(flag);
            uint interest = (uint)((taxes  > 0) ? 1 : 0);
            uint declare  = (uint)((taxes == 3) ? 1 : 0);
            var interest_value = new Dictionary<string, object>() { { "interest_code", interest }, { "declare_code", declare } };
            PayProcess.AddTerm(PayTagGateway.REF_TAX_INCOME_BASE, interest_value);
        }

        [Given(@"(YES|NO) Employee is Regular Health insurance payer with (YES|NO)")]
        public void GivenYESEmployeeIsRegularHealthInsurancePayer(string flag, string fmin)
        {
            var interest_value = new Dictionary<string, object>() { 
                { "interest_code", CaptureBool(flag) },
                { "minimum_asses", CaptureBool(fmin) }
            };

            PayProcess.AddTerm(PayTagGateway.REF_INSURANCE_HEALTH_BASE, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_INSURANCE_HEALTH, interest_value);
        }

        [Given(@"(YES|NO) Employee is Regular Social insurance payer")]
        public void GivenYESEmployeeIsRegularSocialInsurancePayer(string flag)
        {
            var interest_value = new Dictionary<string, object>() { { "interest_code", CaptureBool(flag) } };

            PayProcess.AddTerm(PayTagGateway.REF_INSURANCE_SOCIAL_BASE, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_INSURANCE_SOCIAL, interest_value);
        }

        [Given(@"(YES|NO) Employee is Regular Pension savings payer")]
        public void GivenNOEmployeeIsRegularPensionSavingsPayer(string flag)
        {
            var interest_value = new Dictionary<string, object>() { { "interest_code", CaptureBool(flag) } };

            PayProcess.AddTerm(PayTagGateway.REF_SAVINGS_PENSIONS, interest_value);
        }

        [Given(@"(YES|NO) Employee claims tax benefit on tax payer")]
        public void GivenYESEmployeeClaimsTaxBenefitOnTaxPayer(string flag)
        {
            var relief_payers_value = new Dictionary<string, object>() { { "relief_code", CaptureBool(flag) } };

            PayProcess.AddTerm(PayTagGateway.REF_TAX_CLAIM_PAYER, relief_payers_value);
        }
        
        [Given(@"Employee claims tax benefit on (.*) child")]
        public void GivenEmployeeClaimsTaxBenefitOnChild(int childCount)
        {
            var relief_payers_value = new Dictionary<string, object>() { { "relief_code", (uint)1 } };

            for (uint count = 0; count < childCount; count++)
            {
                PayProcess.AddTerm(PayTagGateway.REF_TAX_CLAIM_CHILD, relief_payers_value);
            }
        }

        [Given(@"(YES|NO)\:(YES|NO)\:(YES|NO) Employee claims tax benefit on disability")]
        public void GivenNONONOEmployeeClaimsTaxBenefitOnDisability(string flag1, string flag2, string flag3)
        {
            uint yes_no = Capture3bool(flag1, flag2, flag3);

            var relief_payers_value = new Dictionary<string, object>() {  
                { "relief_code_1", (yes_no % 10) },
                { "relief_code_2", ((yes_no/10) % 10) },
                { "relief_code_3", ((yes_no/100) % 10) } 
            };

            PayProcess.AddTerm(PayTagGateway.REF_TAX_CLAIM_DISABILITY, relief_payers_value);
        }

        [Given(@"(YES|NO) Employee claims tax benefit on preparing by studying")]
        public void GivenNOEmployeeClaimsTaxBenefitOnPreparingByStudying(string flag)
        {
            var relief_payers_value = new Dictionary<string, object>() { { "relief_code", CaptureBool(flag) } };

            PayProcess.AddTerm(PayTagGateway.REF_TAX_CLAIM_STUDYING, relief_payers_value);
        }

        [Given(@"(YES|NO) Employee is Employer contribution for Health insurance payer")]
        public void GivenYESEmployeeIsEmployerContributionForHealthInsurancePayer(string flag)
        {
            var interest_value = new Dictionary<string, object>() { { "interest_code", CaptureBool(flag) } };
            PayProcess.AddTerm(PayTagGateway.REF_TAX_EMPLOYERS_HEALTH, interest_value);
        }

        [Given(@"(YES|NO) Employee is Employer contribution for Social insurance payer")]
        public void GivenYESEmployeeIsEmployerContributionForSocialInsurancePayer(string flag)
        {
            var interest_value = new Dictionary<string, object>() { { "interest_code", CaptureBool(flag) } };
            PayProcess.AddTerm(PayTagGateway.REF_TAX_EMPLOYERS_SOCIAL, interest_value);
        }
        
        [When(@"Payroll process calculate results")]
        public void WhenPayrollProcessCalculateResults()
        {
            calculate_payroll_process();
        }
        
        [Then(@"Accounted tax income should be CZK (.*)")]
        public void ThenAccountedTaxIncomeShouldBeCZK(int expected)
        {
            decimal result_value = GetResultIncomeBase(PayTagGateway.REF_TAX_INCOME_BASE);
            Assert.AreEqual(expected, result_value);
        }
        
        [Then(@"Premium insurance should be CZK (.*)")]
        public void ThenPremiumInsuranceShouldBeCZK(int expected)
        {
            decimal result_health = GetResultPayment(PayTagGateway.REF_TAX_EMPLOYERS_HEALTH);
            decimal result_social = GetResultPayment(PayTagGateway.REF_TAX_EMPLOYERS_SOCIAL);
            decimal result_value = result_health + result_social;
            Assert.AreEqual(expected, result_value);
        }
        
        [Then(@"Tax base should be CZK (.*)")]
        public void ThenTaxBaseShouldBeCZK(int expected)
        {
            decimal result_value = GetResultIncomeBase(PayTagGateway.REF_TAX_ADVANCE_BASE);
            Assert.AreEqual(expected, result_value);
        }
        
        [Then(@"Accounted income for Health insurance should be CZK (.*)")]
        public void ThenAccountedIncomeForHealthInsuranceShouldBeCZK(int expected)
        {
            decimal result_value = GetResultEmployeeBase(PayTagGateway.REF_INSURANCE_HEALTH_BASE);
            Assert.AreEqual(expected, result_value);
        }
        
        [Then(@"Accounted income for Social insurance should be CZK (.*)")]
        public void ThenAccountedIncomeForSocialInsuranceShouldBeCZK(int expected)
        {
            decimal result_value = GetResultEmployeeBase(PayTagGateway.REF_INSURANCE_SOCIAL_BASE);
            Assert.AreEqual(expected, result_value);
        }
        
        [Then(@"Contribution to Health insurance should be CZK (.*)")]
        public void ThenContributionToHealthInsuranceShouldBeCZK(int expected)
        {
            decimal result_value = GetResultPayment(PayTagGateway.REF_INSURANCE_HEALTH);
            Assert.AreEqual(expected, result_value);
        }
        
        [Then(@"Contribution to Social insurance should be CZK (.*)")]
        public void ThenContributionToSocialInsuranceShouldBeCZK(int expected)
        {
            decimal result_value = GetResultPayment(PayTagGateway.REF_INSURANCE_SOCIAL);
            Assert.AreEqual(expected, result_value);
        }
        
        [Then(@"Tax advance before tax relief on payer should be CZK (.*)")]
        public void ThenTaxAdvanceBeforeTaxReliefOnPayerShouldBeCZK(int expected)
        {
            decimal result_value = GetResultPayment(PayTagGateway.REF_TAX_ADVANCE);
            Assert.AreEqual(expected, result_value);
        }
        
        [Then(@"Tax relief on payer should be CZK (.*)")]
        public void ThenTaxReliefOnPayerShouldBeCZK(int expected)
        {
            decimal result_value = GetResultTaxRelief(PayTagGateway.REF_TAX_RELIEF_PAYER);
            Assert.AreEqual(expected, result_value);
        }
        
        [Then(@"Tax advance after relief on payer should be CZK (.*)")]
        public void ThenTaxAdvanceAfterReliefOnPayerShouldBeCZK(int expected)
        {
            decimal result_value = GetResultAfterReliefA(PayTagGateway.REF_TAX_ADVANCE_FINAL);
            Assert.AreEqual(expected, result_value);
        }
        
        [Then(@"Tax relief on child should be CZK (.*)")]
        public void ThenTaxReliefOnChildShouldBeCZK(int expected)
        {
            decimal result_value = GetResultTaxRelief(PayTagGateway.REF_TAX_RELIEF_CHILD);
            Assert.AreEqual(expected, result_value);
        }
        
        [Then(@"Tax advance after relief on child should be CZK (.*)")]
        public void ThenTaxAdvanceAfterReliefOnChildShouldBeCZK(int expected)
        {
            decimal result_value = GetResultAfterReliefC(PayTagGateway.REF_TAX_ADVANCE_FINAL);
            Assert.AreEqual(expected, result_value);
        }
        
        [Then(@"Tax advance should be CZK (.*)")]
        public void ThenTaxAdvanceShouldBeCZK(int expected)
        {
            decimal result_value = GetResultPayment(PayTagGateway.REF_TAX_ADVANCE_FINAL);
            Assert.AreEqual(expected, result_value);
        }
        
        [Then(@"Tax bonus should be CZK (.*)")]
        public void ThenTaxBonusShouldBeCZK(int expected)
        {
            decimal result_value = GetResultPayment(PayTagGateway.REF_TAX_BONUS_CHILD);
            Assert.AreEqual(expected, result_value);
        }
        
        [Then(@"Gross income should be CZK (.*)")]
        public void ThenGrossIncomeShouldBeCZK(int expected)
        {
            decimal result_value = GetResultAmount(PayTagGateway.REF_INCOME_GROSS);
            Assert.AreEqual(expected, result_value);
        }
        
        [Then(@"Netto income should be CZK (.*)")]
        public void ThenNettoIncomeShouldBeCZK(int expected)
        {
            decimal result_value = GetResultAmount(PayTagGateway.REF_INCOME_NETTO);
            Assert.AreEqual(expected, result_value);
        }
    }
}
