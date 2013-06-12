using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;

namespace PayrollLibrary.Business.Results
{
    public class PaymentDeductionResult : PaymentResult
    {
        public PaymentDeductionResult(uint code, uint conceptCode, PayrollConcept conceptItem, IDictionary<string, object> values)
            : base(code, conceptCode, conceptItem, values)
        {
            InitValues(values);
        }

        public override decimal Deduction() { return payment; }

        public override void InitValues(IDictionary<string, object> values)
        {
            base.InitValues(values);
        }

        public override void ExportXmlResult(/*xmlBuilder*/)
        {
        }

        public override string XmlValue()
        {
            return @"";
        }

        public override string ExportValueResult()
        {
            return @"";
        }
    }
}
