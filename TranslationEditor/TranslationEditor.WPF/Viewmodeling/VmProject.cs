using SCR.Tools.TranslationEditor.Data;
using SCR.Tools.UndoRedo;
using SCR.Tools.Viewmodeling;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCR.Tools.TranslationEditor.WPF.Viewmodeling
{
    public class VmProject : BaseViewModel
    {
        public HeaderNode Data { get; }

        public ChangeTracker ProjectTracker { get; }

        public int UntranslatedNodes { get; private set; }

        public int OutdatedNodes { get; private set; }

        public int TranslatedNodes { get; private set; }


        public string Author
        {
            get => Data.Author;
            set
            {
                if (Data.Author == value)
                    return;

                ProjectTracker.BeginGroup();

                Data.Author = value;
                TrackNotifyProperty(nameof(Author));

                ProjectTracker.EndGroup();
            }
        }

        public string Language
        {
            get => Data.Language;
            set
            {
                if (Data.Language == value)
                    return;

                ProjectTracker.BeginGroup();

                Data.Language = value;
                TrackNotifyProperty(nameof(Language));

                ProjectTracker.EndGroup();
            }
        }

        public string TargetName
            => Data.Name;

        public string Version
            => Data.Version.ToString();


        private readonly ObservableCollection<VmNode> _nodes;

        public ReadOnlyObservableCollection<VmNode> Nodes { get; }


        public VmProject(HeaderNode data, ChangeTracker projectTracker)
        {
            Data = data;
            ProjectTracker = projectTracker;

            _nodes = new();
            Nodes = new(_nodes);
            CreateNodes();
            CountNodes();
        }

        private void CreateNodes()
        {
            foreach (Node node in Data.ChildNodes)
            {
                if (node is ParentNode p)
                {
                    _nodes.Add(new VmParentNode(this, p));
                }
                else if (node is StringNode s)
                {
                    _nodes.Add(new VmStringNode(this, s));
                }
            }
        }

        private void CountNodes()
        {
            TranslatedNodes = 0;
            OutdatedNodes = 0;
            UntranslatedNodes = 0;

            foreach (StringNode sNode in Data.StringNodes)
            {
                switch (sNode.State)
                {
                    case NodeState.Translated:
                        TranslatedNodes++;
                        break;
                    case NodeState.Outdated:
                        OutdatedNodes++;
                        break;
                    case NodeState.Untranslated:
                        UntranslatedNodes++;
                        break;
                }
            }
        }
        
        private void TrackNotifyProperty(string propertyName)
        {
            ProjectTracker.GroupNotifyPropertyChanged(OnPropertyChanged, propertyName);
        }


        public void IncreaseNodeCounter(NodeState state)
        {
            ChangedValue<int> change;

            switch (state)
            {
                case NodeState.Translated:
                    change = new((v) => TranslatedNodes = v,
                        TranslatedNodes,
                        TranslatedNodes + 1);
                    break;
                case NodeState.Outdated:
                    change = new((v) => OutdatedNodes = v,
                        OutdatedNodes,
                        OutdatedNodes + 1);
                    break;
                case NodeState.Untranslated:
                    change = new((v) => UntranslatedNodes = v,
                        UntranslatedNodes,
                        UntranslatedNodes + 1);
                    break;
                default:
                    return;
            }

            ProjectTracker.TrackChange(change);
        }

        public void DecreaseNodeCounter(NodeState state)
        {
            ChangedValue<int> change;

            switch (state)
            {
                case NodeState.Translated:
                    change = new((v) => TranslatedNodes = v,
                        TranslatedNodes,
                        TranslatedNodes - 1);
                    break;
                case NodeState.Outdated:
                    change = new((v) => OutdatedNodes = v,
                        OutdatedNodes,
                        OutdatedNodes - 1);
                    break;
                case NodeState.Untranslated:
                    change = new((v) => UntranslatedNodes = v,
                        UntranslatedNodes,
                        UntranslatedNodes - 1);
                    break;
                default:
                    return;
            }

            ProjectTracker.TrackChange(change);
        }

        public void RefreshNodeValues()
        {
            ProjectTracker.BeginGroup();
            foreach (VmNode node in _nodes)
            {
                node.RefreshNodeValues();
            }
            ProjectTracker.EndGroup();
        }
        


        public void LoadProject(string data)
        {
            ProjectTracker.BeginGroup();
            ProjectTracker.ResetOnNextChange = true;

            Data.LoadProject(data);
            RefreshNodeValues();

            ProjectTracker.EndGroup();
        }

        public string WriteProject()
        {
            string data = Data.CompileProject();
            return data;
        }

        public void NewProject()
        {
            ProjectTracker.BeginGroup();
            ProjectTracker.ResetOnNextChange = true;

            Data.ResetAllStrings();
            RefreshNodeValues();

            ProjectTracker.EndGroup();
        }

        public void ExportLanguage(string filepath)
        {
            (string keys, string values) = Data.ExportLanguageData();

            File.WriteAllText(filepath, values);

            string keyFilePath = Path.ChangeExtension(filepath, "langkey");
            File.WriteAllText(keyFilePath, keys);
        }

        public void ImportLanguage(string filepath)
        {
            string values = File.ReadAllText(filepath);

            string keyFilePath = Path.ChangeExtension(filepath, "langkey");
            string keys = File.ReadAllText(keyFilePath);

            ProjectTracker.BeginGroup();
            ProjectTracker.ResetOnNextChange = true;

            Data.ImportLanguageData(keys, values);

            RefreshNodeValues();

            ProjectTracker.EndGroup();
        }

    }
}
