using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PayrollLibrary.Business.CoreItems
{
    public class PayrollPeriod
    {
        public static readonly uint NOW = 0;

        public PayrollPeriod(uint code)
        {
            this.Code = code;
        }

        public PayrollPeriod(uint year, byte month) : this(year*100 + month)
        {
        }

        public uint Code { get; private set; }

        public uint Year()
        {
            return (Code / 100);
        }

        public byte Month()
        {
            return (byte)(Code % 100);
        }
    }
}
