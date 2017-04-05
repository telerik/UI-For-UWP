using System;
using System.Runtime.CompilerServices;
using Telerik.Core;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;

namespace Telerik.UI.Automation.Peers
{
    public class SliderBaseAutomationPeer : RangeInputBaseAutomationPeer, IRangeValueProvider
    {
        private bool hasMaximumDirection;

        /// <summary>
        /// Initializes a new instance of the SliderBaseAutomationPeer class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public SliderBaseAutomationPeer(SliderBase owner)
            : base(owner)
        {
            this.hasMaximumDirection = true;
        }

        private SliderBase SliderBase
        {
            get
            {
                return (SliderBase)this.Owner;
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
                return !this.SliderBase.IsEnabled;
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
                return this.SliderBase.LargeChange;
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
                return this.SliderBase.Maximum;
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
                return this.SliderBase.Minimum;
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
                return this.SliderBase.SmallChange;
            }
        }

        /// <summary>
        /// Gets the value of the control.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The value of the control or null (Nothing in Microsoft Visual Basic .NET) if the control does not support <see cref="P:Windows.UI.Xaml.Automation.Provider.IRangeValueProvider.Value"/>.
        /// </returns>
        public virtual double Value
        {
            get
            {
                if (this.SliderBase != null)
                {
                    var rangeSliderPrimitive = ElementTreeHelper.FindVisualDescendant<RangeSliderPrimitive>(this.SliderBase);
                    if (rangeSliderPrimitive != null)
                    {
                        var rangeSelection = rangeSliderPrimitive.VisualSelection;
                        if (rangeSelection.End == this.Maximum)
                        {
                            this.hasMaximumDirection = false;
                        }
                        else if (rangeSelection.Start == this.Minimum)
                        {
                            this.hasMaximumDirection = true;
                        }

                        return rangeSelection.End - rangeSelection.Start;
                    }
                }

                throw new NotSupportedException("Value property is not supported by SliderBase.");
            }
        }

        /// <summary>
        /// IRangeValueProvider implementation.
        /// </summary>
        public void SetValue(double value)
        {
            var rangeSliderPrimitive = ElementTreeHelper.FindVisualDescendant<RangeSliderPrimitive>(this.SliderBase);
            if (rangeSliderPrimitive != null)
            {
                var currentValue = rangeSliderPrimitive.VisualSelection.End - rangeSliderPrimitive.VisualSelection.Start;
                if (this.hasMaximumDirection)
                {
                    if (value > currentValue)
                    {
                        rangeSliderPrimitive.UpdateSelectionEnd(this.LargeChange, true);
                    }
                    else
                    {
                        rangeSliderPrimitive.UpdateSelectionStart(this.LargeChange, true);
                    }
                }
                else
                {
                    if (value > currentValue)
                    {
                        rangeSliderPrimitive.UpdateSelectionStart(-this.LargeChange, true);
                    }
                    else
                    {
                        rangeSliderPrimitive.UpdateSelectionEnd(-this.LargeChange, true);
                    }
                }
            }
        }

        /// <inheritdoc />
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Slider;
        }

        /// <inheritdoc />
        protected override object GetPatternCore(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.RangeValue)
            {
                return this;
            }

            return base.GetPatternCore(patternInterface);
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.Primitives.SliderBase);
        }

        /// <inheritdoc />
        protected override string GetAutomationIdCore()
        {
            var automationId = base.GetAutomationIdCore();
            if (!string.IsNullOrEmpty(automationId))
            {
                return automationId;
            }

            return nameof(Telerik.UI.Xaml.Controls.Primitives.SliderBase);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "slider base";
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
