using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Telerik.UI.Xaml.Controls.DataVisualization;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// AutomationPeer class for <see cref="RadGauge"/>.
    /// </summary>
    public class RadGaugeAutomationPeer : RadControlAutomationPeer, IRangeValueProvider
    {
        /// <summary>
        /// Initializes a new instance of the RadGaugeAutomationPeer class.
        /// </summary>
        /// <param name="owner">The RadGauge that is associated with this RadGaugeAutomationPeer.</param>
        public RadGaugeAutomationPeer(RadGauge owner) 
            : base(owner)
        {
        }
        
        /// <inheritdoc />
        public double Value
        {
            get
            {
                var arrowIndicator = this.Gauge.Indicators.Where(a => a is ArrowGaugeIndicator).FirstOrDefault();
                if (arrowIndicator != null)
                {
                    return arrowIndicator.Value;
                }

                var markerIndicator = this.Gauge.Indicators.Where(a => a is MarkerGaugeIndicator).FirstOrDefault();
                if (markerIndicator != null)
                {
                    return markerIndicator.Value;
                }

                if (this.Gauge is RadLinearGauge)
                {
                    var linearIndicator = this.Gauge.Indicators.Where(a => a is LinearBarGaugeIndicator).FirstOrDefault();
                    if (linearIndicator != null)
                    {
                        return linearIndicator.Value;
                    }
                }

                if (this.Gauge is RadRadialGauge)
                {
                    var radialIndicator = this.Gauge.Indicators.Where(a => a is RadialBarGaugeIndicator).FirstOrDefault();
                    if (radialIndicator != null)
                    {
                        return radialIndicator.Value;
                    }
                }

                var defaultIndicator = this.Gauge.Indicators.FirstOrDefault();
                if (defaultIndicator != null)
                {
                    return defaultIndicator.Value;
                }

                return this.Gauge.MinValue;
            }
        }

        /// <summary>
        /// IRangeValueProvider implementation.
        /// </summary>
        public bool IsReadOnly => !this.Gauge.IsEnabled;

        /// <summary>
        /// IRangeValueProvider implementation.
        /// </summary>
        public double LargeChange => this.Gauge.MajorTickStep;

        /// <summary>
        /// IRangeValueProvider implementation.
        /// </summary>
        public double Maximum => this.Gauge.MaxValue;

        /// <summary>
        /// IRangeValueProvider implementation.
        /// </summary>
        public double Minimum => this.Gauge.MinValue;

        /// <summary>
        /// IRangeValueProvider implementation.
        /// </summary>
        public double SmallChange => this.Gauge.TickStep;

        private RadGauge Gauge
        {
            get
            {
                return (RadGauge)this.Owner;
            }
        }

        /// <summary>
        /// IRangeValueProvider implementation.
        /// </summary>
        public void SetValue(double value)
        {
            if (!this.IsEnabled())
            {
                throw new ElementNotEnabledException();
            }

            if ((value < this.Gauge.MinValue) || (value > this.Gauge.MaxValue))
            {
                throw new ArgumentOutOfRangeException("value");
            }

            foreach (var indicator in this.Gauge.Indicators)
            {
                indicator.Value = value;
            }
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

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.DataVisualization.RadGauge);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.DataVisualization.RadGauge);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "rad gauge";
        }

        /// <inheritdoc />
        protected override bool HasKeyboardFocusCore()
        {
            return true;
        }

        /// <inheritdoc />
        protected override string GetAutomationIdCore()
        {
            var automationId = base.GetAutomationIdCore();
            if (!string.IsNullOrEmpty(automationId))
            {
                return automationId;
            }

            return nameof(Telerik.UI.Xaml.Controls.DataVisualization.RadGauge);
        }

        /// <inheritdoc />
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Spinner;
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
    }
}
