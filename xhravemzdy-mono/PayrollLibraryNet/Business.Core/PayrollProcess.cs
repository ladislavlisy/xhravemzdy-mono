using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PayrollLibrary.Business.CoreItems;

namespace PayrollLibrary.Business.Core
{
    public class PayrollProcess
    {
        public PayrollProcess(PayTagGateway tags, PayConceptGateway concepts, PayrollPeriod period)
        {
            this.Tags = tags;

            this.Concepts = concepts;

            this.Period = period;

            this.Terms = new Dictionary<TagRefer, PayrollConcept>();

            this.Results = new Dictionary<TagRefer, PayrollResult>();
        }

        public PayTagGateway Tags { get; private set; }

        public PayConceptGateway Concepts { get; private set; }

        public PayrollPeriod Period  { get; private set; }

        public IDictionary<TagRefer, PayrollConcept> Terms { get; private set;}

        public IDictionary<TagRefer, PayrollResult> Results { get; private set; }

        public PayrollTag FindTag(uint tagCode)
        {
            return Tags.FindTag(tagCode);
        }

        public PayrollConcept FindConcept(uint conceptCode)
        {
            return Concepts.FindConcept(conceptCode);
        }

        //  add fetched term
        public TagRefer InsTerm(uint periodBase, CodeNameRefer termRefer, uint codeOrder, Dictionary<string, object> termValues)
        {
            return InsTermToHash(Terms, periodBase, termRefer, codeOrder, termValues);
        }

        private TagRefer InsTermToHash(IDictionary<TagRefer, PayrollConcept> termHash, uint periodBase, CodeNameRefer termRefer, uint codeOrder, IDictionary<string, object> termValues)
        {
            KeyValuePair<TagRefer, PayrollConcept> termToInsert = NewTermPairWithOrder(periodBase, termRefer, codeOrder, termValues);
            termHash.Add(termToInsert.Key, termToInsert.Value);
            return termToInsert.Key;
        }

        // add a new term
        public TagRefer AddTerm(CodeNameRefer termRefer, Dictionary<string, object> termValues)
        {
            uint periodBase = PayrollPeriod.NOW;
            return AddTermToHash(Terms, periodBase, termRefer, termValues);
        }

        private TagRefer AddTermToHash(IDictionary<TagRefer, PayrollConcept> termHash, uint periodBase, CodeNameRefer termRefer, IDictionary<string, object> termValues)
        {
            KeyValuePair<TagRefer, PayrollConcept> termToAdd = NewTermPair(termHash, periodBase, termRefer, termValues);
            termHash.Add(termToAdd.Key, termToAdd.Value);
            return termToAdd.Key;
        }

        // add term for a new or ignore for existing term
        private IDictionary<TagRefer, PayrollConcept> MergeTermToHash(IDictionary<TagRefer, PayrollConcept> termHash, uint periodBase, PayrollTag termRefer, IDictionary<string, object> termValues)
        {
            uint mergeCodeOrder = GetTagOrderFrom(termHash, periodBase, termRefer.Code);
            KeyValuePair<TagRefer, PayrollConcept> termToMerge = NewTermPairWithOrder(periodBase, termRefer, mergeCodeOrder, termValues);
            if (!termHash.ContainsKey(termToMerge.Key))
            {
                termHash.Add(termToMerge.Key, termToMerge.Value);
            }
            ////term_hash.merge!(term_to_merge) do |tag, term_concept, _|
            ////  term_concept
            ////end
            //var hashToMerge = new Dictionary<TagRefer, PayrollConcept>() { { termToMerge.Key, termToMerge.Value } };
            //var hashMerged = termHash.Union(hashToMerge).ToDictionary(key => key.Key, value => value.Value);
            //return hashMerged;
            return termHash;
        }
  
        // get term from Terms by key of tag
        public IDictionary<TagRefer, PayrollConcept> GetTerm(TagRefer payTag)
        {
            return Terms.Where( tag => tag.Key==payTag).ToDictionary( key => key.Key, value => value.Value);
        }

        // get term from Results by key of tag
        public IDictionary<TagRefer, PayrollResult> GetResult(TagRefer payTag)
        {
            return Results.Where( tag => tag.Key==payTag).ToDictionary( key => key.Key, value => value.Value);
        }

        // get term from Results by key of tag
        public IDictionary<TagRefer, PayrollResult> GetResults()
        {
            return Results;
        }

        // evaluate and return result from Results by key of tag
        public IDictionary<TagRefer, PayrollResult> Evaluate(TagRefer payTag)
        {
            uint periodBase = PayrollPeriod.NOW;

            PayrollTag[] pendingUniqCodes = Concepts.CollectPendingCodesFor(Terms);

            var calculationSteps = CreateCalculationSteps(Terms, periodBase, pendingUniqCodes);

            var sortedCalculation = calculationSteps.OrderBy( x => (x.Value) );

            this.Results = sortedCalculation.Aggregate(new Dictionary<TagRefer, PayrollResult>(), 
                (agr, x) => (agr.Union(new Dictionary<TagRefer, PayrollResult>() { 
                    { x.Key, x.Value.Evaluate(Period, Tags, agr) } 
                    }).ToDictionary(key => key.Key, value => value.Value)));
            return GetResult(payTag);
        }

        private TagRefer NewTermKeyWithOrder(uint periodBase, CodeNameRefer termRefer, uint codeOrder)
        {
            TagRefer termKey = new TagRefer(periodBase, termRefer.Code, codeOrder);
            return termKey;
        }

        private PayrollConcept NewTermConcept(CodeNameRefer termRefer, IDictionary<string, object> termValues)
        {
            PayrollTag termTag = Tags.TagFromModels(termRefer);
            PayrollConcept baseConcept = Concepts.ConceptFromModels(termTag);
            PayrollConcept termConcept = baseConcept.CloneWithValue(termTag.Code, termValues);
            return termConcept; 
        }

        // create pair of TagRefer and PayrollConcept with code order
        private KeyValuePair<TagRefer, PayrollConcept> NewTermPairWithOrder(uint periodBase, CodeNameRefer termRefer, uint codeOrder, IDictionary<string, object> termValues)
        {
            TagRefer termKey = NewTermKeyWithOrder(periodBase, termRefer, codeOrder);
            PayrollConcept termConcept = NewTermConcept(termRefer, termValues);
            return new KeyValuePair<TagRefer, PayrollConcept>(termKey, termConcept);
        }

        private KeyValuePair<TagRefer, PayrollConcept> NewTermPair(IDictionary<TagRefer, PayrollConcept> termHash, uint periodBase, CodeNameRefer termRefer, IDictionary<string, object> termValues)
        {
            uint newCodeOrder = GetNewTagOrderFrom(termHash, periodBase, termRefer.Code);
            TagRefer termKey = NewTermKeyWithOrder(periodBase, termRefer, newCodeOrder);
            PayrollConcept termConcept = NewTermConcept(termRefer, termValues);
            return new KeyValuePair<TagRefer, PayrollConcept>(termKey, termConcept);
        }

        // get a new code order
        public uint GetNewTagOrder(uint periodBase, uint code)
        {
            return GetNewTagOrderFrom(Terms, periodBase, code);
        }

        private uint GetNewTagOrderFrom(IDictionary<TagRefer, PayrollConcept> termHash, uint periodBase, uint code)
        {
            return GetNewTagOrderInArray(termHash.Keys, periodBase, code);
        }

        private uint GetNewTagOrderInArray(ICollection<TagRefer> keysArray, uint periodBase, uint code)
        {
            ICollection<TagRefer> selectedTags = SelectTagsForCode(keysArray, periodBase, code);
            uint[] mappedOrders = MapTagsToCodeOrders(selectedTags);
            return GetNewOrderFrom(mappedOrders.OrderBy(x => x).ToArray());
         }

        private uint GetTagOrderFrom(IDictionary<TagRefer, PayrollConcept> termHash, uint periodBase, uint code)
        {
            return GetTagOrderInArray(termHash.Keys, periodBase, code);
        }

        private uint GetTagOrderInArray(ICollection<TagRefer> keysArray, uint periodBase, uint code)
        {
            ICollection<TagRefer> selectedTags = SelectTagsForCode(keysArray, periodBase, code);
            uint[] mappedOrders = MapTagsToCodeOrders(selectedTags);
            return GetFirstOrderFrom(mappedOrders.OrderBy(x => x).ToArray());
        }

        private uint[] MapTagsToCodeOrders(ICollection<TagRefer> keysArray)
        {
            return keysArray.Select(x => x.CodeOrder).ToArray();
        }

        private ICollection<TagRefer> SelectTagsForCode(ICollection<TagRefer> keysArray, uint periodBase, uint code)
        {
            return keysArray.Where(x => (x.PeriodBase == periodBase && x.Code == code)).ToArray();
        }

        private uint GetNewOrderFrom(uint[] ordersSorted)
        {
            uint lastCodeOrder = ordersSorted.Aggregate((uint)0, (agr, x) => (((x > agr) && (x - agr) > 1) ? agr : x));
            return (lastCodeOrder + 1);
        }

        uint GetFirstOrderFrom(uint[] ordersSorted)
        {
            uint firstCodeOrder = 1;
            if (ordersSorted.Length != 0)
            {
                firstCodeOrder = ordersSorted.ElementAt(0);
            }
            return firstCodeOrder;
        }

        PayrollConcept ConceptFor(PayrollTag code)
        {
            var emptyValues = new Dictionary<string, object>();
            PayrollConcept concept = Concepts.ConceptFor(code.ConceptCode(), code.ConceptName(), emptyValues);
            return concept;
        }

        IDictionary<TagRefer, PayrollConcept> CreateCalculationSteps(IDictionary<TagRefer, PayrollConcept> termHash, uint periodBase, PayrollTag[] pendingCodes)
        {
            var emptyValues = new Dictionary<string, object>();
            var pendingInit = termHash.ToDictionary(
                // Typically no cloning necessary (immuable)
                key => key.Key, 
                // Do the copy how you want
                value => (PayrollConcept)value.Value.Clone());

            var calculationSteps = pendingCodes.Aggregate(pendingInit, 
                (agr, code) => (this.MergeTermToHash(agr, periodBase, code, emptyValues).ToDictionary(key => key.Key, value => value.Value)));
            return calculationSteps;
        }
    }
}
