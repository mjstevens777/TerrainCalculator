using ICities;
using TerrainCalculator.Grid;
using TerrainCalculator.OptionsFramework;
using TerrainCalculator.UnityUI;
using UnityEngine;

namespace TerrainCalculator
{
    public class LoadingExtension : LoadingExtensionBase
    {
        public const string ObjectName = "TerrainCalculator";

        public override void OnCreated(ILoading loading)
        {
            Debug.Log("Creating loading extension");
            var go = GameObject.Find(ObjectName);
            if (go == null)
            {
                new GameObject(ObjectName);
            } else {
                Debug.Log("Detected hot reload");
                _setupComponents();
            }
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            Debug.Log("Loading level");
            _setupComponents();

        }

        public override void OnLevelUnloading()
        {
            Debug.Log("Unloading level");
            DestroyComponents();
        }

        public override void OnReleased()
        {
            Debug.Log("Destroying loading extension");
            DestroyComponents();
        }

        private void _setupComponents()
        {
            var go = GameObject.Find(ObjectName);
            go.AddComponent<State>();
            go.AddComponent<NodeCollection>();
            go.AddComponent<EdgeCollection>();
            go.AddComponent<NodeDragger>();
            go.AddComponent<PlaceNodeMode>();
            go.AddComponent<BaseMode>();
            go.AddComponent<EditNodeMode>();
            go.AddComponent<GraphBuilder>();
        }

        public static void DestroyComponents()
        {
            var go = GameObject.Find(ObjectName);
            foreach (var comp in go.GetComponents<Component>())
            {
                if (comp is Transform) continue;
                Object.Destroy(comp);
            }
        }
    }

    public class ThreadingExtension : ThreadingExtensionBase
    {
        IManagers _managers;
        GridBuilder _gridBuilder;

        public override void OnCreated(IThreading threading)
        {
            _gridBuilder = new GridBuilder(threading.managers);
            Debug.Log("Creating threading extension");
        }

        public override void OnBeforeSimulationTick()
        {
            _gridBuilder.Tick();
        }

        public override void OnReleased()
        {
            Debug.Log("Destroying threading extension");
            // NOTE: Loading extension not released, so we have to do this here
            LoadingExtension.DestroyComponents();
        }
    }
}