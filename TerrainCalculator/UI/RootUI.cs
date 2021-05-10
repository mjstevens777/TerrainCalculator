using System;
using System.Collections.Generic;
using ColossalFramework.UI;
using UnityEngine;

namespace TerrainCalculator.UI
{
    public enum PanelType
    {
        ACTION,
        PATH,
        NODE
    };

    public class RootUI
    {
        private static string _title => Mod.translation.GetTranslation("TC_NAME");
        private const float _width = 215f;
        private const float _right = 200f;
        private const float _top = 40f;
        private const float _titleHeight = 40f;

        public static UIPanel Build(UIView view, State state)
        {
            var root = view.AddUIComponent(typeof(UIPanel)) as UIPanel;
            root.backgroundSprite = "MenuPanel2";
            root.name = "TCToolPanel";
            // Anchor to top-right
            root.anchor = UIAnchorStyle.Top | UIAnchorStyle.Right;
            root.width = _width;
            root.relativePosition = new Vector3(
                view.GetScreenResolution().x - _right - _width, _top);
            Util.DebugComponent(root);

            var titlePanel = root.AddUIComponent<UIPanel>();
            titlePanel.canFocus = true;
            titlePanel.isInteractive = true;
            titlePanel.relativePosition = Vector3.zero;
            titlePanel.size = new Vector2(_width, _titleHeight);
            titlePanel.name = "TCTitlePanel";

            var title = titlePanel.AddUIComponent<UILabel>();
            title.text = _title;
            title.name = "TCTitle";
            title.autoSize = true;
            title.anchor = UIAnchorStyle.CenterHorizontal | UIAnchorStyle.CenterVertical;

            var dragHandle = titlePanel.AddUIComponent<UIDragHandle>();
            dragHandle.target = root;
            dragHandle.relativePosition = Vector3.zero;
            dragHandle.size = titlePanel.size;
            dragHandle.name = "TCDragHandle";


            var actionPanel = ActionPanel.Build(root, state);
            var pathPanel = PathPanel.Build(root, state);
            var nodePanel = NodePanel.Build(root, state);

            State.ActivatePanel activatePanel = (PanelType panelType) =>
            {
                actionPanel.isVisible = (panelType == PanelType.ACTION);
                pathPanel.isVisible = (panelType == PanelType.PATH);
                nodePanel.isVisible = (panelType == PanelType.NODE);
            };

            state.eventActivatePanel += activatePanel;
            activatePanel(PanelType.ACTION);

            root.autoLayoutDirection = LayoutDirection.Vertical;
            root.autoFitChildrenVertically = true;
            root.autoLayout = true;

            Util.PrintDone(root);
            return root;
        }
    }
}

