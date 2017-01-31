using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Core;
using Telerik.UI.Xaml.Controls.Primitives.RangeSlider;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    public partial class RangeSliderPrimitive : SliderBase
    {
        internal const double DefaultMinimumValue = 0d;
        internal const double DefaultMaximumValue = 10d;
        internal const double DefaultSelectionStartValue = 4d;
        internal const double DefaultSelectionEndValue = 6d;

        private bool isLoaded;
        private SelectionRange visualSelection;

        internal double DesiredSelectionStart
        {
            get;
            set;
        }

        internal double DesiredSelectionEnd
        {
            get;
            set;
        }

        internal bool ShouldRefreshValues
        {
            get;
            set;
        }

        internal SelectionRange VisualSelection
        {
            get
            {
                return this.visualSelection;
            }

            set
            {
                this.visualSelection = value;
            }
        }

        internal static double CalculateSelectionStart(double minimum, double maximum, double selectionStart, double desiredSelectionStart)
        {
            double actualSelectionStart = selectionStart;

            if (minimum > selectionStart)
            {
                actualSelectionStart = minimum;
            }
            else if (desiredSelectionStart >= minimum && desiredSelectionStart < maximum)
            {
                actualSelectionStart = desiredSelectionStart;
            }
            else if (selectionStart > maximum)
            {
                actualSelectionStart = maximum;
            }

            return actualSelectionStart;
        }

        internal void TryUpdate()
        {
            if (this.isLoaded && this.ShouldRefreshValues)
            {
                this.UpdateValues();
                this.InvalidateThumbsPanelArrange();
            }
        }

        internal void UpdateValues()
        {
            double minimum = this.Minimum;
            double maximum = this.Maximum;
            double selectionStart = this.SelectionStart;
            double selectionEnd = this.SelectionEnd;

            this.ShouldRefreshValues = false;

            this.DesiredSelectionEnd = Math.Min(this.DesiredSelectionEnd, maximum);

            selectionStart = this.Snap(RangeSliderPrimitive.CalculateSelectionStart(minimum, maximum, selectionStart, this.DesiredSelectionStart));
            selectionEnd = this.Snap(this.CalculateSelectionEnd(maximum, selectionStart, selectionEnd, this.DesiredSelectionEnd));

            this.ChangePropertyInternally(RangeSliderPrimitive.SelectionStartProperty, selectionStart);
            this.ChangePropertyInternally(RangeSliderPrimitive.SelectionEndProperty, selectionEnd);

            this.ShouldRefreshValues = true;
        }

        internal double Snap(double value)
        {
            var step = 0d;
            var minimum = this.Minimum;
            var maximum = this.Maximum;
            var isInRange = value <= maximum && value >= minimum;

            switch (this.SnapsTo)
            {
                case SnapsTo.Ticks:
                    step = this.TickFrequency;
                    break;
                case SnapsTo.SmallChange:
                    step = this.SmallChange;
                    break;
                case SnapsTo.LargeChange:
                    step = this.LargeChange;
                    break;
                default:
                    step = 0d;
                    break;
            }

            if (step > 0 && isInRange)
            {
                minimum = minimum + (Math.Round((value - minimum) / step) * step);
                maximum = Math.Min(maximum, minimum + step);

                if (minimum > maximum)
                {
                    minimum = maximum;
                }

                value = GreaterThan(value, (minimum + maximum) * 0.5) ? maximum : minimum;
            }

            return value;
        }

        internal double CalculateSelectionEnd(double maximum, double selectionStart, double selectionEnd, double desiredSelectionEnd)
        {
            double actualSelectionEnd = selectionEnd;

            if (desiredSelectionEnd <= maximum && desiredSelectionEnd > selectionStart)
            {
                actualSelectionEnd = desiredSelectionEnd;
            }
            else if (selectionEnd > maximum)
            {
                actualSelectionEnd = maximum;
            }
            else if (selectionEnd < selectionStart)
            {
                actualSelectionEnd = selectionStart;
                this.DesiredSelectionEnd = selectionStart;
            }

            return actualSelectionEnd;
        }

        /// <summary>
        /// Sets the SelectionEnd value.Exposed as internal for testing purposes.
        /// </summary>
        internal void UpdateSelectionEnd(double delta, bool forceUpdate = false)
        {
            if (this.isLoaded)
            {
                var selectionRange = this.VisualSelection;
                var actualValue = selectionRange.End;
                var desizeredValue = actualValue + delta;
                actualValue = CoerceValue(desizeredValue, selectionRange.Start, this.Maximum);

                this.UpdateSelectionStateValue(RangeSliderPrimitive.SelectionEndProperty, actualValue, (s) => this.visualSelection.End = s, forceUpdate);
            }
        }

        /// <summary>
        /// Sets the SelectionStart value.Exposed as internal for testing purposes.
        /// </summary>
        internal void UpdateSelectionStart(double delta, bool forceUpdate = false)
        {
            if (this.isLoaded)
            {
                var selectionRange = this.VisualSelection;
                var actualValue = selectionRange.Start;
                var desizeredValue = actualValue + delta;
                actualValue = CoerceValue(desizeredValue, this.Minimum, selectionRange.End);

                this.UpdateSelectionStateValue(RangeSliderPrimitive.SelectionStartProperty, actualValue, (s) => this.visualSelection.Start = s, forceUpdate);
            }
        }

        private static double CoerceValue(double value, double rangeStart, double rangeEnd)
        {
            double actualValue = value;

            if (value < rangeStart)
            {
                actualValue = rangeStart;
            }
            else if (value > rangeEnd)
            {
                actualValue = rangeEnd;
            }

            return actualValue;
        }

        private static bool GreaterThan(double value1, double value2)
        {
            return (value1 > value2) && !RadMath.AreClose(value1, value2);
        }

        private void InitializeModel()
        {
            this.DesiredSelectionStart = DefaultSelectionStartValue;
            this.DesiredSelectionEnd = DefaultSelectionEndValue;

            this.ShouldRefreshValues = true;

            this.visualSelection = new SelectionRange(this.SelectionStart, this.SelectionEnd);
        }

        private double CalculateCoeficient(double width)
        {
            double coef = 1;

            var range = this.Maximum - this.Minimum;
            if (range > 0)
            {
                coef = (width - this.SelectionStartOffset - this.SelectionEndOffset) / range;
            }
            return Math.Max(1, coef);
        }

        private double GetRelativeOffset(double offset)
        {
            double selectionOffsets = this.SelectionEndOffset + this.SelectionStartOffset;
            var dimention = this.Orientation == Windows.UI.Xaml.Controls.Orientation.Horizontal ? this.ActualWidth : this.ActualHeight;
            var delta = offset * (this.Maximum - this.Minimum) / (dimention - selectionOffsets);
            return delta;
        }

        private void UpdateSelectionStateValue(DependencyProperty property, double actualValue, Action<double> action, bool forceUpdate = false)
        {
            if (this.isLoaded)
            {
                double propertyValue = (double)this.GetValue(property);
                var snapValue = this.Snap(actualValue);

                if (!forceUpdate && snapValue != propertyValue && !this.IsDeferredDraggingEnabled)
                {
                    this.SetValue(property, snapValue);
                    action(actualValue);
                    return;
                }

                if (forceUpdate)
                {
                    this.SetValue(property, snapValue);
                }
                else
                {
                    snapValue = actualValue;
                }

                action(snapValue);
            }
        }
    }
}
