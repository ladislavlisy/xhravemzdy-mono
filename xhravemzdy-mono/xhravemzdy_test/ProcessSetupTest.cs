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
	public class ProcessSetupTest
	{
		PayrollProcess PayProcess  { get; set; }

		PayrollPeriod PayPeriod { get; set; } 

		PayTagGateway PayTags { get; set; }

		PayConceptGateway PayConcepts { get; set; }

        public ProcessSetupTest()
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
		public void test_insert_CodeOrder_1_at_beginning()
		{
			uint period = PayrollPeriod.NOW;
			CodeNameRefer tagCodeName = PayTagGateway.REF_SALARY_BASE;
			var values1 = new Dictionary<string, object>() { {"amount_monthly", 1000} };
			var values2 = new Dictionary<string, object>() { {"amount_monthly", 2000} };
			var values3 = new Dictionary<string, object>() { {"amount_monthly", 3000} };
			var values4 = new Dictionary<string, object>() { {"amount_monthly", 4000} };
			var values5 = new Dictionary<string, object>() { {"amount_monthly", 5000} };
			PayProcess.InsTerm(period, tagCodeName, 3, values3);
			PayProcess.InsTerm(period, tagCodeName, 5, values5);
			PayProcess.InsTerm(period, tagCodeName, 4, values4);
			PayProcess.InsTerm(period, tagCodeName, 2, values2);
			var valuesI = new Dictionary<string, object>() { { "amount_monthly", 15000 } };
			TagRefer payTag = PayProcess.AddTerm(tagCodeName, valuesI);
			Assert.AreEqual((uint)1, payTag.CodeOrder);
		}

		[Test]
		public void test_insert_CodeOrder_3_in_middle()
		{
			uint period = PayrollPeriod.NOW;
			CodeNameRefer tagCodeName = PayTagGateway.REF_SALARY_BASE;
			var values1 = new Dictionary<string, object>() { {"amount_monthly", 1000} };
			var values2 = new Dictionary<string, object>() { {"amount_monthly", 2000} };
			var values3 = new Dictionary<string, object>() { {"amount_monthly", 3000} };
			var values4 = new Dictionary<string, object>() { {"amount_monthly", 4000} };
			var values5 = new Dictionary<string, object>() { {"amount_monthly", 5000} };
			PayProcess.InsTerm(period, tagCodeName, 5, values5);
			PayProcess.InsTerm(period, tagCodeName, 1, values1);
			PayProcess.InsTerm(period, tagCodeName, 4, values4);
			PayProcess.InsTerm(period, tagCodeName, 2, values2);
			var valuesI = new Dictionary<string, object>() { { "amount_monthly", 15000 } };
			TagRefer payTag = PayProcess.AddTerm(tagCodeName, valuesI);
			Assert.AreEqual((uint)3, payTag.CodeOrder);
		}

		[Test]
		public void test_insert_CodeOrder_6_at_end()
		{
			uint period = PayrollPeriod.NOW;
			CodeNameRefer tagCodeName = PayTagGateway.REF_SALARY_BASE;
			var values1 = new Dictionary<string, object>() { {"amount_monthly", 1000} };
			var values2 = new Dictionary<string, object>() { {"amount_monthly", 2000} };
			var values3 = new Dictionary<string, object>() { {"amount_monthly", 3000} };
			var values4 = new Dictionary<string, object>() { {"amount_monthly", 4000} };
			var values5 = new Dictionary<string, object>() { {"amount_monthly", 5000} };
			PayProcess.InsTerm(period, tagCodeName, 3, values3);
			PayProcess.InsTerm(period, tagCodeName, 5, values5);
			PayProcess.InsTerm(period, tagCodeName, 1, values1);
			PayProcess.InsTerm(period, tagCodeName, 4, values4);
			PayProcess.InsTerm(period, tagCodeName, 2, values2);
			var valuesI = new Dictionary<string, object>() { { "amount_monthly", 15000 } };
			TagRefer payTag = PayProcess.AddTerm(tagCodeName, valuesI);
			Assert.AreEqual((uint)6, payTag.CodeOrder);
		}

		[Test]
		public void test_payroll_period_january_2013()
		{
			Assert.AreEqual((uint)201301, PayProcess.Period.Code);
		}
	}
}

