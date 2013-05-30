using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Core;

namespace PayrollLibrary.Business.PayTagNames
{
    class TaxReliefPayerName : PayrollName
    {
        public TaxReliefPayerName()
            : base(PayTagGateway.REF_UNKNOWN,
                "Tax relief - payer", "Tax relief - payer (§ 35ba)",
                PayNameGateway.VPAYGRP_TAX_SOURCE, PayNameGateway.HPAYGRP_UNKNOWN)
        {
        }
    }
}
