using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.Core;
using PayrollLibrary.Business.CoreItems;

namespace PayrollLibrary.Business.PayTags
{
    public class HoursWorkingTag : PayrollTag
    {
        public HoursWorkingTag()
            : base(PayTagGateway.REF_HOURS_WORKING, PayConceptGateway.REFCON_HOURS_WORKING)
        {
        }
    }
}
