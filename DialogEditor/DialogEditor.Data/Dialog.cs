﻿using SCR.Tools.UndoRedo;
using SCR.Tools.UndoRedo.ListChange;
using System.Collections.ObjectModel;

namespace SCR.Tools.DialogEditor.Data
{
    /// <summary>
    /// Complete Dialog that encapsulates all nodes
    /// </summary>
    public class Dialog
    {
        #region Private field
        /// <summary>
        /// Nodes list
        /// </summary>
        private readonly List<Node> _nodes;

        private string _name;

        private string _description;

        private string _author;

        #endregion

        /// <summary>
        /// Starter Node
        /// </summary>
        public Node StartNode => _nodes.First(x => x.Inputs.Count == 0);

        /// <summary>
        /// Node Contents
        /// </summary>
        public ReadOnlyCollection<Node> Nodes { get; private set; }

        /// <summary>
        /// Name of the dialog
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                string oldValue = _name;
                ChangeTracker.Global.TrackChange(new ChangedValue<string>(
                    (v) => _name = v,
                    oldValue,
                    value
                ));
            }
        }

        /// <summary>
        /// Dialog description
        /// </summary>
        public string Description
        {
            get => _description;
            set
            {
                string oldValue = _description;
                ChangeTracker.Global.TrackChange(new ChangedValue<string>(
                    (v) => _description = v,
                    oldValue,
                    value
                ));
            }
        }

        /// <summary>
        /// Author of the dialog
        /// </summary>
        public string Author
        {
            get => _author;
            set
            {
                string oldValue = _author;
                ChangeTracker.Global.TrackChange(new ChangedValue<string>(
                    (v) => _author = v,
                    oldValue,
                    value
                ));
            }
        }

        public Dialog()
        {
            _name = "";
            _description = "";
            _author = "";

            _nodes = new();
            Nodes = _nodes.AsReadOnly();
        }

        /// <summary>
        /// Creates a new node
        /// </summary>
        public Node CreateNode()
        {
            Node n = new();

            ChangeTracker.Global.TrackChange(
                new ChangeListAdd<Node>(
                    _nodes, n));

            return n;
        }

        /// <summary>
        /// Removes a node from the network
        /// </summary>
        public void RemoveNode(Node node)
        {
            ChangeTracker.Global.BeginGroup();
            node.Disconnect();

            ChangeTracker.Global.TrackChange(
                new ChangeListRemove<Node>(
                _nodes, node));

            ChangeTracker.Global.EndGroup();
        }

        /// <summary>
        ///  (Somewhat) Sorts the nodes
        /// </summary>
        public void Sort()
        {
            List<Node> sorted = new();

            Queue<Node> nodeQueue = new();
            while(sorted.Count < _nodes.Count)
            {
                Node? next = _nodes.Find(x => x.Inputs.Count == 0 && !sorted.Contains(x));
                if(next == null)
                    next = _nodes.Find(x => !sorted.Contains(x));

                while(nodeQueue.Count > 0 || next != null)
                {
                    Node q = next ?? nodeQueue.Dequeue();
                    next = null;

                    if(sorted.Contains(q))
                        continue;

                    sorted.Add(q);
                    if(q.Outputs.Count == 1)
                        next = q.Outputs[0].Output;
                    else
                    {
                        foreach(NodeOutput o in q.Outputs)
                        {
                            nodeQueue.Enqueue(o.Output ?? throw new InvalidOperationException());
                        }
                    }
                }
            }

            ChangeTracker.Global.TrackChange(new ChangedList<Node>(
                _nodes,
                sorted.ToArray(),
                null
            ));
        }


        public override string ToString()
            => $"{Name} - {Description}";
    }
}