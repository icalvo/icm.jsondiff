using Newtonsoft.Json.Linq;

namespace Icm.JsonDiff.Differences
{

    public abstract class Difference { }

    public abstract class Difference<T> : Difference where T : JToken
    {
        protected Difference(T token1, T token2)
        {
            Token1 = token1;
            Token2 = token2;
        }

        public abstract string Kind { get; }
        public T Token1 { get; }
        public T Token2 { get; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Difference<T>) obj);
        }

        private bool Equals(Difference<T> other)
        {
            return Equals(Token1, other.Token1) && Equals(Token2, other.Token2);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Token1?.GetHashCode() ?? 0;
                hashCode = (hashCode*397) ^ (Token2?.GetHashCode() ?? 0);
                return hashCode;
            }
        }

        public static bool operator ==(Difference<T> left, Difference<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Difference<T> left, Difference<T> right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return $"'{Kind}' '{Token1.Path}'";
        }
    }
}
