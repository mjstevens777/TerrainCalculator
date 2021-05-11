using System;
using System.Collections.Generic;
using ColossalFramework.UI;
using UnityEngine;

namespace TerrainCalculator.CitiesUI
{
    public class PathPanel
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
            panel.name = "TCPathPanel";
            Util.DebugComponent(panel);

            List<UIButton> buttons = new List<UIButton>();

            var lakeButton = DefaultButton.Build(panel, "TCDoneButton", _done, state.OnPathDone);
            buttons.Add(lakeButton);

            var riverButton = DefaultButton.Build(panel, "TCDeleteButton", _delete, state.OnPathDelete);
            buttons.Add(riverButton);

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

        private static void _doneHandler(UIComponent component, UIButton.ButtonState value)
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
