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

        public override string Kind =>
            $"Value {Token1.Value<string>()} != {Token2.Value<string>()}";
    }
}