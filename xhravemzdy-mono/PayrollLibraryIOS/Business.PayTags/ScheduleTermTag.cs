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
        public ScheduleTermTag() : base(PayTagGateway.REFScheduleTerm, PayConceptGateway.REFCON_SCHEDULE_TERM)
        {
        }
    }
}
