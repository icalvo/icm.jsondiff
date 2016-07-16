using Newtonsoft.Json.Linq;

namespace Icm.JsonDiff.Differences
{
    public class DifferenceArrayCount : Difference<JArray>
    {
        public DifferenceArrayCount(JToken token1, JToken token2) 
            : base((JArray)token1, (JArray)token2)
        {
        }

        public DifferenceArrayCount(JArray token1, JArray token2) : base(token1, token2)
        {
        }

        public override string Kind =>
            "Arrays don't have same number of elements";
    }
}