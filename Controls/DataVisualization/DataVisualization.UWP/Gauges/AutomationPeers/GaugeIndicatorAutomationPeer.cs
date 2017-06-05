using Telerik.UI.Xaml.Controls.DataVisualization;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// AutomationPeer class for <see cref="GaugeIndicator"/>.
    /// </summary>
    public class GaugeIndicatorAutomationPeer : RadControlAutomationPeer, IValueProvider
    {
        /// <summary>
        /// Initializes a new instance of the GaugeIndicatorAutomationPeer class.
        /// </summary>
        public GaugeIndicatorAutomationPeer(GaugeIndicator owner) 
            : base(owner)
        {
        }

        /// <inheritdoc />
        public bool IsReadOnly
        {
            get
            {
                return !this.OwnerGaugeIndicator.IsEnabled;
            }
        }

        /// <inheritdoc />
        public string Value
        {
            get
            {
                return this.OwnerGaugeIndicator.Value.ToString();
            }
        }

        private GaugeIndicator OwnerGaugeIndicator
        {
            get
            {
                return this.Owner as GaugeIndicator;
            }
        }

        /// <summary>
        /// IValueProvider implementation.
        /// </summary>
        public void SetValue(string value)
        {
            double parsedValue = 0;
            if (double.TryParse(value, out parsedValue))
            {
                this.OwnerGaugeIndicator.Value = parsedValue;
            }
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        internal void RaiseValueChangedAutomationEvent(string oldValue, string newValue)
        {
            this.RaisePropertyChangedEvent(ValuePatternIdentifiers.ValueProperty, oldValue, newValue);
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.DataVisualization.GaugeIndicator);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.DataVisualization.GaugeIndicator);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "gauge indicator";
        }

        /// <inheritdoc />
        protected override object GetPatternCore(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Value)
            {
                return this;
            }
            return base.GetPatternCore(patternInterface);
        }
    }
}
