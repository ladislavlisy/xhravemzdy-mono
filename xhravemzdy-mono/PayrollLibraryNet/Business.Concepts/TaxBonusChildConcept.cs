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
        static readonly uint TAG_AMOUNT_BASE = PayTagGateway.REF_TAX_INCOME_BASE.Code;
        static readonly uint TAX_ADVANCE = PayTagGateway.REF_TAX_ADVANCE.Code;
        static readonly uint TAX_RELIEF_PAYER = PayTagGateway.REF_TAX_RELIEF_PAYER.Code;
        static readonly uint TAX_RELIEF_CHILD = PayTagGateway.REF_TAX_RELIEF_CHILD.Code;
        static readonly uint TAX_CLAIMS_CHILD = PayTagGateway.REF_TAX_CLAIM_CHILD.Code;

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
            return new PayrollTag[] {
                new TaxAdvanceTag(),
                new TaxReliefPayerTag(),
                new TaxReliefChildTag()
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
            IncomeBaseResult resultIncome = (IncomeBaseResult)GetResultBy(results, TAG_AMOUNT_BASE);
            
            PaymentResult advanceBaseResult = (PaymentResult)GetResultBy(results, TAX_ADVANCE);
            TaxReliefResult reliefPayerResult = (TaxReliefResult)GetResultBy(results, TAX_RELIEF_PAYER);
            TaxReliefResult reliefChildResult = (TaxReliefResult)GetResultBy(results, TAX_RELIEF_CHILD);
            decimal reliefClaimValue = SumReliefBy(results, TAX_CLAIMS_CHILD);

            bool isTaxInterest = resultIncome.Interest();
            decimal advanceBaseValue = advanceBaseResult.Payment();
            decimal reliefPayerValue = reliefPayerResult.TaxRelief();
            decimal reliefChildValue = reliefChildResult.TaxRelief();

            decimal taxAdvanceValue = ComputeResultValue(period.Year(), isTaxInterest, advanceBaseValue, 
                reliefPayerValue, reliefChildValue, reliefClaimValue);

            var resultValues = new Dictionary<string, object>() { { "payment", taxAdvanceValue } };
            return new PaymentResult(TagCode, Code, this, resultValues);
        }

        private decimal ComputeResultValue(uint year, bool isTaxInterest, decimal advanceBaseValue, 
            decimal reliefPayerValue, decimal reliefChildValue, decimal reliefClaimValue)
        {
            decimal taxAdvanceValue = 0m;
            if (!isTaxInterest)
            {
                taxAdvanceValue = 0m;
            }
            else
            {
                decimal reliefBonusValue = BonusAfterRelief(advanceBaseValue,
                                                            reliefPayerValue,
                                                            reliefChildValue,
                                                            reliefClaimValue);
                taxAdvanceValue = MaxMinBonus(year, reliefBonusValue);
            }

            return taxAdvanceValue;
        }

        private decimal MaxMinBonus(uint year, decimal taxChildBonus)
        {
            if (taxChildBonus < MinBonusMonthly(year))
            {
                return 0m;
            }
            else if (taxChildBonus > MaxBonusMonthly(year))
            {
                return MaxBonusMonthly(year);
            }
            else
            {
                return taxChildBonus;
            }
        }

        private decimal MaxBonusMonthly(uint year)
        {
            if (year >= 2012)
            {
                return 5025m;
            }
            else if (year >= 2009)
            {
                return 4350m;
            }
            else if (year == 2008)
            {
                return 4350m;
            }
            else if (year >= 2005)
            {
                return 2500m;
            }
            else
            {
                return 0m;
            }
        }

        private decimal MinBonusMonthly(uint year)
        {
            if (year >= 2005)
            {
                return 50m;
            }
            else
            {
                return 0m;
            }
        }

        private decimal BonusAfterRelief(decimal advanceBaseValue, decimal reliefPayerValue, decimal reliefChildValue, decimal reliefClaimValue)
        {
            decimal bonusForChild = decimal.Negate(Math.Min(0, reliefChildValue - reliefClaimValue));
            if (bonusForChild >= 50m)
            {
                return bonusForChild;
            }
            else
            {
                return 0m;
            }
        }

        private decimal SumReliefBy(IDictionary<TagRefer, PayrollResult> results, uint payTag)
        {
            var resultHash = results.Where( (keyValue) => (keyValue.Key.Code == payTag))
                .ToDictionary(key => key.Key, value => value.Value);
            decimal resultSuma = resultHash.Aggregate(decimal.Zero,
                (agr, termItem) => (decimal.Add(agr, termItem.Value.TaxRelief())));
            return resultSuma;
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
