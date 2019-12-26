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
         *  <Language Lang="MyLanguage" Version="1.0">
         *      <Cat Name="My Category" Desc="My Category Description!">
         *          <Str name="MyString" Desc="My String Description!">Default content</Str>
         *      </Cat>
         *  </Language>
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
            LoadNodes(file.DocumentElement.ChildNodes, nodes, stringNodes, out Dictionary<int, Version> versions);

            HeaderNode lang = new HeaderNode(versions, nodes, stringNodes);

            return lang;
        }

        /// <summary>
        /// Creates nodes from XMLNodes and adds them to the child list
        /// </summary>
        /// <param name="children">The children of the XML node, which need to be converted into our nodes</param>
        /// <param name="resultNodes">The output list (should be taken from a parentnode or languagenode)</param>
        private static void LoadNodes(XmlNodeList children, List<Node> resultNodes, List<StringNode> stringNodes, out Dictionary<int, Version> versions)
        {
            versions = null;
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
                        string value = n.InnerText;
                        int versionID = int.Parse(attribs.GetNamedItem("vID").Value);
                        if (value == null) value = "";
                        StringNode strNode;
                        
                        if (attribs.Count == 2)
                        {
                            strNode = new StringNode(name, value, versionID);
                        }
                        else
                        {
                            strNode = new StringNode(name, value, versionID, attribs.GetNamedItem("Desc").Value);
                        }
                        resultNodes.Add(strNode);
                        stringNodes.Add(strNode);
                        break;
                    case "Versions":
                        versions = new Dictionary<int, Version>();
                        foreach(XmlNode node in n.ChildNodes)
                        {
                            versions.Add(int.Parse(node.Attributes.GetNamedItem("Index").Value), Version.Parse(node.InnerText));
                        }
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

            LoadNodes(parent.ChildNodes, node.ChildNodes, stringNodes, out _);

            return node;
        }
    }
}
