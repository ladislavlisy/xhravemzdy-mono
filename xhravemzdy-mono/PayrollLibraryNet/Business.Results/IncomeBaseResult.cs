using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;

namespace PayrollLibrary.Business.Results
{
    public class IncomeBaseResult : PayrollResult
    {
        public IncomeBaseResult(uint code, uint conceptCode, PayrollConcept conceptItem, IDictionary<string, object> values)
            : base(code, conceptCode, conceptItem)
        {
            InitValues(values);
        }

        public decimal IncomeBase   { get; private set; }

        public decimal EmployerBase { get; private set; }

        public decimal EmployeeBase { get; private set; }

        public uint InterestCode { get; private set; }

        public uint MinimumAsses { get; private set; }

        public uint DeclareCode  { get; private set; }

        public override void InitValues(IDictionary<string, object> values)
        {
            this.IncomeBase = GetDecimalOrZero(values["income_base"]);
            this.EmployerBase = GetDecimalOrZero(values["employer_base"]);
            this.EmployeeBase = GetDecimalOrZero(values["employee_base"]);
            this.InterestCode = GetUIntOrZero(values["interest_code"]);
            this.MinimumAsses = GetUIntOrZero(values["minimum_asses"]);
            this.DeclareCode  = GetUIntOrZero(values["declare_code"]);
        }

        public override void ExportXmlResult(/*xmlBuilder*/)
        {
        }

        public string XmlValue()
        {
            return @"";
        }

        public override string ExportValueResult()
        {
            return @"";
        }

        public bool Interest()
        {
            return InterestCode != 0;
        }

        public bool Declared()
        {
            return DeclareCode != 0;
        }

        public bool MinimumAssesment()
        {
            return MinimumAsses != 0;
        }

    }
}
