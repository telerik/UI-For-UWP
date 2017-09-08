namespace Telerik.UI.Xaml.Controls.Data.DataForm
{
    /// <summary>
    /// Represents an ITypeEditor interface.
    /// </summary>
    public interface ITypeEditor
    {
        /// <summary>
        /// Method used for generating bindings for the <see cref="ITypeEditor"/> properties.
        /// </summary>
        void BindEditor();
    }
}
