using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Core;

namespace PayrollLibrary.Business.PayTags
{
    class TaxEmployersHealthTag : PayrollTag
    {
        public TaxEmployersHealthTag() : base(PayTagGateway.REFTaxEmployersHealth, PayConceptGateway.REFCON_TAX_EMPLOYERS_HEALTH)
        {
        }
    }
}
