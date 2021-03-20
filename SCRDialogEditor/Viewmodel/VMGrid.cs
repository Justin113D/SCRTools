﻿using SCRCommon.Viewmodels;
using SCRCommon.WpfStyles;
using SCRDialogEditor.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SCRDialogEditor.Viewmodel
{
    public class VmGrid : BaseViewModel
    {
        #region Commands

        /// <summary>
        /// Deletes the active node from the grid
        /// </summary>
        public RelayCommand Cmd_DeleteNode
            => new(DeleteActiveNode);

        public RelayCommand Cmd_RecenterContents
            => new(RecenterContents);

        public RelayCommand Cmd_SortNodes
            => new(Sort);

        #endregion

        #region Private Fields

        private VmNode _active;

        private VmNode _grabbed;

        private VmNodeOutput _connecting;

        #endregion

        #region Properties

        #region Source Properties
        public VmMain Main { get; }

        /// <summary>
        /// Dialog data
        /// </summary>
        public Dialog Data { get; }

        /// <summary>
        /// Nodes to display
        /// </summary>
        public ObservableCollection<VmNode> Nodes { get; private set; }

        /// <summary>
        /// All note outputs
        /// </summary>
        public ObservableCollection<VmNodeOutput> Outputs { get; }

        #endregion

        #region Wrapper Properties

        public string Name
        {
            get => Data.Name;
            set => Data.Name = value;
        }

        public string Author
        {
            get => Data.Author;
            set => Data.Author = value;
        }

        public string Description
        {
            get => Data.Description;
            set => Data.Description = value;
        }

        #endregion

        #region Interaction Properties

        /// <summary>
        /// Selected Node to edit
        /// </summary>
        public VmNode Active
        {
            get => _active;
            set
            {
                VmNode oldActive = _active;
                _active = value;

                oldActive?.RefreshActive();
                _active?.RefreshActive();
            }
        }

        /// <summary>
        /// Grabbed Node
        /// </summary>
        public VmNode Grabbed
        {
            get => _grabbed;
            set
            {
                if(_grabbed == value)
                    return;

                _grabbed?.EndGrab();
                _grabbed = value;
            }
        }

        /// <summary>
        /// The nodeoutput that is currently being "dragged"
        /// </summary>
        public VmNodeOutput Connecting
        {
            get => _connecting;
            set
            {
                if(value == _connecting)
                    return;

                if(value != null)
                    value.Displaying = true;

                if(_connecting != null && _connecting.VmOutput == null)
                    _connecting.Displaying = false;

                _connecting = value;
            }
        }

        #endregion

        #endregion

        public VmGrid(VmMain mainVM, Dialog dialog)
        {
            Main = mainVM;
            Data = dialog;

            Outputs = new();
            Nodes = new();

            Dictionary<Node, VmNode> viewmodelPairs = new();
            foreach(Node node in dialog.Nodes)
            {
                VmNode vmnode = new(this, node);
                Nodes.Add(vmnode);
                viewmodelPairs.Add(node, vmnode);
            }

            foreach(VmNode vmnode in Nodes)
                foreach(VmNodeOutput vmout in vmnode.Outputs)
                    if(vmout.Data.Output != null)
                    {
                        vmout.VmOutput = viewmodelPairs[vmout.Data.Output];
                    }

        }


        /// <summary>
        /// Lets go of the grabbed connection/node
        /// </summary>
        public void LetGo()
        {
            Grabbed = null;
            Connecting = null;
        }

        /// <summary>
        /// Moves whatever is grabbed <br/>
        /// nodes require difference, while connections the absolute position
        /// </summary>
        /// <param name="dif"></param>
        /// <param name="absolute"></param>
        public void MoveGrabbed(Point dif, Point absolute)
        {
            Grabbed?.Move( dif );
            Connecting?.UpdateEndPosition(absolute);
        }

        /// <summary>
        /// Adds a new node at Mouse position
        /// </summary>
        public void CreateNode(Point position)
        {
            Node node = Data.CreateNode();

            VmNode vmnode = new(this, node, position);

            Nodes.Add(vmnode);
        }

        /// <summary>
        /// Deletes a node from the grid
        /// </summary>
        /// <param name="node"></param>
        public void DeleteNode(VmNode node)
        {
            node.Disconnect();
            Nodes.Remove(node);
            Data.RemoveNode(node.Data);
        }

        /// <summary>
        /// Deletes the active noe
        /// </summary>
        private void DeleteActiveNode()
        {
            if(Active == null)
                return;

            DeleteNode(Active);
            Active = null;
        }

        /// <summary>
        /// Recenters the contents by the start of the node structure
        /// </summary>
        private void RecenterContents()
        {
            if(Nodes.Count == 0)
                return;

            // find the starter node
            VmNode start = Nodes.First(x => x.Data == Data.StartNode);
            Point offset = new(-start.Position.X, -start.Position.Y);

            foreach(var n in Nodes)
            {
                n.Move(offset);
                n.UpdateDataPosition();
            }
        }

        private void Sort()
        {
            Data.Sort();
            Nodes = new ObservableCollection<VmNode>(Nodes.OrderBy(x => Data.Nodes.IndexOf(x.Data)));
        }
    }
}

