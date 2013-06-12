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
    public class ScheduleTermConcept : PayrollConcept
    {
        public ScheduleTermConcept(uint tagCode, IDictionary<string, object> values)
            : base(PayConceptGateway.REFCON_SCHEDULE_TERM, tagCode)
        {
            InitValues(values);
        }

        public DateTime? DateFrom { get; private set; }

        public DateTime? DateEnd { get; private set; }

        public override void InitValues(IDictionary<string, object> values)
        {
            this.DateFrom = GetDateOrNullValue(values, "date_from");
            this.DateEnd  = GetDateOrNullValue(values, "date_end");
        }

        public override PayrollConcept CloneWithValue(uint code, IDictionary<string, object> values)
        {
            PayrollConcept newConcept = (ScheduleTermConcept)this.Clone();
            newConcept.InitCode(code);
            newConcept.InitValues(values);
            return newConcept;
        }

        public override PayrollResult Evaluate(PayrollPeriod period, PayTagGateway tagConfig, IDictionary<TagRefer, PayrollResult> results)
        {
            uint dayTermFrom = ComputeResultValueFrom(period, this.DateFrom);
            uint dayTermEnd = ComputeResultValueEnd(period, this.DateEnd);

            var resultValues = new Dictionary<string, object>() {
                { "day_ord_from", dayTermFrom }, { "day_ord_end", dayTermEnd } 
            };
            return new TermEffectResult(TagCode, Code, this, resultValues);
        }

        public uint ComputeResultValueFrom(PayrollPeriod period, DateTime? dateFrom)
        {
            uint dayTermFrom = TERM_BEG_FINISHED;

            DateTime periodDateBeg = new DateTime((int)period.Year(), (int)period.Month(), 1);

            if (dateFrom != null)
            {
                dayTermFrom = (uint)dateFrom.Value.Day;
            }

            if (dateFrom == null || dateFrom < periodDateBeg)
            {
                dayTermFrom = 1;
            }
            return dayTermFrom;
        }

        public uint ComputeResultValueEnd(PayrollPeriod period, DateTime? dateEnd)
        {
            uint dayTermEnd = TERM_END_FINISHED;
            uint daysPeriod = (uint)DateTime.DaysInMonth((int)period.Year(), (int)period.Month());

            DateTime periodDateEnd = new DateTime((int)period.Year(), (int)period.Month(), (int)daysPeriod);

            if (dateEnd != null)
            {
                dayTermEnd = (uint)dateEnd.Value.Day;
            }

            if (dateEnd == null || dateEnd > periodDateEnd)
            {
                dayTermEnd = daysPeriod;
            }
            return dayTermEnd;
        }

        #region ICloneable Members

        public override object Clone()
        {
            ScheduleTermConcept other = (ScheduleTermConcept)this.MemberwiseClone();
            return other;
        }

        #endregion
    }
}
