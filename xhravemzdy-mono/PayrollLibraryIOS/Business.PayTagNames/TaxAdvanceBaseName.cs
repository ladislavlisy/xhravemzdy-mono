using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Core;

namespace PayrollLibrary.Business.PayTagNames
{
    class TaxAdvanceBaseName : PayrollName
    {
        public TaxAdvanceBaseName()
            : base(PayTagGateway.REF_UNKNOWN,
                "Tax advance base", "Tax advance base",
                PayNameGateway.VPAYGRP_TAX_INCOME, PayNameGateway.HPAYGRP_UNKNOWN)
        {
        }
    }
}
