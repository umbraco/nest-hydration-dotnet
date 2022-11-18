using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace NestHydration.Tests.Unit
{
    public class DefinitionTests
    {
        [Fact]
        public void Can_Build_Definition()
        {
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

            var given = new List<Dictionary<string, object>>();
            given.Add(new Dictionary<string, object>
            {
                { "id", 1 },
                { "title", "Tabular to Objects" },
                { "required", true },
                { "teacher_id", 1 },
                { "teacher_name", "David" },
                { "lesson_id", 1 },
                { "lesson_title", "Definitions" }
            });
            given.Add(new Dictionary<string, object>
            {
                { "id", 1 },
                { "title", "Tabular to Objects" },
                { "required", true },
                { "teacher_id", 1 },
                { "teacher_name", "David" },
                { "lesson_id", 2 },
                { "lesson_title", "Table Data" }
            });
            given.Add(new Dictionary<string, object>
            {
                { "id", 1 },
                { "title", "Tabular to Objects" },
                { "required", true },
                { "teacher_id", 1 },
                { "teacher_name", "David" },
                { "lesson_id", 3 },
                { "lesson_title", "Objects" }
            });
            given.Add(new Dictionary<string, object>
            {
                { "id", 2 },
                { "title", "Column Names Define Structure" },
                { "required", false },
                { "teacher_id", 2 },
                { "teacher_name", "Chris" },
                { "lesson_id", 4 },
                { "lesson_title", "Column Names" }
            });
            given.Add(new Dictionary<string, object>
            {
                { "id", 2 },
                { "title", "Column Names Define Structure" },
                { "required", false },
                { "teacher_id", 2 },
                { "teacher_name", "Chris" },
                { "lesson_id", 2 },
                { "lesson_title", "Table Data" }
            });
            given.Add(new Dictionary<string, object>
            {
                { "id", 2 },
                { "title", "Column Names Define Structure" },
                { "required", false },
                { "teacher_id", 2 },
                { "teacher_name", "Chris" },
                { "lesson_id", 3 },
                { "lesson_title", "Objects" }
            });
            given.Add(new Dictionary<string, object>
            {
                { "id", 3 },
                { "title", "Object On Bottom" },
                { "required", true },
                { "teacher_id", 1 },
                { "teacher_name", "David" },
                { "lesson_id", 5 },
                { "lesson_title", "Non Array Input" }
            });

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

            var hydrator = new Hydrator();
            var result = hydrator.Nest(given, definition).ToList();

            Assert.NotEmpty(result);

            Assert.Equal(expected[0].Count, result[0].Count);
            Assert.Equal(expected[0]["id"], result[0]["id"]);
            Assert.Equal(expected[0]["title"], result[0]["title"]);
            Assert.Equal(expected[0]["required"], result[0]["required"]);
            Assert.Equal(expected[0]["teacher"], result[0]["teacher"]);
            Assert.Equal(expected[0]["lesson"], result[0]["lesson"]);

            Assert.Equal(expected[1].Count, result[1].Count);
            Assert.Equal(expected[1]["id"], result[1]["id"]);
            Assert.Equal(expected[1]["title"], result[1]["title"]);
            Assert.Equal(expected[1]["required"], result[1]["required"]);
            Assert.Equal(expected[1]["teacher"], result[1]["teacher"]);
            Assert.Equal(expected[1]["lesson"], result[1]["lesson"]);

            Assert.Equal(expected[2].Count, result[2].Count);
            Assert.Equal(expected[2]["id"], result[2]["id"]);
            Assert.Equal(expected[2]["title"], result[2]["title"]);
            Assert.Equal(expected[2]["required"], result[2]["required"]);
            Assert.Equal(expected[2]["teacher"], result[2]["teacher"]);
            Assert.Equal(expected[2]["lesson"], result[2]["lesson"]);
        }

        [Fact]
        public void Can_Build_Definition_StarWars()
        {
            var definition = new Definition();
            definition.Properties.Add(new Property("id", "id", true));
            definition.Properties.Add(new Property("type"));
            definition.Properties.Add(new Property("name"));
            definition.Properties.Add(new PropertyObject("homePlanet",
                new Properties {
                    new Property("id", "homePlanet__id", true),
                    new Property("name", "homePlanet__name") }
            ));
            definition.Properties.Add(new PropertyArray("friends",
                new Properties {
                    new Property("id", "friends__id", true),
                    new Property("type", "friends__type"),
                    new Property("name", "friends__name")
                }
            ));

            var given = new List<Dictionary<string, object>>();
            given.Add(new Dictionary<string, object>
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
            given.Add(new Dictionary<string, object>
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

            var expected = new List<Dictionary<string, object>>();
            expected.Add(new Dictionary<string, object>
            {
                { "id", 1 },
                { "type", "Human" },
                { "name", "Luke" },
                { "homePlanet", new Dictionary<string, object>{ {"id", 1}, {"name", "Tatooine" } } },
                { "friends", new List<Dictionary<string, object>> {
                    new Dictionary<string, object> { { "id", 3 }, { "type", "Droid" }, {"name", "R2-D2" } },
                    new Dictionary<string, object> { { "id", 4 }, { "type", "Droid" }, {"name", "C-3PO" } }
                } }
            });

            var hydrator = new Hydrator();
            var result = hydrator.Nest(given, definition).ToList();

            Assert.NotEmpty(result);

            Assert.Equal(expected[0].Count, result[0].Count);
            Assert.Equal(expected[0]["id"], result[0]["id"]);
            Assert.Equal(expected[0]["type"], result[0]["type"]);
            Assert.Equal(expected[0]["name"], result[0]["name"]);
            Assert.Equal(expected[0]["homePlanet"], result[0]["homePlanet"]);
            Assert.Equal(expected[0]["friends"], result[0]["friends"]);
        }

        [Fact]
        public void Can_Build_Definition_People_PropertyObject_From_Starterkit()
        {
            var definition = new Definition();
            definition.Properties.Add(new Property("id", "id", true));
            definition.Properties.Add(new Property("contentTypeAlias"));
            definition.Properties.Add(new Property("name"));
            definition.Properties.Add(new PropertyObject("photo",
                new Properties {
                    new Property("id", "photo__id", true),
                    new Property("mediaTypeAlias", "photo__mediaTypeAlias"),
                    new Property("url", "photo__url")}
            ));

            var given = new List<Dictionary<string, object>>();
            given.Add(new Dictionary<string, object>
            {
                { "id", 1 },
                { "contentTypeAlias", "person" },
                { "name", "Jan Skovgaard" },
                { "photo__id", 1 },
                { "photo__mediaTypeAlias", "image" },
                { "photo__url", "/media/jan.png" }
            });
            given.Add(new Dictionary<string, object>
            {
                { "id", 2 },
                { "contentTypeAlias", "person" },
                { "name", "Matt Brailsford" },
                { "photo__id", 2 },
                { "photo__mediaTypeAlias", "image" },
                { "photo__url", "/media/matt.png" }
            });
            given.Add(new Dictionary<string, object>
            {
                { "id", 3 },
                { "contentTypeAlias", "person" },
                { "name", "Lee Kelleher" },
                { "photo__id", null },
                { "photo__mediaTypeAlias", null },
                { "photo__url", null }
            });
            given.Add(new Dictionary<string, object>
            {
                { "id", 4 },
                { "contentTypeAlias", "person" },
                { "name", "Jeavon Leopold" },
                { "photo__id", 10 },
                { "photo__mediaTypeAlias", "image" },
                { "photo__url", null }
            });

            var hydrator = new Hydrator();
            var result = hydrator.Nest(given, definition).ToList();

            Assert.NotEmpty(result);

            Assert.Equal("Jan Skovgaard", result[0]["name"]);
            Assert.NotNull(result[0]["photo"]);
            Assert.Null(result[2]["photo"]);

            var photo = result[3]["photo"] as Dictionary<string, object>;
            Assert.Null(photo["url"]);
        }

        [Fact]
        public void Can_Build_Definition_People_PropertyArray_From_Starterkit()
        {
            var definition = new Definition();
            definition.Properties.Add(new Property("id", "id", true));
            definition.Properties.Add(new Property("contentTypeAlias"));
            definition.Properties.Add(new Property("name"));
            definition.Properties.Add(new PropertyArray("photo",
                new Properties {
                    new Property("id", "photo__id", true),
                    new Property("mediaTypeAlias", "photo__mediaTypeAlias"),
                    new Property("url", "photo__url")}
            ));

            var given = new List<Dictionary<string, object>>();
            given.Add(new Dictionary<string, object>
            {
                { "id", 1 },
                { "contentTypeAlias", "person" },
                { "name", "Jan Skovgaard" },
                { "photo__id", 1 },
                { "photo__mediaTypeAlias", "image" },
                { "photo__url", "/media/jan.png" }
            });
            given.Add(new Dictionary<string, object>
            {
                { "id", 2 },
                { "contentTypeAlias", "person" },
                { "name", "Matt Brailsford" },
                { "photo__id", 2 },
                { "photo__mediaTypeAlias", "image" },
                { "photo__url", "/media/matt.png" }
            });
            given.Add(new Dictionary<string, object>
            {
                { "id", 3 },
                { "contentTypeAlias", "person" },
                { "name", "Lee Kelleher" },
                { "photo__id", null },
                { "photo__mediaTypeAlias", null },
                { "photo__url", null }
            });
            given.Add(new Dictionary<string, object>
            {
                { "id", 4 },
                { "contentTypeAlias", "person" },
                { "name", "Jeavon Leopold" },
                { "photo__id", 10 },
                { "photo__mediaTypeAlias", "image" },
                { "photo__url", null }
            });

            var hydrator = new Hydrator();
            var result = hydrator.Nest(given, definition).ToList();

            Assert.NotEmpty(result);

            Assert.Equal("Jan Skovgaard", result[0]["name"]);
            Assert.NotNull(result[0]["photo"]);
            Assert.Null(result[2]["photo"]);

            var photo = result[3]["photo"] as List<Dictionary<string, object>>;
            Assert.NotNull(photo);
            Assert.Single(photo);
            Assert.Null(photo.First()["url"]);
        }
    }
}
