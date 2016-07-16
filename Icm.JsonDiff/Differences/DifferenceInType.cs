using Newtonsoft.Json.Linq;

namespace Icm.JsonDiff.Differences
{
    public class DifferenceInType : Difference<JToken> {
        public DifferenceInType(JToken token1, JToken token2) : base(token1, token2)
        {
        }

        public override string Kind =>
            "Type diff";
    }
}