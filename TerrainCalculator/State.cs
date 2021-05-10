using System;
using UnityEngine;
using TerrainCalculator.Network;

namespace TerrainCalculator
{
    public class State
    {
        public WaterNetwork Net;

        public State()
        {
            Net = new WaterNetwork();
        }

        public delegate void ActivatePanel(UI.PanelType panelType);
        public event ActivatePanel eventActivatePanel;

        public delegate void NodeChanged(Network.Node node);
        public event NodeChanged eventNodeChanged;

        public void OnNewRiver()
        {
            Debug.Log($"New River");
            eventActivatePanel.Invoke(UI.PanelType.PATH);
        }

        public void OnNewLake()
        {
            Debug.Log($"New Lake");
            eventActivatePanel.Invoke(UI.PanelType.PATH);
        }

        // TODO: Remove
        public void OnNewNode()
        {
            Debug.Log($"New Node");
            eventActivatePanel.Invoke(UI.PanelType.NODE);
        }

        public void OnPathDone()
        {
            Debug.Log($"Path Done");
            eventActivatePanel.Invoke(UI.PanelType.ACTION);
        }

        public void OnPathDelete()
        {
            Debug.Log($"Path Delete");
            eventActivatePanel.Invoke(UI.PanelType.ACTION);
        }

        public void OnNodeValueSet(Network.Node.Key key, double value)
        {
            Debug.Log($"Node Value Set {key.ToString()} = {value}");
        }

        public void OnNodeDone()
        {
            Debug.Log($"Node Done");
            eventActivatePanel.Invoke(UI.PanelType.PATH);
        }

        public void OnNodeDelete()
        {
            Debug.Log($"Node Delete");
            eventActivatePanel.Invoke(UI.PanelType.PATH);
        }
    }
}
