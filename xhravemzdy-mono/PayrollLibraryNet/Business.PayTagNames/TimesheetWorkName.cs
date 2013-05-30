using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Core;

namespace PayrollLibrary.Business.PayTagNames
{
    class TimesheetWorkName : PayrollName
    {
        public TimesheetWorkName()
            : base(PayTagGateway.REF_UNKNOWN,
                "Working Timesheet hours", "Working Timesheet hours",
                PayNameGateway.VPAYGRP_SCHEDULE, PayNameGateway.HPAYGRP_UNKNOWN)
        {
        }
    }
}
