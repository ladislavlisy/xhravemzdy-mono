using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Core;

namespace PayrollLibrary.Business.PayTags
{
    class TimesheetPeriodTag : PayrollTag
    {
        public TimesheetPeriodTag() : base(PayTagGateway.REF_TIMESHEET_PERIOD, PayConceptGateway.REFCON_TIMESHEET_PERIOD)
        {
        }
    }
}
