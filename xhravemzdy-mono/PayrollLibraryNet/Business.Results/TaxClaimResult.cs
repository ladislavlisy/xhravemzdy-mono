using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;

namespace PayrollLibrary.Business.Results
{
    public class TaxClaimResult : PayrollResult
    {
        public TaxClaimResult(uint code, uint conceptCode, PayrollConcept conceptItem, IDictionary<string, object> values)
            : base(code, conceptCode, conceptItem)
        {
            InitValues(values);
        }

        public decimal taxRelief;

        public override decimal TaxRelief() { return taxRelief; }

        public override void InitValues(IDictionary<string, object> values)
        {
            this.taxRelief = GetDecimalOrZero(values["tax_relief"]);
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
