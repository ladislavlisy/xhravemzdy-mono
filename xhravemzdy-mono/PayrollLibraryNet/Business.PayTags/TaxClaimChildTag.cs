using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Core;

namespace PayrollLibrary.Business.PayTags
{
    class TaxClaimChildTag : PayrollTag
    {
        public TaxClaimChildTag() : base(PayTagGateway.REF_TAX_CLAIM_CHILD, PayConceptGateway.REFCON_TAX_CLAIM_CHILD)
        {
        }
    }
}
