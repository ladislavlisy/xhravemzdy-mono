using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Core;

namespace PayrollLibrary.Business.PayTagNames
{
    class HoursAbsenceName : PayrollName
    {
        public HoursAbsenceName()
            : base(PayTagGateway.REF_UNKNOWN,
                "Absence hours", "Absence hours",
                PayNameGateway.VPAYGRP_SCHEDULE, PayNameGateway.HPAYGRP_UNKNOWN)
        {
        }
    }
}
