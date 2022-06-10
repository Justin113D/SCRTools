using SCR.Tools.TranslationEditor.Data;

namespace SCR.Tools.TranslationEditor.FormatEditor.CopyPasteData
{
    public struct CPStringNode : ICPNode
    {
        public string Name { get; }

        public string? Description { get; }

        public string DefaultValue { get; }

        public CPStringNode(string name, string? description, string defaultValue)
        {
            Name = name;
            Description = description;
            DefaultValue = defaultValue;
        }

        public static CPStringNode FromNode(StringNode node)
            => new(node.Name, node.Description, node.DefaultValue);

        public void CreateNode(ParentNode parent)
        {
            StringNode node = new(Name, DefaultValue, description: Description);
            parent.AddChildNode(node);
        }
    }
}
