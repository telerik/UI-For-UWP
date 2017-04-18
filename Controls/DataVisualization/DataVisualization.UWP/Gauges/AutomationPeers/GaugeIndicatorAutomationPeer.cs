using Telerik.UI.Xaml.Controls.DataVisualization;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;

namespace Telerik.UI.Automation.Peers
{
    public class GaugeIndicatorAutomationPeer : RadControlAutomationPeer, IValueProvider
    {
        public GaugeIndicatorAutomationPeer(GaugeIndicator owner) 
            : base(owner)
        {
        }

        /// <summary>
        /// IValueProvider implementation.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return !this.OwnerGaugeIndicator.IsEnabled;
            }
        }

        /// <summary>
        /// IValueProvider implementation.
        /// </summary>
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

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(GaugeIndicator);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(GaugeIndicator);
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

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        internal void RaiseValueChangedAutomationEvent(string oldValue, string newValue)
        {
            this.RaisePropertyChangedEvent(ValuePatternIdentifiers.ValueProperty, oldValue, newValue);
        }
    }
}
