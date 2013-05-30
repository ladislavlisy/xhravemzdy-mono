using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Core;

namespace PayrollLibrary.Business.PayTagNames
{
    class TaxClaimDisabilityName : PayrollName
    {
        public TaxClaimDisabilityName()
            : base(PayTagGateway.REF_UNKNOWN,
                "Tax benefit claim - disability", "Tax benefit claim - disability",
                PayNameGateway.VPAYGRP_TAX_SOURCE, PayNameGateway.HPAYGRP_UNKNOWN)
        {
        }
    }
}
