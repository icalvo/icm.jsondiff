using System;

namespace Icm.JsonDiff.Differences
{
    public class DifferencePropertyNotInDestination : Difference
    {
        public DifferencePropertyNotInDestination(string jsonPath, string propertyName)
            : base(jsonPath)
        {
            if (propertyName == null) throw new ArgumentNullException(nameof(propertyName));
            PropertyName = propertyName;
        }

        public string PropertyName { get; }

        protected override string Description => 
            $"property {PropertyName} is in source but not in destination";
    }
}