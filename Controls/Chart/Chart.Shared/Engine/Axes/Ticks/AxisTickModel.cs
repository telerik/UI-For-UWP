using Telerik.Core;

namespace Telerik.Charting
{
    /// <summary>
    /// Defines the possible types of an axis tick.
    /// </summary>
    public enum TickType
    {
        /// <summary>
        /// Identifies major ticks.
        /// </summary>
        Major,

        /// <summary>
        /// Identifies minor ticks.
        /// </summary>
        Minor,
    }

    internal enum TickPosition
    {
        First,
        Inner,
        Last,
    }

    internal abstract class AxisTickModel : Node
    {
        internal decimal value;
        internal decimal normalizedValue;
        internal int virtualIndex; // the index of the tick on the axis that it would occupy if no zoom is applied
        internal AxisTickModel next;
        internal AxisTickModel previous;
        internal AxisLabelModel associatedLabel;
        internal TickPosition position;

        public AxisTickModel()
        {
        }

        internal abstract TickType Type
        {
            get;
        }

        internal AxisTickModel NextMajorTick
        {
            get
            {
                AxisTickModel tick = this.next;
                while (tick != null)
                {
                    if (tick.Type == TickType.Major)
                    {
                        return tick;
                    }

                    tick = tick.next;
                }

                return null;
            }
        }

        internal decimal NormalizedForwardLength
        {
            get
            {
                if (this.next == null)
                {
                    return 0m;
                }

                return this.next.normalizedValue - this.normalizedValue;
            }
        }

        internal decimal ForwardLength
        {
            get
            {
                if (this.next == null)
                {
                    return 0m;
                }

                return this.next.value - this.value;
            }
        }

        internal decimal BackwardLength
        {
            get
            {
                if (this.previous == null)
                {
                    return 0m;
                }

                return this.value - this.previous.value;
            }
        }

        internal decimal NormalizedBackwardLength
        {
            get
            {
                if (this.previous == null)
                {
                    return 0m;
                }

                return this.normalizedValue - this.previous.normalizedValue;
            }
        }
    }
}
