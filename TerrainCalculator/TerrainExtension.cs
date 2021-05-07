using ICities;
using ColossalFramework;

namespace TerrainCalculator
{
	public class TerrainExtension : TerrainExtensionBase
	{
		public override void OnCreated(ITerrain terrain)
		{
			Tool.terrain = terrain;
			Tool.res = Tool.terrain.heightMapResolution + 1;
			Tool.heights = Singleton<TerrainManager>.instance.RawHeights;
		}
		public override void OnAfterHeightsModified(float minX, float minZ, float maxX, float maxZ)
		{
			Tool.updateHeight = true;
		}
	}
}
