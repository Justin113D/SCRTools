using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private string _author;

        /// <summary>
        /// The language of the current file
        /// </summary>
        private string _language;

        /// <summary>
        /// Gets and sets the documents author accordingly
        /// </summary>
        public string Author
        {
            get => _author;
            set => _author = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        /// <summary>
        /// Sets and gets the language
        /// </summary>
        public string Language
        {
            get => _language;
            set => _language = string.IsNullOrWhiteSpace(value) ? "-" : value.Trim();
        }

        /// <summary>
        /// The version map inside the format file, which is used to assign a "last updated" version for each string
        /// </summary>
        public List<Version> Versions;

        /// <summary>
        /// Version of the game that the file is for
        /// </summary>
        public Version Version => Versions.Count == 0 ? null : Versions[^1];

        /// <summary>
        /// The version of the loaded file
        /// </summary>
        public Version LoadedVersion { get; private set; }

        /// <summary>
        /// Contains all stringnodes that appear in the hierarchy
        /// </summary>
        public SortedList<string, StringNode> StringNodes { get; private set; }

        /// <summary>
        /// The children of this node
        /// </summary>
        public List<Node> ChildNodes { get; private set; }

        public override int NodeState => throw new NotImplementedException();

        public HeaderNode() : base("New", NodeType.HeaderNode)
        {
            Versions = new List<Version>()
            {
                new Version("0.0.0.1")
            };
            Language = "English";
            ChildNodes = new List<Node>();
            StringNodes = new SortedList<string, StringNode>();
        }


        /// <summary>
        /// Resets all string in the stringnodes of the headers hierarchy
        /// </summary>
        public void ResetAllStrings()
        {
            foreach(KeyValuePair<string, StringNode> n in StringNodes)
                n.Value.ResetValue();
        }

        /// <summary>
        /// Updates the update variable of all nodes in a list (recursively)
        /// </summary>
        /// <param name="nodes">The children of a parent or header node</param>
        /// <param name="minVerIndex">If the vID of a stringnode is bigger than this value, then it requires an update</param>
        /// <returns>Whether any of the updated nodes require an update</returns>
        private void CheckVersionDifference(List<Node> nodes, int minVerIndex)
        {
            foreach(Node n in nodes)
            {
                if(n is StringNode)
                {
                    StringNode sn = n as StringNode;
                    sn.requiresUpdate = sn.VersionID >= minVerIndex;
                }
                else if(n is ParentNode)
                {
                    ParentNode pn = n as ParentNode;
                    CheckVersionDifference(pn.ChildNodes, minVerIndex);
                }
            }
        }

        /// <summary>
        /// Creates a new top-level parent node
        /// </summary>
        /// <returns></returns>
        public ParentNode NewParentNode()
        {
            ParentNode node = new ParentNode("New Parent");
            ChildNodes.Add(node);
            return node;
        }

        /// <summary>
        /// Creates a string node and adds it to the database
        /// </summary>
        /// <returns></returns>
        private StringNode CreateStringNode()
        {
            int number = 1;
            while(StringNodes.ContainsKey("newString" + number))
                number++;
            StringNode node = new StringNode("newString" + number, "value", Versions.Count - 1);
            StringNodes.Add(node.Name, node);
            return node;
        }

        /// <summary>
        /// Creates a new top-level string node
        /// </summary>
        /// <returns></returns>
        public StringNode NewStringNode()
        {
            StringNode node = CreateStringNode();
            ChildNodes.Add(node);
            return node;
        }

        /// <summary>
        /// Creates a new string node and makes it a child of another parent node
        /// </summary>
        /// <param name="parent">Parent of the new node</param>
        public StringNode NewStringNode(ParentNode parent)
        {
            StringNode node = CreateStringNode();
            parent.ChildNodes.Add(node);
            return node;
        }

        #region Reading and writing files

        public static HeaderNode LoadFormatFromFile(string path)
        {
            HeaderNode result = new HeaderNode();

            using JsonTextReader reader = new JsonTextReader(new StringReader(File.ReadAllText(path)));

            while(reader.Read() && reader.TokenType != JsonToken.EndObject)
            {
                if(reader.TokenType == JsonToken.PropertyName)
                {
                    string tokenName = (string)reader.Value;
                    reader.Read();
                    switch(tokenName)
                    {
                        case "Name":
                            result.Name = (string)reader.Value;
                            break;
                        case "Description":
                            result.Description = (string)reader.Value;
                            break;
                        case "DefaultLanguage":
                            result._language = (string)reader.Value;
                            break;
                        case "Versions":
                            result.Versions.Clear();
                            while(reader.Read() && reader.TokenType != JsonToken.EndArray)
                            {
                                result.Versions.Add(new Version((string)reader.Value));
                            }
                            break;
                        case "ChildNodes":
                            while(reader.Read() && reader.TokenType != JsonToken.EndArray)
                            {
                                result.ChildNodes.Add(ReadJson(reader));
                            }
                            break;
                    }
                }
            }

            void GetStringNodes(List<Node> nodes)
            {
                foreach(Node node in nodes)
                {
                    if(node is ParentNode pn)
                        GetStringNodes(pn.ChildNodes);
                    else if(node is StringNode sn)
                        result.StringNodes.Add(sn.Name.ToLower(), sn);
                }
            }

            GetStringNodes(result.ChildNodes);

            return result;
        }
        
        public void SaveFormatToFile(string path)
        {
            using StringWriter strWriter = new StringWriter();
            using JsonTextWriter writer = new JsonTextWriter(strWriter)
            {
                Formatting = Properties.Settings.Default.JsonIndenting ? Formatting.Indented : Formatting.None
            };

            WriteJson(writer);

            File.WriteAllText(path, strWriter.ToString());
        }

        protected override void WriteJsonInner(JsonTextWriter writer)
        {
            writer.WritePropertyName("DefaultLanguage");
            writer.WriteValue(_language);

            writer.WritePropertyName("Versions");
            writer.WriteStartArray();
            foreach(var v in Versions)
                writer.WriteValue(v.ToString());
            writer.WriteEndArray();

            writer.WritePropertyName("ChildNodes");
            writer.WriteStartArray();
            foreach(Node n in ChildNodes)
                n.WriteJson(writer);
            writer.WriteEndArray();
        }
    
        /// <summary>
        /// Save the current set strings to two files
        /// </summary>
        /// <param name="path">The file path</param>
        public void SaveContentsToFile(string path)
        {
            List<string> lines = new List<string>()
            {
                Name,
                Version.ToString(),
                Language,
                Author ?? "",
            };
            List<string> baseLines = new List<string>();
            
            foreach(KeyValuePair<string, StringNode> s in StringNodes)
            {
                lines.Add(s.Value.NodeValue.Replace("\n", "\\n"));
                baseLines.Add(s.Key);
            }

            File.WriteAllText(path, string.Join("\n", lines));
            File.WriteAllText(Path.ChangeExtension(path, "langkey"), string.Join("\n", baseLines));
        }
        
        /// <summary>
        /// Loads String values from a file
        /// </summary>
        /// <param name="path"></param>
        public void LoadContentsFromFile(string path)
        {
            if(!File.Exists(path))
                throw new FileNotFoundException(".lang file not found!");

            string keyPath = Path.ChangeExtension(path, "langkey");
            if(!File.Exists(keyPath))
                throw new FileNotFoundException(".base file not found!");

            //getting the lines
            string[] lines = File.ReadAllLines(path);
            string[] keyLines = File.ReadAllLines(keyPath);

            //checking if the targetname is correct

            if(lines[0] != Name)
            {
                throw new InvalidDataException("Language file target does not match format target!");
            }

            //Loading the different file specifications
            LoadedVersion = Version.Parse(lines[1]);
            Language = lines[2];
            Author = lines[3];

            //loading the strings into a dictionary and assigning the strings to the values
            Dictionary<string, string> languageDictionary = new Dictionary<string, string>();
            for(int i = 0; i < keyLines.Length; i++)
                languageDictionary.Add(keyLines[i], lines[i + 4]);

            foreach(KeyValuePair<string, StringNode> n in StringNodes)
            {
                if(languageDictionary.ContainsKey(n.Key))
                {
                    n.Value.NodeValue = languageDictionary[n.Key];
                    n.Value.retranslated = false;
                }
                else
                    n.Value.ResetValue();
            }

            //updating the version check based on the file version
            // TODO there's gotta be a better way to do this
            for(int i = 0; i < Versions.Count; i++)
            {
                if(Versions[i] > LoadedVersion)
                {
                    CheckVersionDifference(ChildNodes, i);
                    return;
                }
            }
        }

        #endregion
    }
}
