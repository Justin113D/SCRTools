using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml;

namespace SCRLanguageEditor.Data
{
    /// <summary>
    /// Used for loading existing xml nodes
    /// </summary>
    public static class FileLoader
    {
        /*
         * XML language file encoding:
         * The head of the file is a "Language" node, with the variable "name", which tells which language the file is for, and a variable "author" 
         * Then follow the categories "Cat", which can have a name and a description. The categories are just for ordering 
         * the different texts so that editing is easier, they dont serve a purpose ingame.
         * The last node type is the string "str", which can also have a name and a description, but the inner text will only hold the string used ingame
         * 
         * Example:
         * 
         *  <language lang="MyLanguage" author="Justin113D" version="1.0">
         *      <cat name="My Category" desc="My Category Description!">
         *          <str name="MyString" desc="My String Description!">My String Content</str>
         *      </cat>
         *  </language>
         */

        /// <summary>
        /// Loades all nodes from a file path
        /// </summary>
        /// <param name="filepath">The path to the xml file</param>
        /// <returns></returns>
        public static HeaderNode LoadXMLFile(string filepath)
        {
            // loading the xml file
            XmlDocument file = new XmlDocument();
            file.Load(filepath);

            //getting the language node

            List<Node> nodes = new List<Node>();
            List<StringNode> stringNodes = new List<StringNode>(); 

            // getting the child nodes
            LoadNodes(file.DocumentElement.ChildNodes, nodes, stringNodes);

            stringNodes = stringNodes.OrderBy(x => x.Name).ToList();

            HeaderNode lang = new HeaderNode(file.DocumentElement.Attributes.GetNamedItem("Version").Value, nodes, stringNodes);

            return lang;
        }

        /// <summary>
        /// Creates nodes from XMLNodes and adds them to the child list
        /// </summary>
        /// <param name="children">The children of the XML node, which need to be converted into our nodes</param>
        /// <param name="resultNodes">The output list (should be taken from a parentnode or languagenode)</param>
        private static void LoadNodes(XmlNodeList children, List<Node> resultNodes, List<StringNode> stringNodes)
        {
            foreach (XmlNode n in children)
            {
                switch (n.Name)
                {
                    case "Cat":
                        resultNodes.Add(LoadParentNode(n, stringNodes));
                        break;
                    case "Str":
                        XmlAttributeCollection attribs = n.Attributes;
                        string name = attribs.GetNamedItem("Name").Value;
                        StringNode strNode;
                        if (attribs.Count == 1)
                        {
                            strNode = new StringNode(name);
                        }
                        else
                        {
                            strNode = new StringNode(name, attribs.GetNamedItem("Desc").Value);
                        }
                        resultNodes.Add(strNode);
                        stringNodes.Add(strNode);
                        break;
                    default:
                        // not a valid node type
                        break;
                }
            }
        }

        /// <summary>
        /// Creates a parent node from an xml node and loads the children too
        /// </summary>
        /// <param name="parent">The XML source node</param>
        /// <returns>The parent node with its children</returns>
        private static ParentNode LoadParentNode(XmlNode parent, List<StringNode> stringNodes)
        {
            // get the parent node values
            XmlAttributeCollection attribs = parent.Attributes;

            ParentNode node;
            string name = parent.Attributes.GetNamedItem("Name").Value;

            if(attribs.Count == 1)
            {
                node = new ParentNode(name);
            }
            else
            {
                node = new ParentNode(name, attribs.GetNamedItem("Desc").Value);
            }

            LoadNodes(parent.ChildNodes, node.ChildNodes, stringNodes);

            return node;
        }
    }
}
