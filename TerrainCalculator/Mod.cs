using ICities;
using TerrainCalculator.OptionsFramework.Extensions;
using TerrainCalculator.TranslationFramework;

namespace TerrainCalculator
{
    public class Mod : IUserMod
    {
        public static Translation translation = new Translation();

        public string Name => "Terrain Calculator";

        public string Description => translation.GetTranslation("TC_DESCRIPTION");

        public void OnSettingsUI(UIHelperBase helper)
        {
            helper.AddOptionsGroup<Options>(s => translation.GetTranslation(s));
        }
	}

}