using System;

namespace Icm.JsonDiff.Differences
{
    public abstract class Difference
    {
        protected Difference(string jsonPath)
        {
            JsonPath = jsonPath;
            if (jsonPath == null) throw new ArgumentNullException(nameof(jsonPath));
        }

        protected abstract string Description { get; }
        public string JsonPath { get; }

        public override string ToString()
        {
            var path = JsonPath == "" ? "root object" : JsonPath;
            return $"In {path}, {Description}";
        }


        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Difference)obj);
        }

        private bool Equals(Difference other)
        {
            return Equals(JsonPath, other.JsonPath);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = JsonPath?.GetHashCode() ?? 0;
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

    }
}
