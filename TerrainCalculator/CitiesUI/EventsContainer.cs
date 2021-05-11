using System;
using System.Collections.Generic;

namespace TerrainCalculator.CitiesUI
{
    public class EventsContainer
    {
        public event EventHandler RootNewLake;
        public event EventHandler RootNewRiver;
        public event EventHandler PathDone;
        public event EventHandler PathDelete;

        public class NodeValueArgs : EventArgs
        {
            Network.Node.Key Key;
            double Value;
            public NodeValueArgs(Network.Node.Key key, double value)
            {
                Key = key;
                Value = value;
            }
        }
        public event EventHandler<NodeValueArgs> NodeValue;

        public class NodeFixedArgs : EventArgs
        {
            Network.Node.Key Key;
            bool IsFixed;
            public NodeFixedArgs(Network.Node.Key key, bool isFixed)
            {
                Key = key;
                IsFixed = isFixed;
            }
        }
        public event EventHandler<NodeFixedArgs> NodeFixed;

        public event EventHandler NodeDone;
        public event EventHandler NodeDelete;
    }
}
