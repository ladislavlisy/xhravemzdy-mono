using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Concepts;
using System.Text.RegularExpressions;
using PayrollLibrary.Business.Libs;

namespace PayrollLibrary.Business.Core
{
    public enum ConceptCode : uint
    {
        CONCEPT_UNKNOWN = 0,
        CONCEPT_SALARY_MONTHLY = 100,
        CONCEPT_SCHEDULE_WEEKLY = 200,
        CONCEPT_SCHEDULE_TERM = 201,
        CONCEPT_TIMESHEET_PERIOD = 202,
        CONCEPT_TIMESHEET_WORK = 203,
        CONCEPT_HOURS_WORKING = 204,
        CONCEPT_HOURS_ABSENCE = 205,
        CONCEPT_TAX_INCOME_BASE = 300,
        CONCEPT_INSURANCE_HEALTH_BASE = 301,
        CONCEPT_INSURANCE_SOCIAL_BASE = 302,
        CONCEPT_INSURANCE_HEALTH = 303,
        CONCEPT_INSURANCE_SOCIAL = 304,
        CONCEPT_SAVINGS_PENSIONS = 305,
        CONCEPT_TAX_EMPLOYERS_HEALTH = 306,
        CONCEPT_TAX_EMPLOYERS_SOCIAL = 307,
        CONCEPT_TAX_CLAIM_PAYER = 308,
        CONCEPT_TAX_CLAIM_DISABILITY = 309,
        CONCEPT_TAX_CLAIM_STUDYING = 312,
        CONCEPT_TAX_CLAIM_CHILD = 313,
        CONCEPT_TAX_ADVANCE_BASE = 314,
        CONCEPT_TAX_ADVANCE = 315,
        CONCEPT_TAX_RELIEF_PAYER = 316,
        CONCEPT_TAX_RELIEF_DISABILITY = 317,
        CONCEPT_TAX_RELIEF_STUDYING = 318,
        CONCEPT_TAX_RELIEF_CHILD = 319,
        CONCEPT_TAX_BONUS_CHILD = 320,
        CONCEPT_TAX_ADVANCE_FINAL = 321,
        CONCEPT_TAX_WITHHOLD_BASE = 325,
        CONCEPT_TAX_WITHHOLD = 326,
        CONCEPT_INCOME_GROSS = 901,
        CONCEPT_INCOME_NETTO = 902
    };

    public class PayConceptGateway
    {
        public static readonly string CONCEPTS_NAMESPACE = "PayrollLibrary.Business.Concepts.";

        public static readonly CodeNameRefer REFCON_UNKNOWN = new CodeNameRefer((uint)ConceptCode.CONCEPT_UNKNOWN, ConceptCode.CONCEPT_UNKNOWN.ToString());
        public static readonly CodeNameRefer REFCON_SALARY_MONTHLY = new CodeNameRefer((uint)ConceptCode.CONCEPT_SALARY_MONTHLY, ConceptCode.CONCEPT_SALARY_MONTHLY.ToString());
        public static readonly CodeNameRefer REFCON_SCHEDULE_WEEKLY = new CodeNameRefer((uint)ConceptCode.CONCEPT_SCHEDULE_WEEKLY, ConceptCode.CONCEPT_SCHEDULE_WEEKLY.ToString());
        public static readonly CodeNameRefer REFCON_SCHEDULE_TERM = new CodeNameRefer((uint)ConceptCode.CONCEPT_SCHEDULE_TERM, ConceptCode.CONCEPT_SCHEDULE_TERM.ToString());
        public static readonly CodeNameRefer REFCON_TIMESHEET_PERIOD = new CodeNameRefer((uint)ConceptCode.CONCEPT_TIMESHEET_PERIOD, ConceptCode.CONCEPT_TIMESHEET_PERIOD.ToString());
        public static readonly CodeNameRefer REFCON_TIMESHEET_WORK = new CodeNameRefer((uint)ConceptCode.CONCEPT_TIMESHEET_WORK, ConceptCode.CONCEPT_TIMESHEET_WORK.ToString());
        public static readonly CodeNameRefer REFCON_HOURS_WORKING = new CodeNameRefer((uint)ConceptCode.CONCEPT_HOURS_WORKING, ConceptCode.CONCEPT_HOURS_WORKING.ToString());
        public static readonly CodeNameRefer REFCON_HOURS_ABSENCE = new CodeNameRefer((uint)ConceptCode.CONCEPT_HOURS_ABSENCE, ConceptCode.CONCEPT_HOURS_ABSENCE.ToString());
        public static readonly CodeNameRefer REFCON_TAX_INCOME_BASE = new CodeNameRefer((uint)ConceptCode.CONCEPT_TAX_INCOME_BASE, ConceptCode.CONCEPT_TAX_INCOME_BASE.ToString());
        public static readonly CodeNameRefer REFCON_INSURANCE_HEALTH_BASE = new CodeNameRefer((uint)ConceptCode.CONCEPT_INSURANCE_HEALTH_BASE, ConceptCode.CONCEPT_INSURANCE_HEALTH_BASE.ToString());
        public static readonly CodeNameRefer REFCON_INSURANCE_SOCIAL_BASE = new CodeNameRefer((uint)ConceptCode.CONCEPT_INSURANCE_SOCIAL_BASE, ConceptCode.CONCEPT_INSURANCE_SOCIAL_BASE.ToString());
        public static readonly CodeNameRefer REFCON_INSURANCE_HEALTH = new CodeNameRefer((uint)ConceptCode.CONCEPT_INSURANCE_HEALTH, ConceptCode.CONCEPT_INSURANCE_HEALTH.ToString());
        public static readonly CodeNameRefer REFCON_INSURANCE_SOCIAL = new CodeNameRefer((uint)ConceptCode.CONCEPT_INSURANCE_SOCIAL, ConceptCode.CONCEPT_INSURANCE_SOCIAL.ToString());
        public static readonly CodeNameRefer REFCON_SAVINGS_PENSIONS = new CodeNameRefer((uint)ConceptCode.CONCEPT_SAVINGS_PENSIONS, ConceptCode.CONCEPT_SAVINGS_PENSIONS.ToString());
        public static readonly CodeNameRefer REFCON_TAX_EMPLOYERS_HEALTH = new CodeNameRefer((uint)ConceptCode.CONCEPT_TAX_EMPLOYERS_HEALTH, ConceptCode.CONCEPT_TAX_EMPLOYERS_HEALTH.ToString());
        public static readonly CodeNameRefer REFCON_TAX_EMPLOYERS_SOCIAL = new CodeNameRefer((uint)ConceptCode.CONCEPT_TAX_EMPLOYERS_SOCIAL, ConceptCode.CONCEPT_TAX_EMPLOYERS_SOCIAL.ToString());
        public static readonly CodeNameRefer REFCON_TAX_CLAIM_PAYER = new CodeNameRefer((uint)ConceptCode.CONCEPT_TAX_CLAIM_PAYER, ConceptCode.CONCEPT_TAX_CLAIM_PAYER.ToString());
        public static readonly CodeNameRefer REFCON_TAX_CLAIM_DISABILITY = new CodeNameRefer((uint)ConceptCode.CONCEPT_TAX_CLAIM_DISABILITY, ConceptCode.CONCEPT_TAX_CLAIM_DISABILITY.ToString());
        public static readonly CodeNameRefer REFCON_TAX_CLAIM_STUDYING = new CodeNameRefer((uint)ConceptCode.CONCEPT_TAX_CLAIM_STUDYING, ConceptCode.CONCEPT_TAX_CLAIM_STUDYING.ToString());
        public static readonly CodeNameRefer REFCON_TAX_CLAIM_CHILD = new CodeNameRefer((uint)ConceptCode.CONCEPT_TAX_CLAIM_CHILD, ConceptCode.CONCEPT_TAX_CLAIM_CHILD.ToString());
        public static readonly CodeNameRefer REFCON_TAX_ADVANCE_BASE = new CodeNameRefer((uint)ConceptCode.CONCEPT_TAX_ADVANCE_BASE, ConceptCode.CONCEPT_TAX_ADVANCE_BASE.ToString());
        public static readonly CodeNameRefer REFCON_TAX_ADVANCE = new CodeNameRefer((uint)ConceptCode.CONCEPT_TAX_ADVANCE, ConceptCode.CONCEPT_TAX_ADVANCE.ToString());
        public static readonly CodeNameRefer REFCON_TAX_RELIEF_PAYER = new CodeNameRefer((uint)ConceptCode.CONCEPT_TAX_RELIEF_PAYER, ConceptCode.CONCEPT_TAX_RELIEF_PAYER.ToString());
        public static readonly CodeNameRefer REFCON_TAX_RELIEF_DISABILITY = new CodeNameRefer((uint)ConceptCode.CONCEPT_TAX_RELIEF_DISABILITY, ConceptCode.CONCEPT_TAX_RELIEF_DISABILITY.ToString());
        public static readonly CodeNameRefer REFCON_TAX_RELIEF_STUDYING = new CodeNameRefer((uint)ConceptCode.CONCEPT_TAX_RELIEF_STUDYING, ConceptCode.CONCEPT_TAX_RELIEF_STUDYING.ToString());
        public static readonly CodeNameRefer REFCON_TAX_RELIEF_CHILD = new CodeNameRefer((uint)ConceptCode.CONCEPT_TAX_RELIEF_CHILD, ConceptCode.CONCEPT_TAX_RELIEF_CHILD.ToString());
        public static readonly CodeNameRefer REFCON_TAX_BONUS_CHILD = new CodeNameRefer((uint)ConceptCode.CONCEPT_TAX_BONUS_CHILD, ConceptCode.CONCEPT_TAX_BONUS_CHILD.ToString());
        public static readonly CodeNameRefer REFCON_TAX_ADVANCE_FINAL = new CodeNameRefer((uint)ConceptCode.CONCEPT_TAX_ADVANCE_FINAL, ConceptCode.CONCEPT_TAX_ADVANCE_FINAL.ToString());
        public static readonly CodeNameRefer REFCON_TAX_WITHHOLD_BASE = new CodeNameRefer((uint)ConceptCode.CONCEPT_TAX_WITHHOLD_BASE, ConceptCode.CONCEPT_TAX_WITHHOLD_BASE.ToString());
        public static readonly CodeNameRefer REFCON_TAX_WITHHOLD = new CodeNameRefer((uint)ConceptCode.CONCEPT_TAX_WITHHOLD, ConceptCode.CONCEPT_TAX_WITHHOLD.ToString());
        public static readonly CodeNameRefer REFCON_INCOME_GROSS = new CodeNameRefer((uint)ConceptCode.CONCEPT_INCOME_GROSS, ConceptCode.CONCEPT_INCOME_GROSS.ToString());
        public static readonly CodeNameRefer REFCON_INCOME_NETTO = new CodeNameRefer((uint)ConceptCode.CONCEPT_INCOME_NETTO, ConceptCode.CONCEPT_INCOME_NETTO.ToString());

        public PayConceptGateway()
        {
            this.Models = new Dictionary<ConceptCode, PayrollConcept>();
            this.Models[ConceptCode.CONCEPT_UNKNOWN] = new UnknownConcept((uint)TagCode.TAG_UNKNOWN, null);
        }

        public IDictionary<ConceptCode, PayrollConcept> Models { get; private set; }
 
        public PayrollConcept ConceptFor(uint tagCode, string conceptName, IDictionary<string, object> values)
        {
            string conceptClass = ClassNameFor(conceptName);
            object[] conceptParams = new object[2] {tagCode, values};
            PayrollConcept conceptInstance = (PayrollConcept)Activator.CreateInstance(Type.GetType(conceptClass), conceptParams);
            return conceptInstance;
        }

        public string ClassNameFor(string codeName)
        {
            Regex regexObj = new Regex("CONCEPT_(.*)", RegexOptions.Singleline);
            Match matchResult = regexObj.Match(codeName);
            string conceptName = "";
            if (matchResult.Success)
            {
                GroupCollection regexCol = matchResult.Groups;
                if (regexCol.Count == 2)
                {
                    conceptName = regexCol[1].Value;
                }
            }
            string className = conceptName.Underscore().Camelize() + "Concept";
            return CONCEPTS_NAMESPACE + className;
        }

        // concept tree
        public PayrollConcept ConceptFromModels(PayrollTag termTag)
        {
            PayrollConcept baseConcept = null;
            if (!Models.ContainsKey((ConceptCode)termTag.ConceptCode()))
            {
                baseConcept = EmptyConceptFor(termTag);
                Models[(ConceptCode)termTag.ConceptCode()] = baseConcept;
            }
            else
            {
                baseConcept = Models[(ConceptCode)termTag.ConceptCode()];
            }
            return baseConcept;
        }

        public PayrollConcept FindConcept(uint conceptCode)
        {
            PayrollConcept baseConcept = null;
            if (Models.ContainsKey((ConceptCode)conceptCode))
            {
                baseConcept = Models[(ConceptCode)conceptCode];
            }
            else
            {
                baseConcept = Models[ConceptCode.CONCEPT_UNKNOWN];
            }
            return baseConcept;
        }

        public PayrollConcept EmptyConceptFor(PayrollTag termTag)
        {
            var emptyValues = new Dictionary<string, object>();
            PayrollConcept emptyConcept = ConceptFor(termTag.Code, termTag.ConceptName(), emptyValues);
            PayrollTag[] emptyPending = RecPendingCodes(emptyConcept.PendingCodes());
            emptyConcept.SetPendingCodes(emptyPending);
            return emptyConcept;
        }
 
        public PayrollTag[] CollectPendingCodesFor(IDictionary<TagRefer, PayrollConcept> termDict)
        {
            IDictionary<TagRefer, PayrollConcept> termsForPending = termDict;

            PayrollTag[] pending = termsForPending.SelectMany((termKvp) => (this.CollectPendingCodesForFunc(termKvp.Key, termKvp.Value))).ToArray();

            PayrollTag[] pendingUnique = pending.Distinct().ToArray();
            return pendingUnique;
        }

        public PayrollTag[] CollectPendingCodesForFunc(TagRefer termRefer, PayrollConcept termConcept)
        {
            PayrollTag[] termPendingCodes = termConcept.TagPendingCodes;
            if (termPendingCodes == null) 
            {
                return new PayrollTag[0];
            }
            else
            {
                return (PayrollTag[])termPendingCodes.Clone();
            }
        }

        public PayrollTag[] RecPendingCodes(PayrollTag[] pendingCodes)
        {
            var pendingInit = pendingCodes.ToArray();

            PayrollTag[] retCodes = pendingCodes.Aggregate(pendingInit,
                (agr, tag) => (agr.Concat(this.PendingCodesForTagCode(tag))).ToArray());

            PayrollTag[] retCodesUnique = retCodes.Distinct().ToArray();
            return retCodesUnique;
        }

        public PayrollTag[] PendingCodesForTagCode(PayrollTag tagRefer)
        {
            PayrollConcept baseConcept = ConceptFromModels(tagRefer);
            if (baseConcept.TagPendingCodes == null) 
            {
                return RecPendingCodes(baseConcept.PendingCodes());
            }
            else
            {
                return baseConcept.TagPendingCodes;
            }
        }
    }
}
