using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Core;

namespace PayrollLibrary.Business.PayTags
{
    public class UnknownTag : PayrollTag
    {
        public UnknownTag()
            : base(PayTagGateway.REF_UNKNOWN, PayConceptGateway.REFCON_UNKNOWN)
        {
        }
    }
}
