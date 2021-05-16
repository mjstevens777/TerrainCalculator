using System;
using ColossalFramework.UI;
using TerrainCalculator.Network;
using UnityEngine;

namespace TerrainCalculator.UnityUI
{
    public class BaseMode : MonoBehaviour
    {
        private Node _dragNode;
        private Vector2 _dragStart;

        private const float _clickThreshold = 1f;

        public void Start()
        {
            enabled = false;
        }

        public void EnterMode()
        {
            enabled = true;
            _dragNode = null;
            _dragStart = Vector2.zero;
        }

        public void ExitMode()
        {
            enabled = false;
            _dragNode = null;
            _dragStart = Vector2.zero;
            GetComponent<NodeDragger>().ExitMode();
        }

        public void Update()
        {
            if (_dragNode != null)
            {
                _updateDragging();
            }
            else
            {
                _updateSelecting();
            }
        }

        private void _updateDragging()
        {
            NodeCollection collection = GetComponent<NodeCollection>();
            collection.HighlightNode = _dragNode;

            if (Input.GetMouseButtonUp(0))
            {
                GetComponent<NodeDragger>().ExitMode();
                Vector2 delta = _dragNode.Pos - _dragStart;
                if (delta.magnitude < _clickThreshold)
                {
                    GetComponent<State>().EnterEditNode(_dragNode);
                    return;
                }
                _dragNode = null;
                _dragStart = Vector2.zero;
            }
        }

        private void _updateSelecting()
        {
            if (UIView.IsInsideUI()) return;

            NodeCollection collection = GetComponent<NodeCollection>();
            Node node = collection.CheckCollisions();
            collection.HighlightNode = node;

            if (Input.GetMouseButtonDown(0))
            {
                // Left mouse, start drag
                _dragNode = node;
                _dragStart = node.Pos;
                GetComponent<NodeDragger>().EnterMode(node, setElevation: false);
            }

            if (Input.GetMouseButtonDown(1))
            {
                // Right mouse, delete node
                GetComponent<GraphBuilder>().IsDirty = true;
                GetComponent<State>().Net.RemoveNode(node);
            }
        }
    }
}
