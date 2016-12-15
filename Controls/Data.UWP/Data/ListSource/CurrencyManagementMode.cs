namespace Telerik.Core.Data
{
    /// <summary>
    /// Specifies the mode used by a <see cref="ICurrencyManager"/> instance to update its <see cref="ICurrencyManager.CurrentItem"/> property.
    /// </summary>
    public enum CurrencyManagementMode
    {
        /// <summary>
        /// Current item is not managed at all. Useful for optimization purposes.
        /// </summary>
        None,

        /// <summary>
        /// The default mode. That is local current item will be managed only.
        /// </summary>
        Local,

        /// <summary>
        /// Local and external currencies will be synchronized together.
        /// </summary>
        LocalAndExternal,
    }
}