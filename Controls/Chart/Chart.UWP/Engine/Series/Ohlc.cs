using System.Diagnostics;

namespace Telerik.Charting
{
    // TODO: Override equals and = if needed.

    /// <summary>
    /// Represents a struct, which defines a set of four values - High, Low, Open, Close.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes", Justification = "These operators will be overrided as needed."), DebuggerDisplay("{Open} - {High} - {Low} - {Close}")]
    public struct Ohlc
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Ohlc" /> structure.
        /// </summary>
        /// <param name="high">The high value.</param>
        /// <param name="low">The low value.</param>
        /// <param name="open">The open value.</param>
        /// <param name="close">The close value.</param>
        public Ohlc(double high, double low, double open, double close)
            : this()
        {
            this.High = high;
            this.Low = low;
            this.Open = open;
            this.Close = close;
        }

        /// <summary>
        /// Gets or sets the high value.
        /// </summary>
        /// <value>The high value.</value>
        public double High { get; set; }

        /// <summary>
        /// Gets or sets the low value.
        /// </summary>
        /// <value>The low value.</value>
        public double Low { get; set; }

        /// <summary>
        /// Gets or sets the open value.
        /// </summary>
        /// <value>The open value.</value>
        public double Open { get; set; }

        /// <summary>
        /// Gets or sets the close value.
        /// </summary>
        /// <value>The close value.</value>
        public double Close { get; set; }
    }
}
