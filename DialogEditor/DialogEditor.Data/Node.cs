using SCR.Tools.UndoRedo;
using SCR.Tools.UndoRedo.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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
                ChangeTracker.Global.BlankValueChange(
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
                ChangeTracker.Global.BlankValueChange(
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
                ChangeTracker.Global.BlankValueChange(
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
                new()
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
            ChangeTracker.Global.BeginGroup();
            foreach (NodeOutput no in _inputs)
                no.Disconnect();
            foreach (NodeOutput no in _outputs)
                no.Disconnect();
            ChangeTracker.Global.EndGroup();
        }

        /// <summary>
        /// Adds output to the Node
        /// </summary>
        /// <returns></returns>
        public NodeOutput CreateOutput()
        {
            NodeOutput result = new()
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

            ChangeTracker.Global.BeginGroup();

            nodeOutput.Disconnect();
            _outputs.Remove(nodeOutput);

            ChangeTracker.Global.EndGroup();
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
