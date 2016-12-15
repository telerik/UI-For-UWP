using System;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Identifies the significant parts of a <see cref="System.DateTime"/> structure.
    /// </summary>
    public enum DateTimePart
    {
        /// <summary>
        /// Both the <see cref="Date"/> and <see cref="Time"/> parts are significant.
        /// </summary>
        Ticks,

        /// <summary>
        /// The <see cref="System.DateTime.Date"/> part is significant.
        /// </summary>
        Date,

        /// <summary>
        /// The <see cref="System.DateTime.TimeOfDay"/> part is significant.
        /// </summary>
        Time
    }
}
