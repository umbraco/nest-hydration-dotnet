using System.Collections.Generic;
using FluentAssertions;
using FluentAssertions.Equivalency;
using Xunit;

namespace NestHydration.Tests.Unit
{
    public class DefinitionTests
    {
        [Fact]
        public void Can_Build_Definition()
        {
            var definition = new Definition();
            definition.Properties.Add(new Property("id", "id", "NUMBER", true));
            definition.Properties.Add(new Property("title"));
            definition.Properties.Add(new Property("required", "required", "BOOLEAN"));
            definition.Properties.Add(new PropertyObject("teacher",
                new Properties {
                    new Property("id", "teacher_id", "NUMBER", true),
                    new Property("name", "teacher_name") }
            ));
            definition.Properties.Add(new PropertyArray("lesson",
                new Properties {
                    new Property("id", "lesson_id", "NUMBER", true),
                    new Property("title", "lesson_title")
                }
            ));

            var given = new List<Dictionary<string, Maybe<object>>>();
            given.Add(new Dictionary<string, Maybe<object>>
            {
                { "id", 1 },
                { "title", "Tabular to Objects" },
                { "required", true },
                { "teacher_id", 1 },
                { "teacher_name", "David" },
                { "lesson_id", 1 },
                { "lesson_title", "Definitions" }
            });
            given.Add(new Dictionary<string, Maybe<object>>
            {
                { "id", 1 },
                { "title", "Tabular to Objects" },
                { "required", true },
                { "teacher_id", 1 },
                { "teacher_name", "David" },
                { "lesson_id", 2 },
                { "lesson_title", "Table Data" }
            });
            given.Add(new Dictionary<string, Maybe<object>>
            {
                { "id", 1 },
                { "title", "Tabular to Objects" },
                { "required", true },
                { "teacher_id", 1 },
                { "teacher_name", "David" },
                { "lesson_id", 3 },
                { "lesson_title", "Objects" }
            });
            given.Add(new Dictionary<string, Maybe<object>>
            {
                { "id", 2 },
                { "title", "Column Names Define Structure" },
                { "required", false },
                { "teacher_id", 2 },
                { "teacher_name", "Chris" },
                { "lesson_id", 4 },
                { "lesson_title", "Column Names" }
            });
            given.Add(new Dictionary<string, Maybe<object>>
            {
                { "id", 2 },
                { "title", "Column Names Define Structure" },
                { "required", false },
                { "teacher_id", 2 },
                { "teacher_name", "Chris" },
                { "lesson_id", 2 },
                { "lesson_title", "Table Data" }
            });
            given.Add(new Dictionary<string, Maybe<object>>
            {
                { "id", 2 },
                { "title", "Column Names Define Structure" },
                { "required", false },
                { "teacher_id", 2 },
                { "teacher_name", "Chris" },
                { "lesson_id", 3 },
                { "lesson_title", "Objects" }
            });
            given.Add(new Dictionary<string, Maybe<object>>
            {
                { "id", 3 },
                { "title", "Object On Bottom" },
                { "required", true },
                { "teacher_id", 1 },
                { "teacher_name", "David" },
                { "lesson_id", 5 },
                { "lesson_title", "Non Array Input" }
            });

            var expected = new List<Dictionary<string, Maybe<object>>>();
            expected.Add(new Dictionary<string, Maybe<object>>
            {
                { "id", 1 },
                { "title", "Tabular to Objects" },
                { "required", true },
                { "teacher", new Dictionary<string, Maybe<object>>{ {"id", 1}, {"name", "David"} } },
                { "lesson", new List<Dictionary<string, Maybe<object>>> {
                    new Dictionary<string, Maybe<object>> { { "id", 1 }, {"title", "Definitions" } },
                    new Dictionary<string, Maybe<object>> { { "id", 2 }, {"title", "Table Data"} },
                    new Dictionary<string, Maybe<object>> { { "id", 3 }, {"title", "Objects"} }
                } }
            });
            expected.Add(new Dictionary<string, Maybe<object>>
            {
                { "id", 2 },
                { "title", "Column Names Define Structure" },
                { "required", false },
                { "teacher", new Dictionary<string, Maybe<object>>{ {"id", 2}, {"name", "Chris"} } },
                { "lesson", new List<Dictionary<string, Maybe<object>>> {
                    new Dictionary<string, Maybe<object>> { { "id", 4 }, {"title", "Column Names"} },
                    new Dictionary<string, Maybe<object>> { { "id", 2 }, {"title", "Table Data"} },
                    new Dictionary<string, Maybe<object>> { { "id", 3 }, {"title", "Objects"} }
                } }
            });
            expected.Add(new Dictionary<string, Maybe<object>>
            {
                { "id", 3 },
                { "title", "Object On Bottom" },
                { "required", true },
                { "teacher", new Dictionary<string, Maybe<object>>{ {"id", 1}, {"name", "David"} } },
                { "lesson", new List<Dictionary<string, Maybe<object>>> {
                    new Dictionary<string, Maybe<object>> { { "id", 5 }, {"title", "Non Array Input" } }
                } }
            });

            var hydrator = new Hydrator(given, definition);
            var result = hydrator.Execute();

            Assert.NotEmpty(result);

            Assert.Equal(expected[0].Count, result[0].Count);
            Assert.Equal(expected[0]["id"], result[0]["id"]);
            Assert.Equal(expected[0]["title"], result[0]["title"]);
            Assert.Equal(expected[0]["required"], result[0]["required"]);
            Assert.Equal(expected[0]["teacher"].Value, result[0]["teacher"].Value);
            Assert.Equal(expected[0]["lesson"].Value, result[0]["lesson"].Value);

            Assert.Equal(expected[1].Count, result[1].Count);
            Assert.Equal(expected[1]["id"], result[1]["id"]);
            Assert.Equal(expected[1]["title"], result[1]["title"]);
            Assert.Equal(expected[1]["required"], result[1]["required"]);
            Assert.Equal(expected[1]["teacher"].Value, result[1]["teacher"].Value);
            Assert.Equal(expected[1]["lesson"].Value, result[1]["lesson"].Value);

            Assert.Equal(expected[2].Count, result[2].Count);
            Assert.Equal(expected[2]["id"], result[2]["id"]);
            Assert.Equal(expected[2]["title"], result[2]["title"]);
            Assert.Equal(expected[2]["required"], result[2]["required"]);
            Assert.Equal(expected[2]["teacher"].Value, result[2]["teacher"].Value);
            Assert.Equal(expected[2]["lesson"].Value, result[2]["lesson"].Value);
        }

        [Fact]
        public void Can_Build_Definition_StarWars()
        {
            var definition = new Definition();
            definition.Properties.Add(new Property("id", "id", "NUMBER", true));
            definition.Properties.Add(new Property("type"));
            definition.Properties.Add(new Property("name"));
            definition.Properties.Add(new PropertyObject("homePlanet",
                new Properties {
                    new Property("id", "homePlanet__id", "NUMBER", true),
                    new Property("name", "homePlanet__name") }
            ));
            definition.Properties.Add(new PropertyArray("friends",
                new Properties {
                    new Property("id", "friends__id", "NUMBER", true),
                    new Property("type", "friends__type"),
                    new Property("name", "friends__name")
                }
            ));

            var given = new List<Dictionary<string, Maybe<object>>>();
            given.Add(new Dictionary<string, Maybe<object>>
            {
                { "id", 1 },
                { "type", "Human" },
                { "name", "Luke" },
                { "homePlanet__id", 1 },
                { "homePlanet__name", "Tatooine" },
                { "friends__id", 3 },
                { "friends__type", "Droid" },
                { "friends__name", "R2-D2" }
            });
            given.Add(new Dictionary<string, Maybe<object>>
            {
                { "id", 1 },
                { "type", "Human" },
                { "name", "Luke" },
                { "homePlanet__id", 1 },
                { "homePlanet__name", "Tatooine" },
                { "friends__id", 4 },
                { "friends__type", "Droid" },
                { "friends__name", "C-3PO" }
            });

            var expected = new List<Dictionary<string, Maybe<object>>>();
            expected.Add(new Dictionary<string, Maybe<object>>
            {
                { "id", 1 },
                { "type", "Human" },
                { "name", "Luke" },
                { "homePlanet", new Dictionary<string, Maybe<object>>{ {"id", 1}, {"name", "Tatooine" } } },
                { "friends", new List<Dictionary<string, Maybe<object>>> {
                    new Dictionary<string, Maybe<object>> { { "id", 3 }, { "type", "Droid" }, {"name", "R2-D2" } },
                    new Dictionary<string, Maybe<object>> { { "id", 4 }, { "type", "Droid" }, {"name", "C-3PO" } }
                } }
            });

            var hydrator = new Hydrator(given, definition);
            var result = hydrator.Execute();

            Assert.NotEmpty(result);

            Assert.Equal(expected[0].Count, result[0].Count);
            Assert.Equal(expected[0]["id"], result[0]["id"]);
            Assert.Equal(expected[0]["type"], result[0]["type"]);
            Assert.Equal(expected[0]["name"], result[0]["name"]);
            Assert.Equal(expected[0]["homePlanet"].Value, result[0]["homePlanet"].Value);
            Assert.Equal(expected[0]["friends"].Value, result[0]["friends"].Value);
        }
    }
}
