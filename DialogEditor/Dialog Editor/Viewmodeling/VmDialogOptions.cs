﻿using SCR.Tools.DialogEditor.Data;
using SCR.Tools.Viewmodeling;
using SCR.Tools.UndoRedo;
using static SCR.Tools.UndoRedo.GlobalChangeTrackerC;
using System.Drawing;

namespace SCR.Tools.DialogEditor.Viewmodeling
{
    public class VmDialogOptions : BaseViewModel
    {
        public ChangeTracker DialogOptionsTracker { get; }

        public DialogOptions Data { get; private set; }

        public VmNodeOptions<Color> CharacterOptions { get; private set; }

        public VmNodeOptions<Color> ExpressionOptions { get; private set; }

        public VmNodeOptions<string> NodeIcons { get; private set; }

        public string? PortraitsPath
        {
            get => Data.PortraitsPath;
            set
            {
                if(value == PortraitsPath)
                {
                    return;
                }

                BeginChangeGroup();

                Data.PortraitsPath = value;
                TrackNotifyProperty(nameof(PortraitsPath));

                EndChangeGroup();
            }
        }

        public VmDialogOptions()
        {
            DialogOptionsTracker = new();
            Data = new();
            CharacterOptions = new(Data.CharacterOptions, Color.Red);
            ExpressionOptions = new(Data.ExpressionOptions, Color.Red);
            NodeIcons = new(Data.NodeIcons, "");
        }


        public void Read(string data, string? path)
        {
            Data = JsonFormatHandler.ReadDialogOptions(data, path);
            CharacterOptions = new(Data.CharacterOptions, Color.Red);
            ExpressionOptions = new(Data.ExpressionOptions, Color.Red);
            NodeIcons = new(Data.NodeIcons, "");
            ResetTracker();
        }

        public string Write(string? path)
        {
            return Data.WriteDialogOptions(Properties.Settings.Default.JsonIndenting, path);
        }

        public void Reset()
        {
            Data = new();
            CharacterOptions = new(Data.CharacterOptions, Color.Red);
            ExpressionOptions = new(Data.ExpressionOptions, Color.Red);
            NodeIcons = new(Data.NodeIcons, "");
            ResetTracker();
        }
    }
}
