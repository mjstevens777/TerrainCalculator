using System;
namespace TerrainCalculator.Network
{
    public struct FlagDouble
    {
        public double Value { get; set; }
        public bool IsFixed { get; set; }
        public bool IsImplicit { get; set; }

        public void SetFixed(double value)
        {
            Value = value;
            IsFixed = true;
            IsImplicit = false;
        }

        public void SetImplicit(double value)
        {
            if (IsFixed)
            {
                throw new ApplicationException("Cannot set implicit value that has already been fixed");
            }
            Value = value;
            IsImplicit = true;
        }

        public void ResetImplicit()
        {
            IsImplicit = false;
        }
    }
}
