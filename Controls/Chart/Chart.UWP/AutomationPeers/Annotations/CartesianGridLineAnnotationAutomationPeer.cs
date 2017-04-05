using System;
using System.Globalization;
using Telerik.UI.Xaml.Controls.Chart;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;

namespace Telerik.UI.Automation.Peers
{
    public class CartesianGridLineAnnotationAutomationPeer : ChartAnnotationAutomationPeer, IValueProvider
    {
        /// <summary>
        /// Initializes a new instance of the ChartAnnotationAutomationPeer class.
        /// </summary>
        public CartesianGridLineAnnotationAutomationPeer(CartesianGridLineAnnotation owner) 
            : base(owner)
        {
        }

        internal CartesianGridLineAnnotation CartesianGridLineAnnotation
        {
            get
            {
                return this.Owner as CartesianGridLineAnnotation;
            }
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(CartesianGridLineAnnotation);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(CartesianGridLineAnnotation);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "cartesian grid line annotation";
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

        /// <inheritdoc />
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Custom;
        }

        /// <summary>
        /// IValueProvider implementation.
        /// </summary>
        public void SetValue(string value)
        {
            bool isParseSuccessful = false;
            Axis axis = this.CartesianGridLineAnnotation.Axis;

            if (axis is DateTimeCategoricalAxis || axis is DateTimeContinuousAxis)
            {
                DateTime dateValue;
                if (DateTime.TryParse(value, out dateValue))
                {
                    this.CartesianGridLineAnnotation.Value = dateValue;
                    isParseSuccessful = true;
                }
            }
            else if (axis is NumericalAxis)
            {
                double doubleValue;
                if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out doubleValue))
                {
                    this.CartesianGridLineAnnotation.Value = doubleValue;
                    isParseSuccessful = true;
                }
            }
            else
            {
                this.CartesianGridLineAnnotation.Value = value;
                isParseSuccessful = true;
            }

            if (!isParseSuccessful)
            {
                throw new ArgumentException("Value cannot be converted from a string to a format the control recognizes.");
            }
        }

        /// <summary>
        /// IValueProvider implementation.
        /// </summary>
        public bool IsReadOnly => !this.CartesianGridLineAnnotation.IsEnabled;

        /// <summary>
        /// IValueProvider implementation.
        /// </summary>
        public string Value => this.CartesianGridLineAnnotation.Value.ToString();

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        internal void RaiseValueChangedAutomationEvent(string oldValue, string newValue)
        {
            this.RaisePropertyChangedEvent(ValuePatternIdentifiers.ValueProperty, oldValue, newValue);
        }
    }
}
