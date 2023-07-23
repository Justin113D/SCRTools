using SCR.Tools.Dialog.Data.Condition;
using SCR.Tools.UndoRedo.Collections;
using System;
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
        public Node? EntryNode => _nodes.FirstOrDefault(x => x.Inputs.Count == 0);

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

        public TrackDictionary<string, ConditionData> ConditionData { get; }

        public DialogContainer()
        {
            _name = "";
            _description = "";
            _author = "";

            _nodes = new();
            Nodes = new(_nodes);

            ConditionData = new();
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

        public Node[][] FindGroups()
        {
            List<Node[]> result = new();
            HashSet<Node> grouped = new();

            while(grouped.Count < Nodes.Count)
            {
                HashSet<Node> group = new();
                Queue<Node> queue = new();

                Node? start = _nodes.Find(x => !grouped.Contains(x));
                if(start == null)
                {
                    throw new InvalidOperationException("No remaining");
                }
                queue.Enqueue(start);

                while(queue.Count > 0)
                {
                    Node dequeued = queue.Dequeue();
                    if(grouped.Contains(dequeued))
                    {
                        continue;
                    }

                    grouped.Add(dequeued);
                    group.Add(dequeued);

                    foreach(NodeOutput input in dequeued.Inputs)
                    {
                        queue.Enqueue(input.Parent);
                    }

                    foreach (NodeOutput output in dequeued.Outputs)
                    {
                        if (output.Connected != null)
                        {
                            queue.Enqueue(output.Connected);
                        }
                    }
                }

                result.Add(group.ToArray());
            }
            
            return result.ToArray();
        }

        public void Sort()
        {
            List<Node> sorted = new();

            Node[][] nodeGroups = FindGroups();

            foreach (Node[] group in nodeGroups)
            {
                sorted.AddRange(SortGroup(group));
            }

            BeginChangeGroup();

            _nodes.Clear();
            _nodes.AddRange(sorted);

            EndChangeGroup();
        }

        public static Node[] SortGroup(Node[] group)
        {
            List<Node> sorted = new();
            HashSet<Node> sortedHashMap = new();

            // int is the amount of sorted nodes during which
            // 
            Queue<(Node node, int sortindex)> multiOutputQueue = new();
            Queue<Node> multiInputQueue = new();

            // sorting the first iteration (the branch endings
            var starters = 
                Array.FindAll(group, x => x.Outputs.Any(y => y.Connected == null))
                .OrderBy(x => x.Outputs.Count(y => y.Connected == null)).ToArray();

            foreach (Node node in starters)
            {
                multiInputQueue.Enqueue(node);
                while (multiInputQueue.Count > 0)
                {
                    Node current = multiInputQueue.Dequeue();

                    if (sortedHashMap.Contains(current))
                        continue;

                    if(current.Outputs.Count > 1)
                    {
                        multiOutputQueue.Enqueue((current, sorted.Count));
                        continue;
                    }

                    sorted.Add(current);
                    sortedHashMap.Add(current);

                    foreach (NodeOutput input in current.Inputs)
                    {
                        multiInputQueue.Enqueue(input.Parent);
                    }
                }
            }

            while(sorted.Count < group.Length)
            {
                Node current;
                if (multiInputQueue.Count > 0)
                {
                    current = multiInputQueue.Dequeue();
                }
                else if (multiOutputQueue.Count > 0)
                {
                    current = multiOutputQueue.Dequeue().node;
                }
                else
                {
                    throw new InvalidOperationException("No nodes left to sort, but not all nodes sorted?");
                }

                if (sortedHashMap.Contains(current))
                    continue;

                if (current.Outputs.Count > 1
                    && (multiInputQueue.Count > 0
                        || multiOutputQueue.Count > 0
                        && multiOutputQueue.Peek().sortindex < sorted.Count))
                {
                    multiOutputQueue.Enqueue((current, sorted.Count));
                    continue;
                }

                sorted.Add(current);
                sortedHashMap.Add(current);

                foreach (NodeOutput input in current.Inputs)
                {
                    multiInputQueue.Enqueue(input.Parent);
                }
            }

            sorted.Reverse();
            return sorted.ToArray();
        }

        public override string ToString()
            => $"{Name} - {Description}";
    }
}
