using SCR.Tools.DialogEditor.Data;
using SCR.Tools.Viewmodeling;

namespace SCR.Tools.DialogEditor.Viewmodeling
{
    public class VmDialogOptions : BaseViewModel
    {
        public DialogOptions Data { get; private set; }

        public VmNodeOptions CharacterOptions { get; private set; }

        public VmNodeOptions ExpressionOptions { get; private set; }

        public VmNodeIcons NodeIcons { get; private set; }

        public VmDialogOptions()
        {
            Data = new();
            CharacterOptions = new(Data.CharacterOptions);
            ExpressionOptions = new(Data.ExpressionOptions);
            NodeIcons = new(Data.NodeIcons);
        }

        public void Read(string data, string? path)
        {
            Data = JsonFormatHandler.ReadDialogOptions(data, path);
            CharacterOptions = new(Data.CharacterOptions);
            ExpressionOptions = new(Data.ExpressionOptions);
            NodeIcons = new(Data.NodeIcons);
        }

        public string Write(string? path)
        {
            return Data.WriteDialogOptions(false, path);
        }

        public void Reset()
        {
            Data = new();
            CharacterOptions = new(Data.CharacterOptions);
            ExpressionOptions = new(Data.ExpressionOptions);
            NodeIcons = new(Data.NodeIcons);
        }
    }
}
