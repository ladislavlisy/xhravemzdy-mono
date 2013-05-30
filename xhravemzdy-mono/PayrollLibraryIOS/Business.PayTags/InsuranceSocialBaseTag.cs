using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.Core;
using PayrollLibrary.Business.CoreItems;

namespace PayrollLibrary.Business.PayTags
{
    public class InsuranceSocialBaseTag : PayrollTag
    {
        public InsuranceSocialBaseTag()
            : base(PayTagGateway.REF_INSURANCE_SOCIAL_BASE, PayConceptGateway.REFCON_INSURANCE_SOCIAL_BASE)
        {
        }
    }
}
