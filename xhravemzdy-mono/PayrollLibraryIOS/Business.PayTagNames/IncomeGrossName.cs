using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Core;

namespace PayrollLibrary.Business.PayTagNames
{
    class IncomeGrossName : PayrollName
    {
        public IncomeGrossName()
            : base(PayTagGateway.REF_UNKNOWN,
                "Gross income", "Gross income",
                PayNameGateway.VPAYGRP_SUMMARY, PayNameGateway.HPAYGRP_UNKNOWN)
        {
        }
    }
}
