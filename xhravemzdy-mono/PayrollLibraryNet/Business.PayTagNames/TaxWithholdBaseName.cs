using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Core;

namespace PayrollLibrary.Business.PayTagNames
{
    class TaxWithholdBaseName : PayrollName
    {
        public TaxWithholdBaseName()
            : base(PayTagGateway.REF_UNKNOWN,
                "Withholding Tax base", "Withholding Tax base",
                PayNameGateway.VPAYGRP_TAX_INCOME, PayNameGateway.HPAYGRP_UNKNOWN)
        {
        }
    }
}
