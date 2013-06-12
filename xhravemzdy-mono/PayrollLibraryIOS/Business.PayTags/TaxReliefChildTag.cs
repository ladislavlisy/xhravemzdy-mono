using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Core;

namespace PayrollLibrary.Business.PayTags
{
    class TaxReliefChildTag : PayrollTag
    {
        public TaxReliefChildTag() : base(PayTagGateway.REF_TAX_RELIEF_CHILD, PayConceptGateway.REFCON_TAX_RELIEF_CHILD)
        {
        }
    }
}
