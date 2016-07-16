using Newtonsoft.Json.Linq;

namespace Icm.JsonDiff.Differences
{
    public class DifferencePropertyNotInDestination : Difference<JObject>
    {
        public DifferencePropertyNotInDestination(JObject token1, JObject token2, JProperty prop) : base(token1, token2)
        {
            Prop = prop;
        }

        private JProperty Prop { get; }

        public override string Kind => 
            $"Property {Prop.Name} in source but not in destination";
    }
}