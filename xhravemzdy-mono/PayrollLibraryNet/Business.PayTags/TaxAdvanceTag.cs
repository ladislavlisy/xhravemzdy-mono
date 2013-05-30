using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Core;

namespace PayrollLibrary.Business.PayTags
{
    class TaxAdvanceTag : PayrollTag
    {
        public TaxAdvanceTag() : base(PayTagGateway.REFTaxAdvance, PayConceptGateway.REFCON_TAX_ADVANCE)
        {
        }
    }
}
