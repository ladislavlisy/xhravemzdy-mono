using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Core;

namespace PayrollLibrary.Business.PayTagNames
{
    class ScheduleWorkName : PayrollName
    {
        public ScheduleWorkName()
            : base(PayTagGateway.REF_UNKNOWN,
                "Working schedule", "Working schedule",
                PayNameGateway.VPAYGRP_SCHEDULE, PayNameGateway.HPAYGRP_UNKNOWN)
        {
        }
    }
}
