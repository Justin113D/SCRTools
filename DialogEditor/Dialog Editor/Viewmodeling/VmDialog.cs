using SCR.Tools.DialogEditor.Data;
using SCR.Tools.UndoRedo;
using SCR.Tools.UndoRedo.Collections;
using SCR.Tools.Viewmodeling;
using System.Collections.ObjectModel;
using System.Linq;

namespace SCR.Tools.DialogEditor.Viewmodeling
{
    public class VmDialog : BaseViewModel
    {
        private readonly TrackList<VmNode> _nodes;

        public VmMain Main { get; }

        /// <summary>
        /// Dialog data
        /// </summary>
        public Dialog Data { get; }

        /// <summary>
        /// Nodes to display
        /// </summary>
        public ReadOnlyObservableCollection<VmNode> Nodes { get; private set; }

        #region Wrapper Properties

        public string Name
        {
            get => Data.Name;
            set
            {
                ChangeTracker.Global.BeginGroup();
                Data.Name = value;
                ChangeTracker.Global.PostGroupAction(() => OnPropertyChanged(nameof(Name)));
                ChangeTracker.Global.EndGroup();
            }
        }

        public string Author
        {
            get => Data.Author;
            set
            {
                ChangeTracker.Global.BeginGroup();
                Data.Author = value;
                ChangeTracker.Global.PostGroupAction(() => OnPropertyChanged(nameof(Author)));
                ChangeTracker.Global.EndGroup();
            }
        }

        public string Description
        {
            get => Data.Description;
            set
            {
                ChangeTracker.Global.BeginGroup();
                Data.Description = value;
                ChangeTracker.Global.PostGroupAction(() => OnPropertyChanged(nameof(Description)));
                ChangeTracker.Global.EndGroup();
            }
        }

        #endregion

        #region Interaction Properties

        #endregion

        #region Relay Commands

        public RelayCommand CmdSortNodes
            => new(SortNodes);

        #endregion

        public VmDialog(VmMain mainVM, Dialog dialog)
        {
            Main = mainVM;
            Data = dialog;

            ObservableCollection<VmNode> internalNodes = new();

            foreach (Node node in dialog.Nodes)
            {
                VmNode vmnode = new(this, node);
                internalNodes.Add(vmnode);
            }

            _nodes = new(internalNodes);
            Nodes = new(internalNodes);
        }

        private void SortNodes()
        {
            ChangeTracker.Global.BeginGroup();

            Data.Sort();

            _nodes.Clear();
            _nodes.AddRange(
                Nodes.OrderBy(x => Data.Nodes.IndexOf(x.Data))
                );

            ChangeTracker.Global.EndGroup();
        }
    }
}
