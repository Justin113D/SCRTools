using SCR.Tools.Dialog.Data;
using SCR.Tools.Dialog.Data.Events;
using SCR.Tools.UndoRedo.Collections;
using SCR.Tools.WPF.Viewmodeling;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using static SCR.Tools.UndoRedo.GlobalChangeTrackerC;

namespace SCR.Tools.Dialog.Editor.Viewmodeling
{
    public class VmNodeOutput : BaseViewModel
    {
        private VmNode? _connected;

        private bool _initiatedConnection;

        private readonly TrackList<VmInstruction> _instructions;


        /// <summary>
        /// Parent Node Viewmodel
        /// </summary>
        public VmNode Parent { get; }

        /// <summary>
        /// Data model
        /// </summary>
        public NodeOutput Data { get; }

        public VmDialogOptions Options
            => Parent.Dialog.Main.DialogOptions;


        #region Property Wrappers

        #region Character

        public string CharacterText
            => string.IsNullOrWhiteSpace(Data.Character) ? "<Character>" : Data.Character;

        public Color CharacterColor
            => Character?.Value ?? Color.Gray;

        public VmNodeOption<Color>? Character
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Data.Character))
                    return null;

                return Options.CharacterOptions.GetOption(Data.Character);
            }
            set
            {
                BeginChangeGroup();
                Data.Character = value?.Name ?? "";

                TrackNotifyProperty(nameof(Character));
                TrackNotifyProperty(nameof(CharacterText));
                TrackNotifyProperty(nameof(CharacterColor));
                TrackNotifyProperty(nameof(Name));

                EndChangeGroup();
            }
        }

        #endregion

        #region Expression

        public string ExpressionText
            => string.IsNullOrWhiteSpace(Data.Expression) ? "<Expression>" : Data.Expression;

        public Color ExpressionColor
            => Expression?.Value ?? Color.Gray;

        public VmNodeOption<Color>? Expression
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Data.Expression))
                    return null;

                return Options.ExpressionOptions.GetOption(Data.Expression);
            }
            set
            {
                BeginChangeGroup();
                Data.Expression = value?.Name ?? "";

                TrackNotifyProperty(nameof(Expression));
                TrackNotifyProperty(nameof(ExpressionText));
                TrackNotifyProperty(nameof(ExpressionColor));
                TrackNotifyProperty(nameof(Name));

                EndChangeGroup();
            }
        }

        #endregion

        #region Node Icon

        public string IconText
            => string.IsNullOrWhiteSpace(Data.Icon) ? "<No icon>" : Data.Icon;

        public string? IconPath
            => Icon?.Value;

        public VmNodeOption<string>? Icon
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Data.Icon))
                    return null;

                return Options.NodeIcons.GetOption(Data.Icon);
            }
            set
            {
                BeginChangeGroup();
                Data.Icon = value?.Name ?? "";

                TrackNotifyProperty(nameof(Icon));
                TrackNotifyProperty(nameof(IconPath));
                TrackNotifyProperty(nameof(IconText));
                TrackNotifyProperty(nameof(Name));

                EndChangeGroup();
            }
        }

        #endregion

        public string Text
        {
            get => Data.Text;
            set
            {
                if (Text == value)
                {
                    return;
                }

                BeginChangeGroup();

                Data.Text = value;
                TrackNotifyProperty(nameof(Text));

                EndChangeGroup();
            }
        }

        public bool FallBack
        {
            get => Data.IsFallback;
            set
            {
                BeginChangeGroup();
                Data.IsFallback = value;
                TrackNotifyProperty(nameof(FallBack));
                EndChangeGroup();
            }
        }

        public bool DisableReuse
        {
            get => Data.DisableReuse;
            set
            {
                BeginChangeGroup();
                Data.DisableReuse = value;
                TrackNotifyProperty(nameof(DisableReuse));
                EndChangeGroup();
            }
        }

        public string Condition
        {
            get => Data.Condition;
            set
            {
                if (Condition == value)
                {
                    return;
                }

                BeginChangeGroup();
                Data.Condition = value;
                TrackNotifyProperty(nameof(Condition));
                EndChangeGroup();
            }
        }

        public ReadOnlyObservableCollection<VmInstruction> Instructions { get; }

        public VmNode? Connected
        {
            get => _connected;
            set => Data.Connect(value?.Data);
        }

        #endregion


        /// <summary>
        /// Name of the output
        /// </summary>
        public string Name
            => $"{Data.Expression ?? "[None]"} {Data.Character ?? "[None]"}";

        /// <summary>
        /// Whether the output is expanded in the list
        /// </summary>
        public bool IsExpanded { get; set; }



        #region Commands

        public RelayCommand CmdDisconnect
            => new(Disconnect);

        public RelayCommand CmdDelete
            => new(Delete);

        public RelayCommand CmdAddInstruction
            => new(AddInstruction);

        #endregion


        public VmNodeOutput(VmNode parent, NodeOutput data)
        {
            Parent = parent;
            Data = data;

            data.ConnectionChanged += ConnectionChanged;

            ObservableCollection<VmInstruction> internalInstructions = new();
            for(int i = 0; i < data.Instructions.Count; i++)
            {
                internalInstructions.Add(new(this));
            }

            _instructions = new(internalInstructions);
            Instructions = new(internalInstructions);
        }

        public void InitConnection()
        {
            if (_initiatedConnection)
            {
                throw new InvalidOperationException("Already initiated node output!");
            }

            _connected = Data.Connected == null
                ? null
                : Parent.Dialog.GetViewmodel(Data.Connected);

            OnPropertyChanged(nameof(Connected));

            _connected?.AddInput(this);

            _initiatedConnection = true;
        }

        private void ConnectionChanged(NodeOutput output, OutputConnectionChangedEventArgs args)
        {
            BeginChangeGroup();

            _connected?.RemoveInput(this);

            VmNode? vmNode = args.NewNode == null
                ? null
                : Parent.Dialog.GetViewmodel(args.NewNode);

            TrackValueChange(
                (v) => _connected = v, _connected, vmNode);

            Connected?.AddInput(this);

            TrackNotifyProperty(nameof(Connected));


            EndChangeGroup();
        }

        public void Disconnect()
        {
            Data.Disconnect();
        }

        public void Delete()
        {
            Parent.DeleteOutput(this);
        }


        public void AddInstruction()
        {
            BeginChangeGroup();
            Data.Instructions.Add("");
            _instructions.Add(new(this));
            EndChangeGroup();
        }

        public void RemoveInstruction(int index)
        {
            BeginChangeGroup();
            _instructions.RemoveAt(index);
            Data.Instructions.RemoveAt(index);
            EndChangeGroup();
        }

        public override string ToString()
            => Name;
    }
}
