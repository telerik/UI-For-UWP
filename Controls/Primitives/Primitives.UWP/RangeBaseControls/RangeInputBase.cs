using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Defines a logical steps for manipulations for its descendants.
    /// </summary>
    public abstract class RangeInputBase : RangeControlBase
    {
        /// <summary>
        /// Identifies the <see cref="SmallChange"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SmallChangeProperty =
          DependencyProperty.Register(nameof(SmallChange), typeof(double), typeof(RangeInputBase), new PropertyMetadata(1d, OnSmallChangeChanged));

        /// <summary>
        /// Identifies the <see cref="LargeChange"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LargeChangeProperty =
            DependencyProperty.Register(nameof(LargeChange), typeof(double), typeof(RangeInputBase), new PropertyMetadata(10d, OnLargeChangeChanged));

        /// <summary>
        /// Gets or sets the value to be added to or subtracted from the value.
        /// </summary>
        /// <value>
        /// The default value is 1.
        /// </value>
        public double SmallChange
        {
            get
            {
                return (double)this.GetValue(SmallChangeProperty);
            }
            set
            {
                this.SetValue(SmallChangeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value to be added to or subtracted from the value.
        /// </summary>
        /// <value>
        /// The default value is 10.
        /// </value>
        public double LargeChange
        {
            get
            {
                return (double)this.GetValue(LargeChangeProperty);
            }
            set
            {
                this.SetValue(LargeChangeProperty, value);
            }
        }

        internal virtual void OnSmallChangeChanged(double oldSmallChange, double newSmallChange)
        {
        }

        internal virtual void OnLargeChangeChanged(double oldLargeChange, double newLargeChange)
        {
        }

        private static void OnSmallChangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RangeInputBase sender = d as RangeInputBase;
            if (sender == null || !sender.IsTemplateApplied || sender.IsInternalPropertyChange)
            {
                return;
            }

            RangeControlBase.VerifyValidDoubleValue((double)e.NewValue);
            sender.OnSmallChangeChanged((double)e.OldValue, (double)e.NewValue);
        }

        private static void OnLargeChangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RangeInputBase sender = d as RangeInputBase;
            if (sender == null || !sender.IsTemplateApplied || sender.IsInternalPropertyChange)
            {
                return;
            }

            RangeControlBase.VerifyValidDoubleValue((double)e.NewValue);
            sender.OnLargeChangeChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new Automation.Peers.RangeInputBaseAutomationPeer(this);
        }
    }
}
