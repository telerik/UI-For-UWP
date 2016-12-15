using System;

namespace Telerik.UI.Xaml.Controls.DataVisualization
{
    /// <summary>
    /// Event arguments for the ValueChanged event of GaugeIndicator.
    /// </summary>
    public class IndicatorValueChangedEventArgs : EventArgs
    {
        private double newVal;
        private double oldVal;

        /// <summary>
        /// Initializes a new instance of the IndicatorValueChangedEventArgs class.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        /// <param name="oldValue">The old value.</param>
        public IndicatorValueChangedEventArgs(double newValue, double oldValue)
        {
            this.newVal = newValue;
            this.oldVal = oldValue;
        }

        /// <summary>
        /// Gets the new indicator value.
        /// </summary>
        public double NewValue
        {
            get
            {
                return this.newVal;
            }
        }

        /// <summary>
        /// Gets the old indicator value.
        /// </summary>
        public double OldValue
        {
            get
            {
                return this.oldVal;
            }
        }
    }
}