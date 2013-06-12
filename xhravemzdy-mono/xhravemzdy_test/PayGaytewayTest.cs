using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Core;
using PayrollLibrary.Business.PayTags;

namespace PayrollLibrary.PayrollTest
{
    /// <summary>
    /// inserting term and get code_order 
    /// Summary description for test_insert_CodeOrder_1_at_beginning
    /// it should return code_order == 1 at beginning
    /// Summary description for test_insert_CodeOrder_3_in_middle
    /// it should return code_order == 3 in the middle
    /// Summary description for test_insert_CodeOrder_6_at_end
    /// it should return code_order == 6 at the end
    /// test_payroll_period_january_2013
    /// it should return payroll period january 2013
    /// </summary>
    [TestClass]
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

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void test_salary_CONCEPT_SALARY_MONTHLY_class_name()
        {
            string className = PayConcepts.ClassNameFor(SalaryConceptTag.ConceptName());
            Assert.AreEqual(className, "PayrollLibrary.Business.Concepts.SalaryMonthlyConcept");
        }

        [TestMethod]
        public void test_salary_CONCEPT_SALARY_MONTHLY_code()
        {
           var values = new Dictionary<string, object>() { { "amount_monthly", 0 } };
           PayrollConcept conceptItem = PayConcepts.ConceptFor(SalaryConceptTag.Code, SalaryConceptTag.ConceptName(), values);
           Assert.AreEqual(conceptItem.Name, "CONCEPT_SALARY_MONTHLY");
           Assert.AreEqual(conceptItem.Code, (uint)ConceptCode.CONCEPT_SALARY_MONTHLY);
           Assert.AreEqual(conceptItem.TagCode, (uint)TagCode.TAG_SALARY_BASE);
        }

        [TestMethod]
        public void test_salary_TAG_SALARY_BASE_class_name()
        {
            string className = PayTags.ClassNameFor(SalaryConceptTag.Name);
            Assert.AreEqual(className, "PayrollLibrary.Business.PayTags.SalaryBaseTag");
        }

        [TestMethod]
        public void test_salary_TAG_SALARY_BASE_code()
        {
           PayrollTag tagItem = PayTags.TagFor(SalaryConceptTag.Name);
           Assert.AreEqual(tagItem.Code, (uint)TagCode.TAG_SALARY_BASE);
           Assert.AreEqual(tagItem.Name, "TAG_SALARY_BASE");
           Assert.AreEqual(tagItem.ConceptCode(), (uint)ConceptCode.CONCEPT_SALARY_MONTHLY);
        }
    }
}
