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
    public class InsuranceHealthBaseConcept : PayrollConcept
    {
        public InsuranceHealthBaseConcept(uint tagCode, IDictionary<string, object> values)
            : base(PayConceptGateway.REFCON_INSURANCE_HEALTH_BASE, tagCode)
        {
            InitValues(values);
        }

        public uint InterestCode { get; private set; }

        public uint MinimumAsses { get; private set; }

        public override void InitValues(IDictionary<string, object> values)
        {
            this.InterestCode = GetUIntOrZeroValue(values, "interest_code");
            this.MinimumAsses = GetUIntOrZeroValue(values, "minimum_asses");
        }

        public override PayrollConcept CloneWithValue(uint code, IDictionary<string, object> values)
        {
            PayrollConcept newConcept = (InsuranceHealthBaseConcept)this.Clone();
            newConcept.InitCode(code);
            newConcept.InitValues(values);
            return newConcept;
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
                { "interest_code", InterestCode }, 
                { "minimum_asses", MinimumAsses }
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
                if (tagConfigItem.InsuranceHealth())
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

        public bool MinimumAssesment()
        {
            return MinimumAsses != 0;
        }

        public decimal MinMaxAssesmentBase(PayrollPeriod period, decimal insBase)
        {
            decimal minBase = MinAssesmentBase(period, insBase);
            decimal maxBase = MaxAssesmentBase(period, minBase);
            return maxBase;
        }

        public decimal MaxAssesmentBase(PayrollPeriod period, decimal incomeBase)
        {
            decimal maximumBase = HealthMaxAssesment(period.Year());

            if (maximumBase.Equals(0m))
            {
                return incomeBase;
            }
            return Math.Min(incomeBase, maximumBase);
        }

        public decimal MinAssesmentBase(PayrollPeriod period, decimal incomeBase)
        {
            if (!MinimumAssesment())
            {
                return incomeBase;
            }
            else
            {
                decimal minimumBase = HealthMinAssesment(period.Year(), period.Month());
                if (minimumBase > incomeBase)
                {
                    return minimumBase;
                }
                else
                {
                    return incomeBase;
                }
            }
        }

        public decimal HealthMaxAssesment(uint year)
        {
            decimal maxAssesment = decimal.Zero;
            if (year >= 2013)
            {
                maxAssesment = 0m; //1863648m
            }
            else if (year == 2012)
            {
                maxAssesment = 1809864m;
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

        public decimal HealthMinAssesment(uint year, byte month)
        {
            decimal minAssesment = decimal.Zero;
            if (year >= 2007)
            {
                minAssesment = 8000m;
            }
            else if (year == 2006 && month >= 7)
            {
                minAssesment = 7955m;
            }
            else if (year == 2006)
            {
                minAssesment = 7570m;
            }
            else if (year == 2005)
            {
                minAssesment = 7185m;
            }
            else if (year == 2004)
            {
                minAssesment = 6700m;
            }
            else if (year == 2003)
            {
                minAssesment = 6200m;
            }
            else if (year == 2002)
            {
                minAssesment = 5700m;
            }
            else if (year == 2001)
            {
                minAssesment = 5000m;
            }
            else if (year == 2000 && month >= 7)
            {
                minAssesment = 4500m;
            }
            else if (year == 2000)
            {
                minAssesment = 4000m;
            }
            else if (year == 1999 && month >= 7)
            {
                minAssesment = 3600m;
            }
            else
            {
                minAssesment = 3250m;
            }
            return minAssesment;
        }


        #region ICloneable Members

        public override object Clone()
        {
            InsuranceHealthBaseConcept other = (InsuranceHealthBaseConcept)this.MemberwiseClone();
            return other;
        }

        #endregion
    }
}
