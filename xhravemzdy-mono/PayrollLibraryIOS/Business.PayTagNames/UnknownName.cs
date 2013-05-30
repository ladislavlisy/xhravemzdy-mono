using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Core;

namespace PayrollLibrary.Business.PayTagNames
{
    class UnknownName : PayrollName
    {
        public UnknownName()
            : base(PayTagGateway.REF_UNKNOWN,
                "Unknown", "Unknown",
                PayNameGateway.VPAYGRP_UNKNOWN, PayNameGateway.HPAYGRP_UNKNOWN)
        {
        }
    }
}
