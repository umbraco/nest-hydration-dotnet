[![Build Status](https://dev.azure.com/umbraco/Umbraco%20Headless/_apis/build/status/umbraco.nest-hydration-dotnet?branchName=master)](https://dev.azure.com/umbraco/Umbraco%20Headless/_build/latest?definitionId=262&branchName=master)
[![NuGet version (NestHydration)](https://img.shields.io/nuget/v/NestHydration.svg?style=flat-square)](https://www.nuget.org/packages/NestHydration/)


# Nest Hydration for .NET

A .NET implementation of the [NestHydrationJS](https://github.com/CoursePark/NestHydrationJS) library. Credit goes to the creators and maintainers of that project for the idea and sample data.

The current 0.1.0 release is an initial implementation, which we will continue to expand on. This means that, currently, types in the dataset remain the same, but we expect to add some kind of type conversation in the future. Performance and allocations is also an area we want to improve. Ideas and suggestions are always welcome.

## Introduction

Converts tabular data into a nested object/array structure based on a definition object or specially named columns.

## Tabular Data With Definition Object

Tabular data is considered to be an array of objects where each object represents a row and the properties of those objects are cell values with the keys representing the column names.

### Tabular Data

```javascript
var table = [
    {
        id: '1', title: 'Tabular to Objects', required: '1',
        teacher_id: '1', teacher_name: 'David',
        lesson_id: '1', lesson_title: 'Definitions'
    },
    {
        id: '1', title: 'Tabular to Objects', required: '1',
        teacher_id: '1', teacher_name: 'David',
        lesson_id: '2', lesson_title: 'Table Data'
    },
    {
        id: '1', title: 'Tabular to Objects', required: '1',
        teacher_id: '1', teacher_name: 'David',
        lesson_id: '3', lesson_title: 'Objects'
    },
    {
        id: '2', title: 'Column Names Define Structure', required: '0',
        teacher_id: '2', teacher_name: 'Chris',
        lesson_id: '4', lesson_title: 'Column Names'
    },
    {
        id: '2', title: 'Column Names Define Structure', required: '0',
        teacher_id: '2', teacher_name: 'Chris',
        lesson_id: '2', lesson_title: 'Table Data'
    },
    {
        id: '2', title: 'Column Names Define Structure', required: '0',
        teacher_id: '2', teacher_name: 'Chris',
        lesson_id: '3', lesson_title: 'Objects'
    },
    {
        id: '3', title: 'Object On Bottom', required: '0',
        teacher_id: '1', teacher_name: 'David',
        lesson_id: '5', lesson_title: 'Non Array Input'
    }
];
```

Above are 7 rows each with the cell data for the columns `id`, `title`, `required`, `teacher_id`, `teacher_name`, `lesson_id`, `lesson_title`.

Mapping from the property keys of the tabular data to nested objects is done in accordance to the definition object.

Same data table as a C# object
```csharp
var data = new List<Dictionary<string, object>>();
data.Add(new Dictionary<string, object>
{
    { "id", 1 },
    { "title", "Tabular to Objects" },
    { "required", true },
    { "teacher_id", 1 },
    { "teacher_name", "David" },
    { "lesson_id", 1 },
    { "lesson_title", "Definitions" }
});
data.Add(new Dictionary<string, object>
{
    { "id", 1 },
    { "title", "Tabular to Objects" },
    { "required", true },
    { "teacher_id", 1 },
    { "teacher_name", "David" },
    { "lesson_id", 2 },
    { "lesson_title", "Table Data" }
});
data.Add(new Dictionary<string, object>
{
    { "id", 1 },
    { "title", "Tabular to Objects" },
    { "required", true },
    { "teacher_id", 1 },
    { "teacher_name", "David" },
    { "lesson_id", 3 },
    { "lesson_title", "Objects" }
});
data.Add(new Dictionary<string, object>
{
    { "id", 2 },
    { "title", "Column Names Define Structure" },
    { "required", false },
    { "teacher_id", 2 },
    { "teacher_name", "Chris" },
    { "lesson_id", 4 },
    { "lesson_title", "Column Names" }
});
data.Add(new Dictionary<string, object>
{
    { "id", 2 },
    { "title", "Column Names Define Structure" },
    { "required", false },
    { "teacher_id", 2 },
    { "teacher_name", "Chris" },
    { "lesson_id", 2 },
    { "lesson_title", "Table Data" }
});
data.Add(new Dictionary<string, object>
{
    { "id", 2 },
    { "title", "Column Names Define Structure" },
    { "required", false },
    { "teacher_id", 2 },
    { "teacher_name", "Chris" },
    { "lesson_id", 3 },
    { "lesson_title", "Objects" }
});
data.Add(new Dictionary<string, object>
{
    { "id", 3 },
    { "title", "Object On Bottom" },
    { "required", true },
    { "teacher_id", 1 },
    { "teacher_name", "David" },
    { "lesson_id", 5 },
    { "lesson_title", "Non Array Input" }
});
```

### Definition

The definition as a strongly typed object.
```csharp
var definition = new Definition();
definition.Properties.Add(new Property("id", "id", true));
definition.Properties.Add(new Property("title"));
definition.Properties.Add(new Property("required", "required"));
definition.Properties.Add(new PropertyObject("teacher",
    new Properties {
        new Property("id", "teacher_id", true),
        new Property("name", "teacher_name") }
));
definition.Properties.Add(new PropertyArray("lesson",
    new Properties {
        new Property("id", "lesson_id", true),
        new Property("title", "lesson_title")
    }
));
```

### Transformation

Use the following two lines of code to transform the tabular data into a nested object/array based on the definition.
```csharp
var hydrator = new Hydrator();
var result = hydrator.Nest(data, definition);
```

### Result

The result as it would look if defined as a C# object
```csharp
var expected = new List<Dictionary<string, object>>();
expected.Add(new Dictionary<string, object>
{
    { "id", 1 },
    { "title", "Tabular to Objects" },
    { "required", true },
    { "teacher", new Dictionary<string, object>{ {"id", 1}, {"name", "David"} } },
    { "lesson", new List<Dictionary<string, object>> {
        new Dictionary<string, object> { { "id", 1 }, {"title", "Definitions" } },
        new Dictionary<string, object> { { "id", 2 }, {"title", "Table Data"} },
        new Dictionary<string, object> { { "id", 3 }, {"title", "Objects"} }
    } }
});
expected.Add(new Dictionary<string, object>
{
    { "id", 2 },
    { "title", "Column Names Define Structure" },
    { "required", false },
    { "teacher", new Dictionary<string, object>{ {"id", 2}, {"name", "Chris"} } },
    { "lesson", new List<Dictionary<string, object>> {
        new Dictionary<string, object> { { "id", 4 }, {"title", "Column Names"} },
        new Dictionary<string, object> { { "id", 2 }, {"title", "Table Data"} },
        new Dictionary<string, object> { { "id", 3 }, {"title", "Objects"} }
    } }
});
expected.Add(new Dictionary<string, object>
{
    { "id", 3 },
    { "title", "Object On Bottom" },
    { "required", true },
    { "teacher", new Dictionary<string, object>{ {"id", 1}, {"name", "David"} } },
    { "lesson", new List<Dictionary<string, object>> {
        new Dictionary<string, object> { { "id", 5 }, {"title", "Non Array Input" } }
    } }
});
```
