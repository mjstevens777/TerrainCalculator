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
        public ModeType Mode {
            get => _mode;
            set
            {
                _exitMode();
                _mode = value;
            }
        }


        public void Start()
        {
            Debug.Log("State start");
            Net = new WaterNetwork();
            var view = UIView.GetAView();
            RootUI.Build(view, this);
        }

        private void Update()
        {
            if (Mode == ModeType.BASE) {
                _updateBase();
            } else if (Mode == ModeType.PLACE_NODE) {
                _updatePlaceNode();
            }
        }

        private void _exitMode()
        {
            if (Mode == ModeType.BASE) {
                _exitBase();
            } else if (Mode == ModeType.PLACE_NODE) {
                _exitPlaceNode();
            }
        }

        private void _enterBase()
        {
            Mode = ModeType.BASE;
            ActivePath = null;
            eventActivatePanel.Invoke(PanelType.ACTION);
        }

        private void _updateBase()
        {

        }

        private void _exitBase()
        {

        }

        private void _enterPlaceNode(Path path)
        {
            Debug.Log("Enter place node");
            Mode = ModeType.PLACE_NODE;
            eventActivatePanel.Invoke(PanelType.PATH);
            ActivePath = path;
            Node node = Net.NewNode();
            ActivePath.Nodes.Add(node);
            ActiveNode = node;
            Debug.Log($"Node count = {ActivePath.Nodes.Count}");
            if (ActivePath.Nodes.Count == 1) {
                Debug.Log($"Place first node");
                node.SetDefault();
                GetComponent<NodeDragger>().StartDrag(node, true);
            } else
            {
                Debug.Log($"Place subsequent node");
                GetComponent<NodeDragger>().StartDrag(node, false);
            }
        }

        private void _updatePlaceNode()
        {
            if (Input.GetMouseButtonDown(0) && !UIView.IsInsideUI())
            {
                Debug.Log("Place node");
                Node node = Net.NewNode();
                // Set to existing location
                node.Pos.x = ActiveNode.Pos.x;
                node.Pos.y = ActiveNode.Pos.y;
                node.Elevation.Value = ActiveNode.Elevation.Value;
                // Set as new active node
                ActivePath.Nodes.Add(node);
                ActiveNode = node;
                GetComponent<NodeDragger>().StartDrag(node, false);
            }
            if (Input.GetMouseButtonDown(1) && !UIView.IsInsideUI())
            {
                _enterBase();
            }
        }

        private void _exitPlaceNode()
        {
            Debug.Log("Exit place node");
            ActivePath.Nodes.Remove(ActiveNode);
            ActivePath = null;
            GetComponent<NodeDragger>().StopDrag();
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
            _enterPlaceNode(Net.NewRiver());
        }

        public void OnNewLake()
        {
            Debug.Log($"New Lake");
            _enterPlaceNode(Net.NewLake());
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
            _enterBase();
        }

        public void OnPathDelete()
        {
            Debug.Log($"Path Delete");
            Path path = ActivePath;
            _enterBase();
            if (path is River)
            {
                Debug.Log($"Delete River");
                Net.RemoveRiver((River)ActivePath);
            } else if (path is Lake)
            {
                Debug.Log($"Delete Lake");
                Net.RemoveLake((Lake)ActivePath);
            }
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
