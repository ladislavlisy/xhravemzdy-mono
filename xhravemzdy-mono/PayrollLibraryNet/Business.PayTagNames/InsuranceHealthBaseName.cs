using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Core;

namespace PayrollLibrary.Business.PayTagNames
{
    class InsuranceHealthBaseName : PayrollName
    {
        public InsuranceHealthBaseName()
            : base(PayTagGateway.REF_UNKNOWN,
                "Health insurance base", "Assessment base for Health insurance",
                PayNameGateway.VPAYGRP_INS_INCOME, PayNameGateway.HPAYGRP_UNKNOWN)
        {
        }
    }
}
