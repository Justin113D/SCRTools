
namespace SCRLanguageEditor.Data
{
    /// <summary>
    /// The Header/Language node of the file, which holds all other nodes.
    /// The name is the language
    /// </summary>
    public class HeaderNode : ParentNode
    {
        /// <summary>
        /// Author of the document
        /// </summary>
        public string Author { get; set; }
        /// <summary>
        /// Version of the game that the file is for
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Creates a Header node from language and version
        /// </summary>
        /// <param name="name">The language of the file</param>
        /// <param name="version">The game version that the file is for</param>
        public HeaderNode(string name, string version) : base(name)
        {
            Version = version;
            Author = "";
        }

        /// <summary>
        /// Creates a Header node from language, version and author
        /// </summary>
        /// <param name="name">The language of the file</param>
        /// <param name="version">The game version that the file is for</param>
        /// <param name="author">The author of the document</param>
        public HeaderNode(string name, string version, string author) : base(name)
        {
            Version = version;
            Author = author;
        }
    }
}
