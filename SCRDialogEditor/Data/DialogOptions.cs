using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;

namespace SCRDialogEditor.Data
{
    /// <summary>
    /// Single type of option for a Node
    /// </summary>
    [Serializable]
    public class NodeOption
    {

        /// <summary>
        /// Name of the Emotion
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Color to represent the emotion
        /// </summary>
        public Color Color { get; set; }

        public NodeOption()
        {
            Color = Colors.LightGray;
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
        /// Saves dialog options to a json file
        /// </summary>
        /// <param name="Path"></param>
        public void SaveToFile(string Path)
        {
            string output = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(Path, output);
        }

        /// <summary>
        /// Reads dialog options from a json file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static DialogOptions ReadFromFile(string path)
        {
            return JsonConvert.DeserializeObject<DialogOptions>(File.ReadAllText(path));
        }
    }
}
