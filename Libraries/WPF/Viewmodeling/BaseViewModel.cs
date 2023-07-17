using System.ComponentModel;
using System.Diagnostics;
using static SCR.Tools.UndoRedo.GlobalChangeTrackerC;

namespace SCR.Tools.WPF.Viewmodeling
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        [DebuggerStepThrough]
        protected void OnPropertyChanged(string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Tells the current change tracker grouping to notify the viewmodel that a property changed on undo/redo
        /// </summary>
        /// <param name="propertyName"></param>
        [DebuggerStepThrough]
        protected void TrackNotifyProperty(string propertyName)
        {
            ChangeGroupNotifyPropertyChanged(OnPropertyChanged, propertyName);
        }
    }
}
