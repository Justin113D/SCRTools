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

        private bool _disableReuse;

        private int _sharedDisabledIndex = -1;

        private bool _fallback;

        private string _condition;

        private Node? _connected;
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
        /// Whether output should be disabled upon using
        /// </summary>
        public bool DisableReuse
        {
            get => _disableReuse;
            set
            {
                BlankValueChange(
                    (v) => _disableReuse = v, _disableReuse, value);
            }
        }

        /// <summary>
        /// Used to share the disabled state among outputs
        /// </summary>
        public int SharedDisabledIndex
        {
            get => _sharedDisabledIndex;
            set
            {
                BlankValueChange(
                    (v) => _sharedDisabledIndex = v, _sharedDisabledIndex, value);
            }
        }

        /// <summary>
        /// Gets enabled when all other nodes are disabled
        /// </summary>
        public bool Fallback
        {
            get => _fallback;
            set
            {
                BlankValueChange(
                    (v) => _fallback = v, _fallback, value);
            }
        }

        /// <summary>
        /// Condition for the output to be visible
        /// </summary>
        public string Condition
        {
            get => _condition;
            set
            {
                BlankValueChange(
                    (v) => _condition = v, _condition, value);
            }
        }

        /// <summary>
        /// The followup node
        /// </summary>
        public Node? Connected
        {
            get => _connected;
            private set
            {
                if(value == _connected)
                {
                    BlankChange();
                }
                else
                {
                    TrackValueChange(
                        (v) => _connected = v, _connected, value);
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
        /// Sets the node output
        /// </summary>
        /// <param name="node">New output</param>
        /// <returns></returns>
        public bool Connect(Node? node)
        {
            if (node?.Outputs.Contains(this) == true)
                return false;

            BeginChangeGroup();

            Node? oldConnection = Connected;

            Connected?.RemoveInput(this);
            Connected = node;
            Connected?.AddInput(this);

            ConnectionChanged?.Invoke(this, new(oldConnection, node));

            EndChangeGroup();

            return true;
        }

        /// <summary>
        /// Disconnects the Output from the network
        /// </summary>
        public void Disconnect()
        {
            Connect(null);
        }

        public override string ToString()
            => $"{Expression} {Character} - {Icon}";
    }
}