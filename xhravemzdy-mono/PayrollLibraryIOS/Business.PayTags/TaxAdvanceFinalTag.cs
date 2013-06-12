using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Core;

namespace PayrollLibrary.Business.PayTags
{
    class TaxAdvanceFinalTag : PayrollTag
    {
        public TaxAdvanceFinalTag() : base(PayTagGateway.REF_TAX_ADVANCE_FINAL, PayConceptGateway.REFCON_TAX_ADVANCE_FINAL)
        {
        }

        public override bool DeductionNetto() { return true; }

    }
}
