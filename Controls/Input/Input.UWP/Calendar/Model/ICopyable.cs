namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    /// <summary>
    /// A generic interface for copying objects.
    /// </summary>
    public interface ICopyable<T>
    {
        /// <summary>
        /// Deep copies this instance.
        /// </summary>
        /// <returns>
        /// A <b>deep</b> copy of the current object.
        /// </returns>
        T Copy();
    }
}
