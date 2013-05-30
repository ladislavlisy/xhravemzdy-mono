using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Core;

namespace PayrollLibrary.Business.PayTagNames
{
    class TaxBonusChildName : PayrollName
    {
        public TaxBonusChildName()
            : base(PayTagGateway.REF_UNKNOWN,
                "Tax bonus", "Tax bonus",
                PayNameGateway.VPAYGRP_TAX_RESULT, PayNameGateway.HPAYGRP_UNKNOWN)
        {
        }
    }
}
