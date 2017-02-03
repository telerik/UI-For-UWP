using System;

namespace Telerik.Charting
{
    /// <summary>
    /// Represents a structure that defines the starting and sweeping angles of an ellipse Arc.
    /// </summary>
    public struct AngleRange
    {
        /// <summary>
        /// The default structure value with its starting angle set to 0 and its sweep angle set to 360.
        /// </summary>
        public static readonly AngleRange Default = new AngleRange(0, 360, ChartSweepDirection.Clockwise);

        internal double startAngle;
        internal double sweepAngle;
        internal ChartSweepDirection sweepDirection;

        /// <summary>
        /// Initializes a new instance of the <see cref="AngleRange"/> struct.
        /// </summary>
        /// <param name="startAngle">The start angle.</param>
        /// <param name="sweepAngle">The sweep angle.</param>
        /// <param name="sweepDirection">The sweep direction.</param>
        public AngleRange(double startAngle, double sweepAngle, ChartSweepDirection sweepDirection)
            : this()
        {
            this.StartAngle = startAngle;
            this.SweepAngle = sweepAngle;
            this.SweepDirection = sweepDirection;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AngleRange"/> struct.
        /// </summary>
        /// <param name="startAngle">The start angle.</param>
        /// <param name="sweepAngle">The sweep angle.</param>
        public AngleRange(double startAngle, double sweepAngle)
            : this(startAngle, sweepAngle, ChartSweepDirection.Clockwise)
        {
        }

        /// <summary>
        /// Gets or sets start angle from which the arc starts.
        /// </summary>
        public double StartAngle
        {
            get
            {
                return this.startAngle;
            }
            set
            {
                this.startAngle = value;
            }
        }

        /// <summary>
        /// Gets or sets the angle that defines the length of the Arc.
        /// </summary>
        public double SweepAngle
        {
            get
            {
                return this.sweepAngle;
            }
            set
            {
                if (value < -360 || value > 360)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
                this.sweepAngle = value;
            }
        }

        /// <summary>
        /// Gets or sets the sweep direction.
        /// </summary>
        /// <value>The sweep direction.</value>
        public ChartSweepDirection SweepDirection
        {
            get
            {
                return this.sweepDirection;
            }
            set
            {
                this.sweepDirection = value;
            }
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="range1">The first range.</param>
        /// <param name="range2">The second range.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(AngleRange range1, AngleRange range2)
        {
            return range1.startAngle == range2.startAngle &&
                   range1.sweepAngle == range2.sweepAngle &&
                   range1.sweepDirection == range2.sweepDirection;
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="range1">The first range.</param>
        /// <param name="range2">The second range.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(AngleRange range1, AngleRange range2)
        {
            return !(range1 == range2);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is AngleRange)
            {
                return (AngleRange)obj == this;
            }

            return false;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}