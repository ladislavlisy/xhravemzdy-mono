using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Core;

namespace PayrollLibrary.Business.PayTagNames
{
    class TaxEmployersSocialName : PayrollName
    {
        public TaxEmployersSocialName()
            : base(PayTagGateway.REF_UNKNOWN,
                "", "",
                PayNameGateway.VPAYGRP_TAX_INCOME, PayNameGateway.HPAYGRP_UNKNOWN)
        {
        }
    }
}
