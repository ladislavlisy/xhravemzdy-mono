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

        public virtual bool InsuranceHealth() { return false; }

        public virtual bool InsuranceSocial() { return false; }

        public virtual bool TaxAdvance() { return false; }

        public virtual bool IncomeGross() { return false; }

        public virtual bool IncomeNetto() { return false; }

        public virtual bool DeductionNetto() { return false; }

    }
}
