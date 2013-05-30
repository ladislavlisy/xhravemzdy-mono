using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Core;

namespace PayrollLibrary.Business.PayTagNames
{
    class HoursWorkingName : PayrollName
    {
        public HoursWorkingName()
            : base(PayTagGateway.REF_UNKNOWN,
                "Working hours", "Working hours",
                PayNameGateway.VPAYGRP_SCHEDULE, PayNameGateway.HPAYGRP_UNKNOWN)
        {
        }
    }
}
