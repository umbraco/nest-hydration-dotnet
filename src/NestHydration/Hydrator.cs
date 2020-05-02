using System.Collections.Generic;
using System.Linq;

namespace NestHydration
{
    /// <summary>
    /// 
    /// </summary>
    public class Hydrator
    {
        private readonly Definition _definition;
        private readonly List<Dictionary<string, Maybe<object>>> _dataSet;

        public Hydrator(List<Dictionary<string, Maybe<object>>> dataSet, Definition definition)
        {
            _dataSet = dataSet;
            _definition = definition;
        }

        public List<Dictionary<string, Maybe<object>>> Execute()
        {
            var result = new List<Dictionary<string, Maybe<object>>>();
            var primaryIdColumn = _definition.Properties.First(x => x is Property property && property.IsId) as Property;

            foreach (var row in _dataSet)
            {
                var mappedEntry = BuildEntry(primaryIdColumn, row, result);

                foreach (var property in _definition.Properties)
                {
                    if (property is Property pId && pId.IsId) continue;
                    if (property is Property p) Extract(p, row, mappedEntry);
                    if (property is PropertyObject po) Extract(po, row, mappedEntry);
                    if (property is PropertyArray pa) Extract(pa, row, mappedEntry);
                }
            }

            return result;
        }

        private void Extract(Property property, Dictionary<string, Maybe<object>> row, Dictionary<string, Maybe<object>> mappedEntry)
        {
            mappedEntry[property.Name] = row[property.Column];
        }

        private void Extract(PropertyObject propertyObject, Dictionary<string, Maybe<object>> row, Dictionary<string, Maybe<object>> mappedEntry)
        {
            var newEntry = new Dictionary<string, Maybe<object>>();
            foreach (var property in propertyObject.Properties)
            {
                if (property is Property p) Extract(p, row, newEntry);
                if (property is PropertyObject po) Extract(po, row, newEntry);
                if (property is PropertyArray pa) Extract(pa, row, newEntry);
            }
            mappedEntry[propertyObject.Name] = newEntry;
        }

        private void Extract(PropertyArray propertyArray, Dictionary<string, Maybe<object>> row, Dictionary<string, Maybe<object>> mappedEntry)
        {
            var primaryIdColumn = propertyArray.Properties.First(x => x is Property property && property.IsId) as Property;
            var entryExists = mappedEntry.ContainsKey(propertyArray.Name);
            var list = entryExists
                ? mappedEntry[propertyArray.Name].Value as List<Dictionary<string, Maybe<object>>>
                : new List<Dictionary<string, Maybe<object>>>();

            var mapped = BuildEntry(primaryIdColumn, row, list);
            foreach (var property in propertyArray.Properties)
            {
                if (property is Property pId && pId.IsId) continue;
                if (property is Property p) Extract(p, row, mapped);
                if (property is PropertyObject po) Extract(po, row, mapped);
                if (property is PropertyArray pa) Extract(pa, row, mapped);
            }

            if(entryExists) return;
            mappedEntry[propertyArray.Name] = list;
        }

        private Dictionary<string, Maybe<object>> BuildEntry(Property primaryIdColumn, Dictionary<string, Maybe<object>> row, List<Dictionary<string, Maybe<object>>> result)
        {
            var value = row[primaryIdColumn.Column];
            var existingEntry = result.FirstOrDefault(x => x.ContainsKey(primaryIdColumn.Name)
                                                           && x.ContainsValue(value));
            if (existingEntry != null)
                return existingEntry;

            var newEntry = new Dictionary<string, Maybe<object>> {{primaryIdColumn.Name, value}};
            result.Add(newEntry);
            return newEntry;
        }
    }
}
