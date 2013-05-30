using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Core;

namespace PayrollLibrary.Business.PayTagNames
{
    class TaxAdvanceFinalName : PayrollName
    {
        public TaxAdvanceFinalName()
            : base(PayTagGateway.REF_UNKNOWN,
                "Tax advance after relief", "Tax advance after relief",
                PayNameGateway.VPAYGRP_TAX_RESULT, PayNameGateway.HPAYGRP_UNKNOWN)
        {
        }
    }
}
