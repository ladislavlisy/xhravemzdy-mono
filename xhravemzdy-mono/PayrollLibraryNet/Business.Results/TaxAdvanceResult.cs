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

        public decimal payment;

        public override decimal Payment() { return payment; }

        public decimal afterReliefA;

        public override decimal AfterReliefA() { return afterReliefA; }

        public decimal afterReliefC;

        public override decimal AfterReliefC() { return afterReliefC; }

        public override void InitValues(IDictionary<string, object> values)
        {
            this.payment = GetDecimalOrZeroValue(values, "payment");
            this.afterReliefA = GetDecimalOrZeroValue(values, "after_reliefA");
            this.afterReliefC = GetDecimalOrZeroValue(values, "after_reliefC");
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
