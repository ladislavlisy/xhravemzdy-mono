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

        public bool insurance_social() { return false; }

        public bool tax_advance() { return false; }

        public bool income_gross() { return false; }

        public bool income_netto() { return false; }

        public bool deduction_netto() { return false; }

    }
}
