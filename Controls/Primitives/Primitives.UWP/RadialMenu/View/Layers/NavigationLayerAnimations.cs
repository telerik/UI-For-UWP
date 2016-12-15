using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Telerik.UI.Xaml.Controls.Primitives.Menu
{
    internal class NavigationLayerAnimations
    {
        internal static double speedFactor = 1;

        private NavigateItemLayer owner;

        internal NavigateItemLayer Owner
        {
            get
            {
                return this.owner;
            }
            set
            {
                this.owner = value;
            }
        }

        internal void SetNavigationBackgroundElementStartAnimation(Storyboard storyboard)
        {
            ObjectAnimationUsingKeyFrames showElementAnimation = new ObjectAnimationUsingKeyFrames();
            showElementAnimation.KeyFrames.Add(new DiscreteObjectKeyFrame { KeyTime = TimeSpan.Zero, Value = Visibility.Visible });

            Storyboard.SetTargetProperty(showElementAnimation, "(UIElement.Visibility)");
            Storyboard.SetTarget(showElementAnimation, this.Owner.BackgroundElement);

            DoubleAnimation showScaleWidthAnimation = new DoubleAnimation();
            showScaleWidthAnimation.EnableDependentAnimation = true;
            showScaleWidthAnimation.From = 0;
            showScaleWidthAnimation.To = this.Owner.BackgroundElement.Width;
            showScaleWidthAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(160 * NavigationLayerAnimations.speedFactor));
            Storyboard.SetTarget(showScaleWidthAnimation, this.Owner.BackgroundElement);
            Storyboard.SetTargetProperty(showScaleWidthAnimation, "Width");

            DoubleAnimation showScaleHeightAnimation = new DoubleAnimation();
            showScaleHeightAnimation.EnableDependentAnimation = true;
            showScaleHeightAnimation.From = 0;
            showScaleHeightAnimation.To = this.Owner.BackgroundElement.Height;
            showScaleHeightAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(160 * NavigationLayerAnimations.speedFactor));
            Storyboard.SetTarget(showScaleHeightAnimation, this.Owner.BackgroundElement);
            Storyboard.SetTargetProperty(showScaleHeightAnimation, "Height");

            storyboard.Children.Add(showElementAnimation);
            storyboard.Children.Add(showScaleWidthAnimation);
            storyboard.Children.Add(showScaleHeightAnimation);
        }

        internal void SetNavigationBackgroundElementEndAnimation(Storyboard storyboard)
        {
            DoubleAnimation showScaleWidthAnimation = new DoubleAnimation();
            showScaleWidthAnimation.EnableDependentAnimation = true;
            showScaleWidthAnimation.From = this.Owner.BackgroundElement.Width;
            showScaleWidthAnimation.To = 0;
            showScaleWidthAnimation.BeginTime = TimeSpan.FromMilliseconds(200 * NavigationLayerAnimations.speedFactor);
            showScaleWidthAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(160 * NavigationLayerAnimations.speedFactor));
            Storyboard.SetTarget(showScaleWidthAnimation, this.Owner.BackgroundElement);
            Storyboard.SetTargetProperty(showScaleWidthAnimation, "Width");

            DoubleAnimation showScaleHeightAnimation = new DoubleAnimation();
            showScaleHeightAnimation.EnableDependentAnimation = true;
            showScaleHeightAnimation.From = this.Owner.BackgroundElement.Height;
            showScaleHeightAnimation.To = 0;
            showScaleHeightAnimation.BeginTime = TimeSpan.FromMilliseconds(200 * NavigationLayerAnimations.speedFactor);
            showScaleHeightAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(160 * NavigationLayerAnimations.speedFactor));
            Storyboard.SetTarget(showScaleHeightAnimation, this.Owner.BackgroundElement);
            Storyboard.SetTargetProperty(showScaleHeightAnimation, "Height");

            storyboard.Children.Add(showScaleWidthAnimation);
            storyboard.Children.Add(showScaleHeightAnimation);
        }

        internal void SetButtonsPanelStartAnimation(Storyboard storyboard)
        {
            DoubleAnimationUsingKeyFrames showScaleWidthAnimation = new DoubleAnimationUsingKeyFrames();
            showScaleWidthAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(0), Value = 0 });
            showScaleWidthAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(110 * NavigationLayerAnimations.speedFactor), Value = 0.7 });
            showScaleWidthAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(160 * NavigationLayerAnimations.speedFactor), Value = 1 });

            Storyboard.SetTargetProperty(showScaleWidthAnimation, "(UIElement.RenderTransform).(CompositeTransform.ScaleY)");
            Storyboard.SetTarget(showScaleWidthAnimation, this.Owner.ButtonsPanel);

            DoubleAnimationUsingKeyFrames showScaleHeightAnimation = new DoubleAnimationUsingKeyFrames();
            showScaleHeightAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(0), Value = 0 });
            showScaleHeightAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(110 * NavigationLayerAnimations.speedFactor), Value = 0.7 });
            showScaleHeightAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(160 * NavigationLayerAnimations.speedFactor), Value = 1 });

            Storyboard.SetTargetProperty(showScaleHeightAnimation, "(UIElement.RenderTransform).(CompositeTransform.ScaleX)");
            Storyboard.SetTarget(showScaleHeightAnimation, this.Owner.ButtonsPanel);

            DoubleAnimationUsingKeyFrames buttonsPanelOpacityAnimation = new DoubleAnimationUsingKeyFrames();
            buttonsPanelOpacityAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(0), Value = 0 });
            buttonsPanelOpacityAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(160 * NavigationLayerAnimations.speedFactor), Value = 0 });
            buttonsPanelOpacityAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(320 * NavigationLayerAnimations.speedFactor), Value = 1 });

            Storyboard.SetTargetProperty(buttonsPanelOpacityAnimation, "Opacity");
            Storyboard.SetTarget(buttonsPanelOpacityAnimation, this.Owner.ButtonsPanel);

            DoubleAnimationUsingKeyFrames buttonsPanelStartAnimation = new DoubleAnimationUsingKeyFrames();

            buttonsPanelStartAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(0), Value = -90 });
            buttonsPanelStartAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(160 * NavigationLayerAnimations.speedFactor), Value = -90 });
            buttonsPanelStartAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(320 * NavigationLayerAnimations.speedFactor), Value = 0 });

            Storyboard.SetTargetProperty(buttonsPanelStartAnimation, "(UIElement.RenderTransform).(CompositeTransform.Rotation)");
            Storyboard.SetTarget(buttonsPanelStartAnimation, this.Owner.ButtonsPanel);

            storyboard.Children.Add(buttonsPanelStartAnimation);
            storyboard.Children.Add(buttonsPanelOpacityAnimation);

            storyboard.Children.Add(showScaleWidthAnimation);
            storyboard.Children.Add(showScaleHeightAnimation);
        }

        internal void SetButtonsPanelEndAnimation(Storyboard storyboard)
        {
            DoubleAnimationUsingKeyFrames buttonsPanelRotationAnimation = new DoubleAnimationUsingKeyFrames();

            buttonsPanelRotationAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(0), Value = 0 });
            buttonsPanelRotationAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(160 * NavigationLayerAnimations.speedFactor), Value = -90 });

            Storyboard.SetTargetProperty(buttonsPanelRotationAnimation, "(UIElement.RenderTransform).(CompositeTransform.Rotation)");
            Storyboard.SetTarget(buttonsPanelRotationAnimation, this.Owner.ButtonsPanel);

            DoubleAnimationUsingKeyFrames showScaleWidthAnimation = new DoubleAnimationUsingKeyFrames();
            showScaleWidthAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(0), Value = 1 });
            showScaleWidthAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(160 * NavigationLayerAnimations.speedFactor), Value = 0.8 });

            Storyboard.SetTargetProperty(showScaleWidthAnimation, "(UIElement.RenderTransform).(CompositeTransform.ScaleY)");
            Storyboard.SetTarget(showScaleWidthAnimation, this.Owner.ButtonsPanel);

            DoubleAnimationUsingKeyFrames showScaleHeightAnimation = new DoubleAnimationUsingKeyFrames();
            showScaleHeightAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(0), Value = 1 });
            showScaleHeightAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(160 * NavigationLayerAnimations.speedFactor), Value = 0.8 });

            Storyboard.SetTargetProperty(showScaleHeightAnimation, "(UIElement.RenderTransform).(CompositeTransform.ScaleX)");
            Storyboard.SetTarget(showScaleHeightAnimation, this.Owner.ButtonsPanel);

            DoubleAnimationUsingKeyFrames buttonsPanelOpacityAnimation = new DoubleAnimationUsingKeyFrames();
            buttonsPanelOpacityAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(0), Value = 1 });
            buttonsPanelOpacityAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(160 * NavigationLayerAnimations.speedFactor), Value = 0 });

            Storyboard.SetTargetProperty(buttonsPanelOpacityAnimation, "Opacity");
            Storyboard.SetTarget(buttonsPanelOpacityAnimation, this.Owner.ButtonsPanel);

            storyboard.Children.Add(buttonsPanelRotationAnimation);
            storyboard.Children.Add(buttonsPanelOpacityAnimation);
            storyboard.Children.Add(showScaleWidthAnimation);
            storyboard.Children.Add(showScaleHeightAnimation);
        }

        internal void SetButtonsPanelNavigateFromAnimation(Storyboard storyboard)
        {
            DoubleAnimationUsingKeyFrames scaleXAnimation = new DoubleAnimationUsingKeyFrames();
            scaleXAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0), Value = 1 });
            scaleXAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(160), Value = 0.4 });
            Storyboard.SetTargetProperty(scaleXAnimation, "(UIElement.RenderTransform).(CompositeTransform.ScaleX)");
            Storyboard.SetTarget(scaleXAnimation, this.Owner.ButtonsPanel);

            DoubleAnimationUsingKeyFrames scaleYAnimation = new DoubleAnimationUsingKeyFrames();
            scaleYAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0), Value = 1 });
            scaleYAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(160), Value = 0.4 });

            Storyboard.SetTargetProperty(scaleYAnimation, "(UIElement.RenderTransform).(CompositeTransform.ScaleY)");
            Storyboard.SetTarget(scaleYAnimation, this.Owner.ButtonsPanel);

            storyboard.Children.Add(scaleXAnimation);
            storyboard.Children.Add(scaleYAnimation);
        }

        internal void SetButtonsPanelNavigateToAnimation(Storyboard storyboard)
        {
            DoubleAnimationUsingKeyFrames scaleXAnimation = new DoubleAnimationUsingKeyFrames();
            scaleXAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0), Value = 0.4 });
            scaleXAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(160), Value = 1 });
            Storyboard.SetTargetProperty(scaleXAnimation, "(UIElement.RenderTransform).(CompositeTransform.ScaleX)");
            Storyboard.SetTarget(scaleXAnimation, this.Owner.ButtonsPanel);

            DoubleAnimationUsingKeyFrames scaleYAnimation = new DoubleAnimationUsingKeyFrames();
            scaleYAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0), Value = 0.4 });
            scaleYAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(160), Value = 1 });

            Storyboard.SetTargetProperty(scaleYAnimation, "(UIElement.RenderTransform).(CompositeTransform.ScaleY)");
            Storyboard.SetTarget(scaleYAnimation, this.Owner.ButtonsPanel);

            storyboard.Children.Add(scaleXAnimation);
            storyboard.Children.Add(scaleYAnimation);
        }
    }
}
