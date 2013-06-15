using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PayrollLibrary.Business.Core;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Concepts;

namespace PayrollLibrary.PayrollTest
{
    /// <summary>
    /// Summary description for PayrollRoundingTest
    /// </summary>
    [TestFixture]
    public class PayrollRoundingTest
    {
        PayrollProcess PayProcess { get; set; }

        PayrollPeriod PayPeriod { get; set; }

        PayTagGateway PayTags { get; set; }

        PayConceptGateway PayConcepts { get; set; }

        public PayrollRoundingTest()
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
        public void test_tax_base_under_100_CZK_should_be_round_up_to_1_CZK()
        {
            var values = new Dictionary<string, object>() { { "amount", 0 } };
            var test_concept = new TaxAdvanceBaseConcept(PayTagGateway.REF_TAX_ADVANCE_BASE.Code, values);
            bool tax_declared = true;
            Assert.AreEqual(test_concept.TaxRoundedBase(PayPeriod, tax_declared, 99m, 99m), 99m);
            Assert.AreEqual(test_concept.TaxRoundedBase(PayPeriod, tax_declared, 99.01m, 99.01m), 100m);
            Assert.AreEqual(test_concept.TaxRoundedBase(PayPeriod, tax_declared, 100m, 100m), 100m);
        }

        [Test]
        public void test_tax_base_over_100_CZK_should_be_round_up_to_100_CZK()
        {
            var values = new Dictionary<string, object>() { { "amount", 0 } };
            var test_concept = new TaxAdvanceBaseConcept(PayTagGateway.REF_TAX_ADVANCE_BASE.Code, values);
            bool tax_declared = true;
            Assert.AreEqual(test_concept.TaxRoundedBase(PayPeriod, tax_declared, 100.01m, 100.01m), 200m);
            Assert.AreEqual(test_concept.TaxRoundedBase(PayPeriod, tax_declared, 101m, 101m), 200m);
        }


        [Test]
        public void test_tax_advance_from_negative_base_should_be_0_CZK()
        {
            var values = new Dictionary<string, object>() { { "amount", 0 } };
            var test_concept = new TaxAdvanceConcept(PayTagGateway.REF_TAX_ADVANCE.Code, values);
            Assert.AreEqual(test_concept.TaxAdvCalculate(PayPeriod, -1m, -1m), 0m);
            Assert.AreEqual(test_concept.TaxAdvCalculate(PayPeriod, 0m, 0m), 0m);
        }


        [Test]
        public void test_tax_advance_should_be_round_up_to_1_CZK()
        {
            var values = new Dictionary<string, object>() { { "amount", 0 } };
            var test_concept = new TaxAdvanceConcept(PayTagGateway.REF_TAX_ADVANCE.Code, values);
            Assert.AreEqual(test_concept.TaxAdvCalculate(PayPeriod, 3333m, 3333m), 500m);
            Assert.AreEqual(test_concept.TaxAdvCalculate(PayPeriod, 2222m, 2222m), 334m);
        }


        [Test]
        public void test_health_insurance_should_be_round_up_to_1_CZK()
        {
            var values = new Dictionary<string, object>() { { "amount", 0 } };
            var test_concept = new InsuranceHealthConcept(PayTagGateway.REF_INSURANCE_HEALTH.Code, values);
            Assert.AreEqual(test_concept.InsuranceContribution(PayPeriod, 3333m, 3333m), 150m);
            Assert.AreEqual(test_concept.InsuranceContribution(PayPeriod, 2222m, 2222m), 100m);
        }

        [Test]
        public void test_social_insurance_should_be_round_up_to_1_CZK()
        {
            var values = new Dictionary<string, object>() { { "amount", 0 } };
            var test_concept = new InsuranceSocialConcept(PayTagGateway.REF_INSURANCE_SOCIAL.Code, values);
            Assert.AreEqual(test_concept.InsuranceContribution(PayPeriod, 3333m, false), 217m);
            Assert.AreEqual(test_concept.InsuranceContribution(PayPeriod, 2222m, false), 145m);
        }
    }
}
