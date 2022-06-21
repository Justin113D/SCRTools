using SCR.Tools.DialogEditor.Data;
using SCR.Tools.UndoRedo.Collections;
using SCR.Tools.Viewmodeling;
using System.Collections.ObjectModel;
using static SCR.Tools.UndoRedo.GlobalChangeTrackerC;
using System;
using System.Collections.Generic;

namespace SCR.Tools.DialogEditor.Viewmodeling
{
    public class VmNode : BaseViewModel
    {


        /// <summary>
        /// Dialog that the node belongs to
        /// </summary>
        public VmDialog Dialog { get; }

        /// <summary>
        /// Data object
        /// </summary>
        public Node Data { get; }


        private readonly TrackList<VmNodeOutput> _outputs;

        /// <summary>
        /// Viewmodel objects for the outputs
        /// </summary>
        public ReadOnlyObservableCollection<VmNodeOutput> Outputs { get; }

        private readonly TrackList<VmNodeOutput> _inputs;

        /// <summary>
        /// Outputs attached to this node
        /// </summary>
        public ReadOnlyObservableCollection<VmNodeOutput> Inputs { get; }


        public bool RightPortrait
        {
            get => Data.RightPortrait;
            set
            {
                if(value == RightPortrait)
                {
                    return;
                }

                BeginChangeGroup();

                Data.RightPortrait = value;
                TrackNotifyProperty(nameof(RightPortrait));

                EndChangeGroup();
            }
        }

        public int LocationX
        {
            get => Data.LocationX;
            set
            {
                if(LocationX == value)
                {
                    return;
                }

                BeginChangeGroup();

                Data.LocationX = value;
                TrackNotifyProperty(nameof(LocationX));

                EndChangeGroup();
            }
        }

        public int LocationY
        {
            get => Data.LocationY;
            set
            {
                if (LocationY == value)
                {
                    return;
                }

                BeginChangeGroup();

                Data.LocationY = value;
                TrackNotifyProperty(nameof(LocationY));

                EndChangeGroup();
            }
        }


        public string Name
            => _outputs[0].Name;

        public string InOutInfo
            => $"[ {_inputs.Count} ; {_outputs.Count} ]";

        public bool Active
        {
            get => Dialog.ActiveNode == this;
            set
            {
                if(value == Active)
                {
                    return;
                }

                if(value)
                {
                    Selected = true;
                    Dialog.ActiveNode = this;
                }
                else
                {
                    Dialog.ActiveNode = null;
                    Selected = false;
                }
            }
        }

        public bool Selected
        {
            get => Dialog.Selected.Contains(this);
            set
            {
                if(Selected == value)
                {
                    return;
                }

                if(value)
                {
                    Dialog.Selected.Add(this);
                }
                else
                {
                    Dialog.Selected.Remove(this);
                }
            }
        }

        

        public RelayCommand CmdAddOutput
            => new(AddOutput);


        public VmNode(VmDialog dialog, Node data)
        {
            Dialog = dialog;
            Data = data;

            ObservableCollection<VmNodeOutput> internalInputs = new();
            _inputs = new(internalInputs);
            Inputs = new(internalInputs);

            ObservableCollection<VmNodeOutput> internalOutputs = new();
            foreach (NodeOutput output in data.Outputs)
            {
                VmNodeOutput VmOutput = new(this, output);
                internalOutputs.Add(VmOutput);
            }
            _outputs = new(internalOutputs);
            Outputs = new(internalOutputs);
        }

        public void InitConnections()
        {
            foreach(VmNodeOutput output in Outputs)
            {
                output.InitConnection();
            }
        }

        public void NotifyActiveChanged()
            => OnPropertyChanged(nameof(Active));


        private void AddOutput()
        {
            BeginChangeGroup();

            NodeOutput output = Data.CreateOutput();
            VmNodeOutput vmOutput = new(this, output);
            _outputs.Add(vmOutput);
            TrackNotifyProperty(nameof(InOutInfo));

            EndChangeGroup();
        }

        public void DeleteOutput(VmNodeOutput vmOutput)
        {
            if (_outputs.Count < 2)
                return;

            BeginChangeGroup();

            Data.RemoveOutput(vmOutput.Data);
            _outputs.Remove(vmOutput);

            TrackNotifyProperty(nameof(InOutInfo));

            EndChangeGroup();
        }


        public void AddInput(VmNodeOutput vmInput)
        {
            if(vmInput.Connected != this)
            {
                throw new ArgumentException("Input invalid!", nameof(vmInput));
            }

            BeginChangeGroup();

            _inputs.Add(vmInput);
            TrackNotifyProperty(nameof(InOutInfo));

            EndChangeGroup();
        }

        public void RemoveInput(VmNodeOutput vmInput)
        {
            if (vmInput.Connected != this)
            {
                throw new ArgumentException("Input invalid!", nameof(vmInput));
            }

            BeginChangeGroup();

            _inputs.Remove(vmInput);
            TrackNotifyProperty(nameof(InOutInfo));

            EndChangeGroup();
        }

        public void Disconnect()
            => Data.Disconnect();

        public void Select(bool multi, bool allowActive)
        {
            if (!multi)
            {
                Dialog.DeselectAll();
                Selected = true;
            }
            else
            {
                Selected = !Selected;
            }

            if(allowActive)
            {
                Active = true;
            }
        }

        public override string ToString()
            => $"{Name} {InOutInfo}";
    }
}
