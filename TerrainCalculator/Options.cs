using TerrainCalculator.OptionsFramework.Attibutes;

namespace TerrainCalculator
{
    [Options("TerrainCalculator.xml", "CSL-TerrainCalculator.xml")]
    public class Options
    {
        public Options()
        {
            noUi = false;
            anarchyOnByDefault = false;
            unhideAllPropsOnLevelLoading = false;
            unhideAllTreesOnLevelLoading = false;
            keyCombo = 0;
        }

        [Checkbox("TC_OPTION_NO_UI")]
        public bool noUi { set; get; }

        [Checkbox("TC_OPTION_UNHIDE_PROPS")]
        public bool unhideAllPropsOnLevelLoading { set; get; }

        [Checkbox("TC_OPTION_UNHIDE_TREES")]
        public bool unhideAllTreesOnLevelLoading { set; get; }

        [Checkbox("TC_OPTION_ALWAYS_ON")]
        public bool anarchyAlwaysOn { set; get; }

        [Checkbox("TC_OPTION_DEFAULT_ON")]
        public bool anarchyOnByDefault { set; get; }

        [DropDown("TC_OPTION_COMBO", nameof(ComboType))]
        public int keyCombo { set; get; }
    }
}