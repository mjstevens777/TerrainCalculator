using ICities;
using TerrainCalculator.Grid;
using TerrainCalculator.OptionsFramework;
using TerrainCalculator.UnityUI;
using UnityEngine;

namespace TerrainCalculator
{

    public class LoadingExtension : LoadingExtensionBase
    {
        IManagers _managers;

        public override void OnCreated(ILoading loading)
        {
            // NOTE: Terrain still null at this point
            _managers = loading.managers;
            Debug.Log("Creating extension");
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
            GridBuilder gridBuilder = go.AddComponent<GridBuilder>();
            gridBuilder.Terrain = _managers.terrain;
            gridBuilder.Resource = _managers.resource;
        }

        public override void OnLevelUnloading()
        {
            var go = GameObject.Find("TerrainCalculator");
            if (go != null) GameObject.Destroy(go);
        }

        public override void OnReleased()
        {
            base.OnReleased();
        }
    }
}