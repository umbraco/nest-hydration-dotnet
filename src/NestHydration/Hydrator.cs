using System.Collections.Generic;
using System.Linq;

namespace NestHydration
{
    /// <summary>
    /// The Hydrator - converts tabular data to nested objects/arrays based on a definition object
    /// </summary>
    public class Hydrator
    {
        public Hydrator() { }

        public List<Dictionary<string, object>> Nest(List<Dictionary<string, object>> dataSet, Definition definition)
        {
            var result = new List<Dictionary<string, object>>();
            var primaryIdColumn = definition.Properties.First(x => x is Property property && property.IsId) as Property;

            foreach (var row in dataSet)
            {
                var mappedEntry = BuildEntry(primaryIdColumn, row, result);

                foreach (var property in definition.Properties)
                {
                    if (property is Property pId && pId.IsId) continue;
                    if (property is Property p) Extract(p, row, mappedEntry);
                    if (property is PropertyObject po) Extract(po, row, mappedEntry);
                    if (property is PropertyArray pa) Extract(pa, row, mappedEntry);
                }
            }

            return result;
        }

        private void Extract(Property property, Dictionary<string, object> row, Dictionary<string, object> mappedEntry)
        {
            mappedEntry[property.Name] = row[property.Column];
        }

        private void Extract(PropertyObject propertyObject, Dictionary<string, object> row, Dictionary<string, object> mappedEntry)
        {
            var newEntry = new Dictionary<string, object>();
            foreach (var property in propertyObject.Properties)
            {
                if (property is Property primaryIdColumn && primaryIdColumn.IsId && row[primaryIdColumn.Column] == null)
                {
                    mappedEntry[propertyObject.Name] = null;
                    return;
                }

                if (property is Property p) Extract(p, row, newEntry);
                if (property is PropertyObject po) Extract(po, row, newEntry);
                if (property is PropertyArray pa) Extract(pa, row, newEntry);
            }
            mappedEntry[propertyObject.Name] = newEntry;
        }

        private void Extract(PropertyArray propertyArray, Dictionary<string, object> row, Dictionary<string, object> mappedEntry)
        {
            var primaryIdColumn = propertyArray.Properties.First(x => x is Property property && property.IsId) as Property;
            var entryExists = mappedEntry.ContainsKey(propertyArray.Name);
            var list = entryExists
                ? mappedEntry[propertyArray.Name] as List<Dictionary<string, object>>
                : new List<Dictionary<string, object>>();

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

        private Dictionary<string, object> BuildEntry(Property primaryIdColumn, Dictionary<string, object> row, List<Dictionary<string, object>> result)
        {
            var value = row[primaryIdColumn.Column];
            var existingEntry = result.FirstOrDefault(x =>
                x != null && x.ContainsKey(primaryIdColumn.Name) && x.ContainsValue(value));
            if (existingEntry != null)
                return existingEntry;

            var newEntry = new Dictionary<string, object> {{primaryIdColumn.Name, value}};
            result.Add(newEntry);
            return newEntry;
        }
    }
}
