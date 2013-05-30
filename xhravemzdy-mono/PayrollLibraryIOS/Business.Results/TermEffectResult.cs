using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;

namespace PayrollLibrary.Business.Results
{
    public class TermEffectResult : PayrollResult
    {
        public TermEffectResult(uint code, uint conceptCode, PayrollConcept conceptItem, IDictionary<string, object> values) : base(code, conceptCode, conceptItem)
        {
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
