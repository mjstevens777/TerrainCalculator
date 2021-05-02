using ColossalFramework.UI;
using TerrainCalculator.OptionsFramework;
using UnityEngine;

namespace TerrainCalculator
{
    public class TerrainCalculatorUI : MonoBehaviour
    {
        private static string Notice => Mod.translation.GetTranslation("TC_NOTICE");

        private UILabel _label;

        public void Awake()
        {
            _label = GameObject.Find("OptionsBar").GetComponent<UIPanel>().AddUIComponent<UILabel>();
            _label.relativePosition += new Vector3(500, 0 , 0);
        }

        public void OnDestroy()
        {
            Destroy(_label.gameObject);
        }

        public void Update()
        {
            if (OptionsWrapper<Options>.Options.noUi)
            {
                _label.Hide();
            }
            if (!OptionsWrapper<Options>.Options.noUi)
            {
                _label.Show();
                SetupText();
            }
        }

        //performed each frame
        public void SetupText()
        {
            if (_label == null || !_label.isVisible)
            {
                return;
            }
            _label.text = Notice;
         }
}
}