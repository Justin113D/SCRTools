using SCR.Tools.UndoRedo.Collections;
using static SCR.Tools.UndoRedo.GlobalChangeTrackerC;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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
        private readonly TrackList<Node> _nodes;

        private string _name;

        private string _description;

        private string _author;

        #endregion

        /// <summary>
        /// Starter Node
        /// </summary>
        public Node? StartNode => _nodes.FirstOrDefault(x => x.Inputs.Count == 0);

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
                TrackValueChange(
                    (v) => _name = v, _name, value);
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
                TrackValueChange(
                    (v) => _description = v, _description, value);
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
                TrackValueChange(
                    (v) => _author = v, _author, value);
            }
        }

        public Dialog()
        {
            _name = "";
            _description = "";
            _author = "";

            _nodes = new();
            Nodes = new(_nodes);
        }

        /// <summary>
        /// Creates a new node
        /// </summary>
        public Node CreateNode()
        {
            Node n = new();
            _nodes.Add(n);
            return n;
        }

        /// <summary>
        /// Removes a node from the network
        /// </summary>
        public void RemoveNode(Node node)
        {
            BeginChangeGroup();
            node.Disconnect();

            _nodes.Remove(node);

            EndChangeGroup();
        }

        public void Sort()
        {
            BeginChangeGroup();

            List<Node> sorted = new();

            Queue<Node> multiOutputQueue = new();
            Queue<Node> multiInputQueue = new();

            while (sorted.Count < _nodes.Count)
            {
                Node? next = _nodes.Find(x => x.Outputs.Any(y => y.Connected == null) && !sorted.Contains(x));
                if (next == null)
                {
                    next = _nodes.Find(x => !sorted.Contains(x));
                }

                while (multiOutputQueue.Count > 0
                    || multiInputQueue.Count > 0
                    || next != null)
                {

                    if(next != null)
                    {
                        Node q = next;
                        next = null;

                        if (sorted.Contains(q))
                        {
                            continue;
                        }

                        if(q.Outputs.Count > 1 && multiInputQueue.Count > 0)
                        {
                            multiOutputQueue.Enqueue(q);
                            continue;
                        }

                        sorted.Add(q);

                        if (q.Inputs.Count > 1)
                        {
                            foreach(NodeOutput input in q.Inputs)
                            {
                                multiInputQueue.Enqueue(input.Parent);
                            }
                        }
                        else if(q.Inputs.Count == 1)
                        {
                            next = q.Inputs[0].Parent;
                            continue;
                        }
                    }

                    if(multiInputQueue.Count > 0)
                    {
                        next = multiInputQueue.Dequeue();
                    }
                    else if (multiOutputQueue.Count > 0)
                    {
                        next = multiOutputQueue.Dequeue();
                    }
                }
            }

            sorted.Reverse();

            _nodes.Clear();
            _nodes.AddRange(sorted);

            EndChangeGroup();
        }


        public override string ToString()
            => $"{Name} - {Description}";
    }
}
