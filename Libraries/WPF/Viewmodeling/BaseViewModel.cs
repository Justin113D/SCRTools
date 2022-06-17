using System.ComponentModel;
using static SCR.Tools.UndoRedo.GlobalChangeTrackerC;

namespace SCR.Tools.Viewmodeling
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Tells the current change tracker grouping to notify the viewmodel that a property changed on undo/redo
        /// </summary>
        /// <param name="propertyName"></param>
        protected void TrackNotifyProperty(string propertyName)
        {
            ChangeGroupNotifyPropertyChanged(OnPropertyChanged, propertyName);
        }
    }
}
