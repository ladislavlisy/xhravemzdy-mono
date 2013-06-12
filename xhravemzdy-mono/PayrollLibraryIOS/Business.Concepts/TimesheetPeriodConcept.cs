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
    public class TimesheetPeriodConcept : PayrollConcept
    {
        static readonly uint TAG_SCHEDULE_WORK = PayTagGateway.REF_SCHEDULE_WORK.Code;

        public TimesheetPeriodConcept(uint tagCode, IDictionary<string, object> values)
            : base(PayConceptGateway.REFCON_TIMESHEET_PERIOD, tagCode)
        {
            InitValues(values);
        }

        public override void InitValues(IDictionary<string, object> values)
        {
        }

        public override PayrollConcept CloneWithValue(uint code, IDictionary<string, object> values)
        {
            PayrollConcept newConcept = (TimesheetPeriodConcept)this.Clone();
            newConcept.InitCode(code);
            newConcept.InitValues(values);
            return newConcept;
        }

        public override PayrollTag[] PendingCodes()
        {
            return new PayrollTag[] {
                new ScheduleWorkTag()
            };
        }

        public override uint CalcCategory()
        {
            return PayrollConcept.CALC_CATEGORY_TIMES;
        }

        public override PayrollResult Evaluate(PayrollPeriod period, PayTagGateway tagConfig, IDictionary<TagRefer, PayrollResult> results)
        {
            ScheduleResult scheduleWorkResult = (ScheduleResult)GetResultBy(results, TAG_SCHEDULE_WORK);

            int[] weekHours = scheduleWorkResult.WeekSchedule;

            int[] hoursCalendar = ComputeResultValue(period, weekHours);

            var resultValues = new Dictionary<string, object>() { { "month_schedule", hoursCalendar } };
            return new TimesheetResult(TagCode, Code, this, resultValues);
        }

        private int[] ComputeResultValue(PayrollPeriod period, int[] weekHours)
        {
            return MonthCalendarDays(period, weekHours);
        }

        private int[] MonthCalendarDays(PayrollPeriod period, int[] weekHours)
        {
            DateTime calendarBeg = new DateTime((int)period.Year(), (int)period.Month(), 1);
            int calendarBegCwd = DayofWeekMondayToSunday(calendarBeg);
            int dateCount = DateTime.DaysInMonth((int)period.Year(), (int)period.Month());
            int[] hoursCalendar = Enumerable.Range(1, dateCount).Select( (x) => (HoursFromWeek(weekHours, x, calendarBegCwd, calendarBeg))).ToArray();
            return hoursCalendar;
        }

        private static int DayofWeekMondayToSunday(DateTime calendarBeg)
        {
            int calendarBegCwd = (int)calendarBeg.DayOfWeek;
            // DayOfWeek Sunday = 0,
            // Monday = 1, Tuesday = 2, Wednesday = 3, Thursday = 4, Friday = 5, Saturday = 6, 
            if (calendarBegCwd == 0)
            {
                calendarBegCwd = 7;
            }
            return calendarBegCwd;
        }

        private int HoursFromWeek(int[] weekHours, int dayOrdinal, int calendarBegCwd, DateTime calendarBeg)
        {
            DateTime calendarDay = new DateTime(calendarBeg.Year, calendarBeg.Month, dayOrdinal);
            int dayOfWeek = (dayOrdinal % 7) + (calendarBegCwd - 1);
            return weekHours[dayOfWeek-1];
        }

        #region ICloneable Members

        public override object Clone()
        {
            TimesheetPeriodConcept other = (TimesheetPeriodConcept)this.MemberwiseClone();
            return other;
        }

        #endregion
    }
}
