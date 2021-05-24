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
        /**
         * Base
         */
        public WaterNetwork Net;
        private UIComponent _root;

        public void Start()
        {
            Debug.Log("State start");
            Net = new WaterNetwork();
            var view = UIView.GetAView();
            _root = RootUI.Build(view, this);
            EnterBase();
        }

        /**
         * Modes
         */

        public enum ModeType
        {
            BASE,
            EDIT_NODE,
            PLACE_NODE
        }

        private ModeType _mode;
        public ModeType Mode
        {
            get => _mode;
            set
            {
                _exitMode();
                _mode = value;
            }
        }

        private void _exitMode()
        {
            if (Mode == ModeType.BASE)
            {
                GetComponent<BaseMode>().ExitMode();
            }
            else if (Mode == ModeType.PLACE_NODE)
            {
                GetComponent<PlaceNodeMode>().ExitMode();
            }
            else if (Mode == ModeType.EDIT_NODE)
            {
                GetComponent<EditNodeMode>().ExitMode();
            }
        }

        public void EnterBase()
        {
            Mode = ModeType.BASE;
            GetComponent<BaseMode>().enabled = true;
            eventActivatePanel.Invoke(PanelType.ACTION);
        }

        public void EnterPlaceNode(Path path)
        {
            Mode = ModeType.PLACE_NODE;
            GetComponent<PlaceNodeMode>().EnterMode(path);
            eventActivatePanel.Invoke(PanelType.PATH);
        }

        public void EnterEditNode(Node node)
        {
            Mode = ModeType.EDIT_NODE;
            GetComponent<EditNodeMode>().EnterMode(node);
            eventActivatePanel.Invoke(PanelType.NODE);
        }

        /**
         * Unity helpers
         */

        private bool _leftMouseDown => Input.GetMouseButtonDown(0) && !UIView.IsInsideUI();
        private bool _rightMouseDown => Input.GetMouseButtonDown(0) && !UIView.IsInsideUI();

        /**
         * UI menu actions
         */

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
            EnterPlaceNode(Net.NewRiver());
        }

        public void OnNewLake()
        {
            Debug.Log($"New Lake");
            EnterPlaceNode(Net.NewLake());
        }

        public void OnPathDone()
        {
            Debug.Log($"Path Done");
            EnterBase();
        }

        public void OnPathDelete()
        {
            Debug.Log($"Path Delete");
            GetComponent<PlaceNodeMode>().DeletePath();
        }

        public void OnNodeValueSet(Network.Node.Key key, float value)
        {
            Debug.Log($"Node Value Set {key.ToString()} = {value}");
            GetComponent<EditNodeMode>().SetValue(key, value);
        }

        public void OnNodeDone()
        {
            Debug.Log($"Node Done");
            EnterBase();
        }

        public void OnNodeDelete()
        {
            Debug.Log($"Node Delete");
            GetComponent<EditNodeMode>().DeleteNode();
        }

        public void OnDestroy()
        {
            Destroy(_root);
        }
    }
}
