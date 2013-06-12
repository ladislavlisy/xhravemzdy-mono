using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Core;

namespace PayrollLibrary.Business.PayTags
{
    class TaxEmployersSocialTag : PayrollTag
    {
        public TaxEmployersSocialTag() : base(PayTagGateway.REF_TAX_EMPLOYERS_SOCIAL, PayConceptGateway.REFCON_TAX_EMPLOYERS_SOCIAL)
        {
        }
    }
}
