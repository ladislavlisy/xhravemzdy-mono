using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Core;

namespace PayrollLibrary.Business.PayTagNames
{
    class IncomeNettoName : PayrollName
    {
        public IncomeNettoName()
            : base(PayTagGateway.REF_UNKNOWN,
                "Net income", "Net income",
                PayNameGateway.VPAYGRP_SUMMARY, PayNameGateway.HPAYGRP_UNKNOWN)
        {
        }
    }
}
