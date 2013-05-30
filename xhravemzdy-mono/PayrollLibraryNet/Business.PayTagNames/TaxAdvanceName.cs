using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Core;

namespace PayrollLibrary.Business.PayTagNames
{
    class TaxAdvanceName : PayrollName
    {
        public TaxAdvanceName()
            : base(PayTagGateway.REF_UNKNOWN,
                "Tax advance", "Tax advance",
                PayNameGateway.VPAYGRP_TAX_SOURCE, PayNameGateway.HPAYGRP_UNKNOWN)
        {
        }
    }
}
