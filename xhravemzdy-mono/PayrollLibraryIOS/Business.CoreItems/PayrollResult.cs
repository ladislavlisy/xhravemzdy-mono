using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PayrollLibrary.Business.CoreItems
{
    public class PayrollResult
    {
        public PayrollResult(uint code, uint conceptCode, PayrollConcept conceptItem)
        {
            this.TagCode = code;
            this.ConceptCode = conceptCode;
            this.Concept = conceptItem;
        }
        
        public uint TagCode { get; private set; }

        public uint ConceptCode { get; private set; }

        public PayrollConcept Concept { get; private set; }

        public bool SummaryFor(uint code)
        {
            uint[] summaryCodes = Concept.SummaryCodes().Select(x => x.Code).ToArray();
            return summaryCodes.Any(x => x==code);
        }

        public void ExportXmlTagRefer(TagRefer tagRefer/*, xmlBuilder*/)
        {
            //attributes = {}
            //attributes[:period_base] = tag_refer.period_base
            //attributes[:code]        = tag_refer.code
            //attributes[:code_order]  = tag_refer.code_order
            //xml_builder.reference(attributes)
        }

        public void ExportXmlConcept(/*xmlBuilder*/)
        {
            Concept.ExportXml(/*xmlBuilder*/);
        }

        public virtual void ExportXmlResult(/*xmlBuilder*/)
        {
        }

        public virtual string ExportValueResult()
        {
            return @"";
        }

        public void ExportXmlNames(PayrollName tagName, PayrollTag tagItem, PayrollConcept tagConcept/*, xmlElement*/)
        {
            var attributes = new Dictionary<string, string>();
            attributes["tag_name"] = tagItem.Name;
            attributes["category"] = tagConcept.Name;

            //xml_element.item(attributes) do |xml_item|
            //  xml_item.title tag_name.title
            //  xml_item.description tag_name.description
            //  xml_item.group(tag_name.get_groups)
            //  export_xml_concept(xml_item)
            //  export_xml_result(xml_item)
            //end
        }

        public void ExportXml(TagRefer tagRefer, PayrollName tagName, PayrollTag tagItem, PayrollConcept tagConcept/*, xmlElement*/)
        {
            ExportXmlTagRefer(tagRefer/*, xmlElement*/);
            ExportXmlNames(tagName, tagItem, tagConcept/*, xmlElement*/);
        }

        public IDictionary<string, string> ExportTitleValue(TagRefer tagRefer, PayrollName tagName, PayrollTag tagItem, PayrollConcept tagConcept)
        {
            return new Dictionary<string, string>() { { "title", tagName.Title }, { "value", ExportValueResult() } };
        }
    }
}
