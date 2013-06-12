using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Core;
using PayrollLibrary.Business.PayTags;
using PayrollLibrary.Business.Results;

namespace PayrollLibrary.Business.Concepts
{
    public class TimesheetWorkConcept : PayrollConcept
    {
        static readonly uint TAG_SCHEDULE_TERM = PayTagGateway.REF_SCHEDULE_TERM.Code;
        static readonly uint TAG_TIMESHEET_PERIOD = PayTagGateway.REF_TIMESHEET_PERIOD.Code;

        public TimesheetWorkConcept(uint tagCode, IDictionary<string, object> values)
            : base(PayConceptGateway.REFCON_TIMESHEET_WORK, tagCode)
        {
            InitValues(values);
        }

        public override void InitValues(IDictionary<string, object> values)
        {
        }

        public override PayrollConcept CloneWithValue(uint code, IDictionary<string, object> values)
        {
            PayrollConcept newConcept = (TimesheetWorkConcept)this.Clone();
            newConcept.InitCode(code);
            newConcept.InitValues(values);
            return newConcept;
        }

        public override PayrollTag[] PendingCodes()
        {
            return new PayrollTag[] {
                new TimesheetPeriodTag(),
                new ScheduleTermTag()
            };
        }

        public override uint CalcCategory()
        {
            return PayrollConcept.CALC_CATEGORY_TIMES;
        }

        public override PayrollResult Evaluate(PayrollPeriod period, PayTagGateway tagConfig, IDictionary<TagRefer, PayrollResult> results)
        {
            TermEffectResult scheduleTermResult = (TermEffectResult)GetResultBy(results, TAG_SCHEDULE_TERM);
            TimesheetResult timesheetPeriodResult = (TimesheetResult)GetResultBy(results, TAG_TIMESHEET_PERIOD);

            uint dayOrdFrom = scheduleTermResult.DayOrdFrom;
            uint dayOrgEnd = scheduleTermResult.DayOrdEnd;

            int[] timesheetPeriod = timesheetPeriodResult.MonthSchedule;
            int[] hoursCalendar = ComputeResultValue(timesheetPeriod, dayOrdFrom, dayOrgEnd);

            var resultValues = new Dictionary<string, object>() { { "month_schedule", hoursCalendar } };
            return new TimesheetResult(TagCode, Code, this, resultValues);
        }

        private int[] ComputeResultValue(int[] timesheetPeriod, uint dayOrdFrom, uint dayOrgEnd)
        {
            int[][] timesheetPeriodWithIndex = timesheetPeriod.Select((x, i) => (new int[] { x, i })).ToArray();

            int[] hoursCalendar = timesheetPeriodWithIndex.Select((x) => (HoursFromCalendar(dayOrdFrom, dayOrgEnd, x[1], x[0]))).ToArray();
            return hoursCalendar;
        }

        private int HoursFromCalendar(uint dayOrdFrom, uint dayOrgEnd, int dayIndex, int workHours)
        {
            int dayOrdinal = dayIndex + 1;
            int hoursInDay = workHours;
            if (dayOrdFrom > dayOrdinal)
            {
                hoursInDay = 0;
            }
            if (dayOrgEnd < dayOrdinal)
            {
                hoursInDay = 0;
            }
            return hoursInDay;
        }

        #region ICloneable Members

        public override object Clone()
        {
            TimesheetWorkConcept other = (TimesheetWorkConcept)this.MemberwiseClone();
            return other;
        }

        #endregion
    }
}
