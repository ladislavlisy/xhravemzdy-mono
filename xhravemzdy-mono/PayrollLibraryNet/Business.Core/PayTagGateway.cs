using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.PayTags;
using System.Text.RegularExpressions;
using PayrollLibrary.Business.Libs;

namespace PayrollLibrary.Business.Core
{
    public enum TagCode : uint
    {
        TAG_UNKNOWN = 0,
        TAG_SCHEDULE_WORK = 10001,
        TAG_SCHEDULE_TERM = 10002,
        TAG_TIMESHEET_PERIOD = 10003,
        TAG_TIMESHEET_WORK = 10004,
        TAG_HOURS_WORKING = 10005,
        TAG_HOURS_ABSENCE = 10006,
        TAG_SALARY_BASE = 10101,
        TAG_TAX_INCOME_BASE = 30001,
        TAG_INSURANCE_HEALTH_BASE = 30002,
        TAG_INSURANCE_SOCIAL_BASE = 30003,
        TAG_INSURANCE_HEALTH = 30005,
        TAG_INSURANCE_SOCIAL = 30006,
        TAG_SAVINGS_PENSIONS = 30007,
        TAG_TAX_EMPLOYERS_HEALTH = 30008,
        TAG_TAX_EMPLOYERS_SOCIAL = 30009,
        TAG_TAX_CLAIM_PAYER = 30010,
        TAG_TAX_CLAIM_DISABILITY = 30011,
        TAG_TAX_CLAIM_STUDYING = 30014,
        TAG_TAX_CLAIM_CHILD = 30019,
        TAG_TAX_RELIEF_PAYER = 30020,
        TAG_TAX_RELIEF_CHILD = 30029,
        TAG_TAX_ADVANCE_BASE = 30030,
        TAG_TAX_ADVANCE = 30031,
        TAG_TAX_BONUS_CHILD = 30033,
        TAG_TAX_ADVANCE_FINAL = 30034,
        TAG_TAX_WITHHOLD_BASE = 30035,
        TAG_TAX_WITHHOLD = 30036,
        TAG_INCOME_GROSS = 90001,
        TAG_INCOME_NETTO = 90002
    };

    public class PayTagGateway
    {
        public static readonly string TAGS_NAMESPACE = "PayrollLibrary.Business.PayTags.";

        public static readonly CodeNameRefer REF_UNKNOWN = new CodeNameRefer((uint)TagCode.TAG_UNKNOWN, TagCode.TAG_UNKNOWN.ToString());
        public static readonly CodeNameRefer REF_SCHEDULE_WORK = new CodeNameRefer((uint)TagCode.TAG_SCHEDULE_WORK, TagCode.TAG_SCHEDULE_WORK.ToString());
        public static readonly CodeNameRefer REF_SCHEDULE_TERM = new CodeNameRefer((uint)TagCode.TAG_SCHEDULE_TERM, TagCode.TAG_SCHEDULE_TERM.ToString());
        public static readonly CodeNameRefer REF_TIMESHEET_PERIOD = new CodeNameRefer((uint)TagCode.TAG_TIMESHEET_PERIOD, TagCode.TAG_TIMESHEET_PERIOD.ToString());
        public static readonly CodeNameRefer REF_TIMESHEET_WORK = new CodeNameRefer((uint)TagCode.TAG_TIMESHEET_WORK, TagCode.TAG_TIMESHEET_WORK.ToString());
        public static readonly CodeNameRefer REF_HOURS_WORKING = new CodeNameRefer((uint)TagCode.TAG_HOURS_WORKING, TagCode.TAG_HOURS_WORKING.ToString());
        public static readonly CodeNameRefer REF_HOURS_ABSENCE = new CodeNameRefer((uint)TagCode.TAG_HOURS_ABSENCE, TagCode.TAG_HOURS_ABSENCE.ToString());
        public static readonly CodeNameRefer REF_SALARY_BASE = new CodeNameRefer((uint)TagCode.TAG_SALARY_BASE, TagCode.TAG_SALARY_BASE.ToString());
        public static readonly CodeNameRefer REF_TAX_INCOME_BASE = new CodeNameRefer((uint)TagCode.TAG_TAX_INCOME_BASE, TagCode.TAG_TAX_INCOME_BASE.ToString());
        public static readonly CodeNameRefer REF_INSURANCE_HEALTH_BASE = new CodeNameRefer((uint)TagCode.TAG_INSURANCE_HEALTH_BASE, TagCode.TAG_INSURANCE_HEALTH_BASE.ToString());
        public static readonly CodeNameRefer REF_INSURANCE_SOCIAL_BASE = new CodeNameRefer((uint)TagCode.TAG_INSURANCE_SOCIAL_BASE, TagCode.TAG_INSURANCE_SOCIAL_BASE.ToString());
        public static readonly CodeNameRefer REF_INSURANCE_HEALTH = new CodeNameRefer((uint)TagCode.TAG_INSURANCE_HEALTH, TagCode.TAG_INSURANCE_HEALTH.ToString());
        public static readonly CodeNameRefer REF_INSURANCE_SOCIAL = new CodeNameRefer((uint)TagCode.TAG_INSURANCE_SOCIAL, TagCode.TAG_INSURANCE_SOCIAL.ToString());
        public static readonly CodeNameRefer REF_SAVINGS_PENSIONS = new CodeNameRefer((uint)TagCode.TAG_SAVINGS_PENSIONS, TagCode.TAG_SAVINGS_PENSIONS.ToString());
        public static readonly CodeNameRefer REF_TAX_EMPLOYERS_HEALTH = new CodeNameRefer((uint)TagCode.TAG_TAX_EMPLOYERS_HEALTH, TagCode.TAG_TAX_EMPLOYERS_HEALTH.ToString());
        public static readonly CodeNameRefer REF_TAX_EMPLOYERS_SOCIAL = new CodeNameRefer((uint)TagCode.TAG_TAX_EMPLOYERS_SOCIAL, TagCode.TAG_TAX_EMPLOYERS_SOCIAL.ToString());
        public static readonly CodeNameRefer REF_TAX_CLAIM_PAYER = new CodeNameRefer((uint)TagCode.TAG_TAX_CLAIM_PAYER, TagCode.TAG_TAX_CLAIM_PAYER.ToString());
        public static readonly CodeNameRefer REF_TAX_CLAIM_DISABILITY = new CodeNameRefer((uint)TagCode.TAG_TAX_CLAIM_DISABILITY, TagCode.TAG_TAX_CLAIM_DISABILITY.ToString());
        public static readonly CodeNameRefer REF_TAX_CLAIM_STUDYING = new CodeNameRefer((uint)TagCode.TAG_TAX_CLAIM_STUDYING, TagCode.TAG_TAX_CLAIM_STUDYING.ToString());
        public static readonly CodeNameRefer REF_TAX_CLAIM_CHILD = new CodeNameRefer((uint)TagCode.TAG_TAX_CLAIM_CHILD, TagCode.TAG_TAX_CLAIM_CHILD.ToString());
        public static readonly CodeNameRefer REF_TAX_RELIEF_PAYER = new CodeNameRefer((uint)TagCode.TAG_TAX_RELIEF_PAYER, TagCode.TAG_TAX_RELIEF_PAYER.ToString());
        public static readonly CodeNameRefer REF_TAX_RELIEF_CHILD = new CodeNameRefer((uint)TagCode.TAG_TAX_RELIEF_CHILD, TagCode.TAG_TAX_RELIEF_CHILD.ToString());
        public static readonly CodeNameRefer REF_TAX_ADVANCE_BASE = new CodeNameRefer((uint)TagCode.TAG_TAX_ADVANCE_BASE, TagCode.TAG_TAX_ADVANCE_BASE.ToString());
        public static readonly CodeNameRefer REF_TAX_ADVANCE = new CodeNameRefer((uint)TagCode.TAG_TAX_ADVANCE, TagCode.TAG_TAX_ADVANCE.ToString());
        public static readonly CodeNameRefer REF_TAX_BONUS_CHILD = new CodeNameRefer((uint)TagCode.TAG_TAX_BONUS_CHILD, TagCode.TAG_TAX_BONUS_CHILD.ToString());
        public static readonly CodeNameRefer REF_TAX_ADVANCE_FINAL = new CodeNameRefer((uint)TagCode.TAG_TAX_ADVANCE_FINAL, TagCode.TAG_TAX_ADVANCE_FINAL.ToString());
        public static readonly CodeNameRefer REF_TAX_WITHHOLD_BASE = new CodeNameRefer((uint)TagCode.TAG_TAX_WITHHOLD_BASE, TagCode.TAG_TAX_WITHHOLD_BASE.ToString());
        public static readonly CodeNameRefer REF_TAX_WITHHOLD = new CodeNameRefer((uint)TagCode.TAG_TAX_WITHHOLD, TagCode.TAG_TAX_WITHHOLD.ToString());
        public static readonly CodeNameRefer REF_INCOME_GROSS = new CodeNameRefer((uint)TagCode.TAG_INCOME_GROSS, TagCode.TAG_INCOME_GROSS.ToString());
        public static readonly CodeNameRefer REF_INCOME_NETTO = new CodeNameRefer((uint)TagCode.TAG_INCOME_NETTO, TagCode.TAG_INCOME_NETTO.ToString());

        public PayTagGateway()
        {
            this.Models = new Dictionary<TagCode, PayrollTag>();
            this.Models[TagCode.TAG_UNKNOWN] = new UnknownTag();
        }

        public IDictionary<TagCode, PayrollTag> Models { get; private set; }

        public PayrollTag TagFor(string tagCodeName)
        {
            string tagClass = ClassNameFor(tagCodeName);
            PayrollTag tagInstance = (PayrollTag)Activator.CreateInstance(Type.GetType(tagClass));
            return tagInstance;
        }

        public string ClassNameFor(string tagCodeName)
        {
            Regex regexObj = new Regex("TAG_(.*)", RegexOptions.Singleline);
            Match matchResult = regexObj.Match(tagCodeName);
            string tagName = "";
            if (matchResult.Success)
            {
                GroupCollection regexCol = matchResult.Groups;
                if (regexCol.Count == 2)
                {
                    tagName = regexCol[1].Value;
                }
            }
            string className = tagName.Underscore().Camelize() + "Tag";
            return TAGS_NAMESPACE + className;
        }

        // pay tag cache
        public PayrollTag TagFromModels(CodeNameRefer termTag)
        {
            PayrollTag baseTag = null;
            if (!Models.ContainsKey((TagCode)termTag.Code))
            {
                baseTag = EmptyTagFor(termTag);
                Models[(TagCode)termTag.Code] = baseTag;
            }
            else
            {
                baseTag = Models[(TagCode)termTag.Code];
            }
            return baseTag;
        }

        public PayrollTag FindTag(uint tagCode)
        {
            PayrollTag baseTag = null;
            if (Models.ContainsKey((TagCode)tagCode))
            {
                baseTag = Models[(TagCode)tagCode];
            }
            else
            {
                baseTag = Models[TagCode.TAG_UNKNOWN];
            }
            return baseTag;
        }

        public PayrollTag EmptyTagFor(CodeNameRefer termTag)
        {
            PayrollTag emptyTag = TagFor(termTag.Name);
            return emptyTag;
        }
    }
}
