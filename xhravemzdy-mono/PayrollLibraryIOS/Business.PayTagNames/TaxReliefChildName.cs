using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Core;

namespace PayrollLibrary.Business.PayTagNames
{
    class TaxReliefChildName : PayrollName
    {
        public TaxReliefChildName()
            : base(PayTagGateway.REF_UNKNOWN,
                "Tax relief - child", "Tax relief - child (§ 35c)",
                PayNameGateway.VPAYGRP_TAX_SOURCE, PayNameGateway.HPAYGRP_UNKNOWN)
        {
        }
    }
}
