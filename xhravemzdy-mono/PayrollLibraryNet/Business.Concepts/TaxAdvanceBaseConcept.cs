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
    public class TaxAdvanceBaseConcept : PayrollConcept
    {
        static readonly uint TAG_HEALTH_BASE = PayTagGateway.REF_TAX_EMPLOYERS_HEALTH.Code;
        static readonly uint TAG_SOCIAL_BASE = PayTagGateway.REF_TAX_EMPLOYERS_SOCIAL.Code;
        static readonly uint TAG_AMOUNT_BASE = PayTagGateway.REF_TAX_INCOME_BASE.Code;

        public TaxAdvanceBaseConcept(uint tagCode, IDictionary<string, object> values)
            : base(PayConceptGateway.REFCON_TAX_ADVANCE_BASE, tagCode)
        {
            InitValues(values);
        }

        public override void InitValues(IDictionary<string, object> values)
        {
        }

        public override PayrollConcept CloneWithValue(uint code, IDictionary<string, object> values)
        {
            PayrollConcept newConcept = (TaxAdvanceBaseConcept)this.Clone();
            newConcept.InitCode(code);
            newConcept.InitValues(values);
            return newConcept;
        }

        public override PayrollTag[] PendingCodes()
        {
            return new PayrollTag[] {
                new TaxIncomeBaseTag(),
                new TaxEmployersHealthTag(),
                new TaxEmployersSocialTag()
            };
        }

        public override uint CalcCategory()
        {
            return PayrollConcept.CALC_CATEGORY_NETTO;
        }

        public override PayrollResult Evaluate(PayrollPeriod period, PayTagGateway tagConfig, IDictionary<TagRefer, PayrollResult> results)
        {
            PaymentResult healthEmployer = (PaymentResult)GetResultBy(results, TAG_HEALTH_BASE);
            PaymentResult socialEmployer = (PaymentResult)GetResultBy(results, TAG_SOCIAL_BASE);
            IncomeBaseResult resultIncome = (IncomeBaseResult)GetResultBy(results, TAG_AMOUNT_BASE);

            decimal taxableHealth = healthEmployer.Payment();
            decimal taxableSocial = socialEmployer.Payment();
            decimal taxableBase = resultIncome.IncomeBase();

            bool isTaxInterest = resultIncome.Interest();
            bool isTaxDeclared = resultIncome.Declared();

            decimal resultPayment = ComputeResultValue(period, isTaxInterest, isTaxDeclared, taxableHealth, taxableSocial, taxableBase);

            var resultValues = new Dictionary<string, object>() { { "income_base", resultPayment } };
            return new IncomeBaseResult(TagCode, Code, this, resultValues);
        }

        private decimal ComputeResultValue(PayrollPeriod period, bool isTaxInterest, bool isTaxDeclared, decimal taxableHealth, decimal taxableSocial, decimal taxableBase)
        {
            decimal taxableSuper = 0;

            if (!isTaxInterest)
            {
                taxableSuper = 0;
            }
            else
            {
                taxableSuper = taxableBase + taxableHealth + taxableSocial;
            }
            decimal paymentValue = TaxRoundedBase(period, isTaxDeclared, taxableBase, taxableSuper);

            return paymentValue;
        }

        public decimal TaxRoundedBase(PayrollPeriod period, bool isTaxDeclared, decimal taxableIncome, decimal taxableBase)
        {
            decimal advanceBase = 0m;
            if (isTaxDeclared)
            {
                advanceBase = AdvanceRoundedBase(period, isTaxDeclared, taxableBase);
            }
            else
            {
                if (taxableIncome > TaxWithholdMax(period.Year()))
                {
                    advanceBase = AdvanceRoundedBase(period, isTaxDeclared, taxableBase);
                }
                else
                {
                    advanceBase = 0;
                }

            }
            return advanceBase;
        }

        private decimal AdvanceRoundedBase(PayrollPeriod period, bool isTaxDeclared, decimal taxableBase)
        {
            uint year = period.Year();

            decimal advanceBase = 0;
            decimal amountForCalc = Math.Max(0, taxableBase);
            if (amountForCalc > 100m)
            {
                advanceBase = BigNearRoundUp(amountForCalc, 100);
            }
            else
            {
                if (year >= 2011)
                {
                    advanceBase = BigTaxRoundUp(amountForCalc);
                }
                else
                {
                    advanceBase = BigTaxRoundDown(amountForCalc);
                }
            }
            return advanceBase;
        }

        private decimal TaxWithholdMax(uint year)
        {
            if (year >= 2004)
            {
                return 5000;
            }
            else if (year >= 2001)
            {
                return 3000;
            }
            else
            {
                return 2000;
            }
        }

        #region ICloneable Members

        public override object Clone()
        {
            TaxAdvanceBaseConcept other = (TaxAdvanceBaseConcept)this.MemberwiseClone();
            return other;
        }

        #endregion
    }
}
