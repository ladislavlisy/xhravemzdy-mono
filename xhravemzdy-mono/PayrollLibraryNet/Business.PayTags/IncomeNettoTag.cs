using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.Core;
using PayrollLibrary.Business.CoreItems;

namespace PayrollLibrary.Business.PayTags
{
    public class IncomeNettoTag : PayrollTag
    {
        public IncomeNettoTag()
            : base(PayTagGateway.REF_INCOME_NETTO, PayConceptGateway.REFCON_INCOME_NETTO)
        {
        }
    }
}
