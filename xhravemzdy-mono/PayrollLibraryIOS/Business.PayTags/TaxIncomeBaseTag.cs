using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.Core;
using PayrollLibrary.Business.CoreItems;

namespace PayrollLibrary.Business.PayTags
{
    public class TaxIncomeBaseTag : PayrollTag
    {
        public TaxIncomeBaseTag()
            : base(PayTagGateway.REF_TAX_INCOME_BASE, PayConceptGateway.REFCON_TAX_INCOME_BASE)
        {
        }
    }
}
