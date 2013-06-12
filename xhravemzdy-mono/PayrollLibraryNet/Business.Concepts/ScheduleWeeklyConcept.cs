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
    public class ScheduleWeeklyConcept : PayrollConcept
    {
        public ScheduleWeeklyConcept(uint tagCode, IDictionary<string, object> values)
            : base(PayConceptGateway.REFCON_SCHEDULE_WEEKLY, tagCode)
        {
            InitValues(values);
        }

        public int HoursWeekly { get; private set; }

        public override void InitValues(IDictionary<string, object> values)
        {
            this.HoursWeekly = GetIntOrZero(values["hours_weekly"]);
        }

        public override PayrollConcept CloneWithValue(uint code, IDictionary<string, object> values)
        {
            PayrollConcept newConcept = (ScheduleWeeklyConcept)this.Clone();
            newConcept.InitCode(code);
            newConcept.InitValues(values);
            return newConcept;
        }

        public override PayrollTag[] PendingCodes()
        {
            return new PayrollTag[0];
        }

        public override PayrollTag[] SummaryCodes()
        {
            return new PayrollTag[0];
        }

        public override uint CalcCategory()
        {
            return PayrollConcept.CALC_CATEGORY_TIMES;
        }

        public override PayrollResult Evaluate(PayrollPeriod period, PayTagGateway tagConfig, IDictionary<TagRefer, PayrollResult> results)
        {
            var resultValues = new Dictionary<string, object>() { { "", 0 } };
            return new PayrollResult(TagCode, Code, this, resultValues);
        }

        #region ICloneable Members

        public override object Clone()
        {
            ScheduleWeeklyConcept other = (ScheduleWeeklyConcept)this.MemberwiseClone();
            return other;
        }

        #endregion
    }
}
