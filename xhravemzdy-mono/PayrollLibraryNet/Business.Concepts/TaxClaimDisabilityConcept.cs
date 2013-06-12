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
    public class TaxClaimDisabilityConcept : PayrollConcept
    {
        public TaxClaimDisabilityConcept(uint tagCode, IDictionary<string, object> values)
            : base(PayConceptGateway.REFCON_TAX_CLAIM_DISABILITY, tagCode)
        {
            InitValues(values);
        }

        public uint ReliefCode1 { get; private set; }
        public uint ReliefCode2 { get; private set; }
        public uint ReliefCode3 { get; private set; }

        public override void InitValues(IDictionary<string, object> values)
        {
            this.ReliefCode1 = GetUIntOrZero(values["relief_code_1"]);
            this.ReliefCode2 = GetUIntOrZero(values["relief_code_2"]);
            this.ReliefCode3 = GetUIntOrZero(values["relief_code_3"]);
        }

        public override PayrollConcept CloneWithValue(uint code, IDictionary<string, object> values)
        {
            PayrollConcept newConcept = (TaxClaimDisabilityConcept)this.Clone();
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

        public override PayrollResult Evaluate(PayrollPeriod period, PayTagGateway tagConfig, IDictionary<TagRefer, PayrollResult> results)
        {
            var resultValues = new Dictionary<string, object>() { { "", 0 } };
            return new PayrollResult(TagCode, Code, this, resultValues);
        }

        #region ICloneable Members

        public override object Clone()
        {
            TaxClaimDisabilityConcept other = (TaxClaimDisabilityConcept)this.MemberwiseClone();
            return other;
        }

        #endregion
    }
}
