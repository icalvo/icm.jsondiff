using Newtonsoft.Json;
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

        public string Kind { get; }
        public JToken Token1 { get; }
        public JToken Token2 { get; }
        public string Path { get; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Difference) obj);
        }

        private bool Equals(Difference other)
        {
            return string.Equals(Kind, other.Kind) && Equals(Token1, other.Token1) && Equals(Token2, other.Token2) && string.Equals(Path, other.Path);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Kind?.GetHashCode() ?? 0;
                hashCode = (hashCode*397) ^ (Token1?.GetHashCode() ?? 0);
                hashCode = (hashCode*397) ^ (Token2?.GetHashCode() ?? 0);
                hashCode = (hashCode*397) ^ (Path?.GetHashCode() ?? 0);
                return hashCode;
            }
        }

        public static bool operator ==(Difference left, Difference right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Difference left, Difference right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return $"'{Kind}' '{Path}'";
        }
    }
}
