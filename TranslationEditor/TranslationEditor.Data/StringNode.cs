using SCR.Tools.UndoRedo;

namespace SCR.Tools.TranslationEditor.Data
{
    /// <summary>
    /// A node holding a text value, the main focus of the language/text file
    /// </summary>
    public class StringNode : Node
    {
        private string _nodeValue;
        private string _defaultValue;
        private int _versionIndex;
        private int _changedVersionIndex;
        private bool _keepDefault;


        /// <summary>
        /// The default value for this node
        /// </summary>
        public string DefaultValue
        {
            get => _defaultValue;
            set
            {
                ChangeTracker.Global.BeginGroup();

                ChangeTracker.Global.TrackChange(
                    new ChangedValue<string>(
                        (v) => _defaultValue = v,
                        _defaultValue,
                        value.Trim()
                ));

                var header = Header;
                if(header != null)
                {
                    VersionIndex = header.Versions.Count - 1;
                }

                if(_changedVersionIndex == -1)
                {
                    SetTrackNodeValue(_defaultValue);
                }

                ChangeTracker.Global.EndGroup();
            }
        }

        /// <summary>
        /// Version index of this node in the format
        /// </summary>
        public int VersionIndex
        {
            get => _versionIndex;
            set
            {
                ChangeTracker.Global.TrackChange(
                    new ChangedValue<int>(
                        (v) => _versionIndex = v,
                        _versionIndex,
                        value
                ));
            }
        }

        /// <summary>
        /// Index of when the node was last changed by the translator <br/>
        /// -1 = untranslated
        /// </summary>
        public int ChangedVersionIndex
        {
            get => _changedVersionIndex;
            set
            {
                value = Math.Clamp(value, -1, VersionIndex);

                if (value == _changedVersionIndex)
                {
                    ChangeTracker.Global.BlankChange();
                    return;
                }

                ChangeTracker.Global.BeginGroup();

                ChangeTracker.Global.TrackChange(
                    new ChangedValue<int>(
                        (v) => _changedVersionIndex = v,
                        _changedVersionIndex,
                        value
                ));

                if (value == -1)
                {
                    State = NodeState.Untranslated;
                }
                else if (value == VersionIndex)
                {
                    State = NodeState.Translated;
                }
                else // value > -1 && value < VersionIndex
                {
                    State = NodeState.Outdated;
                }

                ChangeTracker.Global.EndGroup();
            }
        }


        /// <summary>
        /// The node value - holding the (un)translated text
        /// </summary>
        public string NodeValue
        {
            get => _nodeValue;
            set
            {
                if (_nodeValue == value)
                {
                    ChangeTracker.Global.BlankChange();
                    return;
                }

                ChangeTracker.Global.BeginGroup();

                SetTrackNodeValue(value);

                if (KeepDefault)
                {
                    SetTrackKeepDefault(false);
                }

                ChangedVersionIndex = value == DefaultValue ? -1 : VersionIndex;

                ChangeTracker.Global.EndGroup();
            }
        }

        public bool KeepDefault
        {
            get => _keepDefault;
            set
            {
                if (value == _keepDefault)
                {
                    ChangeTracker.Global.BlankChange();
                    return;
                }

                ChangeTracker.Global.BeginGroup();


                if (value)
                {
                    SetTrackNodeValue(DefaultValue);
                    ChangedVersionIndex = VersionIndex;
                }

                SetTrackKeepDefault(value);

                if (!value)
                {
                    ChangedVersionIndex = -1;
                }

                ChangeTracker.Global.EndGroup();
            }
        }


        public StringNode(string name, string value, int versionIndex = 0, string? description = null)
            : base(name, description, NodeState.Untranslated)
        {
            _nodeValue = _defaultValue = value.Trim();
            _versionIndex = versionIndex;
            _changedVersionIndex = -1;
        }

        private void SetTrackNodeValue(string value)
        {
            ChangeTracker.Global.TrackChange(
                new ChangedValue<string>(
                    (v) => _nodeValue = v,
                    _nodeValue,
                    value.Trim()
            ));
        }

        private void SetTrackKeepDefault(bool value)
        {
            ChangeTracker.Global.TrackChange(
                    new ChangedValue<bool>(
                        (v) => _keepDefault = v,
                        _keepDefault,
                        value
                ));
        }

        internal void ImportValue(string value, int changedVersionIndex, bool keepDefault)
        {
            SetTrackNodeValue(value);
            SetTrackKeepDefault(keepDefault);
            ChangedVersionIndex = changedVersionIndex;
        }

        /// <summary>
        /// Sets the node value to the default value
        /// </summary>
        public void ResetValue()
        {
            ChangeTracker.Global.BeginGroup();
            SetTrackNodeValue(DefaultValue);
            ChangedVersionIndex = -1;
            SetTrackKeepDefault(false);
            ChangeTracker.Global.EndGroup();
        }


        protected override string ValidateName(string name) 
            => Header?.GetFreeStringNodeName(name) ?? name;

        protected override void InternalOnNameChanged(string oldName)
        {
            Header?.StringNodeChangeKey(this, oldName);
        }

    }
}
