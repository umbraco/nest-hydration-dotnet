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
            var primaryIdColumn = definition.Properties.FirstOrDefault(x => x is Property property && property.IsId) as Property;
            if (primaryIdColumn == null) throw new InvalidOperationException("Definition did not contain a Property with IsId=true.");

            var result = new List<IDictionary<string, object?>>();
            var cache = new Dictionary<string, IDictionary<string, object?>>();

            foreach (var row in dataSet)
            {
                var mappedEntry = BuildEntry(primaryIdColumn, row, cache, null, out var isNew);
                if (mappedEntry is null) continue;

                var parentCacheKey = $"{row[primaryIdColumn.Column]}";
                foreach (var property in definition.Properties)
                {
                    if (property is Property pId && pId.IsId) continue;
                    if (property is Property p) Extract(p, row, mappedEntry);
                    else if (property is PropertyObject po) Extract(po, row, mappedEntry, cache, parentCacheKey);
                    else if (property is PropertyArray pa) Extract(pa, row, mappedEntry, cache, parentCacheKey);
                }

                if (isNew) result.Add(mappedEntry);
            }
            return result;
        }

        private void Extract(Property property, IDictionary<string, object?> row, IDictionary<string, object?> mappedEntry)
        {
            mappedEntry[property.Name] = row[property.Column];
        }

        private void Extract(PropertyObject propertyObject, IDictionary<string, object?> row, IDictionary<string, object?> mappedEntry, Dictionary<string, IDictionary<string, object?>> cache, string parentCacheKey)
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
                else if (property is PropertyObject po) Extract(po, row, newEntry, cache, parentCacheKey);
                else if (property is PropertyArray pa) Extract(pa, row, newEntry, cache, parentCacheKey);
            }
            mappedEntry[propertyObject.Name] = newEntry;
        }

        private void Extract(PropertyArray propertyArray, IDictionary<string, object?> row, IDictionary<string, object?> mappedEntry, Dictionary<string, IDictionary<string, object?>> cache, string parentCacheKey)
        {
            var primaryIdColumn = propertyArray.Properties.FirstOrDefault(x => x is Property property && property.IsId) as Property;
            if (primaryIdColumn == null) throw new InvalidOperationException("Definition did not contain a Property with IsId=true.");

            if (mappedEntry.TryGetValue(propertyArray.Name, out var existing) is false || existing is not List<IDictionary<string, object?>> list)
                list = new List<IDictionary<string, object>>();

            var mapped = BuildEntry(primaryIdColumn, row, cache, parentCacheKey, out var isNew);
            if (mapped is null)
            {
                mappedEntry[propertyArray.Name] = null;
                return;
            }

            if (isNew) list.Add(mapped);

            foreach (var property in propertyArray.Properties)
            {
                if (property is Property pId && pId.IsId) continue;
                if (property is Property p) Extract(p, row, mapped);
                else if (property is PropertyObject po) Extract(po, row, mapped, cache, $"{parentCacheKey}__{mapped[primaryIdColumn.Column]}");
                else if (property is PropertyArray pa) Extract(pa, row, mapped, cache, $"{parentCacheKey}__{mapped[primaryIdColumn.Column]}");
            }

            if (existing is not null) return;
            mappedEntry[propertyArray.Name] = list;
        }

        private IDictionary<string, object?>? BuildEntry(Property primaryIdColumn, IDictionary<string, object?> row, Dictionary<string, IDictionary<string, object?>> cache, string? parentCacheKey, out bool isNew)
        {
            isNew = false;
            var id = row[primaryIdColumn.Column];
            if (id is null)
                return null;

            var cacheKey = $"{parentCacheKey}__{id}";
            if (cache.TryGetValue(cacheKey, out var existing))
                return existing;

            var newEntry = new ConcurrentDictionary<string, object?>();
            newEntry[primaryIdColumn.Name] = id;
            cache.Add(cacheKey, newEntry);
            isNew = true;
            return newEntry;
        }
    }
}
