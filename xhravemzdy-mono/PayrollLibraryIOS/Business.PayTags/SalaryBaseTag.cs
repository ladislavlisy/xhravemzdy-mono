using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Core;

namespace PayrollLibrary.Business.PayTags
{
    public class SalaryBaseTag : PayrollTag
    {
        public SalaryBaseTag()
            : base(PayTagGateway.REF_SALARY_BASE, PayConceptGateway.REFCON_SALARY_MONTHLY)
        {
        }
        public override bool InsuranceHealth() { return true; }

        public override bool InsuranceSocial() { return true; }

        public override bool TaxAdvance() { return true; }

        public override bool IncomeGross() { return true; }

        public override bool IncomeNetto() { return true; }

    }
}
