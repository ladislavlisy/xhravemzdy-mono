using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PayrollLibrary.Business.CoreItems
{
    public class PayrollTag : CodeNameRefer
    {
        public PayrollTag(CodeNameRefer codeRefer, CodeNameRefer concept)
            : base(codeRefer.Code, codeRefer.Name)
        {
            this.Concept = concept;
        }

        public CodeNameRefer Concept { get; private set; }

        public string Title() { return Name; }

        public string Decsription() { return Name; }

        public uint ConceptCode() { return Concept.Code; }

        public string ConceptName() { return Concept.Name; }

        public bool InsuranceHealth() { return false; }

        public bool InsuranceSocial() { return false; }

        public bool TaxAdvance() { return false; }

        public bool IncomeGross() { return false; }

        public bool IncomeNetto() { return false; }

        public bool DeductionNetto() { return false; }

    }
}
