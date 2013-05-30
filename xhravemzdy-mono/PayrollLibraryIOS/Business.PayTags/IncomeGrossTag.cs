using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.Core;
using PayrollLibrary.Business.CoreItems;

namespace PayrollLibrary.Business.PayTags
{
    public class IncomeGrossTag : PayrollTag
    {
        public IncomeGrossTag()
            : base(PayTagGateway.REF_INCOME_GROSS, PayConceptGateway.REFCON_INCOME_GROSS)
        {
        }
    }
}
