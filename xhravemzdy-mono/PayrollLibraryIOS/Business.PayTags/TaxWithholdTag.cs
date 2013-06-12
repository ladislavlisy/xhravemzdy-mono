using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Core;

namespace PayrollLibrary.Business.PayTags
{
    class TaxWithholdTag : PayrollTag
    {
        public TaxWithholdTag() : base(PayTagGateway.REF_TAX_WITHHOLD, PayConceptGateway.REFCON_TAX_WITHHOLD)
        {
        }

        public override bool DeductionNetto() { return true; }

    }
}
