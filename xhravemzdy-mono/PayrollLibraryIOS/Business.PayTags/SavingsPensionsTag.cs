using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Core;

namespace PayrollLibrary.Business.PayTags
{
    class SavingsPensionsTag : PayrollTag
    {
        public SavingsPensionsTag() : base(PayTagGateway.REF_SAVINGS_PENSIONS, PayConceptGateway.REFCON_SAVINGS_PENSIONS)
        {
        }

        public override bool DeductionNetto() { return true; }

    }
}
