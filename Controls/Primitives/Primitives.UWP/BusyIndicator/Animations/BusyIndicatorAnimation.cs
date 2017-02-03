using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

namespace Telerik.UI.Xaml.Controls.Primitives.BusyIndicator
{
    /// <summary>
    /// Represents an animation displayed in the <see cref="RadBusyIndicator"/> control.
    /// </summary>
    public sealed class BusyIndicatorAnimation : RadControl
    {
        /// <summary>
        /// Identifies the <see cref="IsActive"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register(nameof(IsActive), typeof(bool), typeof(BusyIndicatorAnimation), new PropertyMetadata(false, OnIsActiveChanged));

        private bool isActive;
        private Storyboard storyboard;

        /// <summary>
        /// Initializes a new instance of the <see cref="BusyIndicatorAnimation"/> class.
        /// </summary>
        public BusyIndicatorAnimation()
        {
            this.DefaultStyleKey = typeof(BusyIndicatorAnimation);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the animation is currently running.
        /// </summary>
        public bool IsActive
        {
            get
            {
                return this.isActive;
            }
            set
            {
                this.SetValue(IsActiveProperty, value);
            }
        }

        /// <summary>
        /// Starts the progress indicator animation.
        /// </summary>
        public void Start()
        {
            this.isActive = true;
            if (this.storyboard != null)
            {
                this.storyboard.Begin();
            }
        }

        /// <summary>
        /// Stops the progress indicator animation.
        /// </summary>
        public void Stop()
        {
            this.isActive = false;
            if (this.storyboard != null)
            {
                this.storyboard.Stop();
            }
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. In simplest terms, this means the method is called just before a UI element displays in an application. For more information, see Remarks.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            FrameworkElement layoutRoot = this.GetTemplateChild("PART_LayoutRoot") as FrameworkElement;
            this.storyboard = layoutRoot.Resources["PART_Animation"] as Storyboard;

            if (this.isActive)
            {
                this.Start();
            }
        }

        private static void OnIsActiveChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            BusyIndicatorAnimation typedSender = sender as BusyIndicatorAnimation;
            typedSender.OnIsActiveChanged(args);
        }

        private void OnIsActiveChanged(DependencyPropertyChangedEventArgs args)
        {
            this.isActive = (bool)args.NewValue;

            if (this.isActive)
            {
                this.Start();
            }
            else
            {
                this.Stop();
            }
        }
    }
}