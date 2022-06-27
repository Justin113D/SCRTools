using SCR.Tools.UndoRedo.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using static SCR.Tools.UndoRedo.GlobalChangeTrackerC;

namespace SCR.Tools.Dialog.Data
{
    /// <summary>
    /// Complete Dialog that encapsulates all nodes
    /// </summary>
    public class DialogContainer
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

        public DialogContainer()
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
            HashSet<Node> sortedHashMap = new();

            Queue<(Node, int)> multiOutputQueue = new();
            Queue<Node> multiInputQueue = new();

            Queue<Node> DisconnectedOutputs = new(_nodes
                .FindAll(x => x.Outputs.Any(y => y.Connected == null))
                .OrderBy(x => x.Outputs.Count(y => y.Connected == null)));


            while (sorted.Count < _nodes.Count)
            {
                Node? next = null;
                while (DisconnectedOutputs.Count > 0)
                {
                    next = DisconnectedOutputs.Dequeue();

                    if (!sortedHashMap.Contains(next))
                    {
                        break;
                    }

                    next = null;
                }

                if (next == null)
                {
                    next = _nodes.Find(x => !sortedHashMap.Contains(x));
                }

                while (multiOutputQueue.Count > 0
                    || multiInputQueue.Count > 0
                    || next != null)
                {

                    if (next != null)
                    {
                        Node q = next;
                        next = null;

                        if (sortedHashMap.Contains(q))
                        {
                            continue;
                        }

                        if (q.Outputs.Count > 1
                            && (multiInputQueue.Count > 0
                            || multiOutputQueue.Peek().Item2 < sorted.Count))
                        {
                            multiOutputQueue.Enqueue((q, sorted.Count));
                            continue;
                        }



                        sorted.Add(q);
                        sortedHashMap.Add(q);

                        if (q.Inputs.Count > 1)
                        {
                            foreach (NodeOutput input in q.Inputs)
                            {
                                multiInputQueue.Enqueue(input.Parent);
                            }
                        }
                        else if (q.Inputs.Count == 1)
                        {
                            next = q.Inputs[0].Parent;
                            continue;
                        }
                    }

                    if (multiInputQueue.Count > 0)
                    {
                        next = multiInputQueue.Dequeue();
                    }
                    else if (multiOutputQueue.Count > 0)
                    {
                        next = multiOutputQueue.Dequeue().Item1;
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
