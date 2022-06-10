using SCR.Tools.TranslationEditor.Data;

namespace SCR.Tools.TranslationEditor.FormatEditor.CopyPasteData
{
    public struct CPParentNode : ICPNode
    {
        public string Name { get; }

        public string? Description { get; }

        public ICPNode[] Children { get; }

        public CPParentNode(string name, string? description, ICPNode[] children)
        {
            Name = name;
            Description = description;
            Children = children;
        }

        public static CPParentNode FromNode(ParentNode node)
        {
            ICPNode[] children = new ICPNode[node.ChildNodes.Count];

            for(int i = 0; i < children.Length; i++)
            {
                Node child = node.ChildNodes[i];
                if(child is StringNode sn)
                {
                    children[i] = CPStringNode.FromNode(sn);
                }
                else if(child is ParentNode pn)
                {
                    children[i] = FromNode(pn);
                }
            }

            return new(node.Name, node.Description, children);
        }

        public void CreateNode(ParentNode parent)
        {
            ParentNode node = new(Name, Description);
            parent.AddChildNode(node);

            foreach(ICPNode cpNode in Children)
            {
                cpNode.CreateNode(node);
            }
        }
    }
}
