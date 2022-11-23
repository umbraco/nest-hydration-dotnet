using System;
using System.Collections.Concurrent;
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

        public IEnumerable<IDictionary<string, object?>> Nest(IEnumerable<IDictionary<string, object?>> dataSet, Definition definition)
        {
            var result = new List<IDictionary<string, object?>>();
            var primaryIdColumn = definition.Properties.First(x => x is Property property && property.IsId) as Property;
            if (primaryIdColumn == null) throw new InvalidOperationException("Definition did not contain a Property with IsId=true.");

            foreach (var row in dataSet)
            {
                var mappedEntry = BuildEntry(primaryIdColumn, row, result);
                if(mappedEntry == null) continue;

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

        private void Extract(Property property, IDictionary<string, object?> row, IDictionary<string, object?> mappedEntry)
        {
            mappedEntry[property.Name] = row[property.Column];
        }

        private void Extract(PropertyObject propertyObject, IDictionary<string, object?> row, IDictionary<string, object?> mappedEntry)
        {
            var newEntry = new ConcurrentDictionary<string, object?>();
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

        private void Extract(PropertyArray propertyArray, IDictionary<string, object?> row, IDictionary<string, object?> mappedEntry)
        {
            var primaryIdColumn = propertyArray.Properties.First(x => x is Property property && property.IsId) as Property;
            if (primaryIdColumn == null) throw new InvalidOperationException("Definition did not contain a Property with IsId=true.");

            if (mappedEntry.TryGetValue(propertyArray.Name, out var exising) is false || exising is not List<IDictionary<string, object?>> list)
                list = new List<IDictionary<string, object?>>();

            var mapped = BuildEntry(primaryIdColumn, row, list);
            if (mapped == null)
            {
                mappedEntry[propertyArray.Name] = null;
                return;
            }

            foreach (var property in propertyArray.Properties)
            {
                if (property is Property pId && pId.IsId) continue;
                if (property is Property p) Extract(p, row, mapped);
                if (property is PropertyObject po) Extract(po, row, mapped);
                if (property is PropertyArray pa) Extract(pa, row, mapped);
            }

            if(exising is not null) return;
            mappedEntry[propertyArray.Name] = list;
        }

        private IDictionary<string, object?>? BuildEntry(Property primaryIdColumn, IDictionary<string, object?> row, List<IDictionary<string, object?>> result)
        {
            var value = row[primaryIdColumn.Column];
            if (value == null)
                return null;

            var existingEntry = result.FirstOrDefault(x =>
                x != null && x.ContainsKey(primaryIdColumn.Name) && x.Values.Contains(value));
            if (existingEntry != null)
                return existingEntry;

            var newEntry = new ConcurrentDictionary<string, object?>();
            newEntry.TryAdd(primaryIdColumn.Name, value);
            result.Add(newEntry);
            return newEntry;
        }
    }
}
