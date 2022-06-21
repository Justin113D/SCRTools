using SCR.Tools.DialogEditor.Data;
using SCR.Tools.DialogEditor.Data.Events;
using SCR.Tools.Viewmodeling;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using static SCR.Tools.UndoRedo.GlobalChangeTrackerC;

namespace SCR.Tools.DialogEditor.Viewmodeling
{
    public class VmNodeOutput : BaseViewModel
    {
        private VmNode? _connected;

        private bool _initiatedConnection;


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
                BeginChangeGroup();

                Data.Text = value;
                TrackNotifyProperty(nameof(Text));

                EndChangeGroup();
            }
        }

        public bool KeepEnabled
        {
            get => Data.KeepEnabled;
            set
            {
                BeginChangeGroup();
                Data.KeepEnabled = value;
                TrackNotifyProperty(nameof(KeepEnabled));
                EndChangeGroup();
            }
        }

        public VmNode? Connected
        {
            get => _connected;
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

        #endregion


        public VmNodeOutput(VmNode parent, NodeOutput data)
        {
            Parent = parent;
            Data = data;

            data.ConnectionChanged += ConnectionChanged;
        }

        public void InitConnection()
        {
            if(_initiatedConnection)
            {
                throw new InvalidOperationException("Already initiated node output!");
            }

            _connected = Data.Output == null
                ? null
                : Parent.Dialog.GetViewmodel(Data.Output);

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

        public override string ToString()
            => Name;
    }
}
