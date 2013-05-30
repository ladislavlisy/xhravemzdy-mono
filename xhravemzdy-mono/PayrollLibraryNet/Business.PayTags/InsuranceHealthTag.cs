using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Core;

namespace PayrollLibrary.Business.PayTags
{
    class InsuranceHealthTag : PayrollTag
    {
        public InsuranceHealthTag() : base(PayTagGateway.REFInsuranceHealth, PayConceptGateway.REFCON_INSURANCE_HEALTH)
        {
        }
    }
}
