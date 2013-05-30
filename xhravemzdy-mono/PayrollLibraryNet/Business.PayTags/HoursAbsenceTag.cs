using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;
using PayrollLibrary.Business.Core;

namespace PayrollLibrary.Business.PayTags
{
    public class HoursAbsenceTag : PayrollTag
    {
        public HoursAbsenceTag()
            : base(PayTagGateway.REF_HOURS_ABSENCE, PayConceptGateway.REFCON_HOURS_ABSENCE)
        {
        }
    }
}
