﻿using SCR.Tools.Dialog.Data;
using System;

namespace SCR.Tools.Dialog.Simulator.Data
{
    public class SimulatorException : Exception
    {
        public Node? Node { get; }

        public NodeOutput? Output { get; }

        public SimulatorException(string message, Node? node, NodeOutput? output) : base(message)
        {
            Node = node;
            Output = output;
        }
    }
}
