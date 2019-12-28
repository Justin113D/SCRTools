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
        public readonly string targetName;

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
        private readonly Version[] Versions;

        /// <summary>
        /// Version of the game that the file is for
        /// </summary>
        public Version Version
        {
            get
            {
                return Versions[Versions.Length - 1];
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
        public HeaderNode(string targetName, List<Version> versions, List<Node> children, List<StringNode> stringNodes) : base(null, NodeType.HeaderNode)
        {
            this.targetName = targetName;
            Versions = versions.OrderBy(x => x).ToArray();
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
        /// Updates the update variable of all nodes in a list (recursively)
        /// </summary>
        /// <param name="nodes">The children of a parent or header node</param>
        /// <param name="minVerIndex">If the vID of a stringnode is bigger than this value, then it requires an update</param>
        /// <returns>Whether any of the updated nodes require an update</returns>
        private bool CheckVersionDifference(List<Node> nodes, int minVerIndex)
        {
            bool anyUpdated = false;
            foreach(Node n in nodes)
            {
                if(n is StringNode)
                {
                    StringNode sn = n as StringNode;
                    sn.RequiresUpdate = sn.versionID > minVerIndex;
                    if (sn.RequiresUpdate) anyUpdated = true;
                }
                else if(n is ParentNode)
                {
                    ParentNode pn = n as ParentNode;
                    pn.RequiresUpdate = CheckVersionDifference(pn.ChildNodes, minVerIndex);
                    if (pn.RequiresUpdate) anyUpdated = true;
                }
            }
            return anyUpdated;
        }

        /// <summary>
        /// Save the current set strings to two files
        /// </summary>
        /// <param name="path">The file path</param>
        public void SaveContentsToFile(string path)
        {
            List<string> lines = new List<string>()
            {
                targetName,
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

            File.WriteAllText(path, string.Join("\n", lines));
            File.WriteAllText(path + ".base", string.Join("\n", baseLines));
        }
        
        /// <summary>
        /// Loads String values from a file
        /// </summary>
        /// <param name="path"></param>
        public void LoadContentsFromFile(string path)
        {
            //checking the files first
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

            //getting the lines
            string[] lines = File.ReadAllLines(path);
            string[] baseLines = File.ReadAllLines(path + ".base");

            //checking if the targetname is correct

            if(lines[0] != targetName)
            {
                throw new InvalidDataException("Language file target does not match format target!");
            }

            //Loading the different file specifications
            LoadedVersion = Version.Parse(lines[1]);
            Language = lines[2];
            Author = lines[3];

            //loading the strings into a dictionary and assigning the strings to the values
            Dictionary<string, string> languageDictionary = new Dictionary<string, string>();

            for(int i = 0; i < baseLines.Length; i++)
            {
                languageDictionary.Add(baseLines[i], lines[i + 4]);
            }

            foreach(StringNode n in stringNodes)
            {
                if(languageDictionary.ContainsKey(n.Name))
                {
                    n.NodeValue = languageDictionary[n.Name];
                }
                else
                {
                    n.ResetValue();
                }
            }

            //updating the version check based on the file version
            for(int i = 0; i < Versions.Length; i++)
            {
                if(Versions[i] > LoadedVersion)
                {
                    CheckVersionDifference(ChildNodes, i - 1);
                    return;
                }
            }
            CheckVersionDifference(ChildNodes, Versions.Length - 1);
        }
    }
}
