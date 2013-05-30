using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Core;

namespace PayrollLibrary.Business.PayTagNames
{
    class TaxClaimStudyingName : PayrollName
    {
        public TaxClaimStudyingName()
            : base(PayTagGateway.REF_UNKNOWN,
                "Tax benefit claim - studying", "Tax benefit claim - studying",
                PayNameGateway.VPAYGRP_TAX_SOURCE, PayNameGateway.HPAYGRP_UNKNOWN)
        {
        }
    }
}
