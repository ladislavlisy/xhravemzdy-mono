using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Core;

namespace PayrollLibrary.Business.PayTagNames
{
    class TaxClaimPayerName : PayrollName
    {
        public TaxClaimPayerName()
            : base(PayTagGateway.REF_UNKNOWN,
                "Tax benefit claim - payer", "Tax benefit claim - payer",
                PayNameGateway.VPAYGRP_TAX_SOURCE, PayNameGateway.HPAYGRP_UNKNOWN)
        {
        }
    }
}
