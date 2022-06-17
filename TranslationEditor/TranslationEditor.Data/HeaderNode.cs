using SCR.Tools.UndoRedo;
using SCR.Tools.UndoRedo.Collections;
using System.Collections.ObjectModel;
using System.Text;
using SCR.Tools.Common;
using System.Diagnostics.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SCR.Tools.TranslationEditor.Data
{
    public class HeaderNode : ParentNode
    {
        #region private fields

        private string _author;
        private string _language;
        private readonly SortedList<string, StringNode> _internalStringNodes; // ONLY USED FOR READING!
        private readonly TrackDictionary<string, StringNode> _stringNodes;
        private readonly TrackList<Version> _versions;

        #endregion

        #region Properties 

        public override HeaderNode Header => this;

        /// <summary>
        /// Gets and sets the documents author accordingly
        /// </summary>
        public string Author
        {
            get => _author;
            set
            {
                ChangeTracker.Global.TrackValueChange(
                    (v) => _author = v, _author, value.Trim());
            }
        }

        /// <summary>
        /// Sets and gets the language
        /// </summary>
        public string Language
        {
            get => _language;
            set
            {
                ChangeTracker.Global.TrackValueChange(
                    (v) => _language = v, _language, value.Trim());
            }
        }

        /// <summary>
        /// Contains all stringnodes that appear in the hierarchy
        /// </summary>
        public ReadOnlyCollection<StringNode> StringNodes { get; }

        /// <summary>
        /// All past Versions of the header
        /// </summary>
        public ReadOnlyCollection<Version> Versions { get; }

        /// <summary>
        /// The headers current active version
        /// </summary>
        public Version Version
        {
            get => _versions[^1];
            set
            {
                if (value <= _versions[^1])
                {
                    ChangeTracker.Global.BlankChange();
                    return;
                }

                ChangeTracker.Global.BeginGroup();

                RemoveUnusedVersions();
                _versions.Add(value);

                ChangeTracker.Global.EndGroup();
            }
        }

        #endregion

        public StringNode this[string key]
        {
            get
            {
                if (_stringNodes.TryGetValue(key.ToLower(), out StringNode? result))
                    return result;
                throw new KeyNotFoundException($"Key {key} not found!");
            }
        }

        public HeaderNode() : base("New Format")
        {
            _author = "";
            _language = "English";

            _internalStringNodes = new();
            _stringNodes = new(_internalStringNodes);
            StringNodes = new(_internalStringNodes.Values);

            _versions = new();
            _versions.Add(new(0, 0, 1));
            Versions = new(_versions);

        }

        public bool TryGetStringNode(string key, [MaybeNullWhen(false)] out StringNode result)
            => _stringNodes.TryGetValue(key.ToLower(), out result);

        /// <summary>
        /// Resets all string in the stringnodes of the headers hierarchy
        /// </summary>
        public void ResetAllStrings()
        {
            ChangeTracker.Global.BeginGroup();
            foreach (StringNode n in _stringNodes.Values)
                n.ResetValue();
            ChangeTracker.Global.EndGroup();
        }


        private void RemoveUnusedVersions()
        {
            ChangeTracker.Global.BeginGroup();

            bool finished = false;
            while (!finished)
            {
                int currentIndex = _versions.Count - 1;

                if (currentIndex == -1)
                    break;

                foreach (StringNode s in _stringNodes.Values)
                {
                    if (s.VersionIndex == currentIndex)
                    {
                        finished = true;
                        break;
                    }
                }

                if (finished)
                    break;

                _versions.RemoveAt(currentIndex);
            }

            ChangeTracker.Global.EndGroup();
        }

        public string GetFreeStringNodeName(string name)
        {
            name = new string(name.Where(c => !char.IsWhiteSpace(c)).ToArray());

            return _stringNodes.FindNextFreeKey(name, true);
        }

        internal void RemoveStringNodes(Node node)
        {
            ChangeTracker.Global.BeginGroup();

            StringNode[] stringNodes = GetStringNodes(node);
            foreach (StringNode stringNode in stringNodes)
            {
                string key = stringNode.Name.ToLower();
                _stringNodes.Remove(key);
            }

            ChangeTracker.Global.EndGroup();
        }

        internal void AddStringNodes(Node node, bool updateVersionIndex)
        {
            ChangeTracker.Global.BeginGroup();

            StringNode[] stringNodes = GetStringNodes(node);
            int currentVersionIndex = _versions.Count - 1;
            foreach (StringNode stringNode in stringNodes)
            {
                string newName = GetFreeStringNodeName(stringNode.Name);
                if (newName != stringNode.Name)
                {
                    stringNode.Name = newName;
                }

                string key = newName.ToLower();
                _stringNodes.Add(key, stringNode);

                if (updateVersionIndex)
                    stringNode.VersionIndex = currentVersionIndex;
            }

            ChangeTracker.Global.EndGroup();
        }

        internal void StringNodeChangeKey(StringNode node, string oldName)
        {
            if (!_internalStringNodes.ContainsValue(node))
                return;

            string lowerCase = oldName.ToLower();

            if (_stringNodes[lowerCase] != node)
            {
                throw new ArgumentException($"Old name \"{lowerCase}\" doesnt match the node \"{node.Name}\" passed. Matches: \"{_stringNodes[lowerCase].Name}\"");
            }

            string newName = node.Name.ToLower();

            _stringNodes.Remove(lowerCase);
            _stringNodes.Add(newName, node);
        }

        /// <summary>
        /// Used for adding versions when reading from a format
        /// </summary>
        /// <param name="versions"></param>
        internal void SetVersions(Version[] versions)
        {
            _versions.Clear();
            _versions.AddRange(versions);
        }

        private static StringNode[] GetStringNodes(Node node)
        {
            if (node is ParentNode parent)
            {
                HashSet<StringNode> stringNodes = new();
                Queue<ParentNode> parentNodes = new();
                parentNodes.Enqueue(parent);

                while (parentNodes.Count > 0)
                {
                    ParentNode parentNode = parentNodes.Dequeue();
                    foreach (Node n in parentNode.ChildNodes)
                    {
                        if (n is StringNode s)
                        {
                            stringNodes.Add(s);
                        }
                        else if (n is ParentNode p)
                        {
                            parentNodes.Enqueue(p);
                        }
                        else
                        {
                            throw new InvalidDataException($"Node {n.Name} in parent {parentNode.Name} is not of valid type!");
                        }
                    }
                }

                return stringNodes.ToArray();
            }
            else if (node is StringNode stringNode)
            {
                return new StringNode[] { stringNode };
            }
            throw new ArgumentException($"Node \"{node.Name}\" is not of valid type!");
        }


        #region Project saving and loading

        /// <summary>
        /// Reads and checks meta data and returns version index
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        /// <exception cref="InvalidDataException"></exception>
        private int ReadMetaData(StringReader reader)
        {
            string[] metaData = new string[4];

            for (int i = 0; i < 4; i++)
            {
                string? metaLine = reader.ReadLine();
                if (metaLine == null)
                    throw new InvalidDataException("Meta data invalid!");
                metaData[i] = metaLine;
            }

            if (metaData[0] != Name)
            {
                throw new InvalidDataException($"Formats dont match! Format Name: {Name}, Project specified for: {metaData[0]}");
            }

            Version version = new(metaData[1]);
            int versionIndex = _versions.FindIndex(x => x.Equals(version));

            if (versionIndex == -1)
            {
                throw new InvalidDataException($"Version invalid! The projects version does not match any version inside the format!");
            }

            Language = metaData[2];
            Author = metaData[3];

            return versionIndex;
        }

        public string CompileProject()
        {
            StringBuilder builder = new($"{Name}\n{Version}\n{Language}\n{Author}\n");

            foreach (KeyValuePair<string, StringNode> pair in _stringNodes)
            {
                StringNode node = pair.Value;
                if (node.ChangedVersionIndex == -1)
                {
                    // nodes that have not received any attention by
                    // the user should just not be added
                    continue;
                }

                if (node.KeepDefault || node.ChangedVersionIndex < node.VersionIndex)
                {
                    builder.Append(' ');
                    if (node.KeepDefault)
                    {
                        builder.Append('#');
                    }
                    if (node.ChangedVersionIndex < node.VersionIndex)
                    {
                        builder.Append(node.ChangedVersionIndex);
                    }
                    builder.Append(' ');
                }

                builder.Append(pair.Key);
                builder.Append(' ');
                builder.AppendLine(pair.Value.NodeValue.Escape());
            }

            return builder.ToString();
        }

        public void LoadProject(string project)
        {
            ChangeTracker.Global.BeginGroup();
            try
            {
                using StringReader reader = new(project);
                int versionIndex = ReadMetaData(reader);

                // working through the strings
                HashSet<string> remainingKeys = _stringNodes.Keys.ToHashSet();
                HashSet<string> missingKeysInFormat = new();

                string? line = reader.ReadLine();
                int lineNumber = 4;
                while (line != null)
                {
                    bool keepDefault = false;
                    int changedVersionIndex = versionIndex;

                    int startIndex = 0;
                    if (line[0] == ' ')
                    {
                        startIndex++; // skipping first space

                        // checking whether it is set to keepDefault
                        if (line[startIndex] == '#')
                        {
                            keepDefault = true;
                            startIndex++;
                        }

                        // if the next symbol isnt a space, then there must be a changed-version index
                        if (line[startIndex] != ' ')
                        {
                            // the changedversionindex ends with a space
                            int keyStart = line.IndexOf(' ', startIndex);
                            string number = line[startIndex..keyStart];
                            if (!int.TryParse(number, out changedVersionIndex))
                            {
                                throw new InvalidDataException($"Version index one line {lineNumber} couldn't be parsed");
                            }
                            startIndex = keyStart;
                        }
                        // skip the last space
                        startIndex++;
                    }
                    int valueStart = line.IndexOf(' ', startIndex);

                    string key = line[startIndex..valueStart];
                    string value = line[++valueStart..].Unescape();

                    if (remainingKeys.Remove(key))
                    {
                        _stringNodes[key].ImportValue(value, changedVersionIndex, keepDefault);
                    }

                    line = reader.ReadLine();
                    lineNumber++;
                }

                // resetting the remaining keys
                foreach (string key in remainingKeys)
                {
                    _stringNodes[key].ResetValue();
                }

            }
            catch
            {
                ChangeTracker.Global.EndGroup(true);
                throw;
            }

            ChangeTracker.Global.EndGroup();
        }


        public (string keys, string values) ExportLanguageData()
        {
            string keys = string.Join('\n', _stringNodes.Keys);

            StringBuilder sb = new($"{Name}\n{Version}\n{Language}\n{Author}\n");
            foreach (var n in _stringNodes.Values)
            {
                sb.AppendLine(n.NodeValue.Escape());
            }

            return (keys, sb.ToString());
        }

        public void ImportLanguageData(string keys, string values)
        {
            ChangeTracker.Global.BeginGroup();

            try
            {
                using StringReader valueReader = new(values);
                using StringReader keyReader = new(keys);

                int versionIndex = ReadMetaData(valueReader);

                ResetAllStrings();
                HashSet<string> remainingKeys = _stringNodes.Keys.ToHashSet();

                string? key = keyReader.ReadLine();
                while (key != null)
                {
                    string? value = valueReader.ReadLine();
                    if (value == null)
                    {
                        throw new InvalidDataException("Key and Value files dont match");
                    }

                    if (remainingKeys.Remove(key))
                    {
                        StringNode node = _stringNodes[key];
                        node.NodeValue = value.Unescape();
                        if (node.ChangedVersionIndex > -1)
                            _stringNodes[key].ChangedVersionIndex = versionIndex;
                    }

                    key = keyReader.ReadLine();
                }
            }
            catch
            {
                ChangeTracker.Global.EndGroup(true);
                throw;
            }

            ChangeTracker.Global.EndGroup();
        }

        #endregion
    }
}
