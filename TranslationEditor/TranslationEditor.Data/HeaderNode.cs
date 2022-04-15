using SCR.Tools.UndoRedo;
using SCR.Tools.UndoRedo.ListChange;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.Text;
using SCR.Tools.Common;

namespace SCR.Tools.TranslationEditor.Data
{
    public class HeaderNode : ParentNode
    {
        #region private fields

        private string _author;
        private string _language;
        private readonly SortedList<string, StringNode> _stringNodes;
        private readonly List<Version> _versions;

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
                ChangeTracker.Global.TrackChange(
                    new ChangedValue<string>(
                        (v) => _author = v,
                        _author,
                        value.Trim()
                ));
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
                ChangeTracker.Global.TrackChange(
                    new ChangedValue<string>(
                        (v) => _language = v,
                        _language,
                        value.Trim()
                ));
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
            set {
                if (value <= _versions[^1])
                {
                    ChangeTracker.Global.BlankChange();
                    return;
                }

                ChangeTracker.Global.BeginGroup();

                RemoveUnusedVersions();

                ChangeTracker.Global.TrackChange(
                    new ChangeListAdd<Version>(_versions, value));

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

            _stringNodes = new();
            StringNodes = new(_stringNodes.Values);

            _versions = new();
            _versions.Add(new(0, 0, 1));
            Versions = new(_versions);

        }

        public bool TryGetStringNode(string key)
            => _stringNodes.TryGetValue(key.ToLower(), out StringNode? result);

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
            while(!finished)
            {
                int currentIndex = _versions.Count - 1;

                if (currentIndex == -1)
                    break;

                foreach(StringNode s in _stringNodes.Values)
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

            // if name is whitespace or doesnt exist, then we can just return it
            if (name.Length == 0 || !_stringNodes.ContainsKey(name.ToLower()))
            {
                return name;
            }

            // check for a .### (e.g. .001) ending
            bool endsWithNumber = false;
            bool hasNumber = false;
            int index = name.Length - 1;
            for (; index > 0; index--)
            {
                char curChar = name[index];
                if (curChar == '.')
                {
                    if (hasNumber)
                        endsWithNumber = true;
                    break;
                }

                if (!char.IsNumber(curChar))
                    break;

                hasNumber = true;
            }

            int nameIndex = 1;
            string baseName = name;

            // if the name ends with a number, then we want to continue off of it
            if (endsWithNumber)
            {
                baseName = name[..index];
                nameIndex = int.Parse(name[++index..]);
            }

            // check names until we find a fitting one
            string newName = $"{baseName}.{nameIndex:D3}";
            while (_stringNodes.ContainsKey(newName.ToLower()))
            {
                nameIndex++;
                newName = $"{baseName}.{nameIndex:D3}";
            }

            return newName;
        }

        internal void RemoveStringNodes(Node node)
        {
            ChangeTracker.Global.BeginGroup();

            StringNode[] stringNodes = GetStringNodes(node);
            foreach (StringNode stringNode in stringNodes)
            {
                string key = stringNode.Name.ToLower();

                ChangeTracker.Global.TrackChange(new Change(
                    () => _stringNodes.Remove(key),
                    () => _stringNodes.Add(key, stringNode)
                ));
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

                ChangeTracker.Global.TrackChange(new Change(
                    () => _stringNodes.Add(key, stringNode),
                    () => _stringNodes.Remove(key)
                ));

                if(updateVersionIndex)
                    stringNode.VersionIndex = currentVersionIndex;
            }

            ChangeTracker.Global.EndGroup();
        }

        internal void StringNodeChangeKey(StringNode node, string oldName)
        {
            if (!_stringNodes.ContainsValue(node))
                return;

            string lowerCase = oldName.ToLower();

            if (_stringNodes[lowerCase] != node)
            {
                throw new ArgumentException($"Old name \"{lowerCase}\" doesnt match the node \"{node.Name}\" passed. Matches: \"{_stringNodes[lowerCase].Name}\"");
            }

            _stringNodes.Remove(lowerCase);

            _stringNodes.Add(node.Name.ToLower(), node);
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
            int versionIndex = _versions.FindIndex(x => x == version);

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
            RemoveUnusedVersions();

            StringBuilder builder = new($"{Name}\n{Version}\n{Language}\n{Author}\n");

            foreach (KeyValuePair<string, StringNode> pair in _stringNodes)
            {
                StringNode node = pair.Value;
                if(node.ChangedVersionIndex == -1)
                {
                    // nodes that have not received any attention by
                    // the user should just not be added
                    continue;
                }

                if(node.KeepDefault || node.ChangedVersionIndex < node.VersionIndex)
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

        public void LoadProject(string project, out string[] missingKeys)
        {
            using StringReader reader = new(project);

            ChangeTracker.Global.BeginGroup();
            int versionIndex;
            try
            {
                versionIndex = ReadMetaData(reader);
            }
            catch
            {
                ChangeTracker.Global.EndGroup();
                throw;
            }

            // working through the strings
            HashSet<string> remainingKeys = _stringNodes.Keys.ToHashSet();
            HashSet<string> missingKeysInFormat = new();

            string? line = reader.ReadLine();
            int lineNumber = 4;
            while(line != null)
            {
                bool keepDefault = false;
                int changedVersionIndex = versionIndex;

                int startIndex = 0;
                if(line[0] == ' ')
                {
                    startIndex++; // skipping first space

                    // checking whether it is set to keepDefault
                    if(line[startIndex] == '#')
                    {
                        keepDefault = true;
                        startIndex++; 
                    }

                    // if the next symbol isnt a space, then there must be a changed-version index
                    if(line[startIndex] != ' ')
                    {
                        // the changedversionindex ends with a space
                        int keyStart = line.IndexOf(' ', startIndex);
                        string number = line[startIndex..keyStart];
                        if(!int.TryParse(number, out changedVersionIndex))
                        {
                            ChangeTracker.Global.EndGroup();
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

                if(remainingKeys.Remove(key))
                {
                    _stringNodes[key].ImportValue(value, changedVersionIndex, keepDefault);
                }
                else if(!missingKeysInFormat.Add(key))
                {
                    ChangeTracker.Global.EndGroup();
                    throw new InvalidDataException($"Key {key} found twice!");
                }

                line = reader.ReadLine();
                lineNumber++;
            }

            // resetting the remaining keys
            foreach(string key in remainingKeys)
            {
                _stringNodes[key].ResetValue();
            }

            missingKeys = missingKeysInFormat.ToArray();

            ChangeTracker.Global.EndGroup();
        }


        public (string keys, string values) ExportLanguageData()
        {
            string keys = string.Join('\n', _stringNodes.Keys);

            StringBuilder sb = new($"{Name}\n{Version}\n{Language}\n{Author}\n");
            foreach(var n in _stringNodes.Values)
            {
                sb.AppendLine(n.NodeValue.Escape());
            }
            
            return (keys, sb.ToString());
        }

        public void ImportLanguageData(string keys, string values)
        {
            using StringReader valueReader = new(values);
            using StringReader keyReader = new(keys);

            ChangeTracker.Global.BeginGroup();
            int versionIndex;
            try
            {
                versionIndex = ReadMetaData(valueReader);
            }
            catch
            {
                ChangeTracker.Global.EndGroup();
                throw;
            }

            ResetAllStrings();
            HashSet<string> remainingKeys = _stringNodes.Keys.ToHashSet();

            string? key = keyReader.ReadLine();
            while(key != null)
            {
                string? value = valueReader.ReadLine();
                if (value == null)
                {
                    ChangeTracker.Global.EndGroup();
                    throw new InvalidDataException("Key and Value files dont match");
                }

                if(remainingKeys.Remove(key))
                {
                    StringNode node = _stringNodes[key];
                    node.NodeValue = value.Unescape();
                    if(node.ChangedVersionIndex > -1)
                        _stringNodes[key].ChangedVersionIndex = versionIndex;
                }

                key = keyReader.ReadLine();
            }

            ChangeTracker.Global.EndGroup();
        }

        #endregion
    }
}
