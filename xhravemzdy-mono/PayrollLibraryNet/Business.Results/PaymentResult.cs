using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;

namespace PayrollLibrary.Business.Results
{
    public class PaymentResult : PayrollResult
    {
        public PaymentResult(uint code, uint conceptCode, PayrollConcept conceptItem, IDictionary<string, object> values)
            : base(code, conceptCode, conceptItem)
        {
            InitValues(values);
        }

        public decimal payment;

        public override decimal Payment() { return payment; }

        public override void InitValues(IDictionary<string, object> values)
        {
            this.payment = GetDecimalOrZero(values["payment"]);
        }

        public override void ExportXmlResult(/*xmlBuilder*/)
        {
        }

        public string XmlValue()
        {
            return @"";
        }

        public override string ExportValueResult()
        {
            return @"";
        }
    }
}
