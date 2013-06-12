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
    public class InsuranceSocialBaseConcept : PayrollConcept
    {
        public InsuranceSocialBaseConcept(uint tagCode, IDictionary<string, object> values)
            : base(PayConceptGateway.REFCON_INSURANCE_SOCIAL_BASE, tagCode)
        {
            InitValues(values);
        }

        public uint InterestCode { get; private set; }

        public override void InitValues(IDictionary<string, object> values)
        {
            this.InterestCode = GetUIntOrZeroValue(values, "interest_code");
        }

        public override PayrollConcept CloneWithValue(uint code, IDictionary<string, object> values)
        {
            PayrollConcept newConcept = (InsuranceSocialBaseConcept)this.Clone();
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
            var resultIncome = ComputeResultValue(tagConfig, results);

            decimal employeeBase = MinMaxAssesmentBase(period, resultIncome);
            decimal employerBase = MaxAssesmentBase(period, resultIncome);

            var resultValues = new Dictionary<string, object>() { 
                { "income_base",   resultIncome }, 
                { "employee_base", employeeBase }, 
                { "employer_base", employerBase }, 
                { "interest_code", InterestCode }
            };
            return new IncomeBaseResult(TagCode, Code, this, resultValues);
        }

        private decimal ComputeResultValue(PayTagGateway tagConfig, IDictionary<TagRefer, PayrollResult> results)
        {
            if (!Interest())
            {
                return 0m;
            }
            else
            {
                decimal resultValue = results.Aggregate(decimal.Zero,
                    (agr, termItem) => (decimal.Add(agr, InnerComputeResultValue(tagConfig, this.TagCode, termItem))));
                return resultValue;
            }
        }

        private decimal InnerComputeResultValue(PayTagGateway tagConfig, uint tagCode, KeyValuePair<TagRefer, PayrollResult> result)
        {
            TagRefer resultKey = result.Key;
            PayrollResult resultItem = result.Value;

            return SumTermFor(tagConfig, tagCode, resultKey, resultItem);
        }

        private decimal SumTermFor(PayTagGateway tagConfig, uint tagCode, TagRefer resultKey, PayrollResult resultItem)
        {
            var tagConfigItem = tagConfig.FindTag(resultKey.Code);
            if (resultItem.SummaryFor(tagCode))
            {
                if (tagConfigItem.InsuranceSocial())
                {
                    return resultItem.Payment();
                }
            }
            return decimal.Zero;
        }

        public bool Interest()
        {
            return InterestCode != 0;
        }

        public decimal MinMaxAssesmentBase(PayrollPeriod period, decimal insBase)
        {
            decimal minBase = MinAssesmentBase(period, insBase);
            decimal maxBase = MaxAssesmentBase(period, minBase);
            return maxBase;
        }

        public decimal MaxAssesmentBase(PayrollPeriod period, decimal incomeBase)
        {
            decimal maximumBase = SocialMaxAssesment(period.Year());

            if (maximumBase.Equals(0m))
            {
                return incomeBase;
            }
            return Math.Min(incomeBase, maximumBase);
        }

        public decimal MinAssesmentBase(PayrollPeriod period, decimal incomeBase)
        {
            return incomeBase;
        }

        public decimal SocialMaxAssesment(uint year)
        {
            decimal maxAssesment = decimal.Zero;
            if (year >= 2013)
            {
                maxAssesment = 1242432m;
            }
            else if (year == 2012)
            {
                maxAssesment = 1206576m;
            }
            else if (year == 2011)
            {
                maxAssesment = 1781280m;
            }
            else if (year == 2010)
            {
                maxAssesment = 1707048m;
            }
            else if (year == 2009)
            {
                maxAssesment = 1130640m;
            }
            else if (year == 2008)
            {
                maxAssesment = 1034880m;
            }
            else
            {
                maxAssesment = 0m;
            }
            return maxAssesment;
        }

        #region ICloneable Members

        public override object Clone()
        {
            InsuranceSocialBaseConcept other = (InsuranceSocialBaseConcept)this.MemberwiseClone();
            return other;
        }

        #endregion
    }
}
