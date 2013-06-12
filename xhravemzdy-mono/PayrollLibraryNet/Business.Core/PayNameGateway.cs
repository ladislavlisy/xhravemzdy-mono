using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.PayTags;
using System.Text.RegularExpressions;
using PayrollLibrary.Business.Libs;
using PayrollLibrary.Business.PayTagNames;

namespace PayrollLibrary.Business.Core
{
    public class PayNameGateway
    {
        public static readonly string VPAYGRP_UNKNOWN = null;
        public static readonly string HPAYGRP_UNKNOWN = null;

        public static readonly string VPAYGRP_SCHEDULE   = "VPAYGRP_SCHEDULE";
        public static readonly string VPAYGRP_PAYMENTS   = "VPAYGRP_PAYMENTS";
        public static readonly string VPAYGRP_TAX_SOURCE = "VPAYGRP_TAX_SOURCE";
        public static readonly string VPAYGRP_TAX_RESULT = "VPAYGRP_TAX_RESULT";
        public static readonly string VPAYGRP_INS_RESULT = "VPAYGRP_INS_RESULT";
        public static readonly string VPAYGRP_TAX_INCOME = "VPAYGRP_TAX_INCOME";
        public static readonly string VPAYGRP_INS_INCOME = "VPAYGRP_INS_INCOME";
        public static readonly string VPAYGRP_SUMMARY    = "VPAYGRP_SUMMARY";

        public static readonly string TAGNAMES_NAMESPACE = "PayrollLibrary.Business.PayTagNames.";

        public PayNameGateway()
        {
            this.Models = new Dictionary<TagCode, PayrollName>();
            this.Models[TagCode.TAG_UNKNOWN] = new UnknownName();
        }

        public void LoadModels()
        {
            NameFromModels(PayTagGateway.REF_SCHEDULE_WORK);
            NameFromModels(PayTagGateway.REF_SCHEDULE_TERM);
            NameFromModels(PayTagGateway.REF_TIMESHEET_PERIOD);
            NameFromModels(PayTagGateway.REF_TIMESHEET_WORK);
            NameFromModels(PayTagGateway.REF_HOURS_WORKING);
            NameFromModels(PayTagGateway.REF_HOURS_ABSENCE);
            NameFromModels(PayTagGateway.REF_SALARY_BASE);
            NameFromModels(PayTagGateway.REF_TAX_INCOME_BASE);
            NameFromModels(PayTagGateway.REF_INSURANCE_HEALTH_BASE);
            NameFromModels(PayTagGateway.REF_INSURANCE_SOCIAL_BASE);
            NameFromModels(PayTagGateway.REF_INSURANCE_HEALTH);
            NameFromModels(PayTagGateway.REF_INSURANCE_SOCIAL);
            NameFromModels(PayTagGateway.REF_SAVINGS_PENSIONS);
            NameFromModels(PayTagGateway.REF_TAX_EMPLOYERS_HEALTH);
            NameFromModels(PayTagGateway.REF_TAX_EMPLOYERS_SOCIAL);
            NameFromModels(PayTagGateway.REF_TAX_CLAIM_PAYER);
            NameFromModels(PayTagGateway.REF_TAX_CLAIM_DISABILITY);
            NameFromModels(PayTagGateway.REF_TAX_CLAIM_STUDYING);
            NameFromModels(PayTagGateway.REF_TAX_CLAIM_CHILD);
            NameFromModels(PayTagGateway.REF_TAX_RELIEF_PAYER);
            NameFromModels(PayTagGateway.REF_TAX_RELIEF_CHILD);
            NameFromModels(PayTagGateway.REF_TAX_ADVANCE_BASE);
            NameFromModels(PayTagGateway.REF_TAX_ADVANCE);
            NameFromModels(PayTagGateway.REF_TAX_BONUS_CHILD);
            NameFromModels(PayTagGateway.REF_TAX_ADVANCE_FINAL);
            NameFromModels(PayTagGateway.REF_TAX_WITHHOLD_BASE);
            NameFromModels(PayTagGateway.REF_TAX_WITHHOLD);
            NameFromModels(PayTagGateway.REF_INCOME_GROSS);
            NameFromModels(PayTagGateway.REF_INCOME_NETTO);
        }

        public IDictionary<TagCode, PayrollName> Models { get; private set; }

        public PayrollName NameFor(string tagCodeName)
        {
            string nameClass = ClassNameFor(tagCodeName);
            PayrollName nameInstance = (PayrollName)Activator.CreateInstance(Type.GetType(nameClass));
            return nameInstance;
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
            string className = tagName.Underscore().Camelize() + "Name";
            return TAGNAMES_NAMESPACE + className;
        }

        // pay tag cache
        public PayrollName NameFromModels(CodeNameRefer termTag)
        {
            PayrollName baseTag = null;
            if (!Models.ContainsKey((TagCode)termTag.Code))
            {
                baseTag = EmptyNameFor(termTag);
                Models[(TagCode)termTag.Code] = baseTag;
            }
            else
            {
                baseTag = Models[(TagCode)termTag.Code];
            }
            return baseTag;
        }

        public PayrollName FindName(uint tagCode)
        {
            PayrollName baseTag = null;
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

        public PayrollName EmptyNameFor(CodeNameRefer termTag)
        {
            PayrollName emptyTag = NameFor(termTag.Name);
            return emptyTag;
        }
    }
}
