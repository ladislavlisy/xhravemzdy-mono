using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Core;

namespace PayrollLibrary.Business.PayTags
{
    class TaxReliefPayerTag : PayrollTag
    {
        public TaxReliefPayerTag() : base(PayTagGateway.REF_TAX_RELIEF_PAYER, PayConceptGateway.REFCON_TAX_RELIEF_PAYER)
        {
        }
    }
}
