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

        private static IEnumerable<Difference> DiffObject(JObject json1, JObject json2, string path1)
        {
            foreach (JProperty property1 in json1.Properties())
            {
                var property2 = json1.Properties().SingleOrDefault(prop2 => prop2.Name == property1.Name);
                if (property2 == null)
                {
                    yield return new Difference("Property " + property1.Name + " in source but not in destination", json1, json2, path1);
                }
                else
                {
                    foreach (Difference difference in Diff(property1.Value, property2.Value, path1 + "/" + property1.Name))
                    {
                        yield return difference;
                    }
                }
            }

            foreach (JProperty property2 in json2.Properties())
            {
                var property1 = json1.Properties().SingleOrDefault(prop1 => prop1.Name == property2.Name);
                if (property1 == null)
                {
                    yield return new Difference("Property " + property2.Name + " in destination but not in source", json1, json2, path1);
                }
                else
                {
                    foreach (Difference difference in Diff(property1.Value, property2.Value, path1 + "/" + property1.Name))
                    {
                        yield return difference;
                    }
                }
            }
        }

        private static IEnumerable<Difference> DiffArray(JArray json1, JArray json2, string path1)
        {
            if (json1.Children().Count() != json2.Children().Count())
            {
                yield return new Difference("Arrays don't have same number of elements", json1, json2, path1);
            }

            for (int i = 0; i < json1.Children().Count(); i++)
            {
                foreach (Difference difference in Diff(json1.Children().ElementAt(i), json1.Children().ElementAt(i), path1 + "[" + i + "]"))
                {
                    yield return difference;
                }
            }
        }

        private static IEnumerable<Difference> DiffValue(JValue json1, JValue json2, string path1)
        {
            string value1 = json1.Value<string>();
            string value2 = json2.Value<string>();
            if (value1 != value2)
            {
                yield return new Difference("Value " + value1 + " != " + value2, json1, json2, path1);
            }
        }        
    }
}