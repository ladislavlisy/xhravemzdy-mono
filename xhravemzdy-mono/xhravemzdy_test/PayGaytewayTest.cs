using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using PayrollLibrary.Business.Core;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Concepts;
using PayrollLibrary.Business.PayTags;

namespace xhravemzdy_test
{
	[TestFixture()]
    public class PayGatewayTest
    {
        PayTagGateway PayTags { get; set; }

        PayConceptGateway PayConcepts { get; set; }

        SalaryBaseTag SalaryConceptTag { get; set; }

        public PayGatewayTest()
        {
            PayTags = new PayTagGateway();

            PayConcepts = new PayConceptGateway();

            SalaryConceptTag = new SalaryBaseTag();

        }

        [Test()]
        public void test_salary_CONCEPT_SALARY_MONTHLY_class_name()
        {
            string className = PayConcepts.ClassNameFor(SalaryConceptTag.ConceptName());
			Assert.AreEqual("PayrollLibrary.Business.Concepts.SalaryMonthlyConcept", className);
        }

        [Test()]
        public void test_salary_CONCEPT_SALARY_MONTHLY_code()
        {
            var values = new Dictionary<string, object>() { { "amount_monthly", 0 } };
            PayrollConcept conceptItem = PayConcepts.ConceptFor(SalaryConceptTag.Code, SalaryConceptTag.ConceptName(), values);
			Assert.AreEqual("CONCEPT_SALARY_MONTHLY", conceptItem.Name);
			Assert.AreEqual((uint)ConceptCode.CONCEPT_SALARY_MONTHLY, conceptItem.Code);
			Assert.AreEqual((uint)TagCode.TAG_SALARY_BASE, conceptItem.TagCode);
        }

		[Test()]
        public void test_salary_TAG_SALARY_BASE_class_name()
        {
            string className = PayTags.ClassNameFor(SalaryConceptTag.Name);
			Assert.AreEqual("PayrollLibrary.Business.PayTags.SalaryBaseTag", className);
        }

		[Test()]
        public void test_salary_TAG_SALARY_BASE_code()
        {
            PayrollTag tagItem = PayTags.TagFor(SalaryConceptTag.Name);
			Assert.AreEqual((uint)TagCode.TAG_SALARY_BASE, tagItem.Code);
			Assert.AreEqual("TAG_SALARY_BASE", tagItem.Name);
			Assert.AreEqual((uint)ConceptCode.CONCEPT_SALARY_MONTHLY, tagItem.ConceptCode());
        }
    }
}
