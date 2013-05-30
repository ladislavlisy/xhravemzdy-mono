using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Core;

namespace PayrollLibrary.Business.PayTagNames
{
    class SalaryBaseName : PayrollName
    {
        public SalaryBaseName()
            : base(PayTagGateway.REF_UNKNOWN,
                "Base Salary", "Base Salary",
                PayNameGateway.VPAYGRP_PAYMENTS, PayNameGateway.HPAYGRP_UNKNOWN)
        {
        }
    }
}
