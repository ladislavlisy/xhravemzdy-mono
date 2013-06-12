using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.Core;

namespace PayrollLibrary.Business.CoreItems
{
    public abstract class PayrollConcept : CodeNameRefer, IComparable<PayrollConcept>, ICloneable
    {
        public static readonly uint TERM_BEG_FINISHED = 32;
        public static readonly uint TERM_END_FINISHED =  0;

        public static readonly uint CALC_CATEGORY_START = 0;
        public static readonly uint CALC_CATEGORY_TIMES = 0;
        public static readonly uint CALC_CATEGORY_AMOUNT = 0;
        public static readonly uint CALC_CATEGORY_GROSS = 1;
        public static readonly uint CALC_CATEGORY_NETTO = 2;
        public static readonly uint CALC_CATEGORY_FINAL = 9;

        public PayrollConcept(CodeNameRefer codeRefer, uint tagCode)
            : base(codeRefer.Code, codeRefer.Name)
        {
            this.TagCode = tagCode;
        }

        public abstract void InitValues(IDictionary<string, object> values);

        public abstract PayrollConcept CloneWithValue(uint code, IDictionary<string, object> values);

        public uint TagCode { get; private set; }

        public PayrollTag[] TagPendingCodes { get; private set; }

        public void InitCode(uint code)
        {
            this.TagCode = code;
        }
        
        public string Description() 
        {
            return Name;
        }

        public virtual void ExportXml(/*xmlBuilder*/)
        {
        }

        public virtual string ExportValueResult()
        {
            return @"";
        }

        public void SetPendingCodes(PayrollTag[] pendingCodes)
        {
            this.TagPendingCodes = (PayrollTag[])pendingCodes.Clone();
        }

        public virtual PayrollTag[] PendingCodes()
        {
            return new PayrollTag[0];
        }

        public virtual PayrollTag[] SummaryCodes()
        {
            return new PayrollTag[0];
        }

        public virtual uint CalcCategory()
        {
            return CALC_CATEGORY_START;
        }

        public abstract PayrollResult Evaluate(PayrollPeriod period, PayTagGateway tagConfig, IDictionary<TagRefer, PayrollResult> results);

        public int CompareTo(PayrollConcept conceptOther)
        {
            if (CountPendingCodes(TagPendingCodes, conceptOther.TagCode)!=0)
            {
                return 1;
            }
            else if (CountPendingCodes(conceptOther.TagPendingCodes, TagCode)!=0)
            {
                return -1;
            }
            else if (CountSummaryCodes(SummaryCodes(), conceptOther.TagCode)!=0)
            {
                return -1;
            }
            else if (CountSummaryCodes(conceptOther.SummaryCodes(), TagCode)!=0)
            {
                return 1;
            }
            else if (CalcCategory() == conceptOther.CalcCategory())
            {
                return TagCode.CompareTo(conceptOther.TagCode);
            }
            else
            {
                return CalcCategory().CompareTo(conceptOther.CalcCategory());
            }
       }


        private int CountPendingCodes(PayrollTag[]conceptPending, uint code)
        {
            PayrollTag[] _codes = conceptPending.Where(x => x.Code==code).ToArray<PayrollTag>();
            return _codes.Length;
        }

        private int CountSummaryCodes(PayrollTag[]conceptSummary, uint code)
        {
            PayrollTag[] _codes = conceptSummary.Where(x => x.Code==code).ToArray<PayrollTag>();
            return _codes.Length;
        }

        // get term from Results by key of tag
        protected PayrollResult GetResultBy(IDictionary<TagRefer, PayrollResult> results, uint payTag)
        {
            IDictionary<TagRefer, PayrollResult> resultHash = results.Where( pair => pair.Key.Code==payTag)
                .ToDictionary(key => key.Key, value => value.Value);
            return resultHash.Values.ElementAt(0);
        }

        protected decimal BigMulti(decimal op1, decimal op2)
        {
            return decimal.Multiply(op1, op2);
        }

        protected decimal BigDiv(decimal op1, decimal op2)
        {
            if (op2 == 0m)
            {
                return 0m;
            }
            return decimal.Divide(op1, op2);
        }

        protected decimal BigMultiAndDiv(decimal op1, decimal op2, decimal div)
        {
            if (div == 0m)
            {
                return 0m;
            }
            return decimal.Divide(decimal.Multiply(op1, op2), div);
        }

        protected decimal BigInsuranceRoundUp(decimal valueDec)
        {
            return RoundUpToBig(valueDec);
        }


        protected int FixInsuranceRoundUp(decimal valueDec)
        {
            return RoundUpToFix(valueDec);
        }

        protected decimal BigTaxRoundUp(decimal valueDec)
        {
            return RoundUpToBig(valueDec);
        }

        protected int FixTaxRoundUp(decimal valueDec)
        {
            return RoundUpToFix(valueDec);
        }


        protected decimal BigTaxRoundDown(decimal valueDec)
        {
            return RoundDownToBig(valueDec);
        }


        protected int FixTaxRoundDown(decimal valueDec)
        {
            return RoundDownToFix(valueDec);
        }


        protected decimal RoundUpToBig(decimal valueDec)
        {
            return (valueDec < 0m ? decimal.Negate(decimal.Ceiling(Math.Abs(valueDec))) : decimal.Ceiling(Math.Abs(valueDec)));
        }


        protected int RoundUpToFix(decimal valueDec)
        {
            return decimal.ToInt32(valueDec < 0m ? decimal.Negate(decimal.Ceiling(Math.Abs(valueDec))) : decimal.Ceiling(Math.Abs(valueDec)));
        }


        protected decimal RoundDownToBig(decimal valueDec)
        {
            return (valueDec < 0m ? decimal.Negate(decimal.Floor(Math.Abs(valueDec))) : decimal.Floor(Math.Abs(valueDec)));
        }


        protected int RoundDownToFix(decimal valueDec)
        {
            return decimal.ToInt32(valueDec < 0m ? decimal.Negate(decimal.Floor(Math.Abs(valueDec))) : decimal.Floor(Math.Abs(valueDec)));
        }


        protected decimal BigNearRoundUp(decimal valueDec, int nearest = 100)
        {
            return BigMulti(RoundUpToBig(BigDiv(valueDec, nearest)), nearest);
        }


        protected decimal BigNearRoundDown(decimal valueDec, int nearest = 100)
        {
            return BigMulti(RoundDownToBig(BigDiv(valueDec, nearest)), nearest);
        }


        protected decimal BigDecimalCast(int number)
        {
            return new decimal(number);
        }

        #region get values from hash 

        protected int GetIntOrZero(object obj)
        {
            if (obj == null || !(obj is int)) return 0;
            return (int)obj;
        }

        protected uint GetUIntOrZero(object obj)
        {
            if (obj == null || !(obj is uint)) return 0;
            return (uint)obj;
        }

        protected decimal GetDecimalOrZero(object obj)
        {
            if (obj == null || !(obj is decimal)) return decimal.Zero;
            return (decimal)obj;
        }

        #endregion


        #region ICloneable Members

        public virtual object Clone()
        {
            PayrollConcept other = (PayrollConcept)this.MemberwiseClone();
            other.TagPendingCodes = (PayrollTag[])this.TagPendingCodes.Clone();
            return other;
        }

        #endregion
    }
}
