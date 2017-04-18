using System;
using System.Globalization;
using Telerik.UI.Xaml.Controls.Chart;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;

namespace Telerik.UI.Automation.Peers
{
    public class PolarAxisGridLineAnnotationAutomationPeer : ChartAnnotationAutomationPeer, IValueProvider
    {
        /// <summary>
        /// Initializes a new instance of the PolarAxisGridLineAnnotationAutomationPeer class.
        /// </summary>
        public PolarAxisGridLineAnnotationAutomationPeer(PolarAxisGridLineAnnotation owner) 
            : base(owner)
        {
        }
        
        internal PolarAxisGridLineAnnotation PolarAxisGridLineAnnotation
        {
            get
            {
                return this.Owner as PolarAxisGridLineAnnotation;
            }
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(PolarAxisGridLineAnnotation);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(PolarAxisGridLineAnnotation);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "polar axis grid line annotation";
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

        /// <summary>
        /// IValueProvider implementation.
        /// </summary>
        public void SetValue(string value)
        {
            double doubleValue;
            if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out doubleValue))
            {
                this.PolarAxisGridLineAnnotation.Value = doubleValue;
            }
            else
            {
                throw new ArgumentException("Value cannot be converted from a string to a format the control recognizes.");
            }
        }

        /// <inheritdoc />
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Custom;
        }

        /// <summary>
        /// IValueProvider implementation.
        /// </summary>
        public bool IsReadOnly => !this.PolarAxisGridLineAnnotation.IsEnabled;

        /// <summary>
        /// IValueProvider implementation.
        /// </summary>
        public string Value => this.PolarAxisGridLineAnnotation.Value.ToString();

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        internal void RaiseValueChangedAutomationEvent(string oldValue, string newValue)
        {
            this.RaisePropertyChangedEvent(ValuePatternIdentifiers.ValueProperty, oldValue, newValue);
        }
    }
}
