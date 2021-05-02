using TerrainCalculator.OptionsFramework.Attibutes;

namespace TerrainCalculator
{
    [Options("TerrainCalculator.xml", "CSL-TerrainCalculator.xml")]
    public class Options
    {
        public Options()
        {
            noUi = false;
        }

        [Checkbox("TC_OPTION_NO_UI")]
        public bool noUi { set; get; }
    }
}