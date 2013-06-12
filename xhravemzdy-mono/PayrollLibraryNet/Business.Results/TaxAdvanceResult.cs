using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;

namespace PayrollLibrary.Business.Results
{
    public class TaxAdvanceResult : PayrollResult
    {
        public TaxAdvanceResult(uint code, uint conceptCode, PayrollConcept conceptItem, IDictionary<string, object> values)
            : base(code, conceptCode, conceptItem)
        {
            InitValues(values);
        }

        public decimal Payment { get; private set; }

        public decimal AfterReliefA { get; private set; }

        public decimal AfterReliefC { get; private set; }

        public override void InitValues(IDictionary<string, object> values)
        {
            this.Payment = GetDecimalOrZero(values["payment"]);
            this.AfterReliefA = GetDecimalOrZero(values["after_reliefA"]);
            this.AfterReliefC = GetDecimalOrZero(values["after_reliefC"]);
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
