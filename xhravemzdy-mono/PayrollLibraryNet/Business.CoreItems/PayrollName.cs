using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PayrollLibrary.Business.CoreItems
{
    public class PayrollName : CodeNameRefer
    {
        public readonly string VGRP_POS = "vgrp_pos";
        public readonly string HGRP_POS = "hgrp_pos";

        public PayrollName(CodeNameRefer tagRefer, string title, string description, string vertGroup, string horizGroup) : base(tagRefer.Code, tagRefer.Name)
        {
            this.XmlGroups = new Dictionary<string, string>() { };

            if (vertGroup != null)
            {
                var vertToAdd  = new Dictionary<string, string>() { { VGRP_POS, vertGroup } };
                var dictMerge = this.XmlGroups.Union(vertToAdd).ToDictionary(key => key.Key, value => value.Value);
                this.XmlGroups = new Dictionary<string, string>( dictMerge ) ;
            }
            if (horizGroup != null)
            {
                var vertToAdd = new Dictionary<string, string>() { { HGRP_POS, horizGroup } };
                var dictMerge = this.XmlGroups.Union(vertToAdd).ToDictionary(key => key.Key, value => value.Value);
                this.XmlGroups = new Dictionary<string, string>(dictMerge);
            }
        }

        public string Title { get; private set; }
        
        public string Description { get; private set; }

        private Dictionary<string, string> XmlGroups { get; set; }
    
        public bool isMatchVGroup(string groupCode) 
        {
            return XmlGroups[VGRP_POS].Equals(groupCode);
        }

        public bool isMatchHGroup(string groupCode)
        {
            return XmlGroups[HGRP_POS].Equals(groupCode);
        }
    }
}
