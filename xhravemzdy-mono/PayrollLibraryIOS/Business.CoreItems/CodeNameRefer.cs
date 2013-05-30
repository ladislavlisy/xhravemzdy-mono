using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PayrollLibrary.Business.CoreItems
{
    public class CodeNameRefer : IComparable
    {
        public CodeNameRefer(uint code, string name)
        {
            this.Code = code;
            this.Name = name;
        }

        public uint Code { get; private set; }
        public string Name { get; private set; }

        public int CompareTo(object obj)
        {
            CodeNameRefer other = obj as CodeNameRefer;

            return (this.Code.CompareTo(other.Code));
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
                return true;
            if (obj==null || this.GetType() != obj.GetType())
                return false;

            CodeNameRefer other = obj as CodeNameRefer;

            return this.Code == other.Code && this.Name.Equals(other.Name);
        }

        public override int GetHashCode()
        {
            int prime = 31;
            int result = 1;

            result += prime * result + (int)this.Code;
            return result;
        }
    }
}
