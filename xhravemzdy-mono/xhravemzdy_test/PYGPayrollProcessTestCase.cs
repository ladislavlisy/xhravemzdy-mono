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
	public class PYGPayrollProcessTestCase
	{
		public PayrollProcess PayProcess  { get; set; }

		public PayrollPeriod PayPeriod { get; set; } 

		public PayTagGateway PayTags { get; set; }

		public PayConceptGateway PayConcepts { get; set; }

        public CodeNameRefer REF_SCHEDULE_WORK         ;
        public CodeNameRefer REF_SCHEDULE_TERM         ;
        public CodeNameRefer REF_HOURS_ABSENCE         ;
        public CodeNameRefer REF_SALARY_BASE           ;
        public CodeNameRefer REF_TAX_INCOME_BASE       ;
        public CodeNameRefer REF_INSURANCE_HEALTH_BASE ;
        public CodeNameRefer REF_INSURANCE_HEALTH      ;
        public CodeNameRefer REF_INSURANCE_SOCIAL_BASE ;
        public CodeNameRefer REF_INSURANCE_SOCIAL      ;
        public CodeNameRefer REF_SAVINGS_PENSIONS      ;
        public CodeNameRefer REF_TAX_CLAIM_PAYER       ;
        public CodeNameRefer REF_TAX_CLAIM_CHILD       ;
        public CodeNameRefer REF_TAX_CLAIM_DISABILITY  ;
        public CodeNameRefer REF_TAX_CLAIM_STUDYING    ;
        public CodeNameRefer REF_TAX_EMPLOYERS_HEALTH  ;
        public CodeNameRefer REF_TAX_EMPLOYERS_SOCIAL  ;
        public CodeNameRefer REF_TAX_ADVANCE_BASE      ;
        public CodeNameRefer REF_TAX_ADVANCE           ;
        public CodeNameRefer REF_TAX_WITHHOLD_BASE     ;
        public CodeNameRefer REF_TAX_WITHHOLD          ;
        public CodeNameRefer REF_TAX_RELIEF_PAYER      ;
        public CodeNameRefer REF_TAX_ADVANCE_FINAL     ;
        public CodeNameRefer REF_TAX_RELIEF_CHILD      ;
        public CodeNameRefer REF_TAX_BONUS_CHILD       ;
        public CodeNameRefer REF_INCOME_GROSS          ;
        public CodeNameRefer REF_INCOME_NETTO;
        
        public uint TAX_PAYER;
        public uint TAX_DECLARED;

		public PYGPayrollProcessTestCase()
		{
		}

        public decimal GetResultIncomeBase(CodeNameRefer result_ref)
        {
            IDictionary<TagRefer, PayrollResult> results = PayProcess.GetResults();

            var result_select = results.Where((x) => (x.Key.Code == result_ref.Code)).ToDictionary(key => key.Key, value => value.Value);
            var result_value = result_select.Aggregate(0m, (agr, item) => (agr + item.Value.IncomeBase()));
            return (decimal)result_value;
        }

        public decimal GetResultEmployeeBase(CodeNameRefer result_ref)
        {
            IDictionary<TagRefer, PayrollResult> results = PayProcess.GetResults();

            var result_select = results.Where((x) => (x.Key.Code == result_ref.Code)).ToDictionary(key => key.Key, value => value.Value);
            var result_value = result_select.Aggregate(0m, (agr, item) => (agr + item.Value.EmployeeBase()));
            return (decimal)result_value;
        }

        public decimal GetResultPayment(CodeNameRefer result_ref)
        {
            IDictionary<TagRefer, PayrollResult> results = PayProcess.GetResults();

            var result_select = results.Where((x) => (x.Key.Code == result_ref.Code)).ToDictionary(key => key.Key, value => value.Value);
            var result_value = result_select.Aggregate(0m, (agr, item) => (agr + item.Value.Payment()));
            return (decimal)result_value;
        }

        public decimal GetResultAfterReliefA(CodeNameRefer result_ref)
        {
            IDictionary<TagRefer, PayrollResult> results = PayProcess.GetResults();

            var result_select = results.Where((x) => (x.Key.Code == result_ref.Code)).ToDictionary(key => key.Key, value => value.Value);
            var result_value = result_select.Aggregate(0m, (agr, item) => (agr + item.Value.AfterReliefA()));
            return (decimal)result_value;
        }

        public decimal GetResultAfterReliefC(CodeNameRefer result_ref)
        {
            IDictionary<TagRefer, PayrollResult> results = PayProcess.GetResults();

            var result_select = results.Where((x) => (x.Key.Code == result_ref.Code)).ToDictionary(key => key.Key, value => value.Value);
            var result_value = result_select.Aggregate(0m, (agr, item) => (agr + item.Value.AfterReliefC()));
            return (decimal)result_value;
        }

        public decimal GetResultTaxRelief(CodeNameRefer result_ref)
        {
            IDictionary<TagRefer, PayrollResult> results = PayProcess.GetResults();

            var result_select = results.Where((x) => (x.Key.Code == result_ref.Code)).ToDictionary(key => key.Key, value => value.Value);
            var result_value = result_select.Aggregate(0m, (agr, item) => (agr + item.Value.TaxRelief()));
            return (decimal)result_value;
        }

        public decimal GetResultAmount(CodeNameRefer result_ref)
        {
            IDictionary<TagRefer, PayrollResult> results = PayProcess.GetResults();

            var result_select = results.Where((x) => (x.Key.Code == result_ref.Code)).ToDictionary(key => key.Key, value => value.Value);
            var result_value = result_select.Aggregate(0m, (agr, item) => (agr + item.Value.Amount()));
            return (decimal)result_value;
        }

        public IDictionary<TagRefer, PayrollResult> GetResultsDictionary(IDictionary<TagRefer, PayrollResult> results, uint tagCode)
        {
            var result_select = results.Where((x) => (x.Key.Code == tagCode)).ToDictionary(key => key.Key, value => value.Value);
            return result_select;
        }

        [SetUp]
		public virtual void Init()
		{
			PayPeriod = new PayrollPeriod(2013, 1);

			PayTags = new PayTagGateway();

			PayConcepts = new PayConceptGateway();

			PayProcess = new PayrollProcess(PayTags, PayConcepts, PayPeriod);
 
            REF_SCHEDULE_WORK          = PayTagGateway.REF_SCHEDULE_WORK;
            REF_SCHEDULE_TERM          = PayTagGateway.REF_SCHEDULE_TERM;
            REF_HOURS_ABSENCE          = PayTagGateway.REF_HOURS_ABSENCE;
            REF_SALARY_BASE            = PayTagGateway.REF_SALARY_BASE;
            REF_TAX_INCOME_BASE        = PayTagGateway.REF_TAX_INCOME_BASE;
            REF_INSURANCE_HEALTH_BASE  = PayTagGateway.REF_INSURANCE_HEALTH_BASE;
            REF_INSURANCE_HEALTH       = PayTagGateway.REF_INSURANCE_HEALTH;
            REF_INSURANCE_SOCIAL_BASE  = PayTagGateway.REF_INSURANCE_SOCIAL_BASE;
            REF_INSURANCE_SOCIAL       = PayTagGateway.REF_INSURANCE_SOCIAL;
            REF_SAVINGS_PENSIONS       = PayTagGateway.REF_SAVINGS_PENSIONS;
            REF_TAX_CLAIM_PAYER        = PayTagGateway.REF_TAX_CLAIM_PAYER;
            REF_TAX_CLAIM_CHILD        = PayTagGateway.REF_TAX_CLAIM_CHILD;
            REF_TAX_CLAIM_DISABILITY   = PayTagGateway.REF_TAX_CLAIM_DISABILITY;
            REF_TAX_CLAIM_STUDYING     = PayTagGateway.REF_TAX_CLAIM_STUDYING;
            REF_TAX_EMPLOYERS_HEALTH   = PayTagGateway.REF_TAX_EMPLOYERS_HEALTH;
            REF_TAX_EMPLOYERS_SOCIAL   = PayTagGateway.REF_TAX_EMPLOYERS_SOCIAL;
            REF_TAX_ADVANCE_BASE       = PayTagGateway.REF_TAX_ADVANCE_BASE;
            REF_TAX_ADVANCE            = PayTagGateway.REF_TAX_ADVANCE;
            REF_TAX_WITHHOLD_BASE      = PayTagGateway.REF_TAX_WITHHOLD_BASE;
            REF_TAX_WITHHOLD           = PayTagGateway.REF_TAX_WITHHOLD;
            REF_TAX_RELIEF_PAYER       = PayTagGateway.REF_TAX_RELIEF_PAYER;
            REF_TAX_ADVANCE_FINAL      = PayTagGateway.REF_TAX_ADVANCE_FINAL;
            REF_TAX_RELIEF_CHILD       = PayTagGateway.REF_TAX_RELIEF_CHILD;
            REF_TAX_BONUS_CHILD        = PayTagGateway.REF_TAX_BONUS_CHILD;
            REF_INCOME_GROSS           = PayTagGateway.REF_INCOME_GROSS;
            REF_INCOME_NETTO           = PayTagGateway.REF_INCOME_NETTO;
            TAX_PAYER                  = 1u;
            TAX_DECLARED               = 3u;
        }

        public string STRTYPE(object obj)
        {
            return (string)obj;
        }

        public uint U_UNBOX(object obj)
        {
            return (uint)obj;
        }

        public int I_UNBOX(object obj)
        {
            return (int)obj;
        }

        public decimal NUMTYPE(object obj)
        {
            return (decimal)obj;
        }

        public Array A_GET_FROM(IDictionary<string, object> valDict, string valSymbol)
        {
            return (Array)valDict[valSymbol];
        }

        public string S_GET_FROM(IDictionary<string, object> valDict, string valSymbol)
        {
            return (string)valDict[valSymbol];
        }
        public decimal D_GET_FROM(IDictionary<string, object> valDict, string valSymbol)
        {
            return (decimal)valDict[valSymbol];
        }
        public int I_GET_FROM(IDictionary<string, object> valDict, string valSymbol)
        {
            return (int)valDict[valSymbol];
        }
        public uint U_GET_FROM(IDictionary<string, object> valDict, string valSymbol)
        {
            return (uint)valDict[valSymbol];
        }
        public uint B_GET_FROM(IDictionary<string, object> valDict, string valSymbol)
        {
            return (uint)valDict[valSymbol];
        }
        public DateTime DT_GET_FROM(IDictionary<string, object> valDict, string valSymbol)
        {
            return (DateTime)valDict[valSymbol];
        }

        public Dictionary<string, object>  A_MAKE_HASH(string symbol, object value)
        {
            return new Dictionary<string, object>() { { symbol,value } };
        }

        public Dictionary<string, object>  S_MAKE_HASH(string symbol, object value)
        {
            return new Dictionary<string, object>() { { symbol,value } };
        }
        public Dictionary<string, object>  D_MAKE_HASH(string symbol, object value)
        {
            return new Dictionary<string, object>() { { symbol,value } };
        }
        public Dictionary<string, object>  I_MAKE_HASH(string symbol, object value)
        {
            return new Dictionary<string, object>() { { symbol,value } };
        }
        public Dictionary<string, object>  U_MAKE_HASH(string symbol, object value)
        {
            return new Dictionary<string, object>() { { symbol,value } };
        }
        public Dictionary<string, object>  B_MAKE_HASH(string symbol, object value)
        {
            return new Dictionary<string, object>() { { symbol,value } };
        }
    }
}
