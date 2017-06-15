namespace Telerik.Charting
{
    /// <summary>
    /// Defines a type that provides information if Label should be created.
    /// </summary>
    public interface ILabelCreator
    {
        /// <summary>
        /// Decides whether the Label should be created or not.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="content">The content to be formatted.</param>
        /// <returns>If True is returned the Label should be created. If False is returned no Label should be created.</returns>
        bool ShouldCreateAxisLabel(object owner, object labelContent);
    }
}
