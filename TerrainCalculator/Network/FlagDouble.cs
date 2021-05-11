﻿using System;
namespace TerrainCalculator.Network
{
    public class FlagDouble
    {
        public float Value { get; set; }
        public bool IsFixed { get; set; }
        public bool IsImplicit { get; set; }

        public bool IsSet { get => IsFixed || IsImplicit; }

        public void SetFixed(float value)
        {
            Value = value;
            IsFixed = true;
            IsImplicit = false;
        }

        public void SetImplicit(float value)
        {
            if (IsFixed)
            {
                throw new ApplicationException("Cannot set implicit value that has already been fixed");
            }
            if (IsImplicit)
            {
                throw new ApplicationException("Cannot set implicit value that has already been set");
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
