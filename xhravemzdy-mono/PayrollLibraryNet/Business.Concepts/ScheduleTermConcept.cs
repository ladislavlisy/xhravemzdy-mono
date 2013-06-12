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

        public DateTime DateFrom { get; private set; }

        public DateTime DateEnd { get; private set; }

        public override void InitValues(IDictionary<string, object> values)
        {
            this.DateFrom = (DateTime)values["date_from"];
            this.DateEnd  = (DateTime)values["date_end"];
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
            uint dayTermFrom = TERM_BEG_FINISHED;
            uint dayTermEnd = TERM_END_FINISHED;

            DateTime periodDateBeg = new DateTime((int)period.Year(), (int)period.Month(), 1);
            DateTime periodDateEnd = new DateTime((int)period.Year(), (int)period.Month(), 
                DateTime.DaysInMonth((int)period.Year(), (int)period.Month()));

            if (this.DateFrom != null)
            {
                dayTermFrom = (uint)DateFrom.Day;
            }

            if (this.DateEnd != null)
            {
                dayTermEnd = (uint)DateEnd.Day;
            }

            if (this.DateFrom == null || this.DateFrom < periodDateBeg)
            {
                dayTermFrom = 1;
            }

            if (this.DateEnd == null || this.DateEnd > periodDateEnd)
            {
                dayTermFrom = (uint)DateTime.DaysInMonth((int)period.Year(), (int)period.Month());
            }

            var resultValues = new Dictionary<string, object>() {
                { "day_ord_from", dayTermFrom }, { "day_ord_end", dayTermEnd } 
            };
            return new TermEffectResult(TagCode, Code, this, resultValues);
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
