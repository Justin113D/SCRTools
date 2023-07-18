using SCR.Tools.Dialog.Data;
using SCR.Tools.Dialog.Simulator.Viewmodeling.Condition;
using SCR.Tools.DynamicDataExpression;
using SCR.Tools.WPF.Viewmodeling;
using System;
using System.Collections.Generic;
using System.IO;
using static SCR.Tools.UndoRedo.GlobalChangeTrackerC;

namespace SCR.Tools.Dialog.Simulator.Viewmodeling
{
    internal class VmNodeOutput : BaseViewModel
    {
        private bool _enabled = true;

        public VmNode Parent { get; }

        public NodeOutput Data { get; }

        public VmNode? Connected { get; set; }

        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (value == Enabled)
                {
                    return;
                }

                BeginChangeGroup();

                TrackValueChange(
                    (v) => _enabled = v, _enabled, value);
                
                EndChangeGroup();
            }
        }

        public bool ConditionValid
        {
            get
            {
                if(DDXCondition == null)
                {
                    return true;
                }

                try
                {
                    return DDXCondition.EvaluateBoolean(Parent.Simulator.Main.ConditionData);
                }
                catch(Exception e)
                {
                    throw new SimulatorException(e.Message, Data.Parent, Data);
                }
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
                if (Data.Expression == null
                    || Data.Character == null
                    || Parent.Simulator.Main.Options.PortraitsPath == null)
                {
                    return null;
                }

                string? portraitPath = Parent.Simulator.Main.Options.PortraitsPath;

                if (portraitPath == null)
                    return null;

                portraitPath += $"\\{Data.Character}_{Data.Expression}.png";
                return File.Exists(portraitPath) ? portraitPath : null;
            }
        }

        public string? IconPath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Data.Icon)
                    || !Parent.Simulator.Main.Options.NodeIcons.TryGetValue(Data.Icon, out string? iconPath))
                {
                    return null;
                }

                return iconPath;
            }
        }

        public string Text
            => Data.Text;

        public DataExpression? DDXCondition { get; }

        public DataInstruction[] DDXInstructions { get; }

        public VmNodeOutput(VmNode parent, NodeOutput data)
        {
            Parent = parent;
            Data = data;
            if (!string.IsNullOrWhiteSpace(data.Condition))
            {
                try
                {
                    DDXCondition = DataExpression.ParseExpression(data.Condition, VmConditionData.AccessKeys, KeyType.Boolean);
                }
                catch (DynamicDataExpressionException e)
                {
                    throw new SimulatorException(e.Message, data.Parent, data);
                }
            }

            List<DataInstruction> instructions = new();
            for(int i = 0; i < data.Instructions.Count; i++)
            {
                string instruction = data.Instructions[i];
                try
                {
                    DataInstruction ddxInstruction = DataInstruction.ParseInstruction(instruction, VmConditionData.SetterKeys, VmConditionData.AccessKeys);
                    instructions.Add(ddxInstruction);
                }
                catch (DynamicDataExpressionException e)
                {
                    throw new SimulatorException($"Instruction no. {i}:\n" + e.Message, data.Parent, data);
                }
            }
            DDXInstructions = instructions.ToArray();
        }

    }
}
