using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Icm.JsonDiff
{
    public static class JsonDiff
    {

        public static IEnumerable<Difference> Diff(JToken json1, JToken json2, string path1)
        {
            if (json1.GetType() != json2.GetType())
            {
                yield return new Difference("Type diff", json1, json2, path1);
            }

            if (json1 is JObject)
            {
                foreach (var diff in DiffObject(json1 as JObject, json2 as JObject, path1))
                {
                    yield return diff;
                }
            }

            if (json1 is JArray)
            {
                foreach (var diff in DiffArray(json1 as JArray, json2 as JArray, path1))
                {
                    yield return diff;
                }
            }

            if (json1 is JValue)
            {
                foreach (var diff in DiffValue(json1 as JValue, json2 as JValue, path1))
                {
                    yield return diff;
                }
            }
        }

        private static IEnumerable<T> Yield<T>(this T obj)
        {
            yield return obj;
        }

        private static IEnumerable<Difference> DiffObject(JObject json1, JObject json2, string path1)
        {
            return Enumerable.Empty<Difference>()
            .Concat(
                json1.Properties()
                .Select(property1 => new
                {
                    checkedProperty = property1,
                    nullProperty = json2.Properties().SingleOrDefault(prop2 => prop2.Name == property1.Name),
                    msg = "Property {0} in source but not in destination"
                })
                .Where(pair => pair.nullProperty == null)
                .Select(pair =>
                    new Difference(string.Format(pair.msg, pair.checkedProperty.Name), json1, json2, path1)))
            .Concat(
                json2.Properties()
                .Select(property2 => new
                {
                    nullProperty = json1.Properties().SingleOrDefault(prop1 => prop1.Name == property2.Name),
                    checkedProperty = property2,
                    msg = "Property {0} in destination but not in source"
                })
                .Where(pair => pair.nullProperty == null)
                .Select(pair =>
                    new Difference(string.Format(pair.msg, pair.checkedProperty.Name), json1, json2, path1)))
            .Concat(
                json1.Properties()
                .Select(property1 => new
                {
                    property1,
                    property2 = json2.Properties().SingleOrDefault(prop2 => prop2.Name == property1.Name)
                })
                .Where(pair => pair.property2 != null)
                .SelectMany(pair => Diff(pair.property1.Value, pair.property2.Value, path1 + "/" + pair.property1.Name)));
        }

        private static IEnumerable<Difference> DiffArray(JArray json1, JArray json2, string path1)
        {
            if (json1.Children().Count() != json2.Children().Count())
            {
                return new[] {new Difference("Arrays don't have same number of elements", json1, json2, path1)};
            }

            return json1.Children().Zip(json2.Children(), Tuple.Create)
                .SelectMany((tup, i) => Diff(tup.Item1, tup.Item2, path1 + "[" + i + "]"));
        }

        private static IEnumerable<Difference> DiffValue(JValue json1, JValue json2, string path1)
        {
            string value1 = json1.Value<string>();
            string value2 = json2.Value<string>();
            if (value1 != value2)
            {
                yield return new Difference($"Value {value1} != {value2}", json1, json2, path1);
            }
        }        
    }
}