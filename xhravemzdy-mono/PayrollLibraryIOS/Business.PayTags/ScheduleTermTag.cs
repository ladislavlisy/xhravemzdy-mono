using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Core;

namespace PayrollLibrary.Business.PayTags
{
    class ScheduleTermTag : PayrollTag
    {
        public ScheduleTermTag() : base(PayTagGateway.REF_SCHEDULE_TERM, PayConceptGateway.REFCON_SCHEDULE_TERM)
        {
        }
    }
}
