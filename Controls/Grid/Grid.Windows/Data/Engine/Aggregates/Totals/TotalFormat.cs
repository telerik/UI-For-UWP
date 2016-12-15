using System;

namespace Telerik.Data.Core.Totals
{
    /// <summary>
    /// A base class for all total formats. For internal use. Please refer to one of the <see cref="SingleTotalFormat"/> or <see cref="SiblingTotalsFormat"/> instead.
    /// </summary>
    internal abstract class TotalFormat : SettingsNode
    {
        internal TotalFormat()
        {
        }

        /// <summary>
        /// Gets a string format suitable to form the produced <see cref="AggregateValue"/>s by this <see cref="TotalFormat"/>.
        /// </summary>
        /// <param name="dataType">The type of the data items.</param>
        /// <param name="stringFormat">A string format selected by other means. You may keep or discard it.</param>
        /// <returns>A string.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Justification = "Design choice.")]
        public virtual string GetStringFormat(Type dataType, string stringFormat)
        {
            return stringFormat;
        }
    }
}
