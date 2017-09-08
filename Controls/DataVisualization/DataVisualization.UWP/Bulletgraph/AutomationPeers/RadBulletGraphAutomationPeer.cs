using System.Runtime.CompilerServices;
using Telerik.UI.Xaml.Controls.DataVisualization;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// AutomationPeer class for <see cref="RadBulletGraph"/>.
    /// </summary>
    public class RadBulletGraphAutomationPeer : RadControlAutomationPeer, IRangeValueProvider
    {
        /// <summary>
        /// Initializes a new instance of the RadBulletGraphAutomationPeer class.
        /// </summary>
        /// <param name="owner">The <see cref="RadBulletGraph"/> that is associated with this RadBulletGraphAutomationPeer.</param>
        public RadBulletGraphAutomationPeer(RadBulletGraph owner) : base(owner)
        {
        }

        /// <summary>
        /// Gets a value indicating whether the value of a the BulletGraph is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return !this.BulletGraphOwner.IsEnabled;
            }
        }

        /// <summary>
        /// Gets the large-change value of the BulletGraph. For testing purposes this returns the value oft the LabelStep property.
        /// </summary>
        public double LargeChange
        {
            get
            {
                return this.BulletGraphOwner.LabelStep;
            }
        }

        /// <summary>
        ///  Gets the maximum range value that is supported by the control.
        /// </summary>
        public double Maximum
        {
            get
            {
                return this.BulletGraphOwner.EndValue;
            }
        }

        /// <summary>
        ///  Gets the minimum range value that is supported by the control.
        /// </summary>
        public double Minimum
        {
            get
            {
                return this.BulletGraphOwner.StartValue;
            }
        }

        /// <summary>
        /// Gets the small-change value of the BulletGraph. For testing purposes this returns the value of the TickStep property.
        /// </summary>
        public double SmallChange
        {
            get
            {
                return this.BulletGraphOwner.TickStep;
            }
        }

        /// <summary>
        /// Gets the value of the FeaturedMeasure property of the BulletGraph.
        /// </summary>
        public double Value
        {
            get
            {
                return this.BulletGraphOwner.FeaturedMeasure;
            }
        }

        private RadBulletGraph BulletGraphOwner
        {
            get
            {
                return (RadBulletGraph)this.Owner;
            }
        }

        /// <summary>
        /// Sets the FeaturedMeasure property of the BulletGraph.
        /// </summary>
        public void SetValue(double value)
        {
            this.BulletGraphOwner.FeaturedMeasure = value;
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
        internal void RaiseValuePropertyChangedEvent(double oldValue, double newValue)
        {
            this.RaisePropertyChangedEvent(RangeValuePatternIdentifiers.ValueProperty, oldValue, newValue);
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
        protected override string GetLocalizedControlTypeCore()
        {
            return "rad bullet graph";
        }
    }
}
