using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Core;

namespace PayrollLibrary.Business.PayTags
{
    class TaxAdvanceBaseTag : PayrollTag
    {
        public TaxAdvanceBaseTag() : base(PayTagGateway.REF_TAX_ADVANCE_BASE, PayConceptGateway.REFCON_TAX_ADVANCE_BASE)
        {
        }
    }
}
