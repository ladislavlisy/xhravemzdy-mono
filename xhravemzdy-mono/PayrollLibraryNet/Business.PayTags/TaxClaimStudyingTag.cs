using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Core;

namespace PayrollLibrary.Business.PayTags
{
    class TaxClaimStudyingTag : PayrollTag
    {
        public TaxClaimStudyingTag() : base(PayTagGateway.REF_TAX_CLAIM_STUDYING, PayConceptGateway.REFCON_TAX_CLAIM_STUDYING)
        {
        }
    }
}
