using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;

namespace PayrollLibrary.Business.Results
{
    public class ScheduleResult : PayrollResult
    {
        public ScheduleResult(uint code, uint conceptCode, PayrollConcept conceptItem, IDictionary<string, object> values)
            : base(code, conceptCode, conceptItem)
        {
            InitValues(values);
        }

        public int[] WeekSchedule { get; private set; }

        public override void InitValues(IDictionary<string, object> values)
        {
            this.WeekSchedule = GetArrayOfIntOrEmpty(values["week_schedule"]);
        }

        public int Hours()
        {
            int weekHours = 0;
            if (WeekSchedule != null)
            {
                weekHours = WeekSchedule.Aggregate(0, (agr, dh) => (agr + dh));
            }
            return weekHours;
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
