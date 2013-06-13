using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using PayrollLibrary.Business.Core;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Concepts;

namespace PayrollLibrary.PayrollTest
{
	[TestFixture()]
	public class ProcessTermTest
	{
		PayrollProcess PayProcess  { get; set; }

		PayrollPeriod PayPeriod { get; set; } 

		PayTagGateway PayTags { get; set; }

		PayConceptGateway PayConcepts { get; set; }

		public ProcessTermTest()
        {
        }

        [SetUp]
        public void Init()
		{
			PayPeriod = new PayrollPeriod(2013, 1);

			PayTags = new PayTagGateway();

			PayConcepts = new PayConceptGateway();

			PayProcess = new PayrollProcess(PayTags, PayConcepts, PayPeriod);
		}

        [TearDown]
        public void Cleanup()
        {
        }

        [Test]
		public void test_working_schedule()
		{
			CodeNameRefer tagCodeName = PayTagGateway.REF_SCHEDULE_WORK;
			var values = new Dictionary<string, object>() { { "hours_weekly", 40 } };
			TagRefer payTag = PayProcess.AddTerm(tagCodeName, values);
			var payTer = PayProcess.GetTerm(payTag);
			Assert.AreEqual(40, ((ScheduleWeeklyConcept)payTer[payTag]).HoursWeekly);
		}

		[Test]
		public void test_base_salary()
		{
			CodeNameRefer tagCodeName = PayTagGateway.REF_SALARY_BASE;
			var values = new Dictionary<string, object>() { { "amount_monthly", 15000m } };
			TagRefer payTag = PayProcess.AddTerm(tagCodeName, values);
			var payTer = PayProcess.GetTerm(payTag);
			Assert.AreEqual(15000m, ((SalaryMonthlyConcept)payTer[payTag]).AmountMonthly);
		}
	}
}

