using System;
using System.Diagnostics;
using Telerik.Core;
using Telerik.UI.Automation.Peers;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Media.Animation;

namespace Telerik.UI.Xaml.Controls.DataVisualization
{
    /// <summary>
    /// This is the base class of the gauge indicators. It is abstract
    /// because more concrete types must implement the arrange logic. Every indicator
    /// knows how to arrange itself in its range container.
    /// </summary>
    public abstract class GaugeIndicator : RadControl
    {
        /// <summary>
        /// Identifies the Value dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(double), typeof(GaugeIndicator), new PropertyMetadata(0d, OnValuePropertyChanged));

        /// <summary>
        /// Identifies the StartValue dependency property.
        /// </summary>
        public static readonly DependencyProperty StartValueProperty =
            DependencyProperty.Register(nameof(StartValue), typeof(double), typeof(GaugeIndicator), new PropertyMetadata(0d, OnStartValuePropertyChanged));

        /// <summary>
        /// Identifies the IsAnimated dependency property.
        /// </summary>
        public static readonly DependencyProperty IsAnimatedProperty =
            DependencyProperty.Register(nameof(IsAnimated), typeof(bool), typeof(GaugeIndicator), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the AnimationEasing dependency property.
        /// </summary>
        public static readonly DependencyProperty AnimationEasingProperty =
            DependencyProperty.Register(nameof(AnimationEasing), typeof(EasingFunctionBase), typeof(GaugeIndicator), new PropertyMetadata(null, OnAnimationEasingPropertyChanged));

        /// <summary>
        /// Identifies the AnimationDuration dependency property.
        /// </summary>
        public static readonly DependencyProperty AnimationDurationProperty =
            DependencyProperty.Register(nameof(AnimationDuration), typeof(Duration), typeof(GaugeIndicator), new PropertyMetadata(new Duration(System.TimeSpan.FromSeconds(0.5)), OnAnimationDurationPropertyChanged));

        /// <summary>
        /// Identifies the ActualValue dependency property.
        /// </summary>
        private static readonly DependencyProperty ActualValueProperty =
            DependencyProperty.Register(nameof(ActualValue), typeof(double), typeof(GaugeIndicator), new PropertyMetadata(0d, OnActualValuePropertyChanged));

        private bool isAnimationRunning = false;
        private Storyboard storyboard = new Storyboard();
        private DoubleAnimation valueAnimation = new DoubleAnimation();
        private GaugePanel ownerPanel;

        /// <summary>
        /// Initializes a new instance of the GaugeIndicator class.
        /// </summary>
        protected GaugeIndicator()
        {
            Storyboard.SetTarget(this.valueAnimation, this);
            Storyboard.SetTargetProperty(this.valueAnimation, "ActualValue");

            this.valueAnimation.EnableDependentAnimation = true;
            this.valueAnimation.Duration = this.AnimationDuration;
            this.valueAnimation.EasingFunction = this.AnimationEasing;

            this.storyboard.Children.Add(this.valueAnimation);
            this.storyboard.Completed += (sender, args) => 
            {
                this.isAnimationRunning = false;
            };
        }
        
        /// <summary>
        /// This event fires after the Value property changes.
        /// The event arguments contain the new as well as the old value.
        /// </summary>
        public event EventHandler<IndicatorValueChangedEventArgs> ValueChanged;

        /// <summary>
        /// Gets or sets the value of this indicator.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "Value is the desired name.")]
        public double Value
        {
            get
            {
                return (double)this.GetValue(GaugeIndicator.ValueProperty);
            }
            set
            {
                this.SetValue(GaugeIndicator.ValueProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value that determines from where in the value range does the indicator start.
        /// By default every indicator starts at 0. The Value and StartValue properties do the exact same
        /// thing in the MarkerGaugeIndicator since it is always on a single point in the range.
        /// </summary>
        public double StartValue
        {
            get
            {
                return (double)this.GetValue(GaugeIndicator.StartValueProperty);
            }

            set
            {
                this.SetValue(GaugeIndicator.StartValueProperty, value);
            }
        }

        /// <summary>
        /// Gets the owner range of this indicator.
        /// </summary>
        public GaugePanel Owner
        {
            get
            {
                return this.ownerPanel;
            }
            internal set
            {
                if (this.ownerPanel == value)
                {
                    return;
                }

                GaugePanel prevOwner = this.ownerPanel;
                this.ownerPanel = value;
                this.OnOwnerChanged(value, prevOwner);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether animated transition
        /// between values is enabled.
        /// </summary>
        public bool IsAnimated
        {
            get
            {
                return (bool)this.GetValue(GaugeIndicator.IsAnimatedProperty);
            }

            set
            {
                this.SetValue(GaugeIndicator.IsAnimatedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the duration of the animation when IsAnimated is true.
        /// </summary>
        public Duration AnimationDuration
        {
            get
            {
                return (Duration)this.GetValue(GaugeIndicator.AnimationDurationProperty);
            }

            set
            {
                this.SetValue(GaugeIndicator.AnimationDurationProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the easing of the animation when IsAnimated is true.
        /// </summary>
        public EasingFunctionBase AnimationEasing
        {
            get
            {
                return (EasingFunctionBase)this.GetValue(GaugeIndicator.AnimationEasingProperty);
            }

            set
            {
                this.SetValue(GaugeIndicator.AnimationEasingProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a callback for testing purposes.
        /// </summary>
        internal Action MeasureCallback
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a callback for testing purposes.
        /// </summary>
        internal Action ArrangeCallback
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the actual value of the indicator.
        /// Inheritors must use this property for their calculations, not the Value property.
        /// </summary>
        internal double ActualValue
        {
            get
            {
                return (double)this.GetValue(GaugeIndicator.ActualValueProperty);
            }
            private set
            {
                this.SetValue(GaugeIndicator.ActualValueProperty, value);
            }
        }
        
        /// <summary>
        /// Gets the difference between the min and max value of the parent range.
        /// </summary>
        internal double MinMaxValueDifference
        {
            get
            {
                if (this.Owner == null)
                {
                    return 1;
                }

                return this.Owner.OwnerGauge.MaxValue - this.Owner.OwnerGauge.MinValue;
            }
        }

        internal Size LastMeasureSize
        {
            get;
            set;
        }

        /// <summary>
        /// This method should be called whenever an indicator needs to be updated.
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public void Update(Size availableSize)
        {
            if (this.ownerPanel == null || this.ownerPanel.OwnerGauge == null)
            {
                return;
            }

            this.ClampValueToRange(ValueProperty);
            this.ClampValueToRange(StartValueProperty);

            if (this.isAnimationRunning)
            {
                double currentActualValue = this.ActualValue;
                this.ClampValueToRange(ActualValueProperty);
                double clampedActualValue = this.ActualValue;

                if (currentActualValue != clampedActualValue)
                {
                    this.valueAnimation.From = clampedActualValue;
                }
            }

            if (this.IsTemplateApplied)
            {
                this.UpdateOverride(availableSize);
            }
        }

        internal void ScheduleUpdate()
        {
            if (this.ownerPanel != null)
            {
                this.ownerPanel.ScheduleUpdate();
            }
        }

        /// <summary>
        /// This method defines how a particular indicator will
        /// arrange itself in the parent range.
        /// </summary>
        /// <param name="finalSize">The size in which the indicator should arrange itself.</param>
        internal virtual Rect GetArrangeRect(Size finalSize)
        {
            if (this.ArrangeCallback != null)
            {
                this.ArrangeCallback();
            }

            return new Rect(new Point(), finalSize);
        }

        /// <summary>
        /// A virtual method that is called when the value of this indicator changes.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        /// <param name="oldValue">The old value.</param>
        internal virtual void OnValueChanged(double newValue, double oldValue)
        {
            if (this.ValueChanged != null)
            {
                this.ValueChanged(this, new IndicatorValueChangedEventArgs(newValue, oldValue));
            }
        }

        /// <summary>
        /// A virtual method that is called when the start value of this indicator changes.
        /// </summary>
        /// <param name="newStartValue">The new start value.</param>
        /// <param name="oldStartValue">The old start value.</param>
        internal virtual void OnStartValueChanged(double newStartValue, double oldStartValue)
        {
        }

        /// <summary>
        /// A virtual method that is called when the Owner of this indicator changes.
        /// </summary>
        /// <param name="newOwner">The new Owner.</param>
        /// <param name="oldOwner">The old Owner.</param>
        internal virtual void OnOwnerChanged(GaugePanel newOwner, GaugePanel oldOwner)
        {
            if (newOwner != null)
            {
                newOwner.ScheduleUpdate();
            }
        }
        
        /// <summary>
        /// This is a virtual method that resets the state of the indicator.
        /// The parent range is responsible to (indirectly) call this method when
        /// a property of importance changes.
        /// </summary>
        /// <param name="availableSize">A size which can be used by the update logic.</param>
        /// <remarks>
        /// The linear range for example triggers the UpdateOverride() method
        /// of its indicators when its Orientation property changes.
        /// </remarks>
        internal virtual void UpdateOverride(Size availableSize)
        {
        }

        /// <summary>
        /// Called in the measure layout pass to determine the desired size.
        /// </summary>
        /// <param name="availableSize">The available size that was given by the layout system.</param>
        /// <returns>Returns the desired size of the indicator.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (this.MeasureCallback != null)
            {
                this.MeasureCallback();
            }

            if (this.ownerPanel == null)
            {
                this.ownerPanel = ElementTreeHelper.FindVisualAncestor<GaugePanel>(this);
            }

            Size result = RadGauge.NormalizeSize(availableSize);
            if (result.Width == 0 || result.Height == 0)
            {
                result = this.ownerPanel.LastMeasureSize;
            }

            this.Update(result);

            this.LastMeasureSize = result;

            return result;
        }

        /// <inheritdoc/>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new GaugeIndicatorAutomationPeer(this);
        }

        private static void OnValuePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            double newVal = (double)args.NewValue;
            RadGauge.ValidateValue(newVal);

            GaugeIndicator indicator = sender as GaugeIndicator;
            if (!indicator.IsTemplateApplied || indicator.Owner == null || indicator.IsInternalPropertyChange)
            {
                indicator.ActualValue = newVal;
                return;
            }
         
            indicator.ClampValueToRange(ValueProperty);      

            if (indicator.isAnimationRunning)
            {
                indicator.isAnimationRunning = false;
                indicator.storyboard.Seek(new TimeSpan(0));
            }
             
            double value = indicator.Value;
            if (indicator.IsAnimated)
            {
                double from = (double)args.OldValue;
                StartValueAnimation(from, newVal, indicator);
            }
            else
            {
                indicator.ActualValue = value;
            }

            var peer = FrameworkElementAutomationPeer.FromElement(indicator) as GaugeIndicatorAutomationPeer;
            if (peer != null)
            {
                peer.RaiseValueChangedAutomationEvent(args.OldValue.ToString(), args.NewValue.ToString());
            }
        }

        private static void OnStartValuePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            GaugeIndicator indicator = sender as GaugeIndicator;
            if (indicator.IsInternalPropertyChange || indicator.Owner == null)
            {
                return;
            }

            indicator.ClampValueToRange(StartValueProperty);
            indicator.OnStartValueChanged(indicator.StartValue, (double)args.OldValue);
        }

        private static void OnActualValuePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            GaugeIndicator indicator = sender as GaugeIndicator;
            if (!indicator.IsTemplateApplied)
            {
                return;
            }

            indicator.OnValueChanged((double)args.NewValue, (double)args.OldValue);
        }

        private static void OnAnimationEasingPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            GaugeIndicator indicator = sender as GaugeIndicator;
            indicator.valueAnimation.EasingFunction = (EasingFunctionBase)args.NewValue;
        }

        private static void OnAnimationDurationPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            GaugeIndicator indicator = sender as GaugeIndicator;
            indicator.valueAnimation.Duration = (Duration)args.NewValue;
        }

        private static void StartValueAnimation(double from, double to, GaugeIndicator indicatorToAnimate)
        {
            indicatorToAnimate.valueAnimation.From = from;
            indicatorToAnimate.valueAnimation.To = to;

            indicatorToAnimate.isAnimationRunning = true;
            indicatorToAnimate.storyboard.Begin();
        }

        private void ClampValueToRange(DependencyProperty valueProperty)
        {
            if (this.ownerPanel == null || this.ownerPanel.OwnerGauge == null)
            {
                return;
            }

            double min = this.ownerPanel.OwnerGauge.MinValue;
            double max = this.ownerPanel.OwnerGauge.MaxValue;

            double currentVal = (double)this.GetValue(valueProperty);

            if (currentVal < min)
            {
                this.ChangePropertyInternally(valueProperty, min);
            }
            else if (currentVal > max)
            {
                this.ChangePropertyInternally(valueProperty, max);
            }
        }
    }
}
