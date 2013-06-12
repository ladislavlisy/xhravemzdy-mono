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
    public class TaxWithholdConcept : PayrollConcept
    {
        static readonly uint TAG_WITHHOLD_BASE = PayTagGateway.REF_TAX_WITHHOLD_BASE.Code;
        static readonly uint TAG_INCOME_BASE = PayTagGateway.REF_TAX_INCOME_BASE.Code;

        public TaxWithholdConcept(uint tagCode, IDictionary<string, object> values)
            : base(PayConceptGateway.REFCON_TAX_WITHHOLD, tagCode)
        {
            InitValues(values);
        }

        public override void InitValues(IDictionary<string, object> values)
        {
        }

        public override PayrollConcept CloneWithValue(uint code, IDictionary<string, object> values)
        {
            PayrollConcept newConcept = (TaxWithholdConcept)this.Clone();
            newConcept.InitCode(code);
            newConcept.InitValues(values);
            return newConcept;
        }

        public override PayrollTag[] PendingCodes()
        {
            return new PayrollTag[] {
                new TaxWithholdBaseTag()    
            };
        }

        public override PayrollTag[] SummaryCodes()
        {
            return new PayrollTag[] {
                new IncomeNettoTag()    
            };
        }

        public override uint CalcCategory()
        {
            return PayrollConcept.CALC_CATEGORY_NETTO;
        }

        public override PayrollResult Evaluate(PayrollPeriod period, PayTagGateway tagConfig, IDictionary<TagRefer, PayrollResult> results)
        {
            IncomeBaseResult resultIncome = (IncomeBaseResult)GetResultBy(results, TAG_INCOME_BASE);
            IncomeBaseResult resultWithhold = (IncomeBaseResult)GetResultBy(results, TAG_WITHHOLD_BASE);

            decimal taxableIncome = resultIncome.IncomeBase;
            decimal taxablePartial = resultWithhold.IncomeBase;

            decimal resultPayment = ComputeResultValue(period, taxableIncome, taxablePartial);

            var resultValues = new Dictionary<string, object>() { { "payment", resultPayment } };
            return new PaymentResult(TagCode, Code, this, resultValues);
        }

        private decimal ComputeResultValue(PayrollPeriod period, decimal taxableIncome, decimal taxablePartial)
        {
            decimal paymentValue = TaxWithholdCalcutate(period, taxableIncome, taxablePartial);
            return paymentValue;
        }

        private decimal TaxWithholdCalcutate(PayrollPeriod period, decimal taxableIncome, decimal taxableBase)
        {
            decimal taxWithhold = 0m;
            
            if (taxableBase <= 0m)
            {
                taxWithhold = 0m;
            }
            else
            {
                taxWithhold = TaxWithholdCalculateMonth(period, taxableIncome, taxableBase);
            }
            return taxWithhold;
        }

        private decimal TaxWithholdCalculateMonth(PayrollPeriod period, decimal taxableIncome, decimal taxableBase)
        {
            uint year = period.Year();

            decimal taxWithhold = 0m;

            if (taxableBase <= 0m)
            {
                taxWithhold = 0m;
            }
            else
            {
                if (period.Year() < 2008)
                {
                    taxWithhold = 0m;
                }
                else if (period.Year() < 2013)
                {
                    taxWithhold = FixTaxRoundUp(BigMulti(taxableBase, TaxAdvBracket1(year)));
                }
                else
                {
                    taxWithhold = FixTaxRoundUp(BigMulti(taxableBase, TaxAdvBracket1(year)));
                }
            }
            return taxWithhold;
        }

        private decimal TaxAdvBracket1(uint year)
        {
            decimal factor = 0m;
            if (year >= 2009)
            {
                factor = 15m;
            }
            else if (year == 2008)
            {
                factor = 15m;
            }
            else if (year >= 2006)
            {
                factor = 12m;
            }
            else
            {
                factor = 15m;
            }
            return decimal.Divide(factor, 100m);
        }

        #region ICloneable Members

        public override object Clone()
        {
            TaxWithholdConcept other = (TaxWithholdConcept)this.MemberwiseClone();
            return other;
        }

        #endregion
    }
}
