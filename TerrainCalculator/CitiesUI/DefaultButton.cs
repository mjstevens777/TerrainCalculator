using System;
using ColossalFramework.UI;
using UnityEngine;

namespace TerrainCalculator.CitiesUI
{
    public class DefaultButton
    {
        public event EventHandler Click;

        public static UIButton Build(UIPanel parent, string name, string text, Action action)
        {
            var button = parent.AddUIComponent<UIButton>() as UIButton;
            button.name = name;
            button.text = text;
            //Util.DebugComponent(button);
            button.normalBgSprite = "ButtonMenu";
            button.disabledBgSprite = "ButtonMenuDisabled";
            button.hoveredBgSprite = "ButtonMenuHovered";
            button.pressedBgSprite = "ButtonMenuPressed";
            button.textScale = 0.75f;
            button.textPadding.left = 8;
            button.textPadding.right = 8;
            button.textPadding.top = 8;
            button.textPadding.bottom = 4;
            // default to autoSize, but allow manual edits
            button.autoSize = true;
            button.autoSize = false;
            // NOTE: autosize disables center alignment
            button.textHorizontalAlignment = UIHorizontalAlignment.Center;
            int horizontalPadding = parent.padding.horizontal + parent.autoLayoutPadding.horizontal;
            button.width = parent.width - horizontalPadding;
            button.eventButtonStateChanged += (UIComponent c, UIButton.ButtonState state) =>
            {
                if (state == UIButton.ButtonState.Pressed) action();
            };

            Util.PrintDone(button);
            return button;
        }
    }
}
