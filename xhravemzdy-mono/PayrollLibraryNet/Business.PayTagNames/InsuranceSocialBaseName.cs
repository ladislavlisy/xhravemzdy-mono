using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Core;

namespace PayrollLibrary.Business.PayTagNames
{
    class InsuranceSocialBaseName : PayrollName
    {
        public InsuranceSocialBaseName()
            : base(PayTagGateway.REF_UNKNOWN,
                "Social insurance base", "Assessment base for Social insurance",
                PayNameGateway.VPAYGRP_INS_INCOME, PayNameGateway.HPAYGRP_UNKNOWN)
        {
        }
    }
}
