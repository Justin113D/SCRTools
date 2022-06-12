using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SCR.Tools.DialogEditor.Data
{
    /// <summary>
    /// Single type of option for a Node
    /// </summary>
    [Serializable]
    public class NodeOption
    {
        private static readonly Color defaultColor = Color.FromArgb(211, 211, 211);

        /// <summary>
        /// Name of the Emotion
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Color to represent the emotion
        /// </summary>
        public Color Color { get; set; }

        public NodeOption(string name, Color? color = null)
        {
            Name = name;
            Color = color ?? defaultColor;
        }

        public override string ToString() => Name;
    }

    /// <summary>
    /// Node output icon
    /// </summary>
    [Serializable]
    public class NodeIcon
    {
        /// <summary>
        /// Descriptor name for the icon
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Image path to the icon
        /// </summary>
        public string IconPath;

        public NodeIcon(string name, string iconPath)
        {
            Name = name;
            IconPath = iconPath;
        }

        public override string ToString() => Name;
    }

    /// <summary>
    /// Dialog options
    /// </summary>
    [Serializable]
    public class DialogOptions
    {
        /// <summary>
        /// All available character options
        /// </summary>
        public List<NodeOption> CharacterOptions { get; }

        /// <summary>
        /// All available emotion options
        /// </summary>
        public List<NodeOption> ExpressionOptions { get; }

        /// <summary>
        /// available node icons
        /// </summary>
        public List<NodeIcon> NodeIcons { get; }

        public DialogOptions()
        {
            CharacterOptions = new();
            ExpressionOptions = new();
            NodeIcons = new();
        }

        /// <summary>
        /// Write these dialog options to a file viable format
        /// </summary>
        public string Write(bool indented)
        {
            return JsonSerializer.Serialize(this, new JsonSerializerOptions()
            {
                WriteIndented = indented,
            });
        }

        /// <summary>
        /// Reads dialog options from json data
        /// </summary>
        /// <param name="data"></param>
        public static DialogOptions ReadFromFile(string data)
        {
            return JsonSerializer.Deserialize<DialogOptions>(data) ?? throw new InvalidDataException();
        }
    }
}
