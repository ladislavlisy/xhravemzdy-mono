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
    public class TaxAdvanceConcept : PayrollConcept
    {
        static readonly uint TAG_ADVANCE_BASE = PayTagGateway.REF_TAX_ADVANCE_BASE.Code;
        static readonly uint TAG_INCOME_BASE = PayTagGateway.REF_TAX_INCOME_BASE.Code;

        public TaxAdvanceConcept(uint tagCode, IDictionary<string, object> values)
            : base(PayConceptGateway.REFCON_TAX_ADVANCE, tagCode)
        {
            InitValues(values);
        }

        public override void InitValues(IDictionary<string, object> values)
        {
        }

        public override PayrollConcept CloneWithValue(uint code, IDictionary<string, object> values)
        {
            PayrollConcept newConcept = (TaxAdvanceConcept)this.Clone();
            newConcept.InitCode(code);
            newConcept.InitValues(values);
            return newConcept;
        }

        public override PayrollTag[] PendingCodes()
        {
            return new PayrollTag[] {
                new TaxAdvanceBaseTag()
            };
        }

        public override uint CalcCategory()
        {
            return PayrollConcept.CALC_CATEGORY_NETTO;
        }

        public override PayrollResult Evaluate(PayrollPeriod period, PayTagGateway tagConfig, IDictionary<TagRefer, PayrollResult> results)
        {
            IncomeBaseResult resultIncome = (IncomeBaseResult)GetResultBy(results, TAG_INCOME_BASE);
            IncomeBaseResult resultAdvance = (IncomeBaseResult)GetResultBy(results, TAG_ADVANCE_BASE);

            decimal taxableIncome = resultIncome.IncomeBase();
            decimal taxablePartial = resultAdvance.IncomeBase();

            decimal resultPayment = ComputeResultValue(period, taxableIncome, taxablePartial);

            var resultValues = new Dictionary<string, object>() { { "payment", resultPayment } };
            return new PaymentResult(TagCode, Code, this, resultValues);
        }

        public decimal ComputeResultValue(PayrollPeriod period, decimal taxableIncome, decimal taxablePartial)
        {
            decimal paymentValue = TaxAdvCalculate(period, taxableIncome, taxablePartial);
            return paymentValue;
        }

        public decimal TaxAdvCalculate(PayrollPeriod period, decimal taxableIncome, decimal taxableBase)
        {
            decimal taxAdvance = 0m;
            
            if (taxableBase <= 0m)
            {
                taxAdvance = 0m;
            }
            else if (taxableBase <= 100m)
            {
                taxAdvance = FixTaxRoundUp(BigMulti(taxableBase, TaxAdvBracket1(period.Year())));
            }
            else
            {
                taxAdvance = TaxAdvCalculateMonth(period, taxableIncome, taxableBase);
            }
            return taxAdvance;
        }

        private decimal TaxAdvCalculateMonth(PayrollPeriod period, decimal taxableIncome, decimal taxableBase)
        {
            uint year = period.Year();

            decimal taxAdvance = 0m;

            if (taxableBase <= 0m)
            {
                taxAdvance = 0m;
            }
            else
            {
                if (period.Year() < 2013)
                {
                    taxAdvance = FixTaxRoundUp(BigMulti(taxableBase, TaxAdvBracket1(year)));
                }
                else
                {
                    decimal taxStandard = FixTaxRoundUp(BigMulti(taxableBase, TaxAdvBracket1(year)));
                    decimal maxSolBase = TaxSolBracketMax(year);
                    decimal effSolBase = Math.Max(0, taxableIncome - maxSolBase);
                    decimal taxSolidary = FixTaxRoundUp(BigMulti(effSolBase, TaxSolBracket(year)));
                    taxAdvance = taxStandard + taxSolidary;
                }
            }
            return taxAdvance;
        }

        private decimal TaxSolBracketMax(uint year)
        {
            if (year >= 2013)
            {
                return decimal.Multiply(4m, 25884m);
            }
            else
            {
                return 0m;
            }
        }

        private decimal TaxSolBracket(uint year)
        {
            decimal factor = 0m;
            if (year >= 2013)
            {
                factor = 7m;
            }
            else
            {
                factor = 0m;
            }
            return decimal.Divide(factor, 100m);
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
            TaxAdvanceConcept other = (TaxAdvanceConcept)this.MemberwiseClone();
            return other;
        }

        #endregion
    }
}
