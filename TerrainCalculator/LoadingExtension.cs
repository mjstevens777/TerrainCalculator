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
            new GameObject("TerrainCalculator").AddComponent<TerrainCalculatorBehavior>();
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
    }
}