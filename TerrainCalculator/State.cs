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

        private Path _activePath;
        private Node _activeNode;
        public Path ActivePath
        {
            get => _activePath;
            set
            {
                _activePath = value;
                ActiveNode = null;
            }
        }
        public Node ActiveNode {
            get => _activeNode;
            set
            {
                _activeNode = value;
                GetComponent<NodeDragger>().StopDrag();
            }
        }
        
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
        }

        private void Update()
        {
            if (_mode == ModeType.PLACE_NODE)
            {
                _updatePlaceNode();
            }
        }

        private void _updatePlaceNode()
        {
            if (Input.GetMouseButtonDown(0) && !UIView.IsInsideUI())
            {
                Node node = Net.NewNode();
                ActivePath.Nodes.Add(node);
                ActiveNode = node;
                GetComponent<NodeDragger>().StartDrag(node, false);
            }
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
            ActivePath = Net.NewRiver();
            _startPath();
        }

        public void OnNewLake()
        {
            Debug.Log($"New Lake");
            ActivePath = Net.NewLake();
            _startPath();
        }

        private void _startPath()
        {
            Node node = Net.NewNode();
            node.SetDefault();
            ActivePath.Nodes.Add(node);
            ActiveNode = node;
            GetComponent<NodeDragger>().StartDrag(ActiveNode, true);
            _mode = ModeType.PLACE_NODE;
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
            ActivePath = null;
            eventActivatePanel.Invoke(PanelType.ACTION);
        }

        public void OnPathDelete()
        {
            Debug.Log($"Path Delete");
            if (ActivePath is River)
            {
                Net.RemoveRiver((River)ActivePath);
            } else if (ActivePath is Lake)
            {
                Net.RemoveLake((Lake)ActivePath);
            }
            ActivePath = null;
            eventActivatePanel.Invoke(PanelType.ACTION);
        }

        public void OnNodeValueSet(Network.Node.Key key, double value)
        {
            Debug.Log($"Node Value Set {key.ToString()} = {value}");
        }

        public void OnNodeDone()
        {
            ActiveNode = null;
            Debug.Log($"Node Done");
            eventActivatePanel.Invoke(PanelType.PATH);
        }

        public void OnNodeDelete()
        {
            Debug.Log($"Node Delete");
            ActivePath.Nodes.Remove(ActiveNode);
            ActiveNode = null;
            eventActivatePanel.Invoke(PanelType.PATH);
        }
    }
}
