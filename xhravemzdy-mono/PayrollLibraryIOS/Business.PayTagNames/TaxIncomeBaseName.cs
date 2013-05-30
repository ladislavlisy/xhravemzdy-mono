using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Core;

namespace PayrollLibrary.Business.PayTagNames
{
    class TaxIncomeBaseName : PayrollName
    {
        public TaxIncomeBaseName()
            : base(PayTagGateway.REF_UNKNOWN,
                "Taxable income", "Taxable income",
                PayNameGateway.VPAYGRP_TAX_INCOME, PayNameGateway.HPAYGRP_UNKNOWN)
        {
        }
    }
}
