using System;
using System.Diagnostics;

namespace SCR.Tools.UndoRedo
{
    [DebuggerStepThrough]
    public static class GlobalChangeTrackerC
    {
        public static ChangeTracker GlobalChangeTracker { get; internal set; } = new();

        public static bool HasChanges
            => GlobalChangeTracker.HasChanges;

        public static void ResetTracker()
            => GlobalChangeTracker.Reset();

        public static bool UndoChange()
            => GlobalChangeTracker.Undo();

        public static bool RedoChange()
            => GlobalChangeTracker.Redo();

        public static void BeginChangeGroup()
            => GlobalChangeTracker.BeginGroup();

        public static void EndChangeGroup(bool discard = false)
            => GlobalChangeTracker.EndGroup(discard);

        public static void PostChangeGroupAction(Action action)
            => GlobalChangeTracker.PostGroupAction(action);

        public static void ChangeGroupNotifyPropertyChanged(Action<string> action, string name)
            => GlobalChangeTracker.GroupNotifyPropertyChanged(action, name);

        public static void TrackChange(Action redo, Action undo)
            => GlobalChangeTracker.TrackChange(redo, undo);

        public static void TrackValueChange<T>(Action<T> modifyCallback, T oldValue, T newValue)
            => GlobalChangeTracker.TrackValueChange(modifyCallback, oldValue, newValue);

        public static void BlankChange()
            => GlobalChangeTracker.BlankChange();

        public static void BlankValueChange<T>(Action<T> modifyCallback, T oldValue, T newValue) where T : IEquatable<T>
            => GlobalChangeTracker.BlankValueChange(modifyCallback, oldValue, newValue);

        public static ChangeTracker.Pin PinCurrentChange()
            => GlobalChangeTracker.PinCurrent();
    }
}
