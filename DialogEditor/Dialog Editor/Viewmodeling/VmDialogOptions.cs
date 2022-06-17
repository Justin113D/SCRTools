using SCR.Tools.DialogEditor.Data;
using SCR.Tools.UndoRedo;
using SCR.Tools.Viewmodeling;
using System.Drawing;

namespace SCR.Tools.DialogEditor.Viewmodeling
{
    public class VmDialogOptions : BaseViewModel
    {
        public DialogOptions Data { get; private set; }

        public VmNodeOptions<Color> CharacterOptions { get; private set; }

        public VmNodeOptions<Color> ExpressionOptions { get; private set; }

        public VmNodeOptions<string> NodeIcons { get; private set; }

        public VmDialogOptions()
        {
            Data = new();
            CharacterOptions = new(Data.CharacterOptions, Color.Red);
            ExpressionOptions = new(Data.ExpressionOptions, Color.Red);
            NodeIcons = new(Data.NodeIcons, "");
        }

        public void Read(string data, string? path)
        {
            Data = JsonFormatHandler.ReadDialogOptions(data, path);
            CharacterOptions = new(Data.CharacterOptions, Color.Red);
            ExpressionOptions = new(Data.ExpressionOptions, Color.Red);
            NodeIcons = new(Data.NodeIcons, "");
            ChangeTracker.Global.Reset();
        }

        public string Write(string? path)
        {
            return Data.WriteDialogOptions(false, path);
        }

        public void Reset()
        {
            Data = new();
            CharacterOptions = new(Data.CharacterOptions, Color.Red);
            ExpressionOptions = new(Data.ExpressionOptions, Color.Red);
            NodeIcons = new(Data.NodeIcons, "");
            ChangeTracker.Global.Reset();
        }
    }
}
