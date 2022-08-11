namespace SCR.Tools.DynamicDataExpression
{
    /// <summary>
    /// Contains key meta data
    /// </summary>
    public struct DataKey
    {
        /// <summary>
        /// Name of the Key
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Data type of the key when using an ID
        /// </summary>
        public KeyType IDType { get; }

        /// <summary>
        /// Data type of the key when not using an ID
        /// </summary>
        public KeyType NoIDType { get; }

        public DataKey(string name, KeyType idType, KeyType noIDType)
        {
            Name = name;
            IDType = idType;
            NoIDType = noIDType;
        }
    }
}
