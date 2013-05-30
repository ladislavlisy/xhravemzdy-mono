using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Core;

namespace PayrollLibrary.Business.PayTags
{
    class TaxWithholdBaseTag : PayrollTag
    {
        public TaxWithholdBaseTag() : base(PayTagGateway.REFTaxWithholdBase, PayConceptGateway.REFCON_TAX_WITHHOLD_BASE)
        {
        }
    }
}
