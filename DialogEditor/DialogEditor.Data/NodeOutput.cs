using SCR.Tools.DialogEditor.Data.Events;
using SCR.Tools.UndoRedo;
using static SCR.Tools.UndoRedo.GlobalChangeTrackerC;

namespace SCR.Tools.DialogEditor.Data
{
    /// <summary>
    /// Single Output for a Node
    /// </summary>
    public class NodeOutput
    {
        #region private fields
        private string _expression;

        private string _character;

        private string _icon;

        private string _text;

        private bool _keepEnabled;

        private string _condition;

        private int _event;

        private Node? _output;
        #endregion

        public OutputConnectionChangedEventHandler? ConnectionChanged { get; set; }

        /// <summary>
        /// Expression of the character
        /// </summary>
        public string Expression
        {
            get => _expression;
            set
            {
                BlankValueChange(
                    (v) => _expression = v, _expression, value);
            }
        }

        /// <summary>
        /// Character that says the text
        /// </summary>
        public string Character
        {
            get => _character;
            set
            {
                BlankValueChange(
                    (v) => _character = v, _character, value);
            }
        }

        /// <summary>
        /// Selection icon for multiple choice nodes
        /// </summary>
        public string Icon
        {
            get => _icon;
            set
            {
                BlankValueChange(
                    (v) => _icon = v, _icon, value);
            }
        }

        /// <summary>
        /// Text for the output
        /// </summary>
        public string Text
        {
            get => _text;
            set
            {
                BlankValueChange(
                    (v) => _text = v, _text, value);
            }
        }

        /// <summary>
        /// Whether output should still be available after returning to the node
        /// </summary>
        public bool KeepEnabled
        {
            get => _keepEnabled;
            set
            {
                BlankValueChange(
                    (v) => _keepEnabled = v, _keepEnabled, value);
            }
        }

        /// <summary>
        /// Condition for the output to be visible
        /// </summary>
        public string Condition
        {
            get => _condition;
            private set
            {
                BlankValueChange(
                    (v) => _condition = v, _condition, value);
            }
        }

        /// <summary>
        /// Event id to trigger
        /// </summary>
        public int Event
        {
            get => _event;
            set
            {
                BlankValueChange(
                    (v) => _event = v, _event, value);
            }
        }

        /// <summary>
        /// The followup node
        /// </summary>
        public Node? Output
        {
            get => _output;
            private set
            {
                if(value == _output)
                {
                    BlankChange();
                }
                else
                {
                    TrackValueChange(
                        (v) => _output = v, _output, value);
                }
                
            }
        }


        public NodeOutput()
        {
            _expression = "";
            _character = "";
            _icon = "";
            _text = "";
            _condition = "";
        }


        /// <summary>
        /// Sets the condition
        /// </summary>
        /// <param name="condition">New Condition</param>
        /// <returns>Whether the condition is valid</returns>
        public bool SetCondition(string condition)
        {
            if (string.IsNullOrWhiteSpace(condition))
            {
                Condition = "";
                return true;
            }

            Condition = condition;
            return true;
        }

        /// <summary>
        /// Sets the node output
        /// </summary>
        /// <param name="node">New output</param>
        /// <returns></returns>
        public bool SetOutput(Node? node)
        {
            if (node?.Outputs.Contains(this) == true)
                return false;

            BeginChangeGroup();

            Node? oldConnection = Output;

            Output?.RemoveInput(this);
            Output = node;
            Output?.AddInput(this);

            ConnectionChanged?.Invoke(this, new(oldConnection, node));

            EndChangeGroup();

            return true;
        }

        /// <summary>
        /// Disconnects the Output from the network
        /// </summary>
        public void Disconnect()
        {
            SetOutput(null);
        }

        public override string ToString()
            => $"{Expression} {Character} - {Icon}";
    }
}