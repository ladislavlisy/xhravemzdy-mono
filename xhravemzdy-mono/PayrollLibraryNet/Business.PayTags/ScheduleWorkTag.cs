using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Core;

namespace PayrollLibrary.Business.PayTags
{
    public class ScheduleWorkTag : PayrollTag
    {
        public ScheduleWorkTag()
            : base(PayTagGateway.REF_SCHEDULE_WORK, PayConceptGateway.REFCON_SCHEDULE_WEEKLY)
        {
        }
    }
}
