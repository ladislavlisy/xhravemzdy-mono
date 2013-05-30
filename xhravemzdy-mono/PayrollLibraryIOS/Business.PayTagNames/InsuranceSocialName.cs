using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Core;

namespace PayrollLibrary.Business.PayTagNames
{
    class InsuranceSocialName : PayrollName
    {
        public InsuranceSocialName()
            : base(PayTagGateway.REF_UNKNOWN,
                "Social insurance", "Social insurance contribution",
                PayNameGateway.VPAYGRP_INS_RESULT, PayNameGateway.HPAYGRP_UNKNOWN)
        {
        }
    }
}
