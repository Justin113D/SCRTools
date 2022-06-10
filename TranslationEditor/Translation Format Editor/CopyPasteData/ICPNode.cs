using SCR.Tools.TranslationEditor.Data;

namespace SCR.Tools.TranslationEditor.FormatEditor.CopyPasteData
{
    public interface ICPNode
    {
        public string Name { get; }

        public string? Description { get; }

        public void CreateNode(ParentNode parent);
    }
}
