using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Core;

namespace PayrollLibrary.Business.PayTagNames
{
    class TaxWithholdName : PayrollName
    {
        public TaxWithholdName()
            : base(PayTagGateway.REF_UNKNOWN,
                "Withholding Tax", "Withholding Tax",
                PayNameGateway.VPAYGRP_TAX_RESULT, PayNameGateway.HPAYGRP_UNKNOWN)
        {
        }
    }
}
