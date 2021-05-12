using System;
using System.Collections.Generic;
using ColossalFramework.UI;
using UnityEngine;

namespace TerrainCalculator.CitiesUI
{
    public class ActionPanel
    {
        private static string _newLake => Mod.translation.GetTranslation("TC_NEW_LAKE");
        private static string _newRiver => Mod.translation.GetTranslation("TC_NEW_RIVER");

        private const int _padding = 4;

        public static UIPanel Build(UIPanel parent, State state)
        {
            var panel = parent.AddUIComponent<UIPanel>() as UIPanel;

            panel.width = parent.width;
            panel.padding = new RectOffset(_padding, _padding, _padding, _padding);
            panel.autoLayoutPadding = new RectOffset(_padding, _padding, _padding, _padding);
            panel.name = "TCActionPanel";
            //Util.DebugComponent(panel);

            List<UIButton> buttons = new List<UIButton>();

            var lakeButton = DefaultButton.Build(
                panel, "TCLakeButton", _newLake, state.OnNewLake);
            lakeButton.eventButtonStateChanged += _newLakeHandler;
            buttons.Add(lakeButton);

            var riverButton = DefaultButton.Build(
                panel, "TCRiverButton", _newRiver, state.OnNewRiver);
            buttons.Add(riverButton);

            var nodeButton = DefaultButton.Build(
                panel, "TCNodeButton", "WIP: Node Menu", state.OnNewNode);
            buttons.Add(nodeButton);

            foreach (var button in buttons)
            {
                int horizontalPadding = panel.padding.horizontal + panel.autoLayoutPadding.horizontal;
                button.width = panel.width - horizontalPadding;
            }

            panel.autoFitChildrenVertically = true;
            panel.autoLayoutDirection = LayoutDirection.Vertical;
            panel.verticalSpacing = panel.padding.bottom + panel.autoLayoutPadding.bottom;
            panel.autoLayout = true;

            Util.PrintDone(panel);
            return panel;
        }

        private static void _newLakeHandler(UIComponent component, UIButton.ButtonState value)
        {
            if (value != UIButton.ButtonState.Pressed) return;
            Util.PrintEvent(component, "button");
        }

        private static void _newRiverHandler(UIComponent component, UIButton.ButtonState value)
        {
            if (value != UIButton.ButtonState.Pressed) return;
            Util.PrintEvent(component, "button");
        }
    }
}