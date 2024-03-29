﻿using SCR.Tools.DialogEditor.Data;
using SCR.Tools.Viewmodeling;
using static SCR.Tools.UndoRedo.GlobalChangeTrackerC;
using System.IO;

namespace SCR.Tools.DialogEditor.Viewmodeling.Simulator
{
    public class VmSimulatorOutput : BaseViewModel
    {
        private bool _enabled = true;

        public VmSimulatorNode Parent { get; }

        public NodeOutput Data { get; }

        public VmSimulatorNode? Connected { get; set; }

        public bool Enabled
        {
            get => Data.SharedDisabledIndex >= 0 
                ? !Parent.Simulator.SharedDisabledIndices.Contains(Data.SharedDisabledIndex) 
                : _enabled;
            set
            {
                if (value == Enabled)
                {
                    return;
                }

                BeginChangeGroup();

                if (Data.SharedDisabledIndex >= 0)
                {
                    var collection = Parent.Simulator.SharedDisabledIndices;

                    if (collection.Contains(Data.SharedDisabledIndex))
                    {
                        collection.Remove(Data.SharedDisabledIndex);
                    }
                    else
                    {
                        collection.Add(Data.SharedDisabledIndex);
                    }
                }
                else
                {
                    TrackValueChange(
                        (v) => _enabled = v, _enabled, value);
                }


                TrackNotifyProperty(nameof(Enabled));

                EndChangeGroup();
            }
        }

        /// <summary>
        /// Name of the output
        /// </summary>
        public string Name
            => $"{Data.Expression ?? "[None]"} {Data.Character ?? "[None]"}";

        public string? PortraitPath
        {
            get
            {
                if(Data.Expression == null 
                    || Data.Character == null
                    || Parent.Simulator.Options.PortraitsPath == null)
                {
                    return null;
                }

                string portraitPath = Parent.Simulator.Options.PortraitsPath;
                portraitPath += $"\\{Data.Character}_{Data.Expression}.png";
                return File.Exists(portraitPath) ? portraitPath : null;
            }
        }

        public string? IconPath
        {
            get
            {
                if(string.IsNullOrWhiteSpace(Data.Icon) 
                    || !Parent.Simulator.Options.NodeIcons.TryGetValue(Data.Icon, out string? iconPath))
                {
                    return null;
                }

                return iconPath;
            }
        }

        public string Text
            => Data.Text;

        public VmSimulatorOutput(VmSimulatorNode parent, NodeOutput data)
        {
            Parent = parent;
            Data = data;
        }
    
        public void Next()
        {

        }
    }
}
