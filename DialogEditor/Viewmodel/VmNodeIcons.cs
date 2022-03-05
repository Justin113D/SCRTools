using Microsoft.Win32;
using PropertyChanged;
using SCR.Tools.Viewmodeling;
using SCR.Tools.DialogEditor.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace SCR.Tools.DialogEditor.Viewmodel
{
    public class VmNodeIcons : BaseViewModel
    {
        public VmDialogOptions DialogOptions { get; }

        public readonly List<NodeIcon> _rawIcons;

        public ObservableCollection<VmNodeIcon> Icons { get; }

        public ObservableCollection<VmNodeIcon> IconsCombobox { get; }

        public VmNodeIcons(List<NodeIcon> icons, VmDialogOptions dialogOptions)
        {
            DialogOptions = dialogOptions;
            _rawIcons = icons;
            Icons = new();
            foreach(var o in _rawIcons)
                Icons.Add(new(this, o));
            IconsCombobox = new(Icons.ToArray());
            Icons.Add(new(this, null));
        }

        public void QualifyNode(NodeIcon option)
        {
            IconsCombobox.Add(Icons[^1]);
            _rawIcons.Add(option);
            Icons.Add(new(this, null));
            OnPropertyChanged(nameof(_rawIcons));
        }

        public VmNodeIcon GetIcon(string name)
            => Icons.First(x => x.Name == name);
    }

    public class VmNodeIcon : BaseViewModel
    {
        public RelayCommand Cmd_SelectPath
            => new(SelectPath);

        private readonly VmNodeIcons _parent;

        private NodeIcon _data;

        public string Name
        {
            get => _data?.Name ?? "";
            set
            {
                if(string.IsNullOrWhiteSpace(value))
                    return;
                if(_data == null)
                    Qualify();
                _data.Name = value;
            }
        }

        public string FilePath
            => _data?.IconPath;

        [DoNotCheckEquality]
        public string FullFilePath
        {
            get => FilePath == null ? null : Path.GetFullPath(_data.IconPath, _parent.DialogOptions.LoadedFilePath);
            set
            {
                if(string.IsNullOrWhiteSpace(value))
                    return;
                if(!File.Exists(value))
                    return;

                _data.IconPath = Path.GetRelativePath(_parent.DialogOptions.LoadedFilePath, value);
                OnPropertyChanged(nameof(FilePath));
            }
        }


        public VmNodeIcon(VmNodeIcons icons, NodeIcon nodeIcon)
        {
            _parent = icons;
            _data = nodeIcon;
        }

        /// <summary>
        /// When the null node gets edited, it will be added to the list of nodes
        /// </summary>
        private void Qualify()
        {
            _data = new();
            _parent.QualifyNode(_data);
        }

        private void SelectPath()
        {
            if(_data == null)
                return;

            OpenFileDialog ofd = new()
            {
                Title = "Select image path",
                Filter = "PNG image (*.png)|*.png"
            };

            if(ofd.ShowDialog() == true)
                FullFilePath = ofd.FileName;
        }

        public override string ToString()
            => Name;
    }
}
