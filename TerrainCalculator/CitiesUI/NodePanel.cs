using System;
using System.Collections.Generic;
using ColossalFramework.UI;
using UnityEngine;

namespace TerrainCalculator.CitiesUI
{
    public class NodePanel
    {
        private static string _done => Mod.translation.GetTranslation("TC_DONE");
        private static string _delete => Mod.translation.GetTranslation("TC_DELETE");

        private const int _padding = 4;

        public static UIPanel Build(UIPanel parent, State state)
        {
            var panel = parent.AddUIComponent<UIPanel>() as UIPanel;

            panel.width = parent.width;
            panel.padding = new RectOffset(_padding, _padding, _padding, _padding);
            panel.autoLayoutPadding = new RectOffset(_padding, _padding, _padding, _padding);
            panel.name = "TCNodePanel";
            //Util.DebugComponent(panel);

            _buildRow(panel, state, Network.Node.Key.RiverSlope, 1, 5, 1);
            _buildRow(panel, state, Network.Node.Key.RiverWidth, 32, 512, 2);
            _buildRow(panel, state, Network.Node.Key.ShoreDepth, 0, 80, 1);
            _buildRow(panel, state, Network.Node.Key.ShoreWidth, 32, 512, 2);
            _buildRow(panel, state, Network.Node.Key.Elevation, 40, 1024, 3);

            var doneButton = DefaultButton.Build(panel, "TCNodeDoneButton", _done, state.OnNodeDone);
            var deleteButton = DefaultButton.Build(panel, "TCNodeDeleteButton", _delete, state.OnNodeDelete);

            panel.autoFitChildrenVertically = true;
            panel.autoLayoutDirection = LayoutDirection.Vertical;
            panel.verticalSpacing = panel.padding.bottom + panel.autoLayoutPadding.bottom;
            panel.autoLayout = true;

            Util.PrintDone(panel);
            return panel;
        }

        private static void _buildRow(UIPanel parent, State state, TerrainCalculator.Network.Node.Key key,
                                         float min, float max, float gamma)
        {
            int horizontalPadding = parent.padding.horizontal + parent.autoLayoutPadding.horizontal;
            string label = key.ToString();

            var textRow = parent.AddUIComponent<UIPanel>();
            textRow.name = $"TCNodeText{label}";
            textRow.height = 20;
            textRow.width = parent.width - horizontalPadding;

            var lockCheck = textRow.AddUIComponent<UICheckBox>();
            lockCheck.name = $"TCLock{label}";
            lockCheck.height = textRow.height;
            lockCheck.width = textRow.height;
            lockCheck.relativePosition = new Vector3(textRow.width - lockCheck.width, 0, 0);

            var lockBg = lockCheck.AddUIComponent<UISprite>();
            lockBg.spriteName = "check-unchecked";
            lockBg.size = lockCheck.size;
            lockBg.relativePosition = Vector3.zero;

            var lockFg = lockCheck.AddUIComponent<UISprite>();
            lockFg.spriteName = "LockIcon";
            lockFg.size = lockCheck.size - new Vector2(4, 4);
            lockFg.relativePosition = new Vector3(2, 2);

            lockCheck.checkedBoxObject = lockFg;

            var textInput = textRow.AddUIComponent<UITextField>();
            textInput.height = 16;
            textInput.width = 60;
            textInput.normalBgSprite = "TextFieldPanel";
            textInput.selectionSprite = "EmptySprite";
            textInput.color = new Color32(72, 72, 72, 255);
            textInput.builtinKeyNavigation = true;
            textInput.relativePosition = new Vector3(
                lockCheck.relativePosition.x - horizontalPadding - textInput.width, 0);

            var title = textRow.AddUIComponent<UILabel>();
            title.text = Mod.translation.GetTranslation($"TC_NODE_{label.ToUpper()}");
            title.height = 16;
            title.width = textInput.relativePosition.x - horizontalPadding;
            title.relativePosition = Vector3.zero;

            var sliderRow = parent.AddUIComponent<UIPanel>();
            sliderRow.name = $"TCNodeSlider{label}";
            sliderRow.width = parent.width - horizontalPadding;
            sliderRow.height = 17;

            var slider = sliderRow.AddUIComponent<UISlider>();
            slider.width = sliderRow.width;
            slider.height = 12;
            slider.relativePosition = Vector3.zero;
            slider.minValue = 0;
            slider.maxValue = 100;
            slider.backgroundSprite = "ScrollbarTrack";

            UISlicedSprite thumb = slider.AddUIComponent<UISlicedSprite>();
            thumb.width = 10;
            thumb.height = 20;
            thumb.spriteName = "ScrollbarThumb";
            slider.thumbObject = thumb;

            textInput.eventTextSubmitted += (UIComponent c, string text) =>
            {
                try { 
                    float value = float.Parse(text);
                    if (float.IsNaN(value)) throw new FormatException("NaN not allowed");
                    if (float.IsInfinity(value)) throw new FormatException("Inf not allowed");
                    value = Mathf.Clamp(value, min, max);
                    textInput.text = Mathf.RoundToInt(value).ToString();
                    state.OnNodeValueSet(key, value);
                    return;
                } catch (FormatException) { }
                textInput.text = "NaN";
            };

            slider.eventValueChanged += (UIComponent c, float value) =>
            {
                value = value / 100f;
                value = Mathf.Pow(value, gamma);
                value = min + (max - min) * value;
                state.OnNodeValueSet(key, value);
                textInput.text = Mathf.RoundToInt(value).ToString();
            };

            //state.eventImplicitValuesChanged += (Network.Node node) =>
            //{
            //    lockCheck.isChecked = node.ImplicitValues[key].IsFixed;
            //    slider.value = (float)(node.ImplicitValues[key].Value);
            //    Debug.Log($"Updating UI to match {key.ToString()} = {slider.value}");
            //};
        }

        private static void _valueHandler(UIComponent component, UIButton.ButtonState value)
        {
            if (value != UIButton.ButtonState.Pressed) return;
            Util.PrintEvent(component, "button");
        }

        private static void _deleteHandler(UIComponent component, UIButton.ButtonState value)
        {
            if (value != UIButton.ButtonState.Pressed) return;
            Util.PrintEvent(component, "button");
        }
    }
}
