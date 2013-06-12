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
    public class TaxIncomeBaseConcept : PayrollConcept
    {
        public TaxIncomeBaseConcept(uint tagCode, IDictionary<string, object> values)
            : base(PayConceptGateway.REFCON_TAX_INCOME_BASE, tagCode)
        {
            InitValues(values);
        }

        public uint InterestCode { get; private set; }

        public uint DeclareCode { get; private set; }

        public override void InitValues(IDictionary<string, object> values)
        {
            this.InterestCode = GetUIntOrZero(values["interest_code"]);
            this.DeclareCode  = GetUIntOrZero(values["declare_code"]);
        }

        public override PayrollConcept CloneWithValue(uint code, IDictionary<string, object> values)
        {
            PayrollConcept newConcept = (TaxIncomeBaseConcept)this.Clone();
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
            return PayrollConcept.CALC_CATEGORY_GROSS;
        }

        public override PayrollResult Evaluate(PayrollPeriod period, PayTagGateway tagConfig, IDictionary<TagRefer, PayrollResult> results)
        {
            var resultValues = new Dictionary<string, object>() { { "", 0 } };
            return new PayrollResult(TagCode, Code, this, resultValues);
        }

        #region ICloneable Members

        public override object Clone()
        {
            TaxIncomeBaseConcept other = (TaxIncomeBaseConcept)this.MemberwiseClone();
            return other;
        }

        #endregion
    }
}
