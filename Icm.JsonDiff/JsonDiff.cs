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
                return new DifferenceInType(json1.Path, json1.Type.ToString(), json2.Type.ToString()).Yield();
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
                    new DifferencePropertyNotInDestination(json1.Path, pair.checkedProperty.Name)))
            .Concat(
                json2.Properties()
                .Select(property2 => new
                {
                    nullProperty = json1.Properties().SingleOrDefault(prop1 => prop1.Name == property2.Name),
                    checkedProperty = property2,
                })
                .Where(pair => pair.nullProperty == null)
                .Select(pair =>
                    new DifferencePropertyNotInSource(json1.Path, pair.checkedProperty.Name)))
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
            var count1 = json1.Children().Count();
            var count2 = json2.Children().Count();
            var min = Math.Min(count1, count2);
            var itemDiffs = json1.Children().Take(min).Zip(json2.Children().Take(min), Tuple.Create)
                .SelectMany((tup, i) => Diff(tup.Item1, tup.Item2));
            return
                Enumerable.Empty<Difference>()
                    .ConsIf(count1 != count2, new DifferenceArrayCount(json1.Path, count1, count2))
                    .Concat(itemDiffs);
        }

        private static IEnumerable<Difference> DiffValue(JValue json1, JValue json2)
        {
            if (!Equals(json1.Value, json2.Value))
            {
                yield return new DifferenceValue(json1.Path, Representation(json1), Representation(json2));
            }
        }

        private static string Representation(JValue value)
        {
            switch (value.Type)
            {
                case JTokenType.String:
                    return $"'{value.Value<string>()}'";
                case JTokenType.Integer:
                case JTokenType.Float:
                case JTokenType.Boolean:
                case JTokenType.Bytes:
                case JTokenType.Guid:
                case JTokenType.Uri:
                case JTokenType.TimeSpan:
                    return value.Value.ToString();
                case JTokenType.Null:
                    return "null";
                case JTokenType.Undefined:
                    return "undefined";
                case JTokenType.Date:
                    return $"'{value.Value<DateTime>():O}'";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static IEnumerable<T> Cons<T>(
            this IEnumerable<T> start, T element)
        {
            return start.Concat(element.Yield());
        }

        private static IEnumerable<T> ConsIf<T>(
            this IEnumerable<T> start, bool condition, T element)
        {
            return start.ConcatIf(condition, element.Yield());
        }

        private static IEnumerable<T> ConcatIf<T>(
            this IEnumerable<T> start, bool condition, IEnumerable<T> second)
        {
            return 
                condition 
                ? start.Concat(second) 
                : start;
        }

        private static IEnumerable<T> Yield<T>(this T obj)
        {
            yield return obj;
        }
    }
}