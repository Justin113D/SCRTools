using PropertyChanged;
using SCRCommon.Viewmodels;
using SCRLanguageEditor.Data;
using System.Windows;
using System.Windows.Input;

namespace SCRLanguageEditor.Viewmodel
{
    /// <summary>
    /// Base viewmodel for a node object
    /// </summary>
    public abstract class VM_Node : BaseViewModel
    {
        #region commands

        /// <summary>
        /// Attached to the red remove button; Removes the node from the hierarchy <br/>
        /// Calls <see cref="Remove"/>
        /// </summary>
        public RelayCommand Cmd_Remove { get; }

        /// <summary>
        /// Attached to the grip button; Initiates a drag-drop <br/>
        /// Calls <see cref="MouseDown(MouseButtonEventArgs)"/>
        /// </summary>
        public RelayCommand<MouseButtonEventArgs> Cmd_MouseDown { get; }

        /// <summary>
        /// Gets called when dragging over the node. <br/>
        /// Calls <see cref="DragOver(DragEventArgs)"/>
        /// </summary>
        public RelayCommand<DragEventArgs> Cmd_DragOver { get; }

        /// <summary>
        /// Gets called when when dragging away from the node. <br/>
        /// Calls <see cref="DragLeave"/>
        /// </summary>
        public RelayCommand Cmd_DragLeave { get; }

        /// <summary>
        /// Gets called when dropping another node on this node <br/>
        /// Calls <see cref="Drop(DragEventArgs)"/>
        /// </summary>
        public RelayCommand<DragEventArgs> Cmd_Drop { get; }

        #endregion

        /// <summary>
        /// The data node
        /// </summary>
        public Node Node { get; }

        /// <summary>
        /// Header node access
        /// </summary>
        protected VM_HeaderNode VMHeader { get; }

        /// <summary>
        /// Viewmodel parent of this node
        /// </summary>
        public VM_ParentNode Parent { get; private set; }


        /// <summary>
        /// Whether the node is expanded
        /// </summary>
        [SuppressPropertyChangedWarnings]
        public abstract bool IsExpanded { get; set; }

        /// <summary>
        /// Whether a the cursor is above the node while dragging another node
        /// </summary>
        public bool IsDragOver { get; private set; }

        /// <summary>
        /// Whether the dragged-over node can be dropped
        /// </summary>
        public bool IsDragValid { get; private set; }

        /// <summary>
        /// The state of the translation
        /// </summary>
        public int NodeState => Node.NodeState;

        /// <summary>
        /// The name of the node
        /// </summary>
        public virtual string Name
        {
            get => Node.Name;
            set
            {
                if(string.IsNullOrWhiteSpace(value))
                    return;

                // support undo/redo
                VMHeader.Tracker.TrackChange(new ChangedValue<string>((v) =>
                {
                    Node.Name = v;
                    OnPropertyChanged(nameof(Name));
                }, Node.Name, value));

                Node.Name = value;

            }
        }

        /// <summary>
        /// The description of the node
        /// </summary>
        public virtual string Description
        {
            get => Node.Description;
            set
            {
                if(string.IsNullOrWhiteSpace(value))
                    value = "";

                // support undo/redo
                VMHeader.Tracker.TrackChange(new ChangedValue<string>((v) =>
                {
                    Node.Description = v;
                    OnPropertyChanged(nameof(Description));
                }, Node.Description, value));

                Node.Description = value;
            }
        }

        /// <summary>
        /// The type of the node
        /// </summary>
        public Node.NodeType Type => Node.Type;

        /// <summary>
        /// Base constructor
        /// </summary>
        /// <param name="node">The assigned node</param>
        protected VM_Node(Node node, VM_HeaderNode vm, VM_ParentNode parent)
        {
            Node = node;
            VMHeader = vm;
            Parent = parent;
            Cmd_Remove = new RelayCommand(() => Remove());
            Cmd_MouseDown = new RelayCommand<MouseButtonEventArgs>((x) => MouseDown(x));
            Cmd_DragOver = new RelayCommand<DragEventArgs>((x) => DragOver(x));
            Cmd_DragLeave = new RelayCommand(() => DragLeave());
            Cmd_Drop = new RelayCommand<DragEventArgs>((x) => Drop(x));
        }

        /// <summary>
        /// Handles grabbing the node
        /// </summary>
        /// <param name="args"></param>
        private void MouseDown(MouseButtonEventArgs args)
        {
            if(args.LeftButton == MouseButtonState.Pressed)
            {
                DataObject data = new DataObject("object", this);
                DragDrop.DoDragDrop((DependencyObject)args.Source, data, DragDropEffects.Move);
                args.Handled = true;
            }
        }

        /// <summary>
        /// Handles dragging nodes over this node
        /// </summary>
        /// <param name="args"></param>
        private void DragOver(DragEventArgs args)
        {
            VM_Node dragging = (VM_Node)args.Data.GetData("object");

            if(dragging == this)
                return;

            // check if dragged object is in the parent hierarchy
            VM_ParentNode current = Parent;
            while(current != null && current != dragging)
                current = current.Parent;

            IsDragOver = dragging != this;
            IsDragValid = current == null;
            args.Effects = IsDragValid ? DragDropEffects.Move : DragDropEffects.None;
            args.Handled = true;
        }

        /// <summary>
        /// Sets dragover to false
        /// </summary>
        private void DragLeave() => IsDragOver = false;

        /// <summary>
        /// Handles node dropping
        /// </summary>
        /// <param name="args"></param>
        private void Drop(DragEventArgs args)
        {
            IsDragOver = false;
            if(!IsDragValid)
                return;
            VM_Node dropped = (VM_Node)args.Data.GetData("object");
            InsertNode(dropped);
            args.Handled = true;
        }

        /// <summary>
        /// Moves a node from its original parent to "below this node"
        /// </summary>
        /// <param name="insertNode"></param>
        protected virtual void InsertNode(VM_Node insertNode)
        {
            VMHeader.Tracker.BeginGroup();

            if(insertNode.Parent == null)
                VMHeader.RemoveChild(insertNode);
            else
                insertNode.Parent.RemoveChild(insertNode);

            // insert it below this node
            int index = (Parent == null ? VMHeader.Children : Parent.Children).IndexOf(this);

            //adjust data nodetree
            if(Parent == null)
                VMHeader.InsertChild(insertNode, index + 1);
            else
                Parent.InsertChild(insertNode, index + 1);

            VMHeader.Tracker.EndGroup();
        }

        protected virtual void ExpandParents()
        {
            Parent?.ExpandParents();
        }

        /// <summary>
        /// Abstract method to update necessary properties manually
        /// </summary>
        public abstract void UpdateProperties();

        /// <summary>
        /// Removes the node and its children from the hierarchy
        /// </summary>
        public virtual void Remove()
        {
            if(Parent != null)
                Parent.RemoveChild(this);
            else
                VMHeader.RemoveChild(this);
        }

        public override string ToString()
        {
            return $"{Type} - {Name}";
        }
    }
}
