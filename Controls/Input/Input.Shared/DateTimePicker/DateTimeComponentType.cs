namespace Telerik.UI.Xaml.Controls.Input
{
    /// <summary>
    /// Defines the possible modifiable components of a date time item in a <see cref="DateTimePickers.DateTimeList"/>.
    /// </summary>
    public enum DateTimeComponentType
    {
        /// <summary>
        /// The day part of the <see cref="System.DateTime"/> structure is considered.
        /// </summary>
        Day,

        /// <summary>
        /// The month part of the <see cref="System.DateTime"/> structure is considered.
        /// </summary>
        Month,

        /// <summary>
        /// The year part of the <see cref="System.DateTime"/> structure is considered.
        /// </summary>
        Year,

        /// <summary>
        /// The hour part of the <see cref="System.DateTime"/> structure is considered.
        /// </summary>
        Hour,

        /// <summary>
        /// The minute part of the <see cref="System.DateTime"/> structure is considered.
        /// </summary>
        Minute,

        /// <summary>
        /// The second part of the <see cref="System.DateTime"/> structure is considered.
        /// </summary>
        Second,

        /// <summary>
        /// The time span of a given <see cref="System.DateTime"/> value is considered.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "AMPM")]
        AMPM
    }
}