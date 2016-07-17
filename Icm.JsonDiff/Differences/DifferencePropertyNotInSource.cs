using Newtonsoft.Json.Linq;

namespace Icm.JsonDiff.Differences
{
    public class DifferencePropertyNotInSource : Difference<JObject>
    {
        public DifferencePropertyNotInSource(JObject token1, JObject token2, JProperty prop) : base(token1, token2)
        {
            Prop = prop;
        }

        private JProperty Prop { get; }


        protected override string Description =>
            $"property {Prop.Name} is in destination but not in source";
    }
}