using SCR.Tools.UndoRedo.Collections;
using static SCR.Tools.UndoRedo.GlobalChangeTrackerC;
using System.Collections.ObjectModel;
using System.Linq;

namespace SCR.Tools.DialogEditor.Data
{
    /// <summary>
    /// Single node of a dialog
    /// </summary>
    public class Node
    {
        #region Private Fields

        private int _locationX;

        private int _locationY;

        private bool _rightPortrait;

        private readonly TrackList<NodeOutput> _outputs;

        private readonly TrackList<NodeOutput> _inputs;

        #endregion

        /// <summary>
        /// Grid location on the X Axis
        /// </summary>
        public int LocationX
        {
            get => _locationX;
            set
            {
                BlankValueChange(
                    (v) => _locationX = v, _locationX, value);
            }
        }

        /// <summary>
        /// Grid location on the Y Axis
        /// </summary>
        public int LocationY
        {
            get => _locationY;
            set
            {
                BlankValueChange(
                    (v) => _locationY = v, _locationY, value);
            }
        }

        /// <summary>
        /// If true, the portrait will be placed/focused on the right. Else on the left
        /// </summary>
        public bool RightPortrait
        {
            get => _rightPortrait;
            set
            {
                BlankValueChange(
                    (v) => _rightPortrait = v, _rightPortrait, value);
            }
        }

        /// <summary>
        /// Input references
        /// </summary>
        public ReadOnlyCollection<NodeOutput> Inputs { get; }

        /// <summary>
        /// Output Sockets
        /// </summary>
        public ReadOnlyCollection<NodeOutput> Outputs { get; }


        /// <summary>
        /// Default constructor
        /// </summary>
        public Node()
        {
            _outputs = new()
            {
                new(this)
            };
            _inputs = new();

            Outputs = new(_outputs);
            Inputs = new(_inputs);
        }


        /// <summary>
        /// Deletes the node from the dialog
        /// </summary>
        public void Disconnect()
        {
            BeginChangeGroup();

            foreach (NodeOutput input in _inputs.ToArray())
            {
                input.Disconnect();
            }

            foreach (NodeOutput output in _outputs)
            {
                output.Disconnect();
            }

            EndChangeGroup();
        }

        /// <summary>
        /// Adds output to the Node
        /// </summary>
        /// <returns></returns>
        public NodeOutput CreateOutput()
        {
            NodeOutput result = new(this)
            {
                Expression = Outputs[0].Expression,
                Character = Outputs[0].Character
            };

            _outputs.Add(result);

            return result;
        }

        /// <summary>
        /// Removes a specific output
        /// </summary>
        /// <param name="nodeOutput"></param>
        /// <returns></returns>
        public bool RemoveOutput(NodeOutput nodeOutput)
        {
            if (_outputs.Count < 2
               || !_outputs.Contains(nodeOutput))
                return false;

            BeginChangeGroup();

            nodeOutput.Disconnect();
            _outputs.Remove(nodeOutput);

            EndChangeGroup();
            return true;
        }

        /// <summary>
        /// Registers a connected input
        /// </summary>
        /// <param name="nodeOutput"></param>
        internal void AddInput(NodeOutput nodeOutput)
            => _inputs.Add(nodeOutput);

        /// <summary>
        /// Deregisters a connected input
        /// </summary>
        /// <param name="nodeOutput"></param>
        internal void RemoveInput(NodeOutput nodeOutput)
            => _inputs.Remove(nodeOutput);

        public override string ToString()
            => $"Loc ({LocationX}, {LocationY}); Out: {_outputs.Count}, In: {_inputs.Count}";
    }
}
