using System;
using TerrainCalculator.Network;
using UnityEngine;

namespace TerrainCalculator.UnityUI
{
    public class EditNodeMode : MonoBehaviour
    {
        private Node _node;

        public void Start()
        {
            enabled = false;
        }

        public void EnterMode(Node node)
        {
            enabled = true;
            _node = node;
        }

        public void ExitMode()
        {
            enabled = false;
            _node = null;
        }

        public void SetValue(Node.Key key, float value)
        {
            _node.ImplicitValues[key].SetFixed(value);
        }

        public void DeleteNode()
        {
            WaterNetwork net = GetComponent<State>().Net;
            net.RemoveNode(_node);
            GetComponent<State>().EnterBase();
        }
    }
}
