using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using PayrollLibrary.Business.Core;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Concepts;

namespace PayrollLibrary.PayrollTest
{
    [TestFixture]
    public class PayrollCalcNetIncomeTests : PYGPayrollProcessTestCase
    {
        IDictionary<string, object> PayrollSpecs { get; set; }
        IDictionary<string, object> PayrollResults { get; set; }

        public PayrollCalcNetIncomeTests()
        {
        }

        [SetUp]
        public override void Init()
        {
            base.Init();
        }

        void SetUpTestSpecs(Array values)
        {
            uint valueIn1 = (uint) values.GetValue(12);
            uint valueIn2 = (uint) values.GetValue(13);
            uint valueIn3 = (uint) values.GetValue(14);
            uint values3I = (valueIn1 + 10*valueIn2 + 100*valueIn3);

            PayrollSpecs = new Dictionary<string, object> () {
                { "name"                  , STRTYPE(values.GetValue( 0)) },
                { "period"                , U_UNBOX(values.GetValue( 1)) }, // Given Payroll process for payroll period 01 2013
                { "schedule"              , I_UNBOX(values.GetValue( 2)) }, // And   Employee works in Weekly schedule 40 hours
                { "absence"               , I_UNBOX(values.GetValue( 3)) }, // And   Employee has 0 hours of absence
                { "salary"                , NUMTYPE(values.GetValue( 4)) }, // And   Employee Salary is CZK 15000 monthly
                { "tax payer"             , U_UNBOX(values.GetValue( 5)) }, // And   YES Employee is Regular Tax payer
                { "health payer"          , U_UNBOX(values.GetValue( 6)) }, // And   YES Employee is Regular Health insurance payer
                { "health minim"          , U_UNBOX(values.GetValue( 7)) },
                { "social payer"          , U_UNBOX(values.GetValue( 8)) }, // And   YES Employee is Regular Social insurance payer
                { "pension payer"         , U_UNBOX(values.GetValue( 9)) }, // And   NO Employee is Regular Pension savings payer
                { "tax payer benefit"     , U_UNBOX(values.GetValue(10)) }, // And   YES Employee claims tax benefit on tax payer
                { "tax child benefit"     , U_UNBOX(values.GetValue(11)) }, // And   Employee claims tax benefit on 0 child
                { "tax disability benefit", values3I                     }, // And   NO:NO:NO Employee claims tax benefit on disability
                { "tax studying benefit"  , U_UNBOX(values.GetValue(15)) }, // And   NO Employee claims tax benefit on preparing by studying
                { "health employer"       , U_UNBOX(values.GetValue(16)) }, // And   YES Employee is Employer contribution for Health insurance payer
                { "social employer"       , U_UNBOX(values.GetValue(17)) }  // And   YES Employee is Employer contribution for Social insurance payer
            };
        }

        void SetUpTestResults(Array values)
        {
            PayrollResults = new Dictionary<string, object> () {
                { "name"               , STRTYPE(values.GetValue( 0)) },
                { "tax income"         , NUMTYPE(values.GetValue( 1)) }, // Then  Accounted tax income should be CZK 15000
                { "premium insurance"  , NUMTYPE(values.GetValue( 2)) }, // And   Premium insurance should be CZK 5100
                { "tax base"           , NUMTYPE(values.GetValue( 3)) }, // And   Tax base should be CZK 20100
                { "health base"        , NUMTYPE(values.GetValue( 4)) }, // And   Accounted income for Health insurance should be CZK 15000
                { "social base"        , NUMTYPE(values.GetValue( 5)) }, // And   Accounted income for Social insurance should be CZK 15000
                { "health ins"         , NUMTYPE(values.GetValue( 6)) }, // And   Contribution to Health insurance should be CZK 675
                { "social ins"         , NUMTYPE(values.GetValue( 7)) }, // And   Contribution to Social insurance should be CZK 975
                { "tax before"         , NUMTYPE(values.GetValue( 8)) }, // And   Tax advance before tax relief on payer should be CZK 3015
                { "payer relief"       , NUMTYPE(values.GetValue( 9)) }, // And   Tax relief on payer should be CZK 2070
                { "tax after A relief" , NUMTYPE(values.GetValue(10)) }, // And   Tax advance after relief on payer should be CZK 945
                { "child relief"       , NUMTYPE(values.GetValue(11)) }, // And   Tax relief on child should be CZK 0
                { "tax after C relief" , NUMTYPE(values.GetValue(12)) }, // And   Tax advance after relief on child should be CZK 945
                { "tax advance"        , NUMTYPE(values.GetValue(13)) }, // And   Tax advance should be CZK 945
                { "tax bonus"          , NUMTYPE(values.GetValue(14)) }, // And   Tax bonus should be CZK 0
                { "gross income"       , NUMTYPE(values.GetValue(15)) }, // And   Gross income should be CZK 15000
                { "netto income"       , NUMTYPE(values.GetValue(16)) }  // And   Netto income should be CZK 12405
            };
        }

        void TestPayrollProcessWithSpec(IDictionary<string, object> specs, IDictionary<string, object> results)
        {
            var EMPTY_VALUES = new Dictionary<string, object>() { };

            var testSpec = specs;
            string testSpecName = S_GET_FROM(testSpec, "name");
            var schedule_work_value = I_MAKE_HASH("hours_weekly", I_GET_FROM(testSpec, "schedule"));
            var schedule_term_value = new Dictionary<string, object>() { { "date_from", null }, { "date_end", null } };

            PayProcess.AddTerm(REF_SCHEDULE_WORK, schedule_work_value);
            PayProcess.AddTerm(REF_SCHEDULE_TERM, schedule_term_value);
            var absence_hours_value = I_MAKE_HASH("hours", I_GET_FROM(testSpec, "absence"));
            PayProcess.AddTerm(REF_HOURS_ABSENCE, absence_hours_value);
            var salary_amount_value = D_MAKE_HASH(@"amount_monthly", D_GET_FROM(testSpec, @"salary"));
            PayProcess.AddTerm(REF_SALARY_BASE, salary_amount_value);
            uint yes_no1 = U_GET_FROM(testSpec, @"tax payer");
            uint tax_interest = yes_no1!=0 ? 1u : 0u;
            uint tax_declare  = yes_no1==3 ? 1u : 0u;
            var interest_value1 = new Dictionary<string, object>() { 
                { "interest_code", tax_interest },
                { "declare_code", tax_declare }
            };
            PayProcess.AddTerm(REF_TAX_INCOME_BASE, interest_value1);
            uint yes_no2 = U_GET_FROM(testSpec, "health payer");
            uint minim  = U_GET_FROM(testSpec,  "health minim");
            var interest_value2 = new Dictionary<string, object>() { 
                { "interest_code", yes_no2 },
                { "minimum_asses", minim }
            };
            PayProcess.AddTerm(REF_INSURANCE_HEALTH_BASE, interest_value2);
            PayProcess.AddTerm(REF_INSURANCE_HEALTH, interest_value2);
            uint yes_no3 = U_GET_FROM(testSpec, "social payer");
            var interest_value3 = U_MAKE_HASH("interest_code", yes_no3);
            PayProcess.AddTerm(REF_INSURANCE_SOCIAL_BASE, interest_value3);
            PayProcess.AddTerm(REF_INSURANCE_SOCIAL, interest_value3);
            uint yes_no4 = U_GET_FROM(testSpec, "pension payer");
            var interest_value4 = I_MAKE_HASH("interest_code", yes_no4);
            PayProcess.AddTerm(REF_SAVINGS_PENSIONS, interest_value4);
            uint yes_no5 = U_GET_FROM(testSpec, "tax payer benefit");
            var relief_value1 = U_MAKE_HASH("relief_code", yes_no5);
            PayProcess.AddTerm(REF_TAX_CLAIM_PAYER, relief_value1);
            uint count = U_GET_FROM(testSpec, "tax child benefit");
            var relief_value2 = U_MAKE_HASH("relief_code", 1u);
            for (int i = 0; i < count; i++) {
                PayProcess.AddTerm(REF_TAX_CLAIM_CHILD, relief_value2);
            }
            uint yes_no6 = U_GET_FROM(testSpec, "tax disability benefit");
            var relief_value3 = new Dictionary<string, object>() { 
                { "relief_code_1", (yes_no6 % 10) },
                { "relief_code_2", ((yes_no6/10) % 10) },
                { "relief_code_3", ((yes_no6/100) % 10) }
            };
            PayProcess.AddTerm(REF_TAX_CLAIM_DISABILITY, relief_value3);
            uint yes_no7 = U_GET_FROM(testSpec, "tax studying benefit");
            var relief_value4 = U_MAKE_HASH("relief_code", yes_no7);
            PayProcess.AddTerm(REF_TAX_CLAIM_STUDYING, relief_value4);
            uint yes_no8 = U_GET_FROM(testSpec, "health employer");
            var interest_value5 = U_MAKE_HASH("interest_code", yes_no8);
            PayProcess.AddTerm(REF_TAX_EMPLOYERS_HEALTH, interest_value5);
            uint yes_no9 = U_GET_FROM(testSpec, "social employer");
            var interest_value6 = U_MAKE_HASH("interest_code", yes_no9);
            PayProcess.AddTerm(REF_TAX_EMPLOYERS_SOCIAL, interest_value6);

            PayProcess.AddTerm(REF_INCOME_GROSS, EMPTY_VALUES);
            TagRefer evResultTermTag = PayProcess.AddTerm(REF_INCOME_NETTO, EMPTY_VALUES);
            var evResultDictVal = PayProcess.Evaluate(evResultTermTag);

            var testResults = results;
            TagRefer resultTag = new TagRefer(PayrollPeriod.NOW, (uint)TagCode.TAG_INCOME_NETTO, 1u);
            decimal result01 = D_GET_FROM(testResults, @"tax income");
            decimal result02 = D_GET_FROM(testResults, @"premium insurance");
            decimal result03 = D_GET_FROM(testResults, @"tax base");
            decimal result04 = D_GET_FROM(testResults, @"health base");
            decimal result05 = D_GET_FROM(testResults, @"social base");
            decimal result06 = D_GET_FROM(testResults, @"health ins");
            decimal result07 = D_GET_FROM(testResults, @"social ins");
            decimal result08 = D_GET_FROM(testResults, @"tax before");
            decimal result09 = D_GET_FROM(testResults, @"payer relief");
            decimal result10 = D_GET_FROM(testResults, @"tax after A relief");
            decimal result11 = D_GET_FROM(testResults, @"child relief");
            decimal result12 = D_GET_FROM(testResults, @"tax after C relief");
            decimal result13 = D_GET_FROM(testResults, @"tax advance");
            decimal result14 = D_GET_FROM(testResults, @"tax bonus");
            decimal result15 = D_GET_FROM(testResults, @"gross income");
            decimal result16 = D_GET_FROM(testResults, @"netto income");

            decimal testVal01 =  GetResultIncomeBase(REF_TAX_INCOME_BASE);
            decimal testVal02a = GetResultPayment(REF_TAX_EMPLOYERS_HEALTH);
            decimal testVal02b = GetResultPayment(REF_TAX_EMPLOYERS_SOCIAL);
            decimal testVal03 =  GetResultIncomeBase(REF_TAX_ADVANCE_BASE);
            decimal testVal03w = GetResultIncomeBase(REF_TAX_WITHHOLD_BASE);
            decimal testVal04 =  GetResultEmployeeBase(REF_INSURANCE_HEALTH_BASE);
            decimal testVal05 =  GetResultEmployeeBase(REF_INSURANCE_SOCIAL_BASE);
            decimal testVal06 =  GetResultPayment(REF_INSURANCE_HEALTH);
            decimal testVal07 =  GetResultPayment(REF_INSURANCE_SOCIAL);
            decimal testVal08 =  GetResultPayment(REF_TAX_ADVANCE);
            decimal testVal09 =  GetResultTaxRelief(REF_TAX_RELIEF_PAYER);
            decimal testVal10 =  GetResultAfterReliefA(REF_TAX_ADVANCE_FINAL);
            decimal testVal11 =  GetResultTaxRelief(REF_TAX_RELIEF_CHILD);
            decimal testVal12 =  GetResultAfterReliefC(REF_TAX_ADVANCE_FINAL);
            decimal testVal13 =  GetResultPayment(REF_TAX_ADVANCE_FINAL);
            decimal testVal14 =  GetResultPayment(REF_TAX_BONUS_CHILD);
            decimal testVal15 =  GetResultAmount(REF_INCOME_GROSS);
            decimal testVal16 =  GetResultAmount(REF_INCOME_NETTO);
            decimal testVal02 =  decimal.Add(testVal02a, testVal02b);

            Assert.AreEqual(testVal01 , result01, @"tax income         should be {0} CZK, NOT {1}!", result01, testVal01);
            Assert.AreEqual(testVal02 , result02, @"premium insurance  should be {0} CZK, NOT {1}!", result02, testVal02);
            Assert.AreEqual(testVal03 , result03, @"tax base           should be {0} CZK, NOT {1}!", result03, testVal03);
            Assert.AreEqual(testVal04 , result04, @"health base        should be {0} CZK, NOT {1}!", result04, testVal04);
            Assert.AreEqual(testVal05 , result05, @"social base        should be {0} CZK, NOT {1}!", result05, testVal05);
            Assert.AreEqual(testVal06 , result06, @"health ins         should be {0} CZK, NOT {1}!", result06, testVal06);
            Assert.AreEqual(testVal07 , result07, @"social ins         should be {0} CZK, NOT {1}!", result07, testVal07);
            Assert.AreEqual(testVal08 , result08, @"tax before         should be {0} CZK, NOT {1}!", result08, testVal08);
            Assert.AreEqual(testVal09 , result09, @"payer relief       should be {0} CZK, NOT {1}!", result09, testVal09);
            Assert.AreEqual(testVal10 , result10, @"tax after A relief should be {0} CZK, NOT {1}!", result10, testVal10);
            Assert.AreEqual(testVal11 , result11, @"child relief       should be {0} CZK, NOT {1}!", result11, testVal11);
            Assert.AreEqual(testVal12 , result12, @"tax after C relief should be {0} CZK, NOT {1}!", result12, testVal12);
            Assert.AreEqual(testVal13 , result13, @"tax advance        should be {0} CZK, NOT {1}!", result13, testVal13);
            Assert.AreEqual(testVal14 , result14, @"tax bonus          should be {0} CZK, NOT {1}!", result14, testVal14);
            Assert.AreEqual(testVal15 , result15, @"gross income       should be {0} CZK, NOT {1}!", result15, testVal15);
            Assert.AreEqual(testVal16 , result16, @"netto income       should be {0} CZK, NOT {1}!", result16, testVal16);
        }

        [Test]
        public void test01_PP_Mzda_DanPoj_SlevyZaklad()
        {
            object[] testSpecValues = new object[] {
                "01-PP-Mzda-DanPoj-SlevyZaklad", // name 01-PP
                201301u ,// period                  201301
                40     ,// schedule                40
                0      ,// absence                 0
                15000m  ,// salary                  CZK 15000
                (TAX_DECLARED),// tax payer               DECLARE
                1u      ,// health payer            YES
                1u      ,// health minim            YES
                1u      ,// social payer            YES
                0u      ,// pension payer           NO
                1u      ,// tax payer benefit       YES
                0u      ,// tax child benefit       0
                0u      ,// tax disability benefit1 NO:NO:NO
                0u      ,// tax disability benefit2 NO:NO:NO
                0u      ,// tax disability benefit3 NO:NO:NO
                0u      ,// tax studying benefit    NO
                1u      ,// health employer         YES
                1u       // social employer         YES
            };
            SetUpTestSpecs(testSpecValues);

            object[] testResultValues = new object[] {
                "01-PP-Mzda-DanPoj-SlevyZaklad",// name 01-PP
                (decimal)15000 ,// tax income              CZK 15000
                (decimal)5100  ,// premium insurance       CZK 5100
                (decimal)20100 ,// tax base                CZK 20100
                (decimal)15000 ,// health base             CZK 15000
                (decimal)15000 ,// social base             CZK 15000
                (decimal)675   ,// health ins              CZK 675
                (decimal)975   ,// social ins              CZK 975
                (decimal)3015  ,// tax before              CZK 3015
                (decimal)2070  ,// payer relief            CZK 2070
                (decimal)945   ,// tax after A relief      CZK 945
                (decimal)0     ,// child relief            CZK 0
                (decimal)945   ,// tax after C relief      CZK 945
                (decimal)945   ,// tax advance             CZK 945
                (decimal)0     ,// tax bonus               CZK 0
                (decimal)15000 ,// gross income            CZK 15000
                (decimal)12405  // netto income            CZK 12405
            };
            SetUpTestResults(testResultValues);

            TestPayrollProcessWithSpec(PayrollSpecs, PayrollResults);
        }

        [Test]
        public void test02_PP_Mzda_DanPoj_SlevyDite1()
        {
            object[] testSpecValues = new object[] {
            @"02-PP-Mzda-DanPoj-SlevyDite1",201301u,40,0,15600m,(TAX_DECLARED),1u,1u,1u,0u,1u,1u,0u,0u,0u,0u,1u,1u
            };
            SetUpTestSpecs(testSpecValues);

            object[] testResultValues = new object[] {
            @"02-PP-Mzda-DanPoj-SlevyDite1",15600m,5304m,21000m,15600m,15600m,702m,1014m,3150m,2070m,1080m,1080m,0m,0m,0m,15600m,13884m
            };
            SetUpTestResults(testResultValues);

            TestPayrollProcessWithSpec(PayrollSpecs, PayrollResults);
        }

        [Test]
        public void test03_PP_Mzda_DanPoj_SlevyDite1_Bonus()
        {
            object[] testSpecValues = new object[] {
            @"03-PP-Mzda-DanPoj-SlevyDite1-Bonus",201301u,40,0,15000m,(TAX_DECLARED),1u,1u,1u,0u,1u,1u,0u,0u,0u,0u,1u,1u
            };
            SetUpTestSpecs(testSpecValues);

            object[] testResultValues = new object[] {
            @"03-PP-Mzda-DanPoj-SlevyDite1-Bonus",15000m,5100m,20100m,15000m,15000m,675m,975m,3015m,2070m,945m,945m,0m,0m,172m,15000m,13522m
            };
            SetUpTestResults(testResultValues);

            TestPayrollProcessWithSpec(PayrollSpecs, PayrollResults);
        }

        [Test]
        public void test04_PP_Mzda_DanPoj_SlevyDite2_Bonus()
        {
            object[] testSpecValues = new object[] {
            @"04-PP-Mzda-DanPoj-SlevyDite2-Bonus",201301u,40,0,15000m,(TAX_DECLARED),1u,1u,1u,0u,1u,2u,0u,0u,0u,0u,1u,1u
            };
            SetUpTestSpecs(testSpecValues);

            object[] testResultValues = new object[] {
            @"04-PP-Mzda-DanPoj-SlevyDite2-Bonus",15000m,5100m,20100m,15000m,15000m,675m,975m,3015m,2070m,945m,945m,0m,0m,1289m,15000m,14639m
            };
            SetUpTestResults(testResultValues);

            TestPayrollProcessWithSpec(PayrollSpecs, PayrollResults);
        }

        [Test]
        public void test05_PP_Mzda_DanPoj_MaxBonus()
        {
            object[] testSpecValues = new object[] {
            @"05-PP-Mzda-DanPoj-MaxBonus",201301u,40,0,10000m,(TAX_DECLARED),1u,1u,1u,0u,1u,7u,0u,0u,0u,0u,1u,1u
            };
            SetUpTestSpecs(testSpecValues);

            object[] testResultValues = new object[] {
            @"05-PP-Mzda-DanPoj-MaxBonus",10000m,3400m,13400m,10000m,10000m,450m,650m,2010m,2010m,0m,0m,0m,0m,5025m,10000m,13925m
            };
            SetUpTestResults(testResultValues);

            TestPayrollProcessWithSpec(PayrollSpecs, PayrollResults);
        }

        [Test]
        public void test06_PP_Mzda_DanPoj_MinZdrav()
        {
            object[] testSpecValues = new object[] {
            @"06-PP-Mzda-DanPoj-MinZdrav",201301u,40,0,7800m,(TAX_DECLARED),1u,1u,1u,0u,1u,0u,0u,0u,0u,0u,1u,1u
            };
            SetUpTestSpecs(testSpecValues);

            object[] testResultValues = new object[] {
            @"06-PP-Mzda-DanPoj-MinZdrav",7800m,2652m,10500m,8000m,7800m,378m,507m,1575m,1575m,0m,0m,0m,0m,0m,7800m,6915m
            };
            SetUpTestResults(testResultValues);

            TestPayrollProcessWithSpec(PayrollSpecs, PayrollResults);
        }

        [Test]
        public void test07_PP_Mzda_DanPoj_MaxZdrav12()
        {
            object[] testSpecValues = new object[] {
            @"07-PP-Mzda-DanPoj-MaxZdrav12",201301u,40,0,1809964m,(TAX_DECLARED),1u,1u,1u,0u,1u,0u,0u,0u,0u,0u,1u,1u
            };
            SetUpTestSpecs(testSpecValues);

            object[] testResultValues = new object[] {
            @"07-PP-Mzda-DanPoj-MaxZdrav12",1809964m,473505m,2283500m,1809964m,1242432m,81449m,80759m,461975m,2070m,459905m,0m,459905m,459905m,0m,1809964m,1187851m
            };
            SetUpTestResults(testResultValues);

            TestPayrollProcessWithSpec(PayrollSpecs, PayrollResults);
        }

        [Test]
        public void test08_PP_Mzda_DanPoj_MaxSocial12()
        {
            object[] testSpecValues = new object[] {
            @"08-PP-Mzda-DanPoj-MaxSocial12",201301u,40,0,1206676m,(TAX_DECLARED),1u,1u,1u,0u,1u,0u,0u,0u,0u,0u,1u,1u
            };
            SetUpTestSpecs(testSpecValues);

            object[] testResultValues = new object[] {
            @"08-PP-Mzda-DanPoj-MaxSocial12",1206676m,410270m,1617000m,1206676m,1206676m,54301m,78434m,319770m,2070m,317700m,0m,317700m,317700m,0m,1206676m,756241m
            };
            SetUpTestResults(testResultValues);

            TestPayrollProcessWithSpec(PayrollSpecs, PayrollResults);
        }

        [Test]
        public void test09_PP_Mzda_DanPoj_MaxSocial13()
        {
            object[] testSpecValues = new object[] {
            @"09-PP-Mzda-DanPoj-MaxSocial13",201301u,40,0,1242532m,(TAX_DECLARED),1u,1u,1u,0u,1u,0u,0u,0u,0u,0u,1u,1u
            };
            SetUpTestSpecs(testSpecValues);

            object[] testResultValues = new object[] {
            @"09-PP-Mzda-DanPoj-MaxSocial13",1242532m,422436m,1665000m,1242532m,1242432m,55914m,80759m,329480m,2070m,327410m,0m,327410m,327410m,0m,1242532m,778449m
            };
            SetUpTestResults(testResultValues);

            TestPayrollProcessWithSpec(PayrollSpecs, PayrollResults);
        }

        [Test]
        public void test10_PP_Mzda_DanPoj_Neodpr064()
        {
            object[] testSpecValues = new object[] {
            @"10-PP-Mzda-DanPoj-Neodpr064",201301u,40,46,20000m,(TAX_DECLARED),1u,1u,1u,0u,1u,0u,0u,0u,0u,0u,1u,1u
            };
            SetUpTestSpecs(testSpecValues);

            object[] testResultValues = new object[] {
            @"10-PP-Mzda-DanPoj-Neodpr064",15000m,5100m,20100m,15000m,15000m,675m,975m,3015m,2070m,945m,0m,945m,945m,0m,15000m,12405m
            };
            SetUpTestResults(testResultValues);

            TestPayrollProcessWithSpec(PayrollSpecs, PayrollResults);
        }

        [Test]
        public void test22_PP_Mzda_DanPoj_SolidarDan()
        {
            object[] testSpecValues = new object[] {
            @"22-PP-Mzda-DanPoj-SolidarDan",201301u,40,0,104536m,(TAX_DECLARED),1u,1u,1u,0u,1u,0u,0u,0u,0u,0u,1u,1u
            };
            SetUpTestSpecs(testSpecValues);

            object[] testResultValues = new object[] {
            @"22-PP-Mzda-DanPoj-SolidarDan",104536m,35542m,140100m,104536m,104536m,4705m,6795m,21085m,2070m,19015m,0m,19015m,19015m,0m,104536m,74021m
            };
            SetUpTestResults(testResultValues);

            TestPayrollProcessWithSpec(PayrollSpecs, PayrollResults);
        }

        [Test]
        public void test23_PP_Mzda_DanPoj_DuchSpor()
        {
            object[] testSpecValues = new object[] {
            @"23-PP-Mzda-DanPoj-DuchSpor",201301u,40,0,15000m,(TAX_DECLARED),1u,1u,1u,0u,1u,0u,0u,0u,0u,0u,1u,1u
            };
            SetUpTestSpecs(testSpecValues);

            object[] testResultValues = new object[] {
            @"23-PP-Mzda-DanPoj-DuchSpor",15000m,5100m,20100m,15000m,15000m,675m,975m,3015m,2070m,945m,0m,945m,945m,0m,15000m,12405m
            };
            SetUpTestResults(testResultValues);

            TestPayrollProcessWithSpec(PayrollSpecs, PayrollResults);
        }

        [Test]
        public void test27_PP_Mzda_DanPoj_SlevyInv1()
        {
            object[] testSpecValues = new object[] {
            @"27-PP-Mzda-DanPoj-SlevyInv1",201301u,40,0,20000m,(TAX_DECLARED),1u,1u,1u,0u,1u,0u,1u,0u,0u,0u,1u,1u
            };
            SetUpTestSpecs(testSpecValues);

            object[] testResultValues = new object[] {
            @"27-PP-Mzda-DanPoj-SlevyInv1",20000m,6800m,26800m,20000m,20000m,900m,1300m,4020m,2280m,1740m,0m,1740m,1740m,0m,20000m,16060m
            };
            SetUpTestResults(testResultValues);

            TestPayrollProcessWithSpec(PayrollSpecs, PayrollResults);
        }

        [Test]
        public void test28_PP_Mzda_DanPoj_SlevyInv2()
        {
            object[] testSpecValues = new object[] {
            @"28-PP-Mzda-DanPoj-SlevyInv2",201301u,40,0,15000m,(TAX_DECLARED),1u,1u,1u,0u,1u,0u,0u,1u,0u,0u,1u,1u
            };
            SetUpTestSpecs(testSpecValues);

            object[] testResultValues = new object[] {
            @"28-PP-Mzda-DanPoj-SlevyInv2",15000m,5100m,20100m,15000m,15000m,675m,975m,3015m,2490m,525m,0m,525m,525m,0m,15000m,12825m
            };
            SetUpTestResults(testResultValues);

            TestPayrollProcessWithSpec(PayrollSpecs, PayrollResults);
        }

        [Test]
        public void test29_PP_Mzda_DanPoj_SlevyInv3()
        {
            object[] testSpecValues = new object[] {
            @"29-PP-Mzda-DanPoj-SlevyInv3",201301u,40,0,15000m,(TAX_DECLARED),1u,1u,1u,0u,1u,0u,0u,1u,0u,0u,1u,1u
            };
            SetUpTestSpecs(testSpecValues);

            object[] testResultValues = new object[] {
            @"29-PP-Mzda-DanPoj-SlevyInv3",15000m,5100m,20100m,15000m,15000m,675m,975m,3015m,2490m,525m,0m,525m,525m,0m,15000m,12825m
            };
            SetUpTestResults(testResultValues);

            TestPayrollProcessWithSpec(PayrollSpecs, PayrollResults);
        }

        [Test]
        public void test30_PP_Mzda_DanPoj_SlevyStud()
        {
            object[] testSpecValues = new object[] {
            @"30-PP-Mzda-DanPoj-SlevyStud",201301u,40,0,15000m,(TAX_DECLARED),1u,1u,1u,0u,1u,0u,0u,0u,0u,1u,1u,1u
            };
            SetUpTestSpecs(testSpecValues);

            object[] testResultValues = new object[] {
            @"30-PP-Mzda-DanPoj-SlevyStud",15000m,5100m,20100m,15000m,15000m,675m,975m,3015m,2405m,610m,0m,610m,610m,0m,15000m,12740m
            };
            SetUpTestResults(testResultValues);

            TestPayrollProcessWithSpec(PayrollSpecs, PayrollResults);
        }

        [Test]
        public void test31_PP_Mzda_DanPoj_SlevyZaklad15()
        {
            object[] testSpecValues = new object[] {
            @"31-PP-Mzda-DanPoj-SlevyZaklad15",201301u,40,0,15000m,(TAX_DECLARED),1u,1u,1u,0u,1u,0u,0u,0u,0u,0u,1u,1u
            };
            SetUpTestSpecs(testSpecValues);

            object[] testResultValues = new object[] {
            @"31-PP-Mzda-DanPoj-SlevyZaklad15",15000m,5100m,20100m,15000m,15000m,675m,975m,3015m,2070m,945m,0m,945m,945m,0m,15000m,12405m
            };
            SetUpTestResults(testResultValues);

            TestPayrollProcessWithSpec(PayrollSpecs, PayrollResults);
        }

        [Test]
        public void test32_PP_Mzda_DanPoj_SlevyZaklad20()
        {
            object[] testSpecValues = new object[] {
            @"32-PP-Mzda-DanPoj-SlevyZaklad20",201301u,40,0,20000m,(TAX_DECLARED),1u,1u,1u,0u,1u,0u,0u,0u,0u,0u,1u,1u
            };
            SetUpTestSpecs(testSpecValues);

            object[] testResultValues = new object[] {
            @"32-PP-Mzda-DanPoj-SlevyZaklad20",20000m,6800m,26800m,20000m,20000m,900m,1300m,4020m,2070m,1950m,0m,1950m,1950m,0m,20000m,15850m
            };
            SetUpTestResults(testResultValues);

            TestPayrollProcessWithSpec(PayrollSpecs, PayrollResults);
        }

        [Test]
        public void test33_PP_Mzda_DanPoj_SlevyZaklad25()
        {
            object[] testSpecValues = new object[] {
            @"33-PP-Mzda-DanPoj-SlevyZaklad25",201301u,40,0,25000m,(TAX_DECLARED),1u,1u,1u,0u,1u,0u,0u,0u,0u,0u,1u,1u
            };
            SetUpTestSpecs(testSpecValues);

            object[] testResultValues = new object[] {
            @"33-PP-Mzda-DanPoj-SlevyZaklad25",25000m,8500m,33500m,25000m,25000m,1125m,1625m,5025m,2070m,2955m,0m,2955m,2955m,0m,25000m,19295m
            };
            SetUpTestResults(testResultValues);

            TestPayrollProcessWithSpec(PayrollSpecs, PayrollResults);
        }

        [Test]
        public void test34_PP_Mzda_DanPoj_SlevyZaklad30()
        {
            object[] testSpecValues = new object[] {
            @"34-PP-Mzda-DanPoj-SlevyZaklad30",201301u,40,0,30000m,(TAX_DECLARED),1u,1u,1u,0u,1u,0u,0u,0u,0u,0u,1u,1u
            };
            SetUpTestSpecs(testSpecValues);

            object[] testResultValues = new object[] {
            @"34-PP-Mzda-DanPoj-SlevyZaklad30",30000m,10200m,40200m,30000m,30000m,1350m,1950m,6030m,2070m,3960m,0m,3960m,3960m,0m,30000m,22740m
            };
            SetUpTestResults(testResultValues);

            TestPayrollProcessWithSpec(PayrollSpecs, PayrollResults);
        }

        [Test]
        public void test35_PP_Mzda_DanPoj_SlevyZaklad35()
        {
            object[] testSpecValues = new object[] {
            @"35-PP-Mzda-DanPoj-SlevyZaklad35",201301u,40,0,35000m,(TAX_DECLARED),1u,1u,1u,0u,1u,0u,0u,0u,0u,0u,1u,1u
            };
            SetUpTestSpecs(testSpecValues);

            object[] testResultValues = new object[] {
            @"35-PP-Mzda-DanPoj-SlevyZaklad35",35000m,11900m,46900m,35000m,35000m,1575m,2275m,7035m,2070m,4965m,0m,4965m,4965m,0m,35000m,26185m
            };
            SetUpTestResults(testResultValues);

            TestPayrollProcessWithSpec(PayrollSpecs, PayrollResults);
        }

        [Test]
        public void test36_PP_Mzda_DanPoj_SlevyZaklad40()
        {
            object[] testSpecValues = new object[] {
            @"36-PP-Mzda-DanPoj-SlevyZaklad40",201301u,40,0,40000m,(TAX_DECLARED),1u,1u,1u,0u,1u,0u,0u,0u,0u,0u,1u,1u
            };
            SetUpTestSpecs(testSpecValues);

            object[] testResultValues = new object[] {
            @"36-PP-Mzda-DanPoj-SlevyZaklad40",40000m,13600m,53600m,40000m,40000m,1800m,2600m,8040m,2070m,5970m,0m,5970m,5970m,0m,40000m,29630m
            };
            SetUpTestResults(testResultValues);

            TestPayrollProcessWithSpec(PayrollSpecs, PayrollResults);
        }

        [Test]
        public void test37_PP_Mzda_DanPoj_SlevyZaklad45()
        {
            object[] testSpecValues = new object[] {
            @"37-PP-Mzda-DanPoj-SlevyZaklad45",201301u,40,0,45000m,(TAX_DECLARED),1u,1u,1u,0u,1u,0u,0u,0u,0u,0u,1u,1u
            };
            SetUpTestSpecs(testSpecValues);

            object[] testResultValues = new object[] {
            @"37-PP-Mzda-DanPoj-SlevyZaklad45",45000m,15300m,60300m,45000m,45000m,2025m,2925m,9045m,2070m,6975m,0m,6975m,6975m,0m,45000m,33075m
            };
            SetUpTestResults(testResultValues);

            TestPayrollProcessWithSpec(PayrollSpecs, PayrollResults);
        }

        [Test]
        public void test38_PP_Mzda_DanPoj_SlevyZaklad50()
        {
            object[] testSpecValues = new object[] {
            @"38-PP-Mzda-DanPoj-SlevyZaklad50",201301u,40,0,50000m,(TAX_DECLARED),1u,1u,1u,0u,1u,0u,0u,0u,0u,0u,1u,1u
            };
            SetUpTestSpecs(testSpecValues);

            object[] testResultValues = new object[] {
            @"38-PP-Mzda-DanPoj-SlevyZaklad50",50000m,17000m,67000m,50000m,50000m,2250m,3250m,10050m,2070m,7980m,0m,7980m,7980m,0m,50000m,36520m
            };
            SetUpTestResults(testResultValues);

            TestPayrollProcessWithSpec(PayrollSpecs, PayrollResults);
        }

        [Test]
        public void test39_PP_Mzda_DanPoj_SlevyZaklad55()
        {
            object[] testSpecValues = new object[] {
            @"39-PP-Mzda-DanPoj-SlevyZaklad55",201301u,40,0,55000m,(TAX_DECLARED),1u,1u,1u,0u,1u,0u,0u,0u,0u,0u,1u,1u
            };
            SetUpTestSpecs(testSpecValues);

            object[] testResultValues = new object[] {
            @"39-PP-Mzda-DanPoj-SlevyZaklad55",55000m,18700m,73700m,55000m,55000m,2475m,3575m,11055m,2070m,8985m,0m,8985m,8985m,0m,55000m,39965m
            };
            SetUpTestResults(testResultValues);

            TestPayrollProcessWithSpec(PayrollSpecs, PayrollResults);
        }

        [Test]
        public void test40_PP_Mzda_DanPoj_SlevyZaklad60()
        {
            object[] testSpecValues = new object[] {
            @"40-PP-Mzda-DanPoj-SlevyZaklad60",201301u,40,0,60000m,(TAX_DECLARED),1u,1u,1u,0u,1u,0u,0u,0u,0u,0u,1u,1u
            };
            SetUpTestSpecs(testSpecValues);

            object[] testResultValues = new object[] {
            @"40-PP-Mzda-DanPoj-SlevyZaklad60",60000m,20400m,80400m,60000m,60000m,2700m,3900m,12060m,2070m,9990m,0m,9990m,9990m,0m,60000m,43410m
            };
            SetUpTestResults(testResultValues);

            TestPayrollProcessWithSpec(PayrollSpecs, PayrollResults);
        }

        [Test]
        public void test41_PP_Mzda_DanPoj_SlevyZaklad65()
        {
            object[] testSpecValues = new object[] {
            @"41-PP-Mzda-DanPoj-SlevyZaklad65",201301u,40,0,65000m,(TAX_DECLARED),1u,1u,1u,0u,1u,0u,0u,0u,0u,0u,1u,1u
            };
            SetUpTestSpecs(testSpecValues);

            object[] testResultValues = new object[] {
            @"41-PP-Mzda-DanPoj-SlevyZaklad65",65000m,22100m,87100m,65000m,65000m,2925m,4225m,13065m,2070m,10995m,0m,10995m,10995m,0m,65000m,46855m
            };
            SetUpTestResults(testResultValues);

            TestPayrollProcessWithSpec(PayrollSpecs, PayrollResults);
        }

        [Test]
        public void test42_PP_Mzda_DanPoj_SlevyZaklad70()
        {
            object[] testSpecValues = new object[] {
            @"42-PP-Mzda-DanPoj-SlevyZaklad70",201301u,40,0,70000m,(TAX_DECLARED),1u,1u,1u,0u,1u,0u,0u,0u,0u,0u,1u,1u
            };
            SetUpTestSpecs(testSpecValues);

            object[] testResultValues = new object[] {
            @"42-PP-Mzda-DanPoj-SlevyZaklad70",70000m,23800m,93800m,70000m,70000m,3150m,4550m,14070m,2070m,12000m,0m,12000m,12000m,0m,70000m,50300m
            };
            SetUpTestResults(testResultValues);

            TestPayrollProcessWithSpec(PayrollSpecs, PayrollResults);
        }

        [Test]
        public void test43_PP_Mzda_DanPoj_SlevyZaklad75()
        {
            object[] testSpecValues = new object[] {
            @"43-PP-Mzda-DanPoj-SlevyZaklad75",201301u,40,0,75000m,(TAX_DECLARED),1u,1u,1u,0u,1u,0u,0u,0u,0u,0u,1u,1u
            };
            SetUpTestSpecs(testSpecValues);

            object[] testResultValues = new object[] {
            @"43-PP-Mzda-DanPoj-SlevyZaklad75",75000m,25500m,100500m,75000m,75000m,3375m,4875m,15075m,2070m,13005m,0m,13005m,13005m,0m,75000m,53745m
            };
            SetUpTestResults(testResultValues);

            TestPayrollProcessWithSpec(PayrollSpecs, PayrollResults);
        }

        [Test]
        public void test44_PP_Mzda_DanPoj_SlevyZaklad80()
        {
            object[] testSpecValues = new object[] {
            @"44-PP-Mzda-DanPoj-SlevyZaklad80",201301u,40,0,80000m,(TAX_DECLARED),1u,1u,1u,0u,1u,0u,0u,0u,0u,0u,1u,1u
            };
            SetUpTestSpecs(testSpecValues);

            object[] testResultValues = new object[] {
            @"44-PP-Mzda-DanPoj-SlevyZaklad80",80000m,27200m,107200m,80000m,80000m,3600m,5200m,16080m,2070m,14010m,0m,14010m,14010m,0m,80000m,57190m
            };
            SetUpTestResults(testResultValues);

            TestPayrollProcessWithSpec(PayrollSpecs, PayrollResults);
        }

        [Test]
        public void test45_PP_Mzda_DanPoj_SlevyZaklad85()
        {
            object[] testSpecValues = new object[] {
            @"45-PP-Mzda-DanPoj-SlevyZaklad85",201301u,40,0,85000m,(TAX_DECLARED),1u,1u,1u,0u,1u,0u,0u,0u,0u,0u,1u,1u
            };
            SetUpTestSpecs(testSpecValues);

            object[] testResultValues = new object[] {
            @"45-PP-Mzda-DanPoj-SlevyZaklad85",85000m,28900m,113900m,85000m,85000m,3825m,5525m,17085m,2070m,15015m,0m,15015m,15015m,0m,85000m,60635m
            };
            SetUpTestResults(testResultValues);

            TestPayrollProcessWithSpec(PayrollSpecs, PayrollResults);
        }

        [Test]
        public void test46_PP_Mzda_DanPoj_SlevyZaklad90()
        {
            object[] testSpecValues = new object[] {
            "46-PP-Mzda-DanPoj-SlevyZaklad90",201301u,40,0,90000m,(TAX_DECLARED),1u,1u,1u,0u,1u,0u,0u,0u,0u,0u,1u,1u
            };
            SetUpTestSpecs(testSpecValues);

            object[] testResultValues = new object[] {
            "46-PP-Mzda-DanPoj-SlevyZaklad90",90000m,30600m,120600m,90000m,90000m,4050m,5850m,18090m,2070m,16020m,0m,16020m,16020m,0m,90000m,64080m
            };
            SetUpTestResults(testResultValues);

            TestPayrollProcessWithSpec(PayrollSpecs, PayrollResults);
        }

        [Test]
        public void test47_PP_Mzda_DanPoj_SlevyZaklad95()
        {
            object[] testSpecValues = new object[] {
            @"47-PP-Mzda-DanPoj-SlevyZaklad95",201301u,40,0,95000m,(TAX_DECLARED),1u,1u,1u,0u,1u,0u,0u,0u,0u,0u,1u,1u
            };
            SetUpTestSpecs(testSpecValues);

            object[] testResultValues = new object[] {
            @"47-PP-Mzda-DanPoj-SlevyZaklad95",95000m,32300m,127300m,95000m,95000m,4275m,6175m,19095m,2070m,17025m,0m,17025m,17025m,0m,95000m,67525m
            };
            SetUpTestResults(testResultValues);

            TestPayrollProcessWithSpec(PayrollSpecs, PayrollResults);
        }

        [Test]
        public void test48_PP_Mzda_DanPoj_SlevyZaklad100()
        {
            object[] testSpecValues = new object[] {
            @"48-PP-Mzda-DanPoj-SlevyZaklad100",201301u,40,0,100000m,(TAX_DECLARED),1u,1u,1u,0u,1u,0u,0u,0u,0u,0u,1u,1u
            };
            SetUpTestSpecs(testSpecValues);

            object[] testResultValues = new object[] {
            @"48-PP-Mzda-DanPoj-SlevyZaklad100",100000m,34000m,134000m,100000m,100000m,4500m,6500m,20100m,2070m,18030m,0m,18030m,18030m,0m,100000m,70970m
            };
            SetUpTestResults(testResultValues);

            TestPayrollProcessWithSpec(PayrollSpecs, PayrollResults);
        }

        [Test]
        public void test49_PP_Mzda_DanPoj_SlevyZaklad105()
        {
            object[] testSpecValues = new object[] {
            @"49-PP-Mzda-DanPoj-SlevyZaklad105",201301u,40,0,105000m,(TAX_DECLARED),1u,1u,1u,0u,1u,0u,0u,0u,0u,0u,1u,1u
            };
            SetUpTestSpecs(testSpecValues);

            object[] testResultValues = new object[] {
            @"49-PP-Mzda-DanPoj-SlevyZaklad105",105000m,35700m,140700m,105000m,105000m,4725m,6825m,21208m,2070m,19138m,0m,19138m,19138m,0m,105000m,74312m
            };
            SetUpTestResults(testResultValues);

            TestPayrollProcessWithSpec(PayrollSpecs, PayrollResults);
        }

        [Test]
        public void test50_PP_Mzda_DanPoj_SlevyZaklad110()
        {
            object[] testSpecValues = new object[] {
            @"50-PP-Mzda-DanPoj-SlevyZaklad110",201301u,40,0,110000m,(TAX_DECLARED),1u,1u,1u,0u,1u,0u,0u,0u,0u,0u,1u,1u
            };
            SetUpTestSpecs(testSpecValues);

            object[] testResultValues = new object[] {
            @"50-PP-Mzda-DanPoj-SlevyZaklad110",110000m,37400m,147400m,110000m,110000m,4950m,7150m,22563m,2070m,20493m,0m,20493m,20493m,0m,110000m,77407m
            };
            SetUpTestResults(testResultValues);

            TestPayrollProcessWithSpec(PayrollSpecs, PayrollResults);
        }

//Examples: Employment with Withholding tax
        [Test]
        public void test12_PP_Mzda_NepodPoj_5000()
        {
            object[] testSpecValues = new object[] {
            @"12-PP-Mzda-NepodPoj-5000",201301u,40,0,5000m,(TAX_PAYER),1u,1u,1u,0u,0u,0u,0u,0u,0u,0u,1u,1u
            };
            SetUpTestSpecs(testSpecValues);

            object[] testResultValues = new object[] {
            @"12-PP-Mzda-NepodPoj-5000",5000m,1700m,0m,8000m,5000m,630m,325m,0m,0m,0m,0m,0m,0m,0m,5000m,3040m
            };
            SetUpTestResults(testResultValues);

            TestPayrollProcessWithSpec(PayrollSpecs, PayrollResults);
        }

        [Test]
        public void test13_PP_Mzda_NepodPoj_5001()
        {
            object[] testSpecValues = new object[] {
            @"13-PP-Mzda-NepodPoj-5001",201301u,40,0,5001m,(TAX_PAYER),1u,1u,1u,0u,0u,0u,0u,0u,0u,0u,1u,1u
            };
            SetUpTestSpecs(testSpecValues);

            object[] testResultValues = new object[] {
            @"13-PP-Mzda-NepodPoj-5001",5001m,1701m,6800m,8000m,5001m,630m,326m,1020m,0m,1020m,0m,1020m,1020m,0m,5001m,3025m
            };
            SetUpTestResults(testResultValues);

            TestPayrollProcessWithSpec(PayrollSpecs, PayrollResults);
        }

//Examples: Employment without Minimum Assessment base for Health Insurance
        [Test]
        public void test24_PP_Mzda_DanPoj_Dan099()
        {
            object[] testSpecValues = new object[] {
            @"24-PP-Mzda-DanPoj-Dan099",201301u,40,0,74m,(TAX_DECLARED),1u,0u,1u,0u,1u,0u,0u,0u,0u,0u,1u,1u
            };
            SetUpTestSpecs(testSpecValues);

            object[] testResultValues = new object[] {
            @"24-PP-Mzda-DanPoj-Dan099",74m,25m,99m,74m,74m,4m,5m,15m,15m,0m,0m,0m,0m,0m,74m,65m
            };
            SetUpTestResults(testResultValues);

            TestPayrollProcessWithSpec(PayrollSpecs, PayrollResults);
        }

        [Test]
        public void test25_PP_Mzda_DanPoj_Dan100()
        {
            object[] testSpecValues = new object[] {
            @"25-PP-Mzda-DanPoj-Dan100",201301u,40,0,75m,(TAX_DECLARED),1u,0u,1u,0u,1u,0u,0u,0u,0u,0u,1u,1u
            };
            SetUpTestSpecs(testSpecValues);

            object[] testResultValues = new object[] {
            @"25-PP-Mzda-DanPoj-Dan100",75m,26m,200m,75m,75m,4m,5m,30m,30m,0m,0m,0m,0m,0m,75m,66m
            };
            SetUpTestResults(testResultValues);

            TestPayrollProcessWithSpec(PayrollSpecs, PayrollResults);
        }

        [Test]
        public void test26_PP_Mzda_DanPoj_Dan101()
        {
            object[] testSpecValues = new object[] {
            @"26-PP-Mzda-DanPoj-Dan101",201301u,40,0,100m,(TAX_DECLARED),1u,0u,1u,0u,1u,0u,0u,0u,0u,0u,1u,1u
            };
            SetUpTestSpecs(testSpecValues);

            object[] testResultValues = new object[] {
            @"26-PP-Mzda-DanPoj-Dan101",100m,34m,200m,100m,100m,5m,7m,30m,30m,0m,0m,0m,0m,0m,100m,88m
            };
            SetUpTestResults(testResultValues);

            TestPayrollProcessWithSpec(PayrollSpecs, PayrollResults);
        }

        [Test]
        public void test11_PP_Mzda_DanPoj_Neodpr184()
        {
            object[] testSpecValues = new object[] {
            @"11-PP-Mzda-DanPoj-Neodpr184",201301u,40,184,20000m,(TAX_DECLARED),1u,0u/*health minim SHOULD BE 1*/,1u,0u,1u,0u,0u,0u,0u,0u,1u,1u
            };
            SetUpTestSpecs(testSpecValues);

            object[] testResultValues = new object[] {
            @"11-PP-Mzda-DanPoj-Neodpr184",0m,0m,0m,0m,0m,0m,0m,0m,0m,0m,0m,0m,0m,0m,0m,0m
            };
            SetUpTestResults(testResultValues);

            TestPayrollProcessWithSpec(PayrollSpecs, PayrollResults);
        }
    }
}
