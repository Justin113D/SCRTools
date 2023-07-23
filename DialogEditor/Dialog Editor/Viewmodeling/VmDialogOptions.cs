using SCR.Tools.Dialog.Data;
using SCR.Tools.UndoRedo;
using SCR.Tools.WPF.Viewmodeling;
using System.Drawing;
using static SCR.Tools.UndoRedo.GlobalChangeTrackerC;

namespace SCR.Tools.Dialog.Editor.Viewmodeling
{
    public class VmDialogOptions : BaseViewModel
    {
        public ChangeTracker DialogOptionsTracker { get; }

        public DialogSettings Data { get; private set; }

        public VmNodeOptions<Color> CharacterOptions { get; private set; }

        public VmNodeOptions<Color> ExpressionOptions { get; private set; }

        public VmNodeOptions<string> NodeIcons { get; private set; }

        public string? PortraitsPath
        {
            get => Data.PortraitsPath;
            set
            {
                if (value == PortraitsPath)
                {
                    return;
                }

                BeginChangeGroup();

                Data.PortraitsPath = value;
                TrackNotifyProperty(nameof(PortraitsPath));

                EndChangeGroup();
            }
        }

        public VmDialogOptions()
        {
            DialogOptionsTracker = new();
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
            ResetTracker();
        }

        public string Write(string? path)
        {
            return Data.WriteDialogOptions(Properties.Settings.Default.JsonIndenting, path);
        }

        public void Reset()
        {
            Data = new();
            CharacterOptions = new(Data.CharacterOptions, Color.Red);
            ExpressionOptions = new(Data.ExpressionOptions, Color.Red);
            NodeIcons = new(Data.NodeIcons, "");
            ResetTracker();
        }
    }
}
