namespace Telerik.Data.Core
{
    /// <summary>
    /// Object that represents Group with null value.
    /// </summary>
    internal class NullValue
    {
        private static readonly NullValue Singleton = new NullValue();

        private NullValue()
        {
        }

        /// <summary>
        /// Gets the singleton instance of NullValue class.
        /// </summary>
        public static NullValue Instance
        {
            get
            {
                return Singleton;
            }
        }

        /// <summary>
        /// Overrides the string representation.
        /// </summary>
        public override string ToString()
        {
            return "(blank)";
        }
    }
}