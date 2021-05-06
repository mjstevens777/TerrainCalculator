using ICities;
using UnityEngine;
using ColossalFramework;
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

	public class TEB : TerrainExtensionBase
	{
		public override void OnCreated(ITerrain terrain)
		{
			TRT.terrain = terrain;
			TRT.res = TRT.terrain.heightMapResolution + 1;
			TRT.heights = Singleton<TerrainManager>.instance.RawHeights;
		}
		public override void OnAfterHeightsModified(float minX, float minZ, float maxX, float maxZ)
		{
			TRT.updateHeight = true;
		}
	}

	public class LEB : LoadingExtensionBase
	{
		public override void OnLevelLoaded(LoadMode mode)
		{
			if (mode == LoadMode.NewGame || mode == LoadMode.LoadGame)
			{
				TRT.loadGame = true;
				CreateObject();
			}
			if (mode == LoadMode.NewMap || mode == LoadMode.LoadMap)
			{
				TRT.loadMap = true;
				CreateObject();
			}
		}
		void CreateObject()
		{
			RiverMenu rm = (RiverMenu)UnityEngine.Object.FindObjectOfType(typeof(RiverMenu));
			if (!rm)
			{
				GameObject riverObject = new GameObject("RiverObject");
				riverObject.AddComponent<RiverMenu>();
			}
		}
	}
}