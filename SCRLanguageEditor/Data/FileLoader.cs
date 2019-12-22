using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

            XmlAttributeCollection attribs = file.DocumentElement.Attributes;

            //getting the language node
            HeaderNode lang;
            string name = attribs.GetNamedItem("name").Value;
            string version = attribs.GetNamedItem("version").Value;
            if (attribs.Count == 2)
            {
                lang = new HeaderNode(name, version);
            }
            else
            {
                lang = new HeaderNode(name, version, attribs.GetNamedItem("author").Value);
            }

            // getting the child nodes
            LoadNodes(file.DocumentElement.ChildNodes, lang.ChildNodes);

            return lang;
        }

        /// <summary>
        /// Creates nodes from XMLNodes and adds them to the child list
        /// </summary>
        /// <param name="children">The children of the XML node, which need to be converted into our nodes</param>
        /// <param name="resultNodes">The output list (should be taken from a parentnode or languagenode)</param>
        private static void LoadNodes(XmlNodeList children, List<Node> resultNodes)
        {
            foreach (XmlNode n in children)
            {
                switch (n.Name)
                {
                    case "cat":
                        resultNodes.Add(LoadParentNode(n));
                        break;
                    case "str":
                        XmlAttributeCollection attribs = n.Attributes;
                        string name = attribs.GetNamedItem("name").Value;
                        string content = n.InnerText;
                        StringNode strNode;
                        if (attribs.Count == 1)
                        {
                            strNode = new StringNode(name, content);
                        }
                        else
                        {
                            strNode = new StringNode(name, content, attribs.GetNamedItem("desc").Value);
                        }
                        resultNodes.Add(strNode);
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
        private static ParentNode LoadParentNode(XmlNode parent)
        {
            // get the parent node values
            XmlAttributeCollection attribs = parent.Attributes;

            ParentNode node;
            string name = parent.Attributes.GetNamedItem("name").Value;

            if(attribs.Count == 1)
            {
                node = new ParentNode(name);
            }
            else
            {
                node = new ParentNode(name, attribs.GetNamedItem("desc").Value);
            }

            LoadNodes(parent.ChildNodes, node.ChildNodes);

            return node;
        }
    }
}
