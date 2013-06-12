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
        public InsuranceHealthTag() : base(PayTagGateway.REF_INSURANCE_HEALTH, PayConceptGateway.REFCON_INSURANCE_HEALTH)
        {
        }

        public override bool DeductionNetto() { return true; }
    }
}
