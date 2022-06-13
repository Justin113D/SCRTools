using SCR.Tools.DialogEditor.Data;
using SCR.Tools.UndoRedo;
using SCR.Tools.UndoRedo.ListChange;
using SCR.Tools.Viewmodeling;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCR.Tools.DialogEditor.Viewmodeling
{
    public class VmDialog : BaseViewModel
    {
        private readonly ObservableCollection<VmNode> _nodes;

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

        #region Relay Commands

        public RelayCommand CmdSortNodes
            => new(SortNodes);

        #endregion

        public VmDialog(VmMain mainVM, Dialog dialog)
        {
            Main = mainVM;
            Data = dialog;

            _nodes = new();
            foreach (Node node in dialog.Nodes)
            {
                VmNode vmnode = new(this, node);
                _nodes.Add(vmnode);
            }
            Nodes = new(_nodes);
        }

        private void SortNodes()
        {
            ChangeTracker.Global.BeginGroup();

            Data.Sort();

            ChangeTracker.Global.TrackChange(
                new ChangeCollectionContent<VmNode>(
                    _nodes, 
                    Nodes.OrderBy(x => Data.Nodes.IndexOf(x.Data)).ToArray()
                ));

            ChangeTracker.Global.EndGroup();
        }
    }
}
