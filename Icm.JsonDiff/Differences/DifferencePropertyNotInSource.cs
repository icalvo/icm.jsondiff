using System;

namespace Icm.JsonDiff.Differences
{
    public class DifferencePropertyNotInSource : Difference
    {
        public DifferencePropertyNotInSource(string jsonPath, string propertyName)
            : base(jsonPath)
        {
            if (propertyName == null) throw new ArgumentNullException(nameof(propertyName));
            PropertyName = propertyName;
        }

        public string PropertyName { get; }

        protected override string Description =>
            $"property {PropertyName} is in destination but not in source";
    }
}