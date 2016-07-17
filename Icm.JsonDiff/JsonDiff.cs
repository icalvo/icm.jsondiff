using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Icm.JsonDiff.Differences;
using Newtonsoft.Json.Linq;

namespace Icm.JsonDiff
{
    public static class JsonDiff
    {
        [SuppressMessage("ReSharper", "TryCastAlwaysSucceeds",
            Justification = "Check for null seems less readable in this case.")]
        public static IEnumerable<Difference> Diff(JToken json1, JToken json2)
        {
            if (json1.GetType() != json2.GetType())
            {
                return new DifferenceInType(json1, json2).Yield();
            }

            if (json1 is JObject)
            {
                return DiffObject(json1 as JObject, json2 as JObject);
            }

            if (json1 is JArray)
            {
                return DiffArray(json1 as JArray, json2 as JArray);
            }

            if (json1 is JValue)
            {
                return DiffValue(json1 as JValue, json2 as JValue);
            }

            return Enumerable.Empty<Difference>();
        }

        private static IEnumerable<T> Yield<T>(this T obj)
        {
            yield return obj;
        }

        private static IEnumerable<Difference> DiffObject(JObject json1, JObject json2)
        {
            return Enumerable.Empty<Difference>()
            .Concat(
                json1.Properties()
                .Select(property1 => new
                {
                    checkedProperty = property1,
                    nullProperty = json2.Properties().SingleOrDefault(prop2 => prop2.Name == property1.Name),
                })
                .Where(pair => pair.nullProperty == null)
                .Select(pair =>
                    new DifferencePropertyNotInDestination(json1, json2, pair.checkedProperty)))
            .Concat(
                json2.Properties()
                .Select(property2 => new
                {
                    nullProperty = json1.Properties().SingleOrDefault(prop1 => prop1.Name == property2.Name),
                    checkedProperty = property2,
                })
                .Where(pair => pair.nullProperty == null)
                .Select(pair =>
                    new DifferencePropertyNotInSource(json1, json2, pair.checkedProperty)))
            .Concat(
                json1.Properties()
                .Select(property1 => new
                {
                    property1,
                    property2 = json2.Properties().SingleOrDefault(prop2 => prop2.Name == property1.Name)
                })
                .Where(pair => pair.property2 != null)
                .SelectMany(pair => Diff(pair.property1.Value, pair.property2.Value)));
        }

        private static IEnumerable<Difference> DiffArray(JArray json1, JArray json2)
        {
            if (json1.Children().Count() != json2.Children().Count())
            {
                return new[] {new DifferenceArrayCount(json1, json2), };
            }

            return json1.Children().Zip(json2.Children(), Tuple.Create)
                .SelectMany((tup, i) => Diff(tup.Item1, tup.Item2));
        }

        private static IEnumerable<Difference> DiffValue(JValue json1, JValue json2)
        {
            if (!Equals(json1.Value, json2.Value))
            {
                yield return new DifferenceValue(json1, json2);
            }
        }        
    }
}