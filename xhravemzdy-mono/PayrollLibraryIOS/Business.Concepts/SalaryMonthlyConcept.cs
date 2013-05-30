using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Core;
using PayrollLibrary.Business.PayTags;

namespace PayrollLibrary.Business.Concepts
{
    public class SalaryMonthlyConcept : PayrollConcept
    {
        public SalaryMonthlyConcept(uint tagCode, IDictionary<string, object> values)
            : base(PayConceptGateway.REFCON_SALARY_MONTHLY, tagCode)
        {
        }

        public decimal AmountMonthly { get; private set; }

        public override void InitValues(IDictionary<string, object> values)
        {
            this.AmountMonthly = new decimal((int)values["amount_monthly"]);
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
            var resultValues = new Dictionary<string, object>() { { "", 0 } };
            return new PayrollResult(TagCode, Code, this, resultValues);
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
