namespace Icm.JsonDiff.Differences
{
    public class DifferenceArrayCount : Difference
    {
        public int Count1 { get; set; }
        public int Count2 { get; set; }

        public DifferenceArrayCount(string jsonPath, int count1, int count2)
            : base(jsonPath)
        {
            Count1 = count1;
            Count2 = count2;
        }

        protected override string Description =>
            $"arrays have {Count1} elements in source and {Count2} elements in destination";

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DifferenceArrayCount) obj);
        }

        private bool Equals(DifferenceArrayCount other)
        {
            return base.Equals(other) && Count1 == other.Count1 && Count2 == other.Count2;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = base.GetHashCode();
                hashCode = (hashCode*397) ^ Count1;
                hashCode = (hashCode*397) ^ Count2;
                return hashCode;
            }
        }

        public static bool operator ==(DifferenceArrayCount left, DifferenceArrayCount right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(DifferenceArrayCount left, DifferenceArrayCount right)
        {
            return !Equals(left, right);
        }
    }
}