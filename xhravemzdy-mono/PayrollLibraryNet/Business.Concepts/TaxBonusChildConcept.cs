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
    public class TaxBonusChildConcept : PayrollConcept
    {
        public TaxBonusChildConcept(uint tagCode, IDictionary<string, object> values)
            : base(PayConceptGateway.REFCON_TAX_BONUS_CHILD, tagCode)
        {
            InitValues(values);
        }

        public override void InitValues(IDictionary<string, object> values)
        {
        }

        public override PayrollConcept CloneWithValue(uint code, IDictionary<string, object> values)
        {
            PayrollConcept newConcept = (TaxBonusChildConcept)this.Clone();
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
            return PayrollConcept.CALC_CATEGORY_NETTO;
        }

        public override PayrollResult Evaluate(PayrollPeriod period, PayTagGateway tagConfig, IDictionary<TagRefer, PayrollResult> results)
        {
            var resultValues = new Dictionary<string, object>() { { "", 0 } };
            return new PayrollResult(TagCode, Code, this, resultValues);
        }

        #region ICloneable Members

        public override object Clone()
        {
            TaxBonusChildConcept other = (TaxBonusChildConcept)this.MemberwiseClone();
            return other;
        }

        #endregion
    }
}
