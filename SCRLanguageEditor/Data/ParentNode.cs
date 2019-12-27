﻿using System.Collections.Generic;

namespace SCRLanguageEditor.Data
{
    /// <summary>
    /// A node which holds more nodes as children (hierarchy node)
    /// </summary>
    public class ParentNode : Node
    {

        /// <summary>
        /// The children of this node
        /// </summary>
        public List<Node> ChildNodes { get; private set; }
         
        /// <summary>
        /// Creates a parent node
        /// </summary>
        /// <param name="name">The name of the parent node</param>
        public ParentNode(string name) : base(name, NodeType.ParentNode)
        {
            ChildNodes = new List<Node>();
        }

        /// <summary>
        /// Creates a parent node with a description
        /// </summary>
        /// <param name="name">The name of the node</param>
        /// <param name="description">The description of the node</param>
        public ParentNode(string name, string description) : base(name, description, NodeType.ParentNode)
        {
            ChildNodes = new List<Node>();
        }

    }
}
