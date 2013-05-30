using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.Core;
using PayrollLibrary.Business.CoreItems;

namespace PayrollLibrary.Business.PayTags
{
    public class InsuranceHealthBaseTag : PayrollTag
    {
        public InsuranceHealthBaseTag()
            : base(PayTagGateway.REF_INSURANCE_HEALTH_BASE, PayConceptGateway.REFCON_INSURANCE_HEALTH_BASE)
        {
        }
    }
}
