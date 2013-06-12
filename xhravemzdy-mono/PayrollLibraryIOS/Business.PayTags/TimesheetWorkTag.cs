using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Core;

namespace PayrollLibrary.Business.PayTags
{
    class TimesheetWorkTag : PayrollTag
    {
        public TimesheetWorkTag() : base(PayTagGateway.REF_TIMESHEET_WORK, PayConceptGateway.REFCON_TIMESHEET_WORK)
        {
        }
    }
}
