using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;

namespace PayrollLibrary.Business.Results
{
    public class TermEffectResult : PayrollResult
    {
        public TermEffectResult(uint code, uint conceptCode, PayrollConcept conceptItem, IDictionary<string, object> values)
            : base(code, conceptCode, conceptItem)
        {
            InitValues(values);
        }

        public uint DayOrdFrom { get; private set; }

        public uint DayOrdEnd  { get; private set; }

        public override void InitValues(IDictionary<string, object> values)
        {
            this.DayOrdFrom = GetUIntOrZero(values["day_ord_from"]);
            this.DayOrdEnd  = GetUIntOrZero(values["day_ord_end"]);
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
