using System;
using System.Collections.Generic;
using ColossalFramework.UI;
using UnityEngine;

namespace TerrainCalculator.UI
{
    public class Util
    {

        public static void DebugComponent(UIComponent component)
        {
            component.eventSizeChanged += (c, v) => PrintEvent(c, "size");
            component.eventPositionChanged += (c, v) => PrintEvent(c, "position");
            component.eventFitChildren += () => PrintEvent(component, "fit children");
            PrintEvent(component, "setup");
        }

        public static void PrintEvent(UIComponent c, string eventName)
        {
            _print($"==================");
            _print($"{c.name}");
            _print($"{eventName}");
            //_printComponentLayout(c);
        }

        public static void PrintDone(UIComponent c)
        {
            _print($"==================");
            _print($"{c.name}");
            _print($"done");
            PrintLayout(c);
        }

        public static void PrintLayout(UIComponent parent, int indent = 1)
        {
            _printVector(parent, "size", parent.size, indent);
            //_printVector(parent, "pos", parent.position, indent);
            _printVector(parent, "relPos", parent.relativePosition, indent);
            //_printVector(parent, "absPos", parent.absolutePosition, indent);
            foreach (UIComponent child in parent.components)
            {
                PrintLayout(child, indent + 1);
            }
        }

        private static void _printVector(UIComponent c, string label, Vector2 v, int indentCount)
        {
            string indent = "";
            for (int i = 0; i < indentCount; i++)
            {
                indent += "  ";
            }
            _print($"{indent}{label} for {c.name}: {v.x}, {v.y}");
        }

        private static void _print(string s)
        {
            Debug.Log(s);
        }
    }
}
