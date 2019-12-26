using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SCRLanguageEditor.Data
{
    /// <summary>
    /// The Header/Language node of the file, which holds all other nodes.
    /// The name is the language
    /// </summary>
    public class HeaderNode : Node
    {
        /// <summary>
        /// Author of the document
        /// </summary>
        private string author;

        /// <summary>
        /// Gets and sets the documents author accordingly
        /// </summary>
        public string Author
        {
            get
            {
                return author;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    author = null;
                }
                else
                {
                    author = value.Trim();
                }
            }
        }

        /// <summary>
        /// The version map inside the format file, which is used to assign a "last updated" version for each string
        /// </summary>
        public Dictionary<int, Version> Versions { get; private set; }

        /// <summary>
        /// Version of the game that the file is for
        /// </summary>
        public Version Version
        {
            get
            {
                return Versions.Last().Value;
            }
        }

        /// <summary>
        /// The version of the loaded file
        /// </summary>
        public Version LoadedVersion { get; private set; }

        /// <summary>
        /// The language of the current file
        /// </summary>
        private string language;

        /// <summary>
        /// Sets and gets the language
        /// </summary>
        public string Language
        {
            get
            {
                return language;
            }
            set
            {
                if(string.IsNullOrWhiteSpace(value))
                {
                    language = "-";
                }
                else
                {
                    language = value.Trim();
                }
            }
        }

        /// <summary>
        /// Contains all stringnodes that appear in the hierarchy
        /// </summary>
        private readonly List<StringNode> stringNodes;

        /// <summary>
        /// The children of this node
        /// </summary>
        public List<Node> ChildNodes { get; private set; }

        /// <summary>
        /// Creates a Header node from language and version
        /// </summary>
        /// <param name="name">The language of the file</param>
        /// <param name="version">The game version that the file is for</param>
        public HeaderNode(Dictionary<int, Version> versions, List<Node> children, List<StringNode> stringNodes) : base(null, NodeType.HeaderNode)
        {
            Versions = versions.OrderBy(x => x.Key).ToDictionary(x => x.Key,x => x.Value);
            Author = null;
            Language = "English";
            ChildNodes = children;
            this.stringNodes = stringNodes.OrderBy(x => x.Name).ToList();
        }

        /// <summary>
        /// Resets all string in the stringnodes of the headers hierarchy
        /// </summary>
        public void ResetAllStrings()
        {
            foreach(StringNode n in stringNodes)
            {
                n.ResetValue();
            }
        }

        /// <summary>
        /// Save the current set strings to two files
        /// </summary>
        /// <param name="path">The file path</param>
        public void SaveContentsToFile(string path)
        {
            List<string> lines = new List<string>()
            {
                Version.ToString(),
                Language,
                Author ?? "",
            };
            List<string> baseLines = new List<string>();
            
            foreach(StringNode s in stringNodes)
            {
                lines.Add(s.NodeValue.Replace("\n", "\\n"));
                baseLines.Add(s.Name);
            }

            File.WriteAllLines(path, lines);
            File.WriteAllLines(path + ".base", baseLines);
        }
        
        /// <summary>
        /// Loads String values from a file
        /// </summary>
        /// <param name="path"></param>
        public void LoadContentsFromFile(string path)
        {
            if(Path.GetExtension(path) == ".base")
            {
                path = path.Substring(0, path.Length - 5);
            }

            if(!File.Exists(path))
            {
                throw new FileNotFoundException(".lang file not found!");
            }
            if(!File.Exists(path + ".base"))
            {
                throw new FileNotFoundException(".base file not found!");
            }

            string[] lines = File.ReadAllLines(path);
            string[] baseLines = File.ReadAllLines(path + ".base");

            LoadedVersion = Version.Parse(lines[0]);
            Language = lines[1];
            Author = lines[2];

            Dictionary<string, string> languageDictionary = new Dictionary<string, string>();

            for(int i = 0; i < baseLines.Length; i++)
            {
                languageDictionary.Add(baseLines[i], lines[i + 1]);
            }

            foreach(StringNode n in stringNodes)
            {
                if(languageDictionary.ContainsKey(n.Name))
                {
                    n.NodeValue = languageDictionary[n.Name];
                }
                else
                {
                    n.NodeValue = "";
                }
            }
        }
    }
}
