using System;

namespace Icm.JsonDiff.Differences
{
    public class DifferenceInType : Difference {
        public DifferenceInType(string jsonPath, string type1, string type2)
            : base(jsonPath)
        {
            if (type1 == null) throw new ArgumentNullException(nameof(type1));
            if (type2 == null) throw new ArgumentNullException(nameof(type2));
            Type1 = type1;
            Type2 = type2;
        }

        public string Type1 { get; }

        public string Type2 { get; }

        protected override string Description =>
            $"it is an {Type1} in source and a {Type2} in destination";

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DifferenceInType) obj);
        }

        private bool Equals(DifferenceInType other)
        {
            return base.Equals(other) && string.Equals(Type1, other.Type1) && string.Equals(Type2, other.Type2);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = base.GetHashCode();
                hashCode = (hashCode*397) ^ (Type1 != null ? Type1.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Type2 != null ? Type2.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(DifferenceInType left, DifferenceInType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(DifferenceInType left, DifferenceInType right)
        {
            return !Equals(left, right);
        }
    }
}