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
            Util.DebugComponent(panel);

            _buildRow(panel, state, Network.Node.Key.RiverSlope);
            _buildRow(panel, state, Network.Node.Key.RiverWidth);
            _buildRow(panel, state, Network.Node.Key.ShoreDepth);
            _buildRow(panel, state, Network.Node.Key.ShoreWidth);
            _buildRow(panel, state, Network.Node.Key.Elevation);

            var doneButton = DefaultButton.Build(panel, "TCNodeDoneButton", _done, state.OnNodeDone);
            var deleteButton = DefaultButton.Build(panel, "TCNodeDeleteButton", _delete, state.OnNodeDelete);

            panel.autoFitChildrenVertically = true;
            panel.autoLayoutDirection = LayoutDirection.Vertical;
            panel.verticalSpacing = panel.padding.bottom + panel.autoLayoutPadding.bottom;
            panel.autoLayout = true;

            Util.PrintDone(panel);
            return panel;
        }

        private static UIPanel _buildRow(UIPanel parent, State state, TerrainCalculator.Network.Node.Key key)
        {
            var row = parent.AddUIComponent<UIPanel>() as UIPanel;
            int horizontalPadding = parent.padding.horizontal + parent.autoLayoutPadding.horizontal;
            row.width = parent.width - horizontalPadding;
            row.height = 17;

            var slider = row.AddUIComponent<UISlider>() as UISlider;
            slider.size = row.size;
            slider.relativePosition = Vector3.zero;
            slider.minValue = 0;
            slider.maxValue = 10;
            slider.backgroundSprite = "OptionsScrollbarTrack";

            UISprite thumb = slider.AddUIComponent<UISprite>() as UISprite;
            thumb.width = 16;
            thumb.height = 16;
            thumb.spriteName = "OptionsScrollbarThumb";
            slider.thumbObject = thumb;

            //slider.eventValueChanged += (UIComponent c, float value) =>
            //{
            //    state.OnNodeValueSet(key, value);
            //};

            //state.eventImplicitValuesChanged += (Network.Node node) =>
            //{
            //    slider.value = (float)(node.ImplicitValues[key].Value);
            //    Debug.Log($"Updating UI to match {key.ToString()} = {slider.value}");
            //};

            return row;

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
