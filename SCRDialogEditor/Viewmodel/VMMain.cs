using PropertyChanged;
using SCRCommon.Viewmodels;
using SCRDialogEditor.Data;
using System.Windows.Media;

namespace SCRDialogEditor.Viewmodel
{
    public class VmMain : FileBaseViewModel
    {
        public VmGrid Grid { get; private set; }

        /// <summary>
        /// Dialog options
        /// </summary>
        public VmDialogOptions DialogOptions { get; }

        public override string FileFilter
            => "Json File (*.json)|*.json";

        public override string FileTypeName
            => "Json";

        public override ChangeTracker PinTracker
            => Grid.Tracker;

        public string FeedbackText { get; private set; }

        [DoNotCheckEquality]
        public Color? FeedbackColor { get; private set; }

        public VmMain()
        {
            DialogOptions = new();
            Grid = new(this, new());
        }


        public override bool Load(string path)
        {
            Dialog data;
            try
            {
                data = Dialog.LoadFromFile(path);
            }
            catch
            {
                SetFeedback($"Could not load file", false);
                return false;
            }

            Grid = new VmGrid(this, data);
            SetFeedback($"Loaded file", true);
            return true;
        }

        public override void Save(string path)
        {
            Grid.Data.SaveToFile(path);
            SetFeedback($"Saved to  \"{path}\"", true);
        }

        public override bool ResetConfirmation()
            => true;

        public override void Reset()
        {
            Grid = new(this, new());
            SetFeedback("Resetted!", true);
        }

        public void SetFeedback(string message, bool success)
        {
            FeedbackColor = null;
            if(string.IsNullOrWhiteSpace(message))
            {
                FeedbackText = null;
                return;
            }

            FeedbackColor = success ? SCRCommon.WpfStyles.Colors.Green
                                    : SCRCommon.WpfStyles.Colors.Red;
            FeedbackText = message;
        }
    }
}
