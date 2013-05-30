using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Core;

namespace PayrollLibrary.Business.PayTags
{
    class InsuranceSocialTag : PayrollTag
    {
        public InsuranceSocialTag() : base(PayTagGateway.REFInsuranceSocial, PayConceptGateway.REFCON_INSURANCE_SOCIAL)
        {
        }
    }
}
