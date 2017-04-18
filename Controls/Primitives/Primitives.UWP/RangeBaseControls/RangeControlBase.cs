using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Represents a control that defines a <see cref="System.Double"/> value that is constrained within a specified range.
    /// </summary>
    public abstract class RangeControlBase : RadHeaderedControl
    {
        /// <summary>
        /// Identifies the <see cref="Minimum"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(nameof(Minimum), typeof(double), typeof(RangeControlBase), new PropertyMetadata(0d, OnMinimumChanged));

        /// <summary>
        /// Identifies the <see cref="Maximum"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MaximumProperty =
         DependencyProperty.Register(nameof(Maximum), typeof(double), typeof(RangeControlBase), new PropertyMetadata(100d, OnMaximumChanged));

        private RangeChange delayedRangeChange;

        private enum RangeChange
        {
            None,
            Min,
            Max
        }

        /// <summary>
        ///  Gets or sets the lowest possible value of a range element.
        /// </summary>
        /// <value>
        /// The default value is 0.
        /// </value>
        public double Minimum
        {
            get
            {
                return (double)this.GetValue(MinimumProperty);
            }
            set
            {
                this.SetValue(MinimumProperty, value);
            }
        }

        /// <summary>
        ///  Gets or sets the highest possible value of a range element.
        /// </summary>
        /// <value>
        /// The default value is 100.
        /// </value>
        public double Maximum
        {
            get
            {
                return (double)this.GetValue(MaximumProperty);
            }
            set
            {
                this.SetValue(MaximumProperty, value);
            }
        }

        internal static void VerifyValidDoubleValue(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                throw new ArgumentException("Double Value is NaN or Infinity.");
            }
        }

        internal virtual void OnMinimumChanged(double oldMinimum, double newMinimum)
        {
        }

        internal virtual void OnMaximumChanged(double oldMaximum, double newMaximum)
        {
        }

        /// <summary>
        /// Occurs when the <see cref="M:OnApplyTemplate" /> method has been called and the template is already successfully applied.
        /// </summary>
        protected override void OnTemplateApplied()
        {
            base.OnTemplateApplied();
            this.FitRange(this.delayedRangeChange);
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new Automation.Peers.RangeControlBaseAutomationPeer(this);
        }

        private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RangeControlBase sender = d as RangeControlBase;
            if (sender == null || sender.IsInternalPropertyChange)
            {
                return;
            }

            if (!sender.IsTemplateApplied)
            {
                sender.delayedRangeChange = RangeChange.Min;
            }
            else
            {
                VerifyValidDoubleValue((double)e.NewValue);
                sender.InvokeAsync(() =>
                 {
                     sender.FitRange(RangeChange.Min);
                     sender.OnMinimumChanged((double)e.OldValue, (double)e.NewValue);
                 });
            }
        }

        private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RangeControlBase sender = d as RangeControlBase;
            if (sender == null || sender.IsInternalPropertyChange)
            {
                return;
            }

            if (!sender.IsTemplateApplied)
            {
                sender.delayedRangeChange = RangeChange.Max;
            }
            else
            {
                VerifyValidDoubleValue((double)e.NewValue);
                sender.InvokeAsync(() =>
                    {
                        sender.FitRange(RangeChange.Max);
                        sender.OnMaximumChanged((double)e.OldValue, (double)e.NewValue);
                    });
            }
        }

        private void FitRange(RangeChange change)
        {
            var min = this.Minimum;
            var max = this.Maximum;

            if (max < min)
            {
                if (change == RangeChange.Min)
                {
                    min = max;
                    this.ChangePropertyInternally(MinimumProperty, min);
                }
                else if (change == RangeChange.Max)
                {
                    max = min;
                    this.ChangePropertyInternally(MaximumProperty, max);
                }
            }
        }
    }
}
