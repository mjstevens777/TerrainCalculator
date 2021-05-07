using ICities;
using TerrainCalculator.OptionsFramework;
using UnityEngine;

namespace TerrainCalculator
{
    public class LoadingExtension : LoadingExtensionBase
    {
        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);
        }


        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            new GameObject("TerrainCalculator").AddComponent<TerrainCalculatorUI>();

            if (mode == LoadMode.NewGame || mode == LoadMode.LoadGame)
            {
                Tool.loadGame = true;
                CreateObject();
            }
            if (mode == LoadMode.NewMap || mode == LoadMode.LoadMap)
            {
                Tool.loadMap = true;
                CreateObject();
            }
        }

        public override void OnLevelUnloading()
        {
            var go = GameObject.Find("TerrainCalculator");
            if (go != null)
            {
                Object.Destroy(go);
            }
        }

        public override void OnReleased()
        {
            base.OnReleased();
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