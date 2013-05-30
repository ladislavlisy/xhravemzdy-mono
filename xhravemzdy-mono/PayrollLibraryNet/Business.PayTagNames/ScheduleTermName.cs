using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Core;

namespace PayrollLibrary.Business.PayTagNames
{
    class ScheduleTermName : PayrollName
    {
        public ScheduleTermName()
            : base(PayTagGateway.REF_UNKNOWN,
                "Working Schedule Terms", "Working Schedule Terms",
                PayNameGateway.VPAYGRP_SCHEDULE, PayNameGateway.HPAYGRP_UNKNOWN)
        {
        }
    }
}
