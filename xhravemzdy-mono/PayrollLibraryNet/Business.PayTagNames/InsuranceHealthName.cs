using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Core;

namespace PayrollLibrary.Business.PayTagNames
{
    class InsuranceHealthName : PayrollName
    {
        public InsuranceHealthName()
            : base(PayTagGateway.REF_UNKNOWN,
                "Health insurance", "Health insurance contribution",
                PayNameGateway.VPAYGRP_INS_RESULT, PayNameGateway.HPAYGRP_UNKNOWN)
        {
        }
    }
}
