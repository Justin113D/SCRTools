namespace SCR.Tools.DynamicDataExpression
{
    /// <summary>
    /// Key return type
    /// </summary>
    public enum KeyType
    {
        /// <summary>
        /// None, Key doesnt support this type
        /// </summary>
        None,

        /// <summary>
        /// Returns a boolean
        /// </summary>
        Boolean,

        /// <summary>
        /// Return a number
        /// </summary>
        Number,

        /// <summary>
        /// Returns a number array
        /// </summary>
        NumberList
    }
}
