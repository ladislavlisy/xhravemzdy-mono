using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Core;

namespace PayrollLibrary.Business.PayTags
{
    class TaxBonusChildTag : PayrollTag
    {
        public TaxBonusChildTag() : base(PayTagGateway.REFTaxBonusChild, PayConceptGateway.REFCON_TAX_BONUS_CHILD)
        {
        }
    }
}
