using SCR.Tools.DialogEditor.Data;
using SCR.Tools.UndoRedo;
using static SCR.Tools.UndoRedo.GlobalChangeTrackerC;
using SCR.Tools.Viewmodeling;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SCR.Tools.UndoRedo.Collections;

namespace SCR.Tools.DialogEditor.Viewmodeling.Simulator
{
    public class VmSimulator : BaseViewModel
    {
        private VmSimulatorNode _activeNode;
        private VmSimulatorOutput? _leftPortrait;
        private VmSimulatorOutput? _rightPortrait;
        private bool _rightPortraitActive;


        public ChangeTracker SimulatorTracker { get; }

        public Dialog Data { get; }

        public DialogOptions Options { get; }

        public VmSimulatorNode EntryNode { get; }

        public ReadOnlyObservableCollection<VmSimulatorNode> Nodes { get; }

        public ReadOnlyDictionary<Node, VmSimulatorNode> NodeViewmodelLUT { get; }

        public TrackCollection<int> SharedDisabledIndices { get; }

        public VmSimulatorNode ActiveNode
        {
            get => _activeNode;
            set
            {
                if(_activeNode == value)
                {
                    return;
                }

                BeginChangeGroup();

                TrackValueChange(
                    (v) => _activeNode = v, _activeNode, value);
                TrackNotifyProperty(nameof(ActiveNode));

                PostChangeGroupAction(_activeNode.InitActive);

                TrackNotifyProperty(nameof(HasNextNode));
                RightPortraitActive = value.Data.RightPortrait;

                EndChangeGroup();
            }
        }

        public VmSimulatorOutput? LeftPortrait
        {
            get => _leftPortrait;
            set
            {
                if (_leftPortrait == value)
                {
                    return;
                }

                BeginChangeGroup();

                TrackValueChange(
                    (v) => _leftPortrait = v, _leftPortrait, value);
                TrackNotifyProperty(nameof(LeftPortrait));

                EndChangeGroup();
            }
        }
        
        public VmSimulatorOutput? RightPortrait
        {
            get => _rightPortrait;
            set
            {
                if (_rightPortrait == value)
                {
                    return;
                }

                BeginChangeGroup();

                TrackValueChange(
                    (v) => _rightPortrait = v, _rightPortrait, value);
                TrackNotifyProperty(nameof(RightPortrait));

                EndChangeGroup();
            }
        }

        public bool RightPortraitActive
        {
            get => _rightPortraitActive;
            set
            {
                if (_rightPortraitActive == value)
                {
                    return;
                }

                BeginChangeGroup();

                TrackValueChange(
                    (v) => _rightPortraitActive = v, _rightPortraitActive, value);
                TrackNotifyProperty(nameof(RightPortraitActive));

                EndChangeGroup();
            }
        }

        public bool HasNextNode
            => ActiveNode.ActiveOutput.Connected != null;

        public RelayCommand CmdReset
            => new(Reset);

        public RelayCommand CmdNext
            => new(Next);

        public VmSimulator(Dialog data, DialogOptions options)
        {
            SimulatorTracker = new();
            Data = data;
            Options = options;

            SharedDisabledIndices = new(new HashSet<int>());

            Node? entryNode = data.StartNode;
            if (entryNode == null)
            {
                throw new InvalidOperationException("Dialog has no valid entry point!");
            }

            ObservableCollection<VmSimulatorNode> nodes = new();
            Dictionary<Node, VmSimulatorNode> nodeViewmodelLUT = new();
            
            foreach(Node node in data.Nodes)
            {
                VmSimulatorNode vmNode = new(this, node);
                nodes.Add(vmNode);
                nodeViewmodelLUT.Add(node, vmNode);
            }

            Nodes = new(nodes);
            NodeViewmodelLUT = new(nodeViewmodelLUT);
            EntryNode = NodeViewmodelLUT[entryNode];
            _activeNode = EntryNode;

            foreach (VmSimulatorNode vmNode in nodes)
            {
                foreach(VmSimulatorOutput vmOutput in vmNode.Outputs)
                {
                    if(vmOutput.Data.Connected != null)
                    {
                        vmOutput.Connected = NodeViewmodelLUT[vmOutput.Data.Connected];
                    }
                }
            }

            VmSimulatorOutput activeOutput = EntryNode.Outputs[0];
            if(EntryNode.Data.RightPortrait)
            {
                _rightPortrait = activeOutput;
                _rightPortraitActive = true;
            }
            else
            {
                _leftPortrait = activeOutput;
            }
        }
    
        public void Next()
        {
            VmSimulatorNode? nextNode = ActiveNode.ActiveOutput.Connected;
            if(nextNode == null)
            {
                return;
            }

            BeginChangeGroup();

            VmSimulatorOutput output = ActiveNode.ActiveOutput;
            if(output.Data.DisableReuse)
            {
                output.Enabled = false;
            }

            ActiveNode = nextNode;

            EndChangeGroup();
        }

        private void Reset()
        {
            foreach(VmSimulatorNode vmNode in Nodes)
            {
                foreach (VmSimulatorOutput vmOutput in vmNode.Outputs)
                {
                    vmOutput.Enabled = true;
                }
            }

            _activeNode = EntryNode;
            _activeNode.InitActive();

            ResetTracker();
        }
    }
}
