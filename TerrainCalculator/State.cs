using System;
using UnityEngine;
using TerrainCalculator.Network;
using TerrainCalculator.CitiesUI;
using ColossalFramework.UI;
using TerrainCalculator.UnityUI;

namespace TerrainCalculator
{
    public class State : MonoBehaviour
    {
        public WaterNetwork Net;
        public Path ActivePath;
        public Node ActiveNode;
        
        public enum ModeType
        {
            BASE,
            EDIT_NODE,
            EDIT_PATH,
            DRAG_NODE,
            DRAG_NODE_Z,
            PLACE_NODE
        }

        private ModeType _mode;
        public ModeType Mode {get => _mode; }


        public void Start()
        {
            Debug.Log("State start");
            Net = new WaterNetwork();
            var view = UIView.GetAView();
            RootUI.Build(view, this);
            gameObject.AddComponent<NetworkRenderer>();
            gameObject.AddComponent<NodeDragger>();
        }

        public delegate void ActivatePanel(PanelType panelType);
        public event ActivatePanel eventActivatePanel;

        public delegate void NodeChanged(Network.Node node);
        public event NodeChanged eventNodeValuesChanged;
        public event NodeChanged eventNodePositionChanged;
        public event NodeChanged eventNodeAdded;
        public event NodeChanged eventNodeRemoved;

        public void OnNewRiver()
        {
            Debug.Log($"New River");
            ActiveNode = Net.NewNode();
            ActivePath = Net.NewRiver();
            ActivePath.Nodes.Add(ActiveNode);
            _mode = ModeType.PLACE_NODE;
            eventActivatePanel.Invoke(PanelType.PATH);
        }

        public void OnNewLake()
        {
            Debug.Log($"New Lake");
            eventActivatePanel.Invoke(PanelType.PATH);
        }

        // TODO: Remove
        public void OnNewNode()
        {
            Debug.Log($"New Node");
            eventActivatePanel.Invoke(PanelType.NODE);
        }

        public void OnPathDone()
        {
            Debug.Log($"Path Done");
            eventActivatePanel.Invoke(PanelType.ACTION);
        }

        public void OnPathDelete()
        {
            Debug.Log($"Path Delete");
            eventActivatePanel.Invoke(PanelType.ACTION);
        }

        public void OnNodeValueSet(Network.Node.Key key, double value)
        {
            Debug.Log($"Node Value Set {key.ToString()} = {value}");
        }

        public void OnNodeDone()
        {
            Debug.Log($"Node Done");
            eventActivatePanel.Invoke(PanelType.PATH);
        }

        public void OnNodeDelete()
        {
            Debug.Log($"Node Delete");
            eventActivatePanel.Invoke(PanelType.PATH);
        }
    }
}
