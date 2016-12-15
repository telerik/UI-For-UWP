namespace Telerik.Data.Core.Fields
{
    /// <summary>
    /// An <see cref="IFieldDescriptionProvider"/> state.
    /// </summary>
    internal enum FieldDescriptionProviderState
    {
        /// <summary>
        /// The provider's initialization is pending.
        /// </summary>
        Uninitialized,

        /// <summary>
        /// The provider is initializing.
        /// </summary>
        Initializing,

        /// <summary>
        /// The provider has completed initialization.
        /// </summary>
        Initialized
    }
}