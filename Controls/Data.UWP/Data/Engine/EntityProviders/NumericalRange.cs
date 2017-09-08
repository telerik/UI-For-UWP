namespace Telerik.Data.Core
{
    /// <summary>
    /// Represents an <see cref="INumericalRange"/> implementation.
    /// </summary>
    public class NumericalRange : INumericalRange
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NumericalRange"/> class.
        /// </summary>
        public NumericalRange(double min, double max, double step)
        {
            this.Min = min;
            this.Max = max;
            this.Step = step;
        }

        /// <summary>
        /// Gets the highest possible value of the range element. 
        /// </summary>
        public double Max
        {
            get; private set;
        }

        /// <summary>
        /// Gets the lowest possible value of the range element. 
        /// </summary>
        public double Min
        {
            get; private set;
        }

        /// <summary>
        /// Gets a value to be added to or subtracted from the value.
        /// </summary>
        public double Step
        {
            get; private set;
        }
    }
}
