using System.Collections.Generic;
using ColossalFramework.UI;
using TerrainCalculator.OptionsFramework;
using TerrainCalculator.UI;
using UnityEngine;

namespace TerrainCalculator
{
    public class TerrainCalculatorBehavior : MonoBehaviour
    {
        private State _state;

        public void Awake()
        {
            _state = new State();
            var view = UIView.GetAView();
            RootUI.Build(view, _state);
        }

        public void OnDestroy()
        {
            // TODO
        }

        public void Update()
        {
            // TODO
        }
    }
}