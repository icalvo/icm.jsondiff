using System;

namespace Icm.JsonDiff.Differences
{
    public class DifferenceValue : Difference
    {
        public DifferenceValue(string jsonPath, string representationToken1, string representationToken2)
            : base(jsonPath)
        {
            if (representationToken1 == null) throw new ArgumentNullException(nameof(representationToken1));
            if (representationToken2 == null) throw new ArgumentNullException(nameof(representationToken2));
            RepresentationToken1 = representationToken1;
            RepresentationToken2 = representationToken2;
        }

        public string RepresentationToken1 { get; }
        public string RepresentationToken2 { get; }

        protected override string Description => 
            $"value {RepresentationToken1} != {RepresentationToken2}";
    }
}