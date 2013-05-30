using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Core;
using PayrollLibrary.Business.Results;

namespace PayrollLibrary.Business.Concepts
{
    public class UnknownConcept : PayrollConcept
    {
        public UnknownConcept(uint tagCode, IDictionary<string, string> values)
            : base(PayConceptGateway.REFCON_UNKNOWN, tagCode)
        {
        }

        public override void InitValues(IDictionary<string, object> values)
        {
        }

        public override PayrollResult Evaluate(PayrollPeriod period, PayTagGateway tagConfig, IDictionary<TagRefer, PayrollResult> results)
        {
            return null;
        }

        #region ICloneable Members

        public override PayrollConcept CloneWithValue(uint code, IDictionary<string, object> values)
        {
            UnknownConcept other = (UnknownConcept)this.Clone();
            other.InitCode(code);
            other.InitValues(values);
            return other;
        }

        #endregion
    }
}
