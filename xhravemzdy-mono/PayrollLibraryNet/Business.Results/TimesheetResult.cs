using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;

namespace PayrollLibrary.Business.Results
{
    public class TimesheetResult : PayrollResult
    {
        public TimesheetResult(uint code, uint conceptCode, PayrollConcept conceptItem, IDictionary<string, object> values)
            : base(code, conceptCode, conceptItem)
        {
            InitValues(values);
        }

        public int[] MonthSchedule { get; private set; }

        public override void InitValues(IDictionary<string, object> values)
        {
            this.MonthSchedule = GetArrayOfIntOrEmpty(values["month_schedule"]);
        }

        public int Hours()
        {
            int monthHours = 0;
            if (MonthSchedule != null)
            {
                monthHours = MonthSchedule.Aggregate(0, (agr, dh) => (agr + dh));
            }
            return monthHours;
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
