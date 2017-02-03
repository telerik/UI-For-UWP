using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

namespace Telerik.UI.Xaml.Controls.Primitives.BusyIndicator
{
    /// <summary>
    /// A utility class that is used to adapt an animation in arbitrary ways.
    /// </summary>
   [Windows.UI.Xaml.Data.Bindable]
    public class BusyIndicatorAnimationAdapter : FrameworkElement
    {
        /// <summary>
        /// Identifies the Animation dependency property.
        /// </summary>
        public static readonly DependencyProperty AnimationProperty =
            DependencyProperty.Register(nameof(Animation), typeof(Storyboard), typeof(BusyIndicatorAnimationAdapter), new PropertyMetadata(null));

        private double[] widthScaleFactors = new double[] { 0.0, 0.33, 0.66, 1.0 };

        /// <summary>
        /// Initializes a new instance of the BusyIndicatorAnimationAdapter class.
        /// </summary>
        public BusyIndicatorAnimationAdapter()
        {
            this.SizeChanged += this.OnBusyIndicatorAnimationAdapterSizeChanged;
        }

        /// <summary>
        /// Gets or sets the animation to adapt.
        /// </summary>
        public Storyboard Animation
        {
            get
            {
                return (Storyboard)this.GetValue(BusyIndicatorAnimationAdapter.AnimationProperty);
            }

            set
            {
                this.SetValue(BusyIndicatorAnimationAdapter.AnimationProperty, value);
            }
        }

        private void OnBusyIndicatorAnimationAdapterSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.Animation == null)
            {
                return;
            }

            if (this.Animation.Children.Count == 0)
            {
                return;
            }

            FrameworkElement owner = ElementTreeHelper.FindVisualAncestor<RadBusyIndicator>(this);
            if (owner == null)
            {
                return;
            }

            BusyIndicatorAnimation animation = ElementTreeHelper.FindVisualAncestor<BusyIndicatorAnimation>(this);
            if (animation == null)
            {
                return;
            }

            Point animationLocation = ElementTreeHelper.SafeTransformPoint(animation, owner, new Point(-10, 0));

            this.AdaptAnimation(this.Animation, owner.RenderSize.Width + 10, animationLocation.X);
        }

        // If more adapters are needed we can make this method abstract and inherit from this class to create a new adapter.
        private void AdaptAnimation(Storyboard storyboard, object info, double offset)
        {
            double width = (double)info;

            foreach (Timeline timeline in storyboard.Children)
            {
                DoubleAnimationUsingKeyFrames animation = timeline as DoubleAnimationUsingKeyFrames;
                if (animation == null)
                {
                    continue;
                }

                if (animation.KeyFrames.Count != this.widthScaleFactors.Length)
                {
                    continue;
                }

                for (int i = 0; i < animation.KeyFrames.Count; ++i)
                {
                    animation.KeyFrames[i].Value = (width * this.widthScaleFactors[i]) - offset;
                }
            }
        }
    }
}
