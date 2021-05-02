using System.ComponentModel;

namespace TerrainCalculator
{
    public enum ComboType
    {
        [Description("Shift+P")]
        ShiftP = 0,
        [Description("Ctrl+P")]
        CtrlP = 1,
        [Description("Alt+P")]
        AltP = 2
    }
}