using System;
using System.Runtime.CompilerServices;
using Telerik.UI.Xaml.Controls.Input;
using Windows.Foundation;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;

namespace Telerik.UI.Automation.Peers
{
    public class RadNumericBoxAutomationPeer : RangeInputBaseAutomationPeer, IRangeValueProvider
    {
        /// <summary>
        /// Initializes a new instance of the RadNumericBoxAutomationPeer class.
        /// </summary>
        /// <param name="owner">The RadNumericBox that is associated with this RadNumericBoxAutomationPeer.</param>
        public RadNumericBoxAutomationPeer(RadNumericBox owner) 
            : base(owner)
        {
        }

        private RadNumericBox NumericBox
        {
            get
            {
                return (RadNumericBox)this.Owner;
            }
        }

        /// <summary>
        /// Gets a value that specifies whether the value of a control is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>true if the value is read-only; false if it can be modified.
        /// </returns>
        public bool IsReadOnly
        {
            get
            {
                return !this.NumericBox.IsEnabled;
            }
        }

        /// <summary>
        /// Gets the value that is added to or subtracted from the <see cref="P:Windows.UI.Xaml.Automation.Provider.IRangeValueProvider.Value"/> property when a large change is made, such as with the PAGE DOWN key.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The large-change value supported by the control or null (Nothing in Microsoft Visual Basic .NET) if the control does not support <see cref="P:Windows.UI.Xaml.Automation.Provider.IRangeValueProvider.LargeChange"/>.
        /// </returns>
        public double LargeChange
        {
            get
            {
                return this.NumericBox.LargeChange;
            }
        }
        
        /// <summary>
        /// Gets the maximum range value supported by the control.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The maximum value supported by the control or null (Nothing in Microsoft Visual Basic .NET) if the control does not support <see cref="P:Windows.UI.Xaml.Automation.Provider.IRangeValueProvider.Maximum"/>.
        /// </returns>
        public double Maximum
        {
            get
            {
                return this.NumericBox.Maximum;
            }
        }

        /// <summary>
        /// Gets the minimum range value supported by the control.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The minimum value supported by the control or null (Nothing in Microsoft Visual Basic .NET) if the control does not support <see cref="P:System.Windows.Automation.Provider.IRangeValueProvider.Minimum"/>.
        /// </returns>
        public double Minimum
        {
            get
            {
                return this.NumericBox.Minimum;
            }
        }
        
        /// <summary>
        /// Gets the value that is added to or subtracted from the <see cref="P:Windows.UI.Xaml.Automation.Provider.IRangeValueProvider.Value"/> property when a small change is made, such as with an arrow key.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The small-change value or null (Nothing in Microsoft Visual Basic .NET) if the control does not support <see cref="P:Windows.UI.Xaml.Automation.Provider.IRangeValueProvider.SmallChange"/>.
        /// </returns>
        public double SmallChange
        {
            get
            {
                return this.NumericBox.SmallChange;
            }
        }

        /// <summary>
        /// Gets the value of the control.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The value of the control or null (Nothing in Microsoft Visual Basic .NET) if the control does not support <see cref="P:Windows.UI.Xaml.Automation.Provider.IRangeValueProvider.Value"/>.
        /// </returns>
        public double Value
        {
            get
            {
                var value = this.NumericBox.Value;

                return value.HasValue ? value.Value : 0;
            }
        }

        protected override object GetPatternCore(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.RangeValue)
            {
                return this;
            }
            return null;
        }

        /// <summary>
        /// Gets the control type for the RadNumericBox that is associated with this RadNumericBoxAutomationPeer.
        /// </summary>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Spinner;
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return Owner.GetType().Name;
        }

        /// <inheritdoc />
        protected override string GetNameCore()
        {
            var nameCore = base.GetNameCore();
            if (!string.IsNullOrEmpty(nameCore))
                return nameCore;

            var numericBox = this.Owner as RadNumericBox;
            if (numericBox != null && !string.IsNullOrEmpty(numericBox.Name))
                return numericBox.Name;

            return string.Empty;
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return "RadNumericBox";
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "rad numeric box";
        }

        /// <summary>
        /// Sets the value of the control.
        /// </summary>
        /// <param name="value">Sets the value of the control.</param>
        public void SetValue(double value)
        {
            if (!this.IsEnabled())
            {
                throw new ElementNotEnabledException();
            }

            RadNumericBox owner = (RadNumericBox)this.Owner;
            if ((value < owner.Minimum) || (value > owner.Maximum))
            {
                throw new ArgumentOutOfRangeException("value");
            }
            owner.Value = value;
        }

        /// <summary>
        /// Called by GetClickablePoint.
        /// </summary>
        protected override Point GetClickablePointCore()
        {
            return new Point(double.NaN, double.NaN);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal void RaiseMaximumPropertyChangedEvent(double oldValue, double newValue)
        {
            this.RaisePropertyChangedEvent(RangeValuePatternIdentifiers.MaximumProperty, oldValue, newValue);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal void RaiseMinimumPropertyChangedEvent(double oldValue, double newValue)
        {
            this.RaisePropertyChangedEvent(RangeValuePatternIdentifiers.MinimumProperty, oldValue, newValue);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal void RaiseValuePropertyChangedEvent(double? oldValue, double? newValue)
        {
            this.RaisePropertyChangedEvent(RangeValuePatternIdentifiers.ValueProperty, oldValue, newValue);
        }
    }
}
