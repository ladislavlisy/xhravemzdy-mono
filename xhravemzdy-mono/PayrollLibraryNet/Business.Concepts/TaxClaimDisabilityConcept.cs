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
            this.ReliefCode1 = GetUIntOrZeroValue(values, "relief_code_1");
            this.ReliefCode2 = GetUIntOrZeroValue(values, "relief_code_2");
            this.ReliefCode3 = GetUIntOrZeroValue(values, "relief_code_3");
        }

        public override PayrollConcept CloneWithValue(uint code, IDictionary<string, object> values)
        {
            PayrollConcept newConcept = (TaxClaimDisabilityConcept)this.Clone();
            newConcept.InitCode(code);
            newConcept.InitValues(values);
            return newConcept;
        }

        public override PayrollResult Evaluate(PayrollPeriod period, PayTagGateway tagConfig, IDictionary<TagRefer, PayrollResult> results)
        {
            decimal reliefValue = ComputeResultValue(period.Year(), ReliefCode1, ReliefCode2, ReliefCode3);

            var resultValues = new Dictionary<string, object>() { { "tax_relief", reliefValue } };
            return new TaxClaimResult(TagCode, Code, this, resultValues);
        }

        decimal ComputeResultValue(uint year, uint reliefCode1, uint reliefCode2, uint reliefCode3)
        {
            decimal reliefValue1 = Relief1ClaimAmount(year, reliefCode1);
            decimal reliefValue2 = Relief2ClaimAmount(year, reliefCode2);
            decimal reliefValue3 = Relief3ClaimAmount(year, reliefCode3);

            decimal reliefValue = (reliefValue1 + reliefValue2 + reliefValue3);
            return reliefValue;
        }

        private decimal Relief1ClaimAmount(uint year, uint reliefCode)
        {
            decimal reliefAmount = 0m;
            if (reliefCode == 0)
            {
                return 0m;
            }
            reliefAmount = Disability1Relief(year);
            return reliefAmount;
        }

        private decimal Relief2ClaimAmount(uint year, uint reliefCode)
        {
            decimal reliefAmount = 0m;
            if (reliefCode == 0)
            {
                return 0m;
            }
            reliefAmount = Disability2Relief(year);
            return reliefAmount;
        }

        private decimal Relief3ClaimAmount(uint year, uint reliefCode)
        {
            decimal reliefAmount = 0m;
            if (reliefCode == 0)
            {
                return 0m;
            }
            reliefAmount = Disability3Relief(year);
            return reliefAmount;
        }

        private decimal Disability1Relief(uint year)
        {
            decimal reliefAmount = 0;
            if (year >= 2009)
            {
                reliefAmount = 210m;
            }
            else if (year == 2008)
            {
                reliefAmount = 210m;
            }
            else if (year >= 2006)
            {
                reliefAmount = 125m;
            }
            else
            {
                reliefAmount = 0m;
            }
            return reliefAmount;
        }

        private decimal Disability2Relief(uint year)
        {
            decimal reliefAmount = 0;
            if (year >= 2009)
            {
                reliefAmount = 420m;
            }
            else if (year == 2008)
            {
                reliefAmount = 420m;
            }
            else if (year >= 2006)
            {
                reliefAmount = 250m;
            }
            else
            {
                reliefAmount = 0m;
            }
            return reliefAmount;
        }

        private decimal Disability3Relief(uint year)
        {
            decimal reliefAmount = 0;
            if (year >= 2009)
            {
                reliefAmount = 1345m;
            }
            else if (year == 2008)
            {
                reliefAmount = 1345m;
            }
            else if (year >= 2006)
            {
                reliefAmount = 800m;
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
            TaxClaimDisabilityConcept other = (TaxClaimDisabilityConcept)this.MemberwiseClone();
            return other;
        }

        #endregion
    }
}
