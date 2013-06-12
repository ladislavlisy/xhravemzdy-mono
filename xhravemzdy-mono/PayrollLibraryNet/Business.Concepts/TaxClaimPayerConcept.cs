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
    public class TaxClaimPayerConcept : PayrollConcept
    {
        public TaxClaimPayerConcept(uint tagCode, IDictionary<string, object> values)
            : base(PayConceptGateway.REFCON_TAX_CLAIM_PAYER, tagCode)
        {
            InitValues(values);
        }

        public uint ReliefCode { get; private set; }

        public override void InitValues(IDictionary<string, object> values)
        {
            this.ReliefCode = GetUIntOrZeroValue(values, "relief_code");
        }

        public override PayrollConcept CloneWithValue(uint code, IDictionary<string, object> values)
        {
            PayrollConcept newConcept = (TaxClaimPayerConcept)this.Clone();
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
            decimal reliefValue = ComputeResultValue(period.Year(), ReliefCode);

            var resultValues = new Dictionary<string, object>() { { "tax_relief", reliefValue } };
            return new TaxClaimResult(TagCode, Code, this, resultValues);
        }

        private decimal ComputeResultValue(uint year, uint reliefCode)
        {
            decimal reliefAmount = 0;

            if (reliefCode == 0)
            {
                return 0m;
            }
            reliefAmount = PayerRelief(year);
            return reliefAmount;
        }

        private decimal PayerRelief(uint year)
        {
            decimal reliefAmount = 0;
            if (year >= 2012)
            {
                reliefAmount = 2070m;
            }
            else if (year == 2011)
            {
                reliefAmount = 1970m;
            }
            else if (year >= 2009)
            {
                reliefAmount = 2070m;
            }
            else if (year == 2008)
            {
                reliefAmount = 2070m;
            }
            else if (year >= 2006)
            {
                reliefAmount = 600m;
            }
            else
            {
                reliefAmount = 0m;
            }
            return reliefAmount;
        }

        #region ICloneable Members

        public override object Clone()
        {
            TaxClaimPayerConcept other = (TaxClaimPayerConcept)this.MemberwiseClone();
            return other;
        }

        #endregion
    }
}
