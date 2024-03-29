﻿using SCR.Tools.TranslationEditor.Data.Events;
using SCR.Tools.UndoRedo;
using static SCR.Tools.UndoRedo.GlobalChangeTrackerC;
using System;

namespace SCR.Tools.TranslationEditor.Data
{
    /// <summary>
    /// Base value container of a translation format
    /// </summary>
    public abstract class Node
    {
        #region privaten fields

        private string _name;
        private string? _description;
        private NodeState _state;
        private ParentNode? _parent;

        #endregion

        #region events

        /// <summary>
        /// On changing <see cref="Name"/> <br/>
        /// Does not get invoked on undo/redo!
        /// </summary>
        public event NodeNameChangedEventHandler? NameChanged;

        /// <summary>
        /// On changing <see cref="Parent"/> <br/>
        /// Does not get invoked on undo/redo!
        /// </summary>
        public event NodeParentChangedEventHandler? ParentChanged;

        /// <summary>
        /// On <see cref="Header"/> has changed <br/>
        /// Does not get invoked on undo/redo!
        /// </summary>
        public event NodeHeaderChangedEventHandler? HeaderChanged;

        /// <summary>
        /// On changing <see cref="State"/> <br/>
        /// Does not get invoked on undo/redo!
        /// </summary>
        public event NodeStateChangedEventHandler? NodeStateChanged;

        #endregion

        #region properties

        /// <summary>
        /// Unique node identifier
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                value = ValidateName(value);

                if (value.Length == 0)
                    throw new ArgumentException("Name cant be Whitespace!", nameof(value));

                if (value == _name)
                {
                    BlankChange();
                    return;
                }

                string oldNodeName = _name;

                BeginChangeGroup();

                TrackValueChange(
                    (v) => _name = v, oldNodeName, value);

                InternalOnNameChanged(oldNodeName);
                NameChanged?.Invoke(this, new(oldNodeName, value));

                EndChangeGroup();
            }
        }

        /// <summary>
        /// Gets and sets the nodes description accordingly
        /// </summary>
        public string? Description
        {
            get => _description;
            set
            {
                string? newValue = value?.Trim();
                newValue = newValue?.Length > 0 ? newValue : null;

                if (newValue == _description)
                {
                    BlankChange();
                    return;
                }

                TrackValueChange(
                    (v) => _description = v, _description, newValue);
            }
        }

        /// <summary>
        /// Update state of the string
        /// </summary>
        public NodeState State
        {
            get => _state;
            protected set
            {
                if (value == _state)
                {
                    BlankChange();
                    return;
                }

                BeginChangeGroup();

                NodeState oldState = _state;
                NodeState newState = value;

                TrackValueChange(
                        (v) => _state = v, oldState, newState);

                Parent?.EvaluateState(this);

                NodeStateChanged?.Invoke(this, new(oldState, newState));

                EndChangeGroup();
            }
        }

        /// <summary>
        /// Parent Node containing this node
        /// </summary>
        public virtual ParentNode? Parent
            => _parent;

        /// <summary>
        /// Header that the node belongs to
        /// </summary>
        public virtual HeaderNode? Header
            => _parent?.Header;

        #endregion

        /// <summary>
        /// Create a node with a name and a descripton
        /// </summary>
        /// <param name="name">The name of the node</param>
        /// <param name="description">The description of the node</param>
        protected Node(string name, string? description, NodeState defaultState)
        {
            _name = name.Trim();

            description = description?.Trim();
            _description = description?.Length > 0 ? description : null;
            _state = defaultState;
        }


        /// <summary>
        /// Returns a validated name for the node
        /// </summary>
        /// <param name="name">The name to validate</param>
        /// <returns></returns>
        protected virtual string ValidateName(string name) 
            => name.Trim();

        /// <summary>
        /// Called before the name change event is invoked
        /// </summary>
        /// <param name="oldName"></param>
        protected virtual void InternalOnNameChanged(string oldName) { }


        public void SetParent(ParentNode? parent)
        {
            if (parent == _parent)
            {
                BlankChange();
                return;
            }

            if (parent != null)
            {
                parent.AddChildNode(this);
            }
            else
            {
                _parent?.RemoveChildNode(this);
            }
        }

        /// <summary>
        /// Internal parent setter
        /// </summary>
        /// <param name="newParent"></param>
        internal void InternalSetParent(ParentNode? newParent, bool updateVersionIndex, int oldParentIndex, int newParentIndex)
        {
            BeginChangeGroup();

            ParentNode? oldParent = _parent;

            TrackValueChange(
                (v) => _parent = v, oldParent, newParent);

            // if the two headers are different, invoke the 
            if (oldParent?.Header != newParent?.Header)
            {
                oldParent?.Header?.RemoveStringNodes(this);
                newParent?.Header?.AddStringNodes(this, updateVersionIndex);
                InvokeHeaderChanged(new(oldParent?.Header, newParent?.Header));
            }

            ParentChanged?.Invoke(this, new(oldParent, newParent));

            oldParent?.InvokeChildrenChanged(oldParentIndex, -1);
            newParent?.InvokeChildrenChanged(-1, newParentIndex);

            EndChangeGroup();
        }

        /// <summary>
        /// Used for relaying the parents header changed event back to this node
        /// </summary>
        /// <param name="node"></param>
        /// <param name="args"></param>
        internal virtual void InvokeHeaderChanged(NodeHeaderChangedEventArgs args)
            => HeaderChanged?.Invoke(this, args);


        public override string ToString()
            => Name;
    }
}
