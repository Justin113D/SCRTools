using SCR.Tools.Dialog.Data;
using SCR.Tools.Dialog.Data.Condition;
using SCR.Tools.Dialog.Simulator.Viewmodeling.Condition;
using SCR.Tools.DynamicDataExpression;
using SCR.Tools.Viewmodeling;
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

                EndChangeGroup();
            }
        }

        public bool ConditionValid
            => DDXCondition == null || DDXCondition.Evaluate(Parent.Simulator.ConditionData.Data);

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
                if (string.IsNullOrWhiteSpace(Data.Icon)
                    || !Parent.Simulator.Options.NodeIcons.TryGetValue(Data.Icon, out string? iconPath))
                {
                    return null;
                }

                return iconPath;
            }
        }

        public string Text
            => Data.Text;

        public DataExpression<ConditionData>? DDXCondition { get; }

        public VmNodeOutput(VmNode parent, NodeOutput data)
        {
            Parent = parent;
            Data = data;
            if (!string.IsNullOrWhiteSpace(data.Condition))
            {
                try
                {
                    DDXCondition = DataExpression<ConditionData>.ParseExpression(data.Condition, ConditionDataAccessor.DA);
                }
                catch (DynamicDataExpressionException e)
                {
                    throw new SimulatorException(e.Message, data.Parent, data);
                }
            }
        }

    }
}
