using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using PayrollLibrary.Business.Core;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Concepts;
using PayrollLibrary.Business.PayTags;
using PayrollLibrary.Business.Results;

namespace xhravemzdy_test
{
	[TestFixture()]
    public class ProcessCalcTest
    {
        static readonly uint INTEREST_YES = 1;
        static readonly uint INTEREST_NO  = 1;
        static readonly uint INTEREST_TWO = 2;

        PayrollProcess PayProcess { get; set; }

        PayrollPeriod PayPeriod { get; set; }

        PayTagGateway PayTags { get; set; }

        PayConceptGateway PayConcepts { get; set; }

        public ProcessCalcTest()
        {
            PayPeriod = new PayrollPeriod(2013, 1);

            PayTags = new PayTagGateway();

            PayConcepts = new PayConceptGateway();

            PayProcess = new PayrollProcess(PayTags, PayConcepts, PayPeriod);
        }

        private object GetResultIncomeBase(IDictionary<TagRefer,PayrollResult> results,CodeNameRefer result_ref)
        {
            var result_select = results.Where( (x) => (x.Key.Code == result_ref.Code)).ToDictionary(key => key.Key, value => value.Value);
            var result_value = result_select.Aggregate(0m, (agr, item) => (agr + item.Value.IncomeBase()));
            return result_value;
        }

        private object GetResultEmployeeBase(IDictionary<TagRefer,PayrollResult> results,CodeNameRefer result_ref)
        {
            var result_select = results.Where((x) => (x.Key.Code == result_ref.Code)).ToDictionary(key => key.Key, value => value.Value);
            var result_value = result_select.Aggregate(0m, (agr, item) => (agr + item.Value.EmployeeBase()));
            return result_value;
        }

        private object GetResultPayment(IDictionary<TagRefer,PayrollResult> results,CodeNameRefer result_ref)
        {
            var result_select = results.Where((x) => (x.Key.Code == result_ref.Code)).ToDictionary(key => key.Key, value => value.Value);
            var result_value = result_select.Aggregate(0m, (agr, item) => (agr + item.Value.Payment()));
            return result_value;
        }

        private object GetResultAfterReliefA(IDictionary<TagRefer,PayrollResult> results,CodeNameRefer result_ref)
        {
            var result_select = results.Where((x) => (x.Key.Code == result_ref.Code)).ToDictionary(key => key.Key, value => value.Value);
            var result_value = result_select.Aggregate(0m, (agr, item) => (agr + item.Value.AfterReliefA()));
            return result_value;
        }

        private object GetResultAfterReliefC(IDictionary<TagRefer,PayrollResult> results,CodeNameRefer result_ref)
        {
            var result_select = results.Where((x) => (x.Key.Code == result_ref.Code)).ToDictionary(key => key.Key, value => value.Value);
            var result_value = result_select.Aggregate(0m, (agr, item) => (agr + item.Value.AfterReliefC()));
            return result_value;
        }

        [Test()]
        public void test_payroll_period_returns_month_1_and_year_2013()
        {
           Assert.AreEqual((uint)1, PayPeriod.Month());
           Assert.AreEqual((uint)2013, PayPeriod.Year());
        }

		[Test()]
        public void test_working_schedule_returns_hours_weekly_schedule_40()
        {
            var schedule_value = new Dictionary<string, object>() { { "hours_weekly", 40 } };

            var schedule_ref = PayTagGateway.REF_SCHEDULE_WORK;
            var schedule_tag = PayProcess.AddTerm(schedule_ref, schedule_value);

            var schedule_list = PayProcess.GetTerm(schedule_tag);
            ScheduleWeeklyConcept conceptTerm = (ScheduleWeeklyConcept)schedule_list[schedule_tag];

            Assert.AreEqual((int)40, conceptTerm.HoursWeekly);
        }


		[Test()]
        public void test_working_schedule_returns_hours_in_week_schedule_8_8_8_8_8_0_0()
        {
            var schedule_value = new Dictionary<string, object>() { { "hours_weekly", 40 } };

            var schedule_ref = PayTagGateway.REF_SCHEDULE_WORK;

            var schedule_tag = PayProcess.AddTerm(schedule_ref, schedule_value);
            var schedule_res = PayProcess.Evaluate(schedule_tag);

            ScheduleResult resultItem = (ScheduleResult)schedule_res[schedule_tag];
            CollectionAssert.AreEqual(new int[] {8,8,8,8,8,0,0}, resultItem.WeekSchedule);
        }


		[Test()]
        public void test_working_schedule_returns_hours_in_first_seven_days_in_month_8_8_8_8_0_0_8()
        {
            var schedule_work_value = new Dictionary<string, object>() { { "hours_weekly", 40 } };
            var schedule_term_value = new Dictionary<string, object>() { { "date_from", null }, {"date_end", null } };
            var timesheet_value = new Dictionary<string, object>() { };

            var schedule_work_tag = PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_WORK, schedule_work_value);
            var schedule_term_tag = PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_TERM, schedule_term_value);

            var result_tag = PayProcess.AddTerm(PayTagGateway.REF_TIMESHEET_WORK, timesheet_value);
            var timesheet_result = PayProcess.Evaluate(result_tag);
            TimesheetResult resultItem = (TimesheetResult)timesheet_result[result_tag];

            // first week
            Assert.AreEqual(8, resultItem.MonthSchedule[0]);
            Assert.AreEqual(8, resultItem.MonthSchedule[1]);
            Assert.AreEqual(8, resultItem.MonthSchedule[2]);
            Assert.AreEqual(8, resultItem.MonthSchedule[3]);
            Assert.AreEqual(0, resultItem.MonthSchedule[4]);
            Assert.AreEqual(0, resultItem.MonthSchedule[5]);
            Assert.AreEqual(8, resultItem.MonthSchedule[6]);
        }

		[Test()]
        public void test_schedule_term_returns_date_from_date_end()
        {
            var schedule_work_value = new Dictionary<string, object>() { { "hours_weekly", 40 } };
            var timesheet_value = new Dictionary<string, object>() { };

            var period = PayProcess.Period;
            DateTime date_test_beg = new DateTime(period.YearInt(), period.MonthInt(), 15);
            DateTime date_test_end = new DateTime(period.YearInt(), period.MonthInt(), 24);
            var schedule_term_value = new Dictionary<string, object>() { { "date_from", date_test_beg }, { "date_end", date_test_end } };

            var schedule_work_tag = PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_WORK, schedule_work_value);

            var schedule_term_tag = PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_TERM, schedule_term_value);
            var schedule_term_res = PayProcess.Evaluate(schedule_term_tag);

            TermEffectResult resultItem = (TermEffectResult)schedule_term_res[schedule_term_tag];
            Assert.AreEqual( (uint)15, resultItem.DayOrdFrom);
            Assert.AreEqual( (uint)24, resultItem.DayOrdEnd);
        }


		[Test()]
        public void test_timesheet_period_returns_array_of_working_days()
        {
            var schedule_work_value = new Dictionary<string, object>() { { "hours_weekly", 40 } };
            var schedule_term_value = new Dictionary<string, object>() { { "date_from", null }, {"date_end", null } };
            var timesheet_value = new Dictionary<string, object>() { };

            var schedule_work_tag = PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_WORK, schedule_work_value);
            var schedule_term_tag = PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_TERM, schedule_term_value);

            var result_tag = PayProcess.AddTerm(PayTagGateway.REF_TIMESHEET_PERIOD, timesheet_value);
            var timesheet_result = PayProcess.Evaluate(result_tag);

            TimesheetResult resultItem = (TimesheetResult)timesheet_result[result_tag];
            Assert.AreEqual(8, resultItem.MonthSchedule[0]);
        }


		[Test()]
        public void test_timesheet_work_returns_array_of_working_days()
        {
            var schedule_work_value = new Dictionary<string, object>() { { "hours_weekly", 40 } };
            var schedule_term_value = new Dictionary<string, object>() { { "date_from", null }, {"date_end", null } };
            var timesheet_value = new Dictionary<string, object>() { };

            var schedule_work_tag = PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_WORK, schedule_work_value);
            var schedule_term_tag = PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_TERM, schedule_term_value);

            var result_tag = PayProcess.AddTerm(PayTagGateway.REF_TIMESHEET_WORK, timesheet_value);
            var timesheet_result = PayProcess.Evaluate(result_tag);
            TimesheetResult resultItem = (TimesheetResult)timesheet_result[result_tag];
            // first week
            Assert.AreEqual(8, resultItem.MonthSchedule[0]);
            Assert.AreEqual(8, resultItem.MonthSchedule[1]);
            Assert.AreEqual(8, resultItem.MonthSchedule[2]);
            Assert.AreEqual(8, resultItem.MonthSchedule[3]);
            Assert.AreEqual(0, resultItem.MonthSchedule[4]);
            Assert.AreEqual(0, resultItem.MonthSchedule[5]);
            Assert.AreEqual(8, resultItem.MonthSchedule[6]);
            Assert.AreEqual(8, resultItem.MonthSchedule[7]);
            // third week
            Assert.AreEqual(8, resultItem.MonthSchedule[14]);
            Assert.AreEqual(8, resultItem.MonthSchedule[15]);
            Assert.AreEqual(8, resultItem.MonthSchedule[16]);
            Assert.AreEqual(8, resultItem.MonthSchedule[17]);
            Assert.AreEqual(0, resultItem.MonthSchedule[18]);
            Assert.AreEqual(0, resultItem.MonthSchedule[19]);
            Assert.AreEqual(8, resultItem.MonthSchedule[20]);
            Assert.AreEqual(8, resultItem.MonthSchedule[21]);
        }


		[Test()]
        public void test_timesheet_work_returns_array_of_working_days_form_10_to_25_of_month()
        {
           Assert.AreEqual(PayPeriod.Month(), (uint)1);

            var schedule_work_value = new Dictionary<string, object>() { { "hours_weekly", 40 } };
            var timesheet_value = new Dictionary<string, object>() { };

            var period = PayProcess.Period;
            DateTime date_test_beg = new DateTime(period.YearInt(), period.MonthInt(), 15);
            DateTime date_test_end = new DateTime(period.YearInt(), period.MonthInt(), 24);
            var schedule_term_value = new Dictionary<string, object>() { { "date_from", date_test_beg }, { "date_end", date_test_end } };

            var schedule_work_tag = PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_WORK, schedule_work_value);
            var schedule_term_tag = PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_TERM, schedule_term_value);

            var result_tag = PayProcess.AddTerm(PayTagGateway.REF_TIMESHEET_WORK, timesheet_value);
            var timesheet_result = PayProcess.Evaluate(result_tag);
            TimesheetResult resultItem = (TimesheetResult)timesheet_result[result_tag];

            Assert.AreEqual(0, resultItem.MonthSchedule[14-1]);
            Assert.AreEqual(0, resultItem.MonthSchedule[25-1]);
            Assert.AreEqual(8, resultItem.MonthSchedule[15-1]);
            Assert.AreEqual(8, resultItem.MonthSchedule[24-1]);
        }

		[Test()]
        public void test_hours_working_should_return_for_period_1_2013_sum_of_working_hours_184()
        {
            var schedule_work_value = new Dictionary<string, object>() { { "hours_weekly", 40 } };
            var timesheet_value = new Dictionary<string, object>() { };

            var period = PayProcess.Period;
            var schedule_term_value = new Dictionary<string, object>() { { "date_from", null }, {"date_end", null } };

            var schedule_work_tag = PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_WORK, schedule_work_value);
            var schedule_term_tag = PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_TERM, schedule_term_value);

            var result_tag = PayProcess.AddTerm(PayTagGateway.REF_HOURS_WORKING, timesheet_value);
            var result_value = PayProcess.Evaluate(result_tag);
            TermHoursResult resultItem = (TermHoursResult)result_value[result_tag];

            Assert.AreEqual(184, resultItem.Hours);
        }


		[Test()]
        public void test_hours_absence_should_return_sum_of_absence_hours_0()
        {
            var schedule_work_value = new Dictionary<string, object>() { { "hours_weekly", 40 } };
            var timesheet_value = new Dictionary<string, object>() { };

            var period = PayProcess.Period;
            var schedule_term_value = new Dictionary<string, object>() { { "date_from", null }, {"date_end", null } };

            var schedule_work_tag = PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_WORK, schedule_work_value);
            var schedule_term_tag = PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_TERM, schedule_term_value);

            var result_tag = PayProcess.AddTerm(PayTagGateway.REF_HOURS_ABSENCE, timesheet_value);
            var result_value = PayProcess.Evaluate(result_tag);
            TermHoursResult resultItem = (TermHoursResult)result_value[result_tag];

            Assert.AreEqual(0, resultItem.Hours);
        }


		[Test()]
        public void test_salary_base_should_return_for_salary_amount_15_000_salary_value_15_000()
        {
            var schedule_work_value = new Dictionary<string, object>() { { "hours_weekly", 40 } };
            var schedule_term_value = new Dictionary<string, object>() { { "date_from", null }, {"date_end", null } };
            var salary_amount_value = new Dictionary<string, object>() { { "amount_monthly", 15000m } };

            var schedule_work_tag = PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_WORK, schedule_work_value);
            var schedule_term_tag = PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_TERM, schedule_term_value);

            var salary_amount_tag = PayProcess.AddTerm(PayTagGateway.REF_SALARY_BASE, salary_amount_value);
            var salary_result = PayProcess.Evaluate(salary_amount_tag);

            var result_value = GetResultPayment(PayProcess.GetResults(), PayTagGateway.REF_SALARY_BASE);
            Assert.AreEqual(15000m, result_value);
        }

		[Test()]
        public void test_salary_base_should_return_for_salary_amount_20_000_salary_value_15_000()
        {
            var schedule_work_value = new Dictionary<string, object>() { { "hours_weekly", 40 } };
            var schedule_term_value = new Dictionary<string, object>() { { "date_from", null }, {"date_end", null } };
            var absence_hours_value = new Dictionary<string, object>() { { "hours", 46 } };
            var salary_amount_value = new Dictionary<string, object>() { { "amount_monthly", 20000m } };

            var schedule_work_tag = PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_WORK, schedule_work_value);
            var schedule_term_tag = PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_TERM, schedule_term_value);
            var absence_hours_tag = PayProcess.AddTerm(PayTagGateway.REF_HOURS_ABSENCE, absence_hours_value);

            var salary_amount_tag = PayProcess.AddTerm(PayTagGateway.REF_SALARY_BASE, salary_amount_value);
            var salary_result = PayProcess.Evaluate(salary_amount_tag);

            var result_value = GetResultPayment(PayProcess.GetResults(), PayTagGateway.REF_SALARY_BASE);
            Assert.AreEqual(15000m, result_value);
        }


		[Test()]
        public void test_insurance_health_base_returns_insurance_base_amount()
        {
            var empty_value = new Dictionary<string, object>() { };

            var interest_value = new Dictionary<string, object>() { { "interest_code", INTEREST_YES } };

            var schedule_work_value = new Dictionary<string, object>() { { "hours_weekly", 40 } };
            var schedule_term_value = new Dictionary<string, object>() { { "date_from", null }, {"date_end", null } };
            var salary_amount_value = new Dictionary<string, object>() { { "amount_monthly", 15000m } };

            var schedule_work_tag = PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_WORK, schedule_work_value);
            var schedule_term_tag = PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_TERM, schedule_term_value);
            var salary_amount_tag = PayProcess.AddTerm(PayTagGateway.REF_SALARY_BASE, salary_amount_value);

            var result_tag = PayProcess.AddTerm(PayTagGateway.REF_INSURANCE_HEALTH_BASE, interest_value);
            var result = PayProcess.Evaluate(result_tag);

            var result_value = GetResultIncomeBase(PayProcess.GetResults(), PayTagGateway.REF_INSURANCE_HEALTH_BASE);
            Assert.AreEqual(15000m, result_value);
        }


		[Test()]
        public void test_insurance_social_base_returns_insurance_base_amount()
        {
            var empty_value = new Dictionary<string, object>() { };

            var schedule_work_value = new Dictionary<string, object>() { { "hours_weekly", 40 } };
            var schedule_term_value = new Dictionary<string, object>() { { "date_from", null }, {"date_end", null } };
            var salary_amount_value = new Dictionary<string, object>() { { "amount_monthly", 15000m } };
            var interest_value = new Dictionary<string, object>() { { "interest_code", INTEREST_YES } };

            var schedule_work_tag = PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_WORK, schedule_work_value);
            var schedule_term_tag = PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_TERM, schedule_term_value);
            var salary_amount_tag = PayProcess.AddTerm(PayTagGateway.REF_SALARY_BASE, salary_amount_value);

            var result_tag = PayProcess.AddTerm(PayTagGateway.REF_INSURANCE_SOCIAL_BASE, interest_value);
            var result = PayProcess.Evaluate(result_tag);

            var result_value = GetResultIncomeBase(PayProcess.GetResults(), PayTagGateway.REF_INSURANCE_SOCIAL_BASE);
            Assert.AreEqual(15000m, result_value);
        }


		[Test()]
        public void test_tax_income_base_returns_tax_base_amount()
        {
            var empty_value = new Dictionary<string, object>() { };

            var schedule_work_value = new Dictionary<string, object>() { { "hours_weekly", 40 } };
            var schedule_term_value = new Dictionary<string, object>() { { "date_from", null }, {"date_end", null } };
            var salary_amount_value = new Dictionary<string, object>() { { "amount_monthly", 15000m } };
            var interest_value = new Dictionary<string, object>() { { "interest_code", INTEREST_YES } };

            var schedule_work_tag = PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_WORK, schedule_work_value);
            var schedule_term_tag = PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_TERM, schedule_term_value);
            var salary_amount_tag = PayProcess.AddTerm(PayTagGateway.REF_SALARY_BASE, salary_amount_value);

            var result_tag = PayProcess.AddTerm(PayTagGateway.REF_TAX_INCOME_BASE, interest_value);
            var result = PayProcess.Evaluate(result_tag);

            var result_value = GetResultIncomeBase(PayProcess.GetResults(), PayTagGateway.REF_TAX_INCOME_BASE);
            Assert.AreEqual(15000m, result_value);
        }


		[Test()]
        public void test_insurance_health_returns_insurance_amount_15000()
        {
            var empty_value = new Dictionary<string, object>() { };

            var interest_value = new Dictionary<string, object>() { { "interest_code", INTEREST_YES } };
            PayProcess.AddTerm(PayTagGateway.REF_INSURANCE_HEALTH_BASE, interest_value);

            var schedule_work_value = new Dictionary<string, object>() { { "hours_weekly", 40 } };
            var schedule_term_value = new Dictionary<string, object>() { { "date_from", null }, {"date_end", null } };
            var salary_amount_value = new Dictionary<string, object>() { { "amount_monthly", 15000m } };

            PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_WORK, schedule_work_value);
            PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_TERM, schedule_term_value);
            PayProcess.AddTerm(PayTagGateway.REF_SALARY_BASE, salary_amount_value);

            var result_tag = PayProcess.AddTerm(PayTagGateway.REF_INSURANCE_HEALTH, interest_value);
            var result = PayProcess.Evaluate(result_tag);

            var result_value = GetResultPayment(PayProcess.GetResults(), PayTagGateway.REF_INSURANCE_HEALTH);
            Assert.AreEqual(675m, result_value);
        }


		[Test()]
        public void test_insurance_social_returns_insurance_amount()
        {
            var empty_value = new Dictionary<string, object>() { };

            var interest_value = new Dictionary<string, object>() { { "interest_code", INTEREST_YES } };
            PayProcess.AddTerm(PayTagGateway.REF_INSURANCE_SOCIAL_BASE, interest_value);

            var schedule_work_value = new Dictionary<string, object>() { { "hours_weekly", 40 } };
            var schedule_term_value = new Dictionary<string, object>() { { "date_from", null }, {"date_end", null } };
            var salary_amount_value = new Dictionary<string, object>() { { "amount_monthly", 15000m } };

            PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_WORK, schedule_work_value);
            PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_TERM, schedule_term_value);
            PayProcess.AddTerm(PayTagGateway.REF_SALARY_BASE, salary_amount_value);

            var result_tag = PayProcess.AddTerm(PayTagGateway.REF_INSURANCE_SOCIAL, interest_value);
            var result = PayProcess.Evaluate(result_tag);

            var result_value = GetResultPayment(PayProcess.GetResults(), PayTagGateway.REF_INSURANCE_SOCIAL);
            Assert.AreEqual(975m, result_value);
        }


// Insurance employee contribution: 1 650 Kč
// Partial tax base:               20 100 Kč
// Tax before relief:               3 015 Kč
// Tax advance:                       945 Kč
// Tax relief:                      2 070 Kč
// Tax bonus:                           0 Kč
// Net income:                     12 405 Kč

		[Test()]
        public void test_tax_advanced_returns_employer_health_tax_base()
        {
            var empty_value = new Dictionary<string, object>() { };

            var interest_value = new Dictionary<string, object>() { { "interest_code", INTEREST_YES } };
            PayProcess.AddTerm(PayTagGateway.REF_INSURANCE_HEALTH_BASE, interest_value);

            var schedule_work_value = new Dictionary<string, object>() { { "hours_weekly", 40 } };
            var schedule_term_value = new Dictionary<string, object>() { { "date_from", null }, {"date_end", null } };
            var salary_amount_value = new Dictionary<string, object>() { { "amount_monthly", 15000m } };
            var relief_payers_value = new Dictionary<string, object>() { { "relief_code", 1} };

            PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_WORK, schedule_work_value); 
            PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_TERM, schedule_term_value);
            PayProcess.AddTerm(PayTagGateway.REF_SALARY_BASE, salary_amount_value);

            var result_tag = PayProcess.AddTerm(PayTagGateway.REF_TAX_EMPLOYERS_HEALTH, interest_value);
            var result = PayProcess.Evaluate(result_tag);

            var result_value = GetResultPayment(PayProcess.GetResults(), PayTagGateway.REF_TAX_EMPLOYERS_HEALTH);
            Assert.AreEqual(1350m, result_value);
        }


		[Test()]
        public void test_tax_advanced_returns_employer_social_tax_base()
        {
            var empty_value = new Dictionary<string, object>() { };

            var interest_value = new Dictionary<string, object>() { { "interest_code", INTEREST_YES } };
            PayProcess.AddTerm(PayTagGateway.REF_INSURANCE_SOCIAL_BASE, interest_value);

            var schedule_work_value = new Dictionary<string, object>() { { "hours_weekly", 40 } };
            var schedule_term_value = new Dictionary<string, object>() { { "date_from", null }, {"date_end", null } };
            var salary_amount_value = new Dictionary<string, object>() { { "amount_monthly", 15000m } };
            var relief_payers_value = new Dictionary<string, object>() { { "relief_code", 1} };

            PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_WORK, schedule_work_value);
            PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_TERM, schedule_term_value);
            PayProcess.AddTerm(PayTagGateway.REF_SALARY_BASE, salary_amount_value);

            var result_tag = PayProcess.AddTerm(PayTagGateway.REF_TAX_EMPLOYERS_SOCIAL, interest_value);
            var result = PayProcess.Evaluate(result_tag);

            var result_value = GetResultPayment(PayProcess.GetResults(), PayTagGateway.REF_TAX_EMPLOYERS_SOCIAL);
            Assert.AreEqual(3750m, result_value);
        }


		[Test()]
        public void test_tax_advanced_returns_tax_claim_payer_relief()
        {
            var empty_value = new Dictionary<string, object>() { };

            var schedule_work_value = new Dictionary<string, object>() { { "hours_weekly", 40 } };
            var schedule_term_value = new Dictionary<string, object>() { { "date_from", null }, {"date_end", null } };
            var salary_amount_value = new Dictionary<string, object>() { { "amount_monthly", 15000m } };
            var relief_payers_value = new Dictionary<string, object>() { { "relief_code", INTEREST_YES} };

            var schedule_work_tag = PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_WORK, schedule_work_value);
            var schedule_term_tag = PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_TERM, schedule_term_value);
            var salary_amount_tag = PayProcess.AddTerm(PayTagGateway.REF_SALARY_BASE, salary_amount_value);

            var result_tag = PayProcess.AddTerm(PayTagGateway.REF_TAX_CLAIM_PAYER, relief_payers_value);
            var result = PayProcess.Evaluate(result_tag);

            TaxClaimResult resultItem = (TaxClaimResult)result[result_tag];
            Assert.AreEqual(2070m, resultItem.TaxRelief());
        }


		[Test()]
        public void test_tax_advanced_returns_tax_claim_disability_1_relief()
        {
            var empty_value = new Dictionary<string, object>() { };

            var schedule_work_value = new Dictionary<string, object>() { { "hours_weekly", 40 } };
            var schedule_term_value = new Dictionary<string, object>() { { "date_from", null }, {"date_end", null } };
            var salary_amount_value = new Dictionary<string, object>() { { "amount_monthly", 15000m } };
            var relief_payers_value = new Dictionary<string, object>() { { "relief_code_1", INTEREST_YES } };

            var schedule_work_tag = PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_WORK, schedule_work_value);
            var schedule_term_tag = PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_TERM, schedule_term_value);
            var salary_amount_tag = PayProcess.AddTerm(PayTagGateway.REF_SALARY_BASE, salary_amount_value);

            var result_tag = PayProcess.AddTerm(PayTagGateway.REF_TAX_CLAIM_DISABILITY, relief_payers_value);
            var result = PayProcess.Evaluate(result_tag);

            TaxClaimResult resultItem = (TaxClaimResult)result[result_tag];
            Assert.AreEqual(210m, resultItem.TaxRelief());
        }


		[Test()]
        public void test_tax_advanced_returns_tax_claim_disability_2_relief()
        {
            var empty_value = new Dictionary<string, object>() { };

            var schedule_work_value = new Dictionary<string, object>() { { "hours_weekly", 40 } };
            var schedule_term_value = new Dictionary<string, object>() { { "date_from", null }, {"date_end", null } };
            var salary_amount_value = new Dictionary<string, object>() { { "amount_monthly", 15000m } };
            var relief_payers_value = new Dictionary<string, object>() { { "relief_code_2", INTEREST_YES } };

            var schedule_work_tag = PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_WORK, schedule_work_value);
            var schedule_term_tag = PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_TERM, schedule_term_value);
            var salary_amount_tag = PayProcess.AddTerm(PayTagGateway.REF_SALARY_BASE, salary_amount_value);

            var result_tag = PayProcess.AddTerm(PayTagGateway.REF_TAX_CLAIM_DISABILITY, relief_payers_value);
            var result = PayProcess.Evaluate(result_tag);

            TaxClaimResult resultItem = (TaxClaimResult)result[result_tag];
            Assert.AreEqual(420m, resultItem.TaxRelief());
        }


		[Test()]
        public void test_tax_advanced_returns_tax_claim_disability_3_relief()
        {
            var empty_value = new Dictionary<string, object>() { };

            var schedule_work_value = new Dictionary<string, object>() { { "hours_weekly", 40 } };
            var schedule_term_value = new Dictionary<string, object>() { { "date_from", null }, {"date_end", null } };
            var salary_amount_value = new Dictionary<string, object>() { { "amount_monthly", 15000m } };
            var relief_payers_value = new Dictionary<string, object>() { { "relief_code_3", INTEREST_YES } };

            var schedule_work_tag = PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_WORK, schedule_work_value);
            var schedule_term_tag = PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_TERM, schedule_term_value);
            var salary_amount_tag = PayProcess.AddTerm(PayTagGateway.REF_SALARY_BASE, salary_amount_value);

            var result_tag = PayProcess.AddTerm(PayTagGateway.REF_TAX_CLAIM_DISABILITY, relief_payers_value);
            var result = PayProcess.Evaluate(result_tag);
 
            TaxClaimResult resultItem = (TaxClaimResult)result[result_tag];
            Assert.AreEqual(1345m, resultItem.TaxRelief());
        }


		[Test()]
        public void test_tax_advanced_returns_tax_claim_studying_relief()
        {
            var empty_value = new Dictionary<string, object>() { };

            var schedule_work_value = new Dictionary<string, object>() { { "hours_weekly", 40 } };
            var schedule_term_value = new Dictionary<string, object>() { { "date_from", null }, {"date_end", null } };
            var salary_amount_value = new Dictionary<string, object>() { { "amount_monthly", 15000m } };
            var relief_payers_value = new Dictionary<string, object>() { { "relief_code", INTEREST_YES } };

            var schedule_work_tag = PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_WORK, schedule_work_value);
            var schedule_term_tag = PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_TERM, schedule_term_value);
            var salary_amount_tag = PayProcess.AddTerm(PayTagGateway.REF_SALARY_BASE, salary_amount_value);

            var result_tag = PayProcess.AddTerm(PayTagGateway.REF_TAX_CLAIM_STUDYING, relief_payers_value);
            var result = PayProcess.Evaluate(result_tag);

            TaxClaimResult resultItem = (TaxClaimResult)result[result_tag];
            Assert.AreEqual(335m, resultItem.TaxRelief());
        }


		[Test()]
        public void test_tax_advanced_returns_tax_claim_child_relief()
        {
            var empty_value = new Dictionary<string, object>() { };

            var interest_value = new Dictionary<string, object>() { { "interest_code", INTEREST_YES }, { "declare_code", INTEREST_YES } };
            PayProcess.AddTerm(PayTagGateway.REF_TAX_INCOME_BASE, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_INSURANCE_HEALTH_BASE, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_INSURANCE_HEALTH, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_INSURANCE_SOCIAL_BASE, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_INSURANCE_SOCIAL, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_TAX_EMPLOYERS_HEALTH, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_TAX_EMPLOYERS_SOCIAL, interest_value);

            var schedule_work_value = new Dictionary<string, object>() { { "hours_weekly", 40 } };
            var schedule_term_value = new Dictionary<string, object>() { { "date_from", null }, {"date_end", null } };
            var salary_amount_value = new Dictionary<string, object>() { { "amount_monthly", 15000m } };
            var relief_payers_value = new Dictionary<string, object>() { { "relief_code", INTEREST_YES } };

            PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_WORK, schedule_work_value);
            PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_TERM, schedule_term_value);
            PayProcess.AddTerm(PayTagGateway.REF_SALARY_BASE, salary_amount_value);

            var result_tag = PayProcess.AddTerm(PayTagGateway.REF_TAX_CLAIM_CHILD, relief_payers_value);
            var result = PayProcess.Evaluate(result_tag);

            TaxClaimResult resultItem = (TaxClaimResult)result[result_tag];
            Assert.AreEqual(1117m, resultItem.TaxRelief());
        }


		[Test()]
        public void test_tax_advanced_returns_rounded_base_for_tax_advance()
        {
            var empty_value = new Dictionary<string, object>() { };

            var interest_value = new Dictionary<string, object>() { { "interest_code", INTEREST_YES } };
            PayProcess.AddTerm(PayTagGateway.REF_TAX_INCOME_BASE, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_INSURANCE_HEALTH_BASE, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_INSURANCE_SOCIAL_BASE, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_TAX_EMPLOYERS_HEALTH, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_TAX_EMPLOYERS_SOCIAL, interest_value);

            var schedule_work_value = new Dictionary<string, object>() { { "hours_weekly", 40 } };
            var schedule_term_value = new Dictionary<string, object>() { { "date_from", null }, {"date_end", null } };
            var salary_amount_value = new Dictionary<string, object>() { { "amount_monthly", 15000m } };
            var relief_payers_value = new Dictionary<string, object>() { { "relief_code", INTEREST_YES } };

            PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_WORK, schedule_work_value);
            PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_TERM, schedule_term_value);
            PayProcess.AddTerm(PayTagGateway.REF_SALARY_BASE, salary_amount_value);

            var result_tag = PayProcess.AddTerm(PayTagGateway.REF_TAX_ADVANCE_BASE, empty_value);
            var result = PayProcess.Evaluate(result_tag);

            var result_value = GetResultIncomeBase(PayProcess.GetResults(), PayTagGateway.REF_TAX_ADVANCE_BASE);
            Assert.AreEqual(20100m, result_value);
        }


		[Test()]
        public void test_tax_advanced_returns_tax_amount_before_relief()
        {
            var empty_value = new Dictionary<string, object>() { };

            var interest_value = new Dictionary<string, object>() { { "interest_code", INTEREST_YES }, { "declare_code", INTEREST_YES } };
            PayProcess.AddTerm(PayTagGateway.REF_TAX_INCOME_BASE, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_INSURANCE_HEALTH_BASE, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_INSURANCE_HEALTH, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_INSURANCE_SOCIAL_BASE, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_INSURANCE_SOCIAL, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_TAX_EMPLOYERS_HEALTH, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_TAX_EMPLOYERS_SOCIAL, interest_value);

            var schedule_work_value = new Dictionary<string, object>() { { "hours_weekly", 40 } };
            var schedule_term_value = new Dictionary<string, object>() { { "date_from", null }, {"date_end", null } };
            var salary_amount_value = new Dictionary<string, object>() { { "amount_monthly", 15000m } };
            var relief_payers_value = new Dictionary<string, object>() { { "relief_code", INTEREST_YES } };

            PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_WORK, schedule_work_value);
            PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_TERM, schedule_term_value);
            PayProcess.AddTerm(PayTagGateway.REF_SALARY_BASE, salary_amount_value);
            PayProcess.AddTerm(PayTagGateway.REF_TAX_CLAIM_PAYER, relief_payers_value);

            var result_tag = PayProcess.AddTerm(PayTagGateway.REF_TAX_ADVANCE, empty_value);
            var result = PayProcess.Evaluate(result_tag);

            var result_value = GetResultPayment(PayProcess.GetResults(), PayTagGateway.REF_TAX_ADVANCE);
            Assert.AreEqual(3015m, result_value);
        }


		[Test()]
        public void test_tax_advanced_returns_tax_amount_after_relief_with_payer_relief()
        {
            var empty_value = new Dictionary<string, object>() { };

            var interest_value = new Dictionary<string, object>() { { "interest_code", INTEREST_YES }, { "declare_code", INTEREST_YES } };
            PayProcess.AddTerm(PayTagGateway.REF_TAX_INCOME_BASE, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_INSURANCE_HEALTH_BASE, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_INSURANCE_HEALTH, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_INSURANCE_SOCIAL_BASE, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_INSURANCE_SOCIAL, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_TAX_EMPLOYERS_HEALTH, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_TAX_EMPLOYERS_SOCIAL, interest_value);

            var schedule_work_value = new Dictionary<string, object>() { { "hours_weekly", 40 } };
            var schedule_term_value = new Dictionary<string, object>() { { "date_from", null }, {"date_end", null } };
            var salary_amount_value = new Dictionary<string, object>() { { "amount_monthly", 15000m } };
            var relief_payers_value = new Dictionary<string, object>() { { "relief_code", INTEREST_YES } };

            PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_WORK, schedule_work_value);
            PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_TERM, schedule_term_value);
            PayProcess.AddTerm(PayTagGateway.REF_SALARY_BASE, salary_amount_value);
            PayProcess.AddTerm(PayTagGateway.REF_TAX_CLAIM_PAYER, relief_payers_value);

            var result_tag = PayProcess.AddTerm(PayTagGateway.REF_TAX_ADVANCE_FINAL, empty_value);
            var result = PayProcess.Evaluate(result_tag);

            var result_value = GetResultPayment(PayProcess.GetResults(), PayTagGateway.REF_TAX_ADVANCE_FINAL);
            Assert.AreEqual(945m, result_value);
        }


		[Test()]
        public void test_tax_advanced_returns_tax_amount_after_relief_with_child_relief_945()
        {
            var empty_value = new Dictionary<string, object>() { };

            var interest_value = new Dictionary<string, object>() { { "interest_code", INTEREST_YES }, { "declare_code", INTEREST_YES } };
            PayProcess.AddTerm(PayTagGateway.REF_TAX_INCOME_BASE, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_INSURANCE_HEALTH_BASE, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_INSURANCE_HEALTH, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_INSURANCE_SOCIAL_BASE, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_INSURANCE_SOCIAL, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_TAX_EMPLOYERS_HEALTH, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_TAX_EMPLOYERS_SOCIAL, interest_value);

            var schedule_work_value = new Dictionary<string, object>() { { "hours_weekly", 40 } };
            var schedule_term_value = new Dictionary<string, object>() { { "date_from", null }, {"date_end", null } };
            var salary_amount_value = new Dictionary<string, object>() { { "amount_monthly", 15000m } };
            var relief_payers_value = new Dictionary<string, object>() { { "relief_code", INTEREST_YES } };

            PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_WORK, schedule_work_value);
            PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_TERM, schedule_term_value);
            PayProcess.AddTerm(PayTagGateway.REF_SALARY_BASE, salary_amount_value);
            PayProcess.AddTerm(PayTagGateway.REF_TAX_CLAIM_PAYER, relief_payers_value);
            PayProcess.AddTerm(PayTagGateway.REF_TAX_CLAIM_CHILD, relief_payers_value);

            var result_tag = PayProcess.AddTerm(PayTagGateway.REF_TAX_ADVANCE_FINAL, empty_value);
            var result = PayProcess.Evaluate(result_tag);

            var result_value = GetResultAfterReliefA(PayProcess.GetResults(), PayTagGateway.REF_TAX_ADVANCE_FINAL);
            Assert.AreEqual(945m, result_value);
        }


		[Test()]
        public void test_tax_advanced_returns_tax_amount_after_relief_with_child_relief_0()
        {
            var empty_value = new Dictionary<string, object>() { };

            var interest_value = new Dictionary<string, object>() { { "interest_code", INTEREST_YES }, { "declare_code", INTEREST_YES } };
            PayProcess.AddTerm(PayTagGateway.REF_TAX_INCOME_BASE, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_INSURANCE_HEALTH_BASE, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_INSURANCE_HEALTH, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_INSURANCE_SOCIAL_BASE, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_INSURANCE_SOCIAL, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_TAX_EMPLOYERS_HEALTH, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_TAX_EMPLOYERS_SOCIAL, interest_value);

            var schedule_work_value = new Dictionary<string, object>() { { "hours_weekly", 40 } };
            var schedule_term_value = new Dictionary<string, object>() { { "date_from", null }, {"date_end", null } };
            var salary_amount_value = new Dictionary<string, object>() { { "amount_monthly", 15000m } };
            var relief_payers_value = new Dictionary<string, object>() { { "relief_code", INTEREST_YES } };

            var schedule_work_tag = PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_WORK, schedule_work_value);
            var schedule_term_tag = PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_TERM, schedule_term_value);
            var salary_amount_tag = PayProcess.AddTerm(PayTagGateway.REF_SALARY_BASE, salary_amount_value);
            var relief_payers_tag = PayProcess.AddTerm(PayTagGateway.REF_TAX_CLAIM_PAYER, relief_payers_value);
            var relief_child_tag = PayProcess.AddTerm(PayTagGateway.REF_TAX_CLAIM_CHILD, relief_payers_value);

            var result_tag = PayProcess.AddTerm(PayTagGateway.REF_TAX_ADVANCE_FINAL, empty_value);
            var result = PayProcess.Evaluate(result_tag);

            var result_value = GetResultPayment(PayProcess.GetResults(), PayTagGateway.REF_TAX_ADVANCE_FINAL);
            Assert.AreEqual(0m, result_value);
        }


		[Test()]
        public void test_tax_advanced_returns_tax_bonus_after_relief_with_child_relief()
        {
            var empty_value = new Dictionary<string, object>() { };

            var interest_value = new Dictionary<string, object>() { { "interest_code", INTEREST_YES }, { "declare_code", INTEREST_YES } };
            PayProcess.AddTerm(PayTagGateway.REF_TAX_INCOME_BASE, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_INSURANCE_HEALTH_BASE, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_INSURANCE_HEALTH, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_INSURANCE_SOCIAL_BASE, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_INSURANCE_SOCIAL, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_TAX_EMPLOYERS_HEALTH, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_TAX_EMPLOYERS_SOCIAL, interest_value);

            var schedule_work_value = new Dictionary<string, object>() { { "hours_weekly", 40 } };
            var schedule_term_value = new Dictionary<string, object>() { { "date_from", null }, {"date_end", null } };
            var salary_amount_value = new Dictionary<string, object>() { { "amount_monthly", 15000m } };
            var relief_payers_value = new Dictionary<string, object>() { { "relief_code", INTEREST_YES } };

            var schedule_work_tag = PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_WORK, schedule_work_value);
            var schedule_term_tag = PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_TERM, schedule_term_value);
            var salary_amount_tag = PayProcess.AddTerm(PayTagGateway.REF_SALARY_BASE, salary_amount_value);
            var relief_payers_tag = PayProcess.AddTerm(PayTagGateway.REF_TAX_CLAIM_PAYER, relief_payers_value);
            var relief_child_tag = PayProcess.AddTerm(PayTagGateway.REF_TAX_CLAIM_CHILD, relief_payers_value);

            var result_tag = PayProcess.AddTerm(PayTagGateway.REF_TAX_BONUS_CHILD, empty_value);
            var result = PayProcess.Evaluate(result_tag);

            var result_value = GetResultPayment(PayProcess.GetResults(), PayTagGateway.REF_TAX_BONUS_CHILD);
            Assert.AreEqual(172m, result_value);
        }


		[Test()]
        public void test_tax_advanced_returns_tax_bonus_after_relief_with_child_relief_ztp()
        {
            var empty_value = new Dictionary<string, object>() { };

            var interest_value = new Dictionary<string, object>() { { "interest_code", INTEREST_YES }, { "declare_code", INTEREST_YES } };
            PayProcess.AddTerm(PayTagGateway.REF_TAX_INCOME_BASE, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_INSURANCE_HEALTH_BASE, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_INSURANCE_HEALTH, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_INSURANCE_SOCIAL_BASE, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_INSURANCE_SOCIAL, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_TAX_EMPLOYERS_HEALTH, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_TAX_EMPLOYERS_SOCIAL, interest_value);

            var schedule_work_value = new Dictionary<string, object>() { { "hours_weekly", 40 } };
            var schedule_term_value = new Dictionary<string, object>() { { "date_from", null }, {"date_end", null } };
            var salary_amount_value = new Dictionary<string, object>() { { "amount_monthly", 15000m } };
            var relief_payers_value = new Dictionary<string, object>() { { "relief_code", INTEREST_YES } };
            var relief_child_value = new Dictionary<string, object>() { { "relief_code", INTEREST_TWO } };

            PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_WORK, schedule_work_value);
            PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_TERM, schedule_term_value);
            PayProcess.AddTerm(PayTagGateway.REF_SALARY_BASE, salary_amount_value);
            PayProcess.AddTerm(PayTagGateway.REF_TAX_CLAIM_PAYER, relief_payers_value);
            PayProcess.AddTerm(PayTagGateway.REF_TAX_CLAIM_CHILD, relief_child_value);

            var result_tag = PayProcess.AddTerm(PayTagGateway.REF_TAX_BONUS_CHILD, empty_value);
            var result = PayProcess.Evaluate(result_tag);

            var result_value = GetResultPayment(PayProcess.GetResults(), PayTagGateway.REF_TAX_BONUS_CHILD);
            Assert.AreEqual(1289m, result_value);
        }


		[Test()]
        public void test_income_gross_returns_income_amount()
        {
            var empty_value = new Dictionary<string, object>() { };

            var interest_value = new Dictionary<string, object>() { { "interest_code", INTEREST_YES } };
            PayProcess.AddTerm(PayTagGateway.REF_TAX_INCOME_BASE, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_INSURANCE_HEALTH_BASE, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_INSURANCE_HEALTH, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_INSURANCE_SOCIAL_BASE, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_INSURANCE_SOCIAL, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_TAX_EMPLOYERS_HEALTH, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_TAX_EMPLOYERS_SOCIAL, interest_value);

            var schedule_work_value = new Dictionary<string, object>() { { "hours_weekly", 40 } };
            var schedule_term_value = new Dictionary<string, object>() { { "date_from", null }, {"date_end", null } };
            var salary_amount_value = new Dictionary<string, object>() { { "amount_monthly", 15000m } };
            var relief_payers_value = new Dictionary<string, object>() { { "relief_code", INTEREST_YES } };

            PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_WORK, schedule_work_value);
            PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_TERM, schedule_term_value);
            PayProcess.AddTerm(PayTagGateway.REF_SALARY_BASE, salary_amount_value);
            PayProcess.AddTerm(PayTagGateway.REF_TAX_CLAIM_PAYER, relief_payers_value);

            var result_tag = PayProcess.AddTerm(PayTagGateway.REF_INCOME_GROSS, empty_value);
            var result = PayProcess.Evaluate(result_tag);
            AmountResult resultItem = (AmountResult)result[result_tag];

            Assert.AreEqual(15000m, resultItem.Amount);
        }


		[Test()]
        public void test_income_netto_returns_income_amount()
        {
            var empty_value = new Dictionary<string, object>() { };

            var interest_value = new Dictionary<string, object>() { { "interest_code", INTEREST_YES } };
            PayProcess.AddTerm(PayTagGateway.REF_TAX_INCOME_BASE, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_INSURANCE_HEALTH_BASE, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_INSURANCE_HEALTH, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_INSURANCE_SOCIAL_BASE, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_INSURANCE_SOCIAL, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_TAX_EMPLOYERS_HEALTH, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_TAX_EMPLOYERS_SOCIAL, interest_value);

            var schedule_work_value = new Dictionary<string, object>() { { "hours_weekly", 40 } };
            var schedule_term_value = new Dictionary<string, object>() { { "date_from", null }, {"date_end", null } };
            var salary_amount_value = new Dictionary<string, object>() { { "amount_monthly", 15000m } };
            var relief_payers_value = new Dictionary<string, object>() { { "relief_code", INTEREST_YES } };

            PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_WORK, schedule_work_value);
            PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_TERM, schedule_term_value);
            PayProcess.AddTerm(PayTagGateway.REF_SALARY_BASE, salary_amount_value);
            PayProcess.AddTerm(PayTagGateway.REF_TAX_CLAIM_PAYER, relief_payers_value);

            var result_tag = PayProcess.AddTerm(PayTagGateway.REF_INCOME_NETTO, empty_value);
            var result = PayProcess.Evaluate(result_tag);
            AmountResult resultItem = (AmountResult)result[result_tag];

            Assert.AreEqual(12405m, resultItem.Amount);
        }


		[Test()]
        public void test_income_netto_returns_netto_income_amount_with_bonus()
        {
            var empty_value = new Dictionary<string, object>() { };

            var interest_value = new Dictionary<string, object>() { { "interest_code", INTEREST_YES }, { "declare_code", INTEREST_YES } };
            PayProcess.AddTerm(PayTagGateway.REF_TAX_INCOME_BASE, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_INSURANCE_HEALTH_BASE, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_INSURANCE_HEALTH, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_INSURANCE_SOCIAL_BASE, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_INSURANCE_SOCIAL, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_TAX_EMPLOYERS_HEALTH, interest_value);
            PayProcess.AddTerm(PayTagGateway.REF_TAX_EMPLOYERS_SOCIAL, interest_value);

            var schedule_work_value = new Dictionary<string, object>() { { "hours_weekly", 40 } };
            var schedule_term_value = new Dictionary<string, object>() { { "date_from", null }, {"date_end", null } };
            var salary_amount_value = new Dictionary<string, object>() { { "amount_monthly", 15000m } };
            var relief_payers_value = new Dictionary<string, object>() { { "relief_code", INTEREST_YES } };
            var relief_child_value = new Dictionary<string, object>() { { "relief_code", INTEREST_TWO } };

            PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_WORK, schedule_work_value);
            PayProcess.AddTerm(PayTagGateway.REF_SCHEDULE_TERM, schedule_term_value);
            PayProcess.AddTerm(PayTagGateway.REF_SALARY_BASE, salary_amount_value);
            PayProcess.AddTerm(PayTagGateway.REF_TAX_CLAIM_PAYER, relief_payers_value);
            PayProcess.AddTerm(PayTagGateway.REF_TAX_CLAIM_CHILD, relief_child_value);

            var result_tag = PayProcess.AddTerm(PayTagGateway.REF_INCOME_NETTO, empty_value);
            var result = PayProcess.Evaluate(result_tag);
            AmountResult resultItem = (AmountResult)result[result_tag];

            Assert.AreEqual((13350m + 1289m), resultItem.Amount);
        }
    }
}
