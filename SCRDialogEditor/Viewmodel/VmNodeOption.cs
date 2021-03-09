using ColorPickerWPF;
using SCRCommon.Viewmodels;
using SCRDialogEditor.Data;
using System.Windows.Media;

namespace SCRDialogEditor.Viewmodel
{
    public class VmNodeOption : BaseViewModel
    {
        public RelayCommand Cmd_ChangeColor { get; }

        private readonly VmNodeOptions _parent;

        private NodeOption _nodeOption;

        public string Name
        {
            get => _nodeOption?.Name ?? "";
            set
            {
                if(string.IsNullOrWhiteSpace(value))
                    return;
                if(_nodeOption == null)
                    Qualify();
                _nodeOption.Name = value;
            }
        }


        public SolidColorBrush Color => new SolidColorBrush(_nodeOption?.Color ?? Colors.Transparent);

        public VmNodeOption(VmNodeOptions options, NodeOption nodeOption)
        {
            Cmd_ChangeColor = new RelayCommand(ChangeColor);
            _parent = options;
            _nodeOption = nodeOption;
        }

        /// <summary>
        /// When the null node gets edited, it will be added to the list of nodes
        /// </summary>
        private void Qualify()
        {
            _nodeOption = new NodeOption();
            _parent.QualifyNode(_nodeOption);
            OnPropertyChanged(nameof(Color));
        }

        private void ChangeColor()
        {
            if(_nodeOption == null)
                return;

            if(ColorPickerWindow.ShowDialog(out Color col, ColorPickerWPF.Code.ColorPickerDialogOptions.SimpleView))
            {
                _nodeOption.Color = col;
                OnPropertyChanged(nameof(Color));
            }
        }
    }
}
