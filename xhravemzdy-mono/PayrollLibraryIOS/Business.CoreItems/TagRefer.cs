using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PayrollLibrary.Business.CoreItems
{
    public class TagRefer : IComparable
    {
        public TagRefer(uint periodBase, uint code, uint codeOrder)
        {
            this.PeriodBase = periodBase;
            this.Code = code;
            this.CodeOrder = codeOrder;
        }

        public uint PeriodBase { get; private set; }
        public uint Code { get; private set; }
        public uint CodeOrder { get; private set; }

        public int CompareTo(object obj)
        {
            TagRefer other = obj as TagRefer;

            if (this.PeriodBase != other.PeriodBase)
            {
                return this.PeriodBase.CompareTo(other.PeriodBase);
            }
            if (this.Code != other.Code)
            {
                return this.Code.CompareTo(other.Code);
            }
            return (this.CodeOrder.CompareTo(other.CodeOrder));
        }

        public bool isEqualToTagRefer(TagRefer other)
        {
            return (this.PeriodBase == other.PeriodBase && this.Code == other.Code && this.CodeOrder == other.CodeOrder);
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
                return true;
            if (obj == null || this.GetType() != obj.GetType())
                return false;

            TagRefer other = obj as TagRefer;

            return this.isEqualToTagRefer(other);
        }

        public override int GetHashCode()
        {
            int prime = 31;
            int result = 1;

            result += prime * result + (int)this.PeriodBase;

            result += prime * result + (int)this.Code;
            
            result += prime * result + (int)this.CodeOrder;
            return result;
        }

    }
}
