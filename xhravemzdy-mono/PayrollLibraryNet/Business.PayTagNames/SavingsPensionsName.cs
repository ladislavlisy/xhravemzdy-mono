using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Core;

namespace PayrollLibrary.Business.PayTagNames
{
    class SavingsPensionsName : PayrollName
    {
        public SavingsPensionsName()
            : base(PayTagGateway.REF_UNKNOWN,
                "Pension savings", "Pension savings contribution",
                PayNameGateway.VPAYGRP_INS_RESULT, PayNameGateway.HPAYGRP_UNKNOWN)
        {
        }
    }
}
