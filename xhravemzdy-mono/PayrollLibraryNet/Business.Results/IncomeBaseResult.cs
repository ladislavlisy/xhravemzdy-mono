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

        public decimal incomeBase;

        public override decimal IncomeBase() { return incomeBase; }

        public decimal employerBase;

        public override decimal EmployerBase() { return employerBase; }
        
        public decimal employeeBase;

        public override decimal EmployeeBase() { return employeeBase; }

        public uint InterestCode { get; private set; }

        public uint MinimumAsses { get; private set; }

        public uint DeclareCode  { get; private set; }

        public override void InitValues(IDictionary<string, object> values)
        {
            this.incomeBase = GetDecimalOrZeroValue(values, "income_base");
            this.employerBase = GetDecimalOrZeroValue(values, "employer_base");
            this.employeeBase = GetDecimalOrZeroValue(values, "employee_base");
            this.InterestCode = GetUIntOrZeroValue(values, "interest_code");
            this.MinimumAsses = GetUIntOrZeroValue(values, "minimum_asses");
            this.DeclareCode  = GetUIntOrZeroValue(values, "declare_code");
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
