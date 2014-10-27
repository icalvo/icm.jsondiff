using Newtonsoft.Json.Linq;

namespace Icm.JsonDiff
{
    public class Difference
    {
        public Difference(string kind, JToken token1, JToken token2, string path)
        {
            Kind = kind;
            Token1 = token1;
            Token2 = token2;
            Path = path;
        }

        public string Kind { get; private set; }
        public JToken Token1 { get; private set; }
        public JToken Token2 { get; private set; }
        public string Path { get; set; }
    }
}