using System;
using Newtonsoft.Json.Linq;

namespace Icm.JsonDiff.Differences
{
    public class DifferenceValue : Difference<JValue>
    {
        public DifferenceValue(JToken token1, JToken token2) 
            : base((JValue)token1, (JValue)token2)
        {
        }

        public DifferenceValue(JValue token1, JValue token2) : base(token1, token2)
        {
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

        protected override string Description => 
            $"value {Representation(Token1)} != {Representation(Token2)}";
    }
}