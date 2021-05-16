using ICities;
using TerrainCalculator.OptionsFramework;
using TerrainCalculator.UnityUI;
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

            GameObject go = new GameObject("TerrainCalculator");
            go.AddComponent<State>();
            go.AddComponent<NodeCollection>();
            go.AddComponent<EdgeCollection>();
            go.AddComponent<NodeDragger>();
            go.AddComponent<PlaceNodeMode>();
            go.AddComponent<BaseMode>();
            go.AddComponent<EditNodeMode>();
            go.AddComponent<GraphBuilder>();
            go.AddComponent<GridBuilder>();
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