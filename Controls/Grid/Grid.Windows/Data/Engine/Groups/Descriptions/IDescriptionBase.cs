using System.Diagnostics.CodeAnalysis;
namespace Telerik.Data.Core
{
    /// <summary>
    /// Base interface for describing FilterDescription, GroupDescription and AggregateDescription.
    /// </summary>
    internal interface IDescriptionBase
    {
        /// <summary>
        /// Gets the display-friendly name.
        /// </summary>
        string DisplayName { get; }
    
        /// <summary>
        /// Returns the member name that is used for grouping.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Design choice.")]
        string GetMemberName();

        /// <summary>
        /// Creates a clone of this instance.
        /// </summary>
        IDescriptionBase Clone();
    }
}