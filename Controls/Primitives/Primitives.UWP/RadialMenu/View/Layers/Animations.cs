using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace Telerik.UI.Xaml.Controls.Primitives.Menu
{
    internal class Animations
    {
        internal static double speedFactor = 1;

        internal static void SetContentItemsPanelStartAnimation(Storyboard storyboard, Panel owner)
        {
            ObjectAnimationUsingKeyFrames showElementAnimation = new ObjectAnimationUsingKeyFrames();
            showElementAnimation.KeyFrames.Add(new DiscreteObjectKeyFrame { KeyTime = TimeSpan.Zero, Value = Visibility.Visible });

            Storyboard.SetTargetProperty(showElementAnimation, "(UIElement.Visibility)");
            Storyboard.SetTarget(showElementAnimation, owner);

            DoubleAnimationUsingKeyFrames itemsPanelOpacityAnimation = new DoubleAnimationUsingKeyFrames() { EnableDependentAnimation = RadControl.IsInTestMode };
            itemsPanelOpacityAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(0), Value = 0 });
            itemsPanelOpacityAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(300 * speedFactor), Value = 0 });
            itemsPanelOpacityAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(300 * speedFactor), Value = 1 });

            Storyboard.SetTargetProperty(itemsPanelOpacityAnimation, "Opacity");
            Storyboard.SetTarget(itemsPanelOpacityAnimation, owner);

            storyboard.Children.Add(itemsPanelOpacityAnimation);
            storyboard.Children.Add(showElementAnimation);
        }

        internal static void SetContentItemsPanelEndAnimation(Storyboard storyboard, Panel owner)
        {
            DoubleAnimationUsingKeyFrames itemsPanelOpacityAnimation = new DoubleAnimationUsingKeyFrames() { EnableDependentAnimation = RadControl.IsInTestMode };
            itemsPanelOpacityAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(0), Value = 1 });
            itemsPanelOpacityAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(160 * speedFactor), Value = 0 });

            Storyboard.SetTargetProperty(itemsPanelOpacityAnimation, "Opacity");
            Storyboard.SetTarget(itemsPanelOpacityAnimation, owner);

            storyboard.Children.Add(itemsPanelOpacityAnimation);
        }

        internal static void SetContentItemsPanelNavigateToAnimation(Storyboard storyboard, Panel owner)
        {
            DoubleAnimationUsingKeyFrames itemsPanelOpacityAnimation = new DoubleAnimationUsingKeyFrames() { EnableDependentAnimation = RadControl.IsInTestMode };
            itemsPanelOpacityAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(0), Value = 0 });
            itemsPanelOpacityAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(160 * speedFactor), Value = 1 });

            Storyboard.SetTargetProperty(itemsPanelOpacityAnimation, "Opacity");
            Storyboard.SetTarget(itemsPanelOpacityAnimation, owner);

            storyboard.Children.Add(itemsPanelOpacityAnimation);
        }

        internal static void SetContentItemsPanelNavigateFromAnimation(Storyboard storyboard, Panel owner)
        {
            ObjectAnimationUsingKeyFrames navigateFromElementAnimation = new ObjectAnimationUsingKeyFrames();
            navigateFromElementAnimation.KeyFrames.Add(new DiscreteObjectKeyFrame { KeyTime = TimeSpan.Zero, Value = Visibility.Visible });

            Storyboard.SetTargetProperty(navigateFromElementAnimation, "(UIElement.Visibility)");
            Storyboard.SetTarget(navigateFromElementAnimation, owner);

            DoubleAnimationUsingKeyFrames itemsPanelOpacityAnimation = new DoubleAnimationUsingKeyFrames() { EnableDependentAnimation = RadControl.IsInTestMode };
            itemsPanelOpacityAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(0), Value = 1 });
            itemsPanelOpacityAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(160 * speedFactor), Value = 0 });

            Storyboard.SetTargetProperty(itemsPanelOpacityAnimation, "Opacity");
            Storyboard.SetTarget(itemsPanelOpacityAnimation, owner);

            storyboard.Children.Add(itemsPanelOpacityAnimation);
            storyboard.Children.Add(navigateFromElementAnimation);
        }

        internal static void SetStateButtonsPanelStartAnimation(Storyboard storyboard, Panel owner)
        {
            DoubleAnimationUsingKeyFrames showScaleWidthAnimation = new DoubleAnimationUsingKeyFrames() { EnableDependentAnimation = RadControl.IsInTestMode };
            showScaleWidthAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(0), Value = 0 });
            showScaleWidthAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(110 * speedFactor), Value = 0.8 });
            showScaleWidthAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(160 * speedFactor), Value = 1 });

            Storyboard.SetTargetProperty(showScaleWidthAnimation, "(UIElement.RenderTransform).(CompositeTransform.ScaleY)");
            Storyboard.SetTarget(showScaleWidthAnimation, owner);

            DoubleAnimationUsingKeyFrames showScaleHeightAnimation = new DoubleAnimationUsingKeyFrames() { EnableDependentAnimation = RadControl.IsInTestMode };
            showScaleHeightAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(0), Value = 0 });
            showScaleHeightAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(110 * speedFactor), Value = 0.8 });
            showScaleHeightAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(160 * speedFactor), Value = 1 });

            Storyboard.SetTargetProperty(showScaleHeightAnimation, "(UIElement.RenderTransform).(CompositeTransform.ScaleX)");
            Storyboard.SetTarget(showScaleHeightAnimation, owner);

            DoubleAnimationUsingKeyFrames stateButtonsPanelRotateAnimation = new DoubleAnimationUsingKeyFrames() { EnableDependentAnimation = RadControl.IsInTestMode };
            stateButtonsPanelRotateAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(0), Value = -45 });
            stateButtonsPanelRotateAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(160 * speedFactor), Value = -45 });
            stateButtonsPanelRotateAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(320 * speedFactor), Value = 0 });

            Storyboard.SetTargetProperty(stateButtonsPanelRotateAnimation, "(UIElement.RenderTransform).(CompositeTransform.Rotation)");
            Storyboard.SetTarget(stateButtonsPanelRotateAnimation, owner);

            DoubleAnimationUsingKeyFrames stateButtonsPanelOpacityAnimation = new DoubleAnimationUsingKeyFrames() { EnableDependentAnimation = RadControl.IsInTestMode };
            stateButtonsPanelOpacityAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(0), Value = 0 });
            stateButtonsPanelOpacityAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(160 * speedFactor), Value = 0 });
            stateButtonsPanelOpacityAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(320 * speedFactor), Value = 1 });

            Storyboard.SetTargetProperty(stateButtonsPanelOpacityAnimation, "Opacity");
            Storyboard.SetTarget(stateButtonsPanelOpacityAnimation, owner);

            storyboard.Children.Add(stateButtonsPanelOpacityAnimation);
            storyboard.Children.Add(stateButtonsPanelRotateAnimation);

            storyboard.Children.Add(showScaleWidthAnimation);
            storyboard.Children.Add(showScaleHeightAnimation);
        }

        internal static void SetStateButtonsPanelEndAnimation(Storyboard storyboard, Panel owner)
        {
            DoubleAnimationUsingKeyFrames showScaleWidthAnimation = new DoubleAnimationUsingKeyFrames() { EnableDependentAnimation = RadControl.IsInTestMode };
            showScaleWidthAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(0), Value = 1 });
            showScaleWidthAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(160 * speedFactor), Value = 0.8 });

            Storyboard.SetTargetProperty(showScaleWidthAnimation, "(UIElement.RenderTransform).(CompositeTransform.ScaleY)");
            Storyboard.SetTarget(showScaleWidthAnimation, owner);

            DoubleAnimationUsingKeyFrames showScaleHeightAnimation = new DoubleAnimationUsingKeyFrames() { EnableDependentAnimation = RadControl.IsInTestMode };
            showScaleHeightAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(0), Value = 1 });
            showScaleHeightAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(160 * speedFactor), Value = 0.8 });

            Storyboard.SetTargetProperty(showScaleHeightAnimation, "(UIElement.RenderTransform).(CompositeTransform.ScaleX)");
            Storyboard.SetTarget(showScaleHeightAnimation, owner);

            DoubleAnimationUsingKeyFrames stateButtonsPanelRotateAnimation = new DoubleAnimationUsingKeyFrames() { EnableDependentAnimation = RadControl.IsInTestMode };
            stateButtonsPanelRotateAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(0), Value = 0 });
            stateButtonsPanelRotateAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(160 * speedFactor), Value = -45 });

            Storyboard.SetTargetProperty(stateButtonsPanelRotateAnimation, "(UIElement.RenderTransform).(CompositeTransform.Rotation)");
            Storyboard.SetTarget(stateButtonsPanelRotateAnimation, owner);

            DoubleAnimationUsingKeyFrames stateButtonsPanelOpacityAnimation = new DoubleAnimationUsingKeyFrames() { EnableDependentAnimation = RadControl.IsInTestMode };
            stateButtonsPanelOpacityAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(0), Value = 1 });
            stateButtonsPanelOpacityAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(160 * speedFactor), Value = 0 });

            Storyboard.SetTargetProperty(stateButtonsPanelOpacityAnimation, "Opacity");
            Storyboard.SetTarget(stateButtonsPanelOpacityAnimation, owner);

            storyboard.Children.Add(stateButtonsPanelOpacityAnimation);
            storyboard.Children.Add(stateButtonsPanelRotateAnimation);

            storyboard.Children.Add(showScaleHeightAnimation);
            storyboard.Children.Add(showScaleWidthAnimation);
        }

        internal static void SetStateButtonsPanelNavigateFromAnimation(Storyboard storyboard, Panel owner)
        {
            DoubleAnimationUsingKeyFrames showScaleWidthAnimation = new DoubleAnimationUsingKeyFrames() { EnableDependentAnimation = RadControl.IsInTestMode };
            showScaleWidthAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(0), Value = 1 });
            showScaleWidthAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(160 * speedFactor), Value = 0.8 });

            Storyboard.SetTargetProperty(showScaleWidthAnimation, "(UIElement.RenderTransform).(CompositeTransform.ScaleY)");
            Storyboard.SetTarget(showScaleWidthAnimation, owner);

            DoubleAnimationUsingKeyFrames showScaleHeightAnimation = new DoubleAnimationUsingKeyFrames() { EnableDependentAnimation = RadControl.IsInTestMode };
            showScaleHeightAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(0), Value = 1 });
            showScaleHeightAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(160 * speedFactor), Value = 0.8 });

            Storyboard.SetTargetProperty(showScaleHeightAnimation, "(UIElement.RenderTransform).(CompositeTransform.ScaleX)");
            Storyboard.SetTarget(showScaleHeightAnimation, owner);

            DoubleAnimationUsingKeyFrames stateButtonsOpacityAnimation = new DoubleAnimationUsingKeyFrames() { EnableDependentAnimation = RadControl.IsInTestMode };
            stateButtonsOpacityAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(0), Value = 1 });
            stateButtonsOpacityAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(160 * speedFactor), Value = 0 });

            Storyboard.SetTargetProperty(stateButtonsOpacityAnimation, "Opacity");
            Storyboard.SetTarget(stateButtonsOpacityAnimation, owner);

            storyboard.Children.Add(stateButtonsOpacityAnimation);
            storyboard.Children.Add(showScaleWidthAnimation);
            storyboard.Children.Add(showScaleHeightAnimation);
        }

        internal static void SetStateButtonsPanelNavigateToAnimation(Storyboard storyboard, Panel owner)
        {
            DoubleAnimationUsingKeyFrames showScaleWidthAnimation = new DoubleAnimationUsingKeyFrames() { EnableDependentAnimation = RadControl.IsInTestMode };
            showScaleWidthAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(0), Value = 0.8 });
            showScaleWidthAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(160 * speedFactor), Value = 1 });

            Storyboard.SetTargetProperty(showScaleWidthAnimation, "(UIElement.RenderTransform).(CompositeTransform.ScaleY)");
            Storyboard.SetTarget(showScaleWidthAnimation, owner);

            DoubleAnimationUsingKeyFrames showScaleHeightAnimation = new DoubleAnimationUsingKeyFrames() { EnableDependentAnimation = RadControl.IsInTestMode };
            showScaleHeightAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(0), Value = 0.8 });
            showScaleHeightAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(160 * speedFactor), Value = 1 });

            Storyboard.SetTargetProperty(showScaleHeightAnimation, "(UIElement.RenderTransform).(CompositeTransform.ScaleX)");
            Storyboard.SetTarget(showScaleHeightAnimation, owner);

            DoubleAnimationUsingKeyFrames stateButtonsOpacityAnimation = new DoubleAnimationUsingKeyFrames() { EnableDependentAnimation = RadControl.IsInTestMode };
            stateButtonsOpacityAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(0), Value = 0 });
            stateButtonsOpacityAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(30 * speedFactor), Value = 0 });
            stateButtonsOpacityAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(190 * speedFactor), Value = 1 });

            Storyboard.SetTargetProperty(stateButtonsOpacityAnimation, "Opacity");
            Storyboard.SetTarget(stateButtonsOpacityAnimation, owner);

            storyboard.Children.Add(stateButtonsOpacityAnimation);
            storyboard.Children.Add(showScaleWidthAnimation);
            storyboard.Children.Add(showScaleHeightAnimation);
        }

        internal static void SetMenuBackgroundElementStartAnimation(Storyboard storyboard, FrameworkElement owner)
        {
            ObjectAnimationUsingKeyFrames showElementAnimation = new ObjectAnimationUsingKeyFrames();
            showElementAnimation.KeyFrames.Add(new DiscreteObjectKeyFrame { KeyTime = TimeSpan.Zero, Value = Visibility.Visible });

            Storyboard.SetTargetProperty(showElementAnimation, "(UIElement.Visibility)");
            Storyboard.SetTarget(showElementAnimation, owner);

            DoubleAnimation showScaleWidthAnimation = new DoubleAnimation();
            showScaleWidthAnimation.EnableDependentAnimation = true;
            showScaleWidthAnimation.From = 0;
            showScaleWidthAnimation.To = owner.Width;
            showScaleWidthAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(160 * speedFactor));
            Storyboard.SetTarget(showScaleWidthAnimation, owner);
            Storyboard.SetTargetProperty(showScaleWidthAnimation, "Width");

            DoubleAnimation showScaleHeightAnimation = new DoubleAnimation();
            showScaleHeightAnimation.EnableDependentAnimation = true;
            showScaleHeightAnimation.From = 0;
            showScaleHeightAnimation.To = owner.Height;
            showScaleHeightAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(160 * speedFactor));
            Storyboard.SetTarget(showScaleHeightAnimation, owner);
            Storyboard.SetTargetProperty(showScaleHeightAnimation, "Height");

            storyboard.Children.Add(showElementAnimation);
            storyboard.Children.Add(showScaleWidthAnimation);
            storyboard.Children.Add(showScaleHeightAnimation);
        }

        internal static void SetMenuBackgroundElementEndAnimation(Storyboard storyboard, FrameworkElement owner)
        {
            DoubleAnimation showScaleWidthAnimation = new DoubleAnimation() { EnableDependentAnimation = RadControl.IsInTestMode };
            showScaleWidthAnimation.EnableDependentAnimation = true;
            showScaleWidthAnimation.From = owner.Width;
            showScaleWidthAnimation.To = 0;
            showScaleWidthAnimation.BeginTime = TimeSpan.FromMilliseconds(200 * speedFactor);
            showScaleWidthAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(160 * speedFactor));
            Storyboard.SetTarget(showScaleWidthAnimation, owner);
            Storyboard.SetTargetProperty(showScaleWidthAnimation, "Width");

            DoubleAnimation showScaleHeightAnimation = new DoubleAnimation() { EnableDependentAnimation = RadControl.IsInTestMode };
            showScaleHeightAnimation.EnableDependentAnimation = true;
            showScaleHeightAnimation.From = owner.Height;
            showScaleHeightAnimation.To = 0;
            showScaleHeightAnimation.BeginTime = TimeSpan.FromMilliseconds(200 * speedFactor);
            showScaleHeightAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(160 * speedFactor));
            Storyboard.SetTarget(showScaleHeightAnimation, owner);
            Storyboard.SetTargetProperty(showScaleHeightAnimation, "Height");

            ObjectAnimationUsingKeyFrames collapseAnimation = new ObjectAnimationUsingKeyFrames() { EnableDependentAnimation = RadControl.IsInTestMode };
            Storyboard.SetTarget(collapseAnimation, owner);
            Storyboard.SetTargetProperty(collapseAnimation, "Visibility");
            collapseAnimation.KeyFrames.Add(new DiscreteObjectKeyFrame { Value = Visibility.Collapsed, KeyTime = showScaleWidthAnimation.Duration.TimeSpan + showScaleWidthAnimation.BeginTime.Value });

            storyboard.Children.Add(showScaleWidthAnimation);
            storyboard.Children.Add(showScaleHeightAnimation);
            storyboard.Children.Add(collapseAnimation);
        }

        internal static void SetMenuBackgroundElementNavigateFromAnimation(Storyboard storyboard, FrameworkElement owner)
        {
            DoubleAnimation navigateScaleWidthAnimation = new DoubleAnimation();
            navigateScaleWidthAnimation.EnableDependentAnimation = true;
            navigateScaleWidthAnimation.From = owner.Width;
            navigateScaleWidthAnimation.To = owner.Width - (owner.Width / 4);
            navigateScaleWidthAnimation.BeginTime = TimeSpan.FromMilliseconds(30 * speedFactor);
            navigateScaleWidthAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(120 * speedFactor));
            Storyboard.SetTarget(navigateScaleWidthAnimation, owner);
            Storyboard.SetTargetProperty(navigateScaleWidthAnimation, "Width");

            DoubleAnimation navigateScaleHeightAnimation = new DoubleAnimation();
            navigateScaleHeightAnimation.EnableDependentAnimation = true;
            navigateScaleHeightAnimation.From = owner.Height;
            navigateScaleHeightAnimation.To = owner.Height - (owner.Height / 4);
            navigateScaleHeightAnimation.BeginTime = TimeSpan.FromMilliseconds(30 * speedFactor);
            navigateScaleHeightAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(120 * speedFactor));
            Storyboard.SetTarget(navigateScaleHeightAnimation, owner);
            Storyboard.SetTargetProperty(navigateScaleHeightAnimation, "Height");

            storyboard.Children.Add(navigateScaleWidthAnimation);
            storyboard.Children.Add(navigateScaleHeightAnimation);
        }

        internal static void SetMenuBackgroundElementNavigateToAnimation(Storyboard storyboard, FrameworkElement owner)
        {
            DoubleAnimation navigateScaleWidthAnimation = new DoubleAnimation();
            navigateScaleWidthAnimation.EnableDependentAnimation = true;
            navigateScaleWidthAnimation.From = owner.Width - (owner.Width / 4);
            navigateScaleWidthAnimation.To = owner.Width;
            navigateScaleWidthAnimation.BeginTime = TimeSpan.FromMilliseconds(0);
            navigateScaleWidthAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(120 * speedFactor));
            Storyboard.SetTarget(navigateScaleWidthAnimation, owner);
            Storyboard.SetTargetProperty(navigateScaleWidthAnimation, "Width");

            DoubleAnimation navigateScaleHeightAnimation = new DoubleAnimation();
            navigateScaleHeightAnimation.EnableDependentAnimation = true;
            navigateScaleHeightAnimation.From = owner.Height - (owner.Height / 4);
            navigateScaleHeightAnimation.To = owner.Height;
            navigateScaleHeightAnimation.BeginTime = TimeSpan.FromMilliseconds(0);
            navigateScaleHeightAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(120 * speedFactor));
            Storyboard.SetTarget(navigateScaleHeightAnimation, owner);
            Storyboard.SetTargetProperty(navigateScaleHeightAnimation, "Height");

            storyboard.Children.Add(navigateScaleWidthAnimation);
            storyboard.Children.Add(navigateScaleHeightAnimation);
        }

        ////SectroBackgroundLayer

        internal static void SetSectorElementsStartAnimation(Storyboard storyboard, FrameworkElement owner)
        {
            DoubleAnimationUsingKeyFrames showScaleWidthAnimation = new DoubleAnimationUsingKeyFrames() { EnableDependentAnimation = RadControl.IsInTestMode };
            showScaleWidthAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(0), Value = 0 });
            showScaleWidthAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(120 * speedFactor), Value = 0.7 });
            showScaleWidthAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(320 * speedFactor), Value = 1 });

            Storyboard.SetTargetProperty(showScaleWidthAnimation, "(UIElement.RenderTransform).(CompositeTransform.ScaleY)");
            Storyboard.SetTarget(showScaleWidthAnimation, owner);

            DoubleAnimationUsingKeyFrames showScaleHeightAnimation = new DoubleAnimationUsingKeyFrames() { EnableDependentAnimation = RadControl.IsInTestMode };
            showScaleHeightAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(0), Value = 0 });
            showScaleHeightAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(120 * speedFactor), Value = 0.7 });
            showScaleHeightAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(320 * speedFactor), Value = 1 });

            Storyboard.SetTargetProperty(showScaleHeightAnimation, "(UIElement.RenderTransform).(CompositeTransform.ScaleX)");
            Storyboard.SetTarget(showScaleHeightAnimation, owner);

            DoubleAnimationUsingKeyFrames buttonsPanelOpacityAnimation = new DoubleAnimationUsingKeyFrames() { EnableDependentAnimation = RadControl.IsInTestMode };
            buttonsPanelOpacityAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(0), Value = 0 });
            buttonsPanelOpacityAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(120 * speedFactor), Value = 0 });
            buttonsPanelOpacityAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(320 * speedFactor), Value = 1 });

            Storyboard.SetTargetProperty(buttonsPanelOpacityAnimation, "Opacity");
            Storyboard.SetTarget(buttonsPanelOpacityAnimation, owner);

            DoubleAnimationUsingKeyFrames buttonsPanelStartAnimation = new DoubleAnimationUsingKeyFrames() { EnableDependentAnimation = RadControl.IsInTestMode };

            buttonsPanelStartAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(0), Value = -45 });
            buttonsPanelStartAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(120 * speedFactor), Value = -45 });
            buttonsPanelStartAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(320 * speedFactor), Value = 0 });

            Storyboard.SetTargetProperty(buttonsPanelStartAnimation, "(UIElement.RenderTransform).(CompositeTransform.Rotation)");
            Storyboard.SetTarget(buttonsPanelStartAnimation, owner);

            storyboard.Children.Add(buttonsPanelStartAnimation);
            storyboard.Children.Add(buttonsPanelOpacityAnimation);

            storyboard.Children.Add(showScaleWidthAnimation);
            storyboard.Children.Add(showScaleHeightAnimation);
        }

        internal static void SetSectorElementsEndAnimation(Storyboard storyboard, FrameworkElement owner)
        {
            DoubleAnimationUsingKeyFrames buttonsPanelOpacityAnimation = new DoubleAnimationUsingKeyFrames() { EnableDependentAnimation = RadControl.IsInTestMode };
            buttonsPanelOpacityAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(0), Value = 1 });
            buttonsPanelOpacityAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(160 * speedFactor), Value = 0 });

            Storyboard.SetTargetProperty(buttonsPanelOpacityAnimation, "Opacity");
            Storyboard.SetTarget(buttonsPanelOpacityAnimation, owner);

            DoubleAnimationUsingKeyFrames buttonsPanelStartAnimation = new DoubleAnimationUsingKeyFrames() { EnableDependentAnimation = RadControl.IsInTestMode };

            buttonsPanelStartAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(0), Value = 0 });
            buttonsPanelStartAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(160 * speedFactor), Value = -45 });

            Storyboard.SetTargetProperty(buttonsPanelStartAnimation, "(UIElement.RenderTransform).(CompositeTransform.Rotation)");
            Storyboard.SetTarget(buttonsPanelStartAnimation, owner);

            storyboard.Children.Add(buttonsPanelStartAnimation);
            storyboard.Children.Add(buttonsPanelOpacityAnimation);
        }

        internal static void SetSectorElementsNavigateFromAnimation(Storyboard storyboard, FrameworkElement owner)
        {
            DoubleAnimationUsingKeyFrames navigateScaleWidthAnimation = new DoubleAnimationUsingKeyFrames() { EnableDependentAnimation = RadControl.IsInTestMode };
            navigateScaleWidthAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(0), Value = 1 });
            navigateScaleWidthAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(120 * speedFactor), Value = 0.4 });

            Storyboard.SetTargetProperty(navigateScaleWidthAnimation, "(UIElement.RenderTransform).(CompositeTransform.ScaleY)");
            Storyboard.SetTarget(navigateScaleWidthAnimation, owner);

            DoubleAnimationUsingKeyFrames navigateScaleHeightAnimation = new DoubleAnimationUsingKeyFrames() { EnableDependentAnimation = RadControl.IsInTestMode };
            navigateScaleHeightAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(0), Value = 1 });
            navigateScaleHeightAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(120 * speedFactor), Value = 0.4 });

            Storyboard.SetTargetProperty(navigateScaleHeightAnimation, "(UIElement.RenderTransform).(CompositeTransform.ScaleX)");
            Storyboard.SetTarget(navigateScaleHeightAnimation, owner);

            DoubleAnimationUsingKeyFrames sectorsPanelOpacityAnimation = new DoubleAnimationUsingKeyFrames() { EnableDependentAnimation = RadControl.IsInTestMode };
            sectorsPanelOpacityAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(0), Value = 1 });
            sectorsPanelOpacityAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(120 * speedFactor), Value = 0 });

            Storyboard.SetTargetProperty(sectorsPanelOpacityAnimation, "Opacity");
            Storyboard.SetTarget(sectorsPanelOpacityAnimation, owner);

            storyboard.Children.Add(sectorsPanelOpacityAnimation);
            storyboard.Children.Add(navigateScaleWidthAnimation);
            storyboard.Children.Add(navigateScaleHeightAnimation);
        }

        internal static void SetSectorElementsNavigateToAnimation(Storyboard storyboard, FrameworkElement owner)
        {
            DoubleAnimationUsingKeyFrames navigateScaleWidthAnimation = new DoubleAnimationUsingKeyFrames() { EnableDependentAnimation = RadControl.IsInTestMode };
            navigateScaleWidthAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(0), Value = 0.4 });
            navigateScaleWidthAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(120 * speedFactor), Value = 0.4 });
            navigateScaleWidthAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(240 * speedFactor), Value = 1 });

            Storyboard.SetTargetProperty(navigateScaleWidthAnimation, "(UIElement.RenderTransform).(CompositeTransform.ScaleY)");
            Storyboard.SetTarget(navigateScaleWidthAnimation, owner);

            DoubleAnimationUsingKeyFrames navigateScaleHeightAnimation = new DoubleAnimationUsingKeyFrames() { EnableDependentAnimation = RadControl.IsInTestMode };
            navigateScaleHeightAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(0), Value = 0.4 });
            navigateScaleHeightAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(120 * speedFactor), Value = 0.4 });
            navigateScaleHeightAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(240 * speedFactor), Value = 1 });

            Storyboard.SetTargetProperty(navigateScaleHeightAnimation, "(UIElement.RenderTransform).(CompositeTransform.ScaleX)");
            Storyboard.SetTarget(navigateScaleHeightAnimation, owner);

            DoubleAnimationUsingKeyFrames sectorsPanelOpacityAnimation = new DoubleAnimationUsingKeyFrames() { EnableDependentAnimation = RadControl.IsInTestMode };
            sectorsPanelOpacityAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(0), Value = 0 });
            sectorsPanelOpacityAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(120 * speedFactor), Value = 0 });
            sectorsPanelOpacityAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(240 * speedFactor), Value = 1 });

            Storyboard.SetTargetProperty(sectorsPanelOpacityAnimation, "Opacity");
            Storyboard.SetTarget(sectorsPanelOpacityAnimation, owner);

            storyboard.Children.Add(sectorsPanelOpacityAnimation);
            storyboard.Children.Add(navigateScaleWidthAnimation);
            storyboard.Children.Add(navigateScaleHeightAnimation);
        }

        ////Navigation layer animations

        internal static void SetNavigationBackgroundElementStartAnimation(Storyboard storyboard, FrameworkElement owner)
        {
            ObjectAnimationUsingKeyFrames showElementAnimation = new ObjectAnimationUsingKeyFrames();
            showElementAnimation.KeyFrames.Add(new DiscreteObjectKeyFrame { KeyTime = TimeSpan.Zero, Value = Visibility.Visible });

            Storyboard.SetTargetProperty(showElementAnimation, "(UIElement.Visibility)");
            Storyboard.SetTarget(showElementAnimation, owner);

            DoubleAnimation showScaleWidthAnimation = new DoubleAnimation();
            showScaleWidthAnimation.EnableDependentAnimation = true;
            showScaleWidthAnimation.From = 0;
            showScaleWidthAnimation.To = owner.Width;
            showScaleWidthAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(160 * speedFactor));
            Storyboard.SetTarget(showScaleWidthAnimation, owner);
            Storyboard.SetTargetProperty(showScaleWidthAnimation, "Width");

            DoubleAnimation showScaleHeightAnimation = new DoubleAnimation();
            showScaleHeightAnimation.EnableDependentAnimation = true;
            showScaleHeightAnimation.From = 0;
            showScaleHeightAnimation.To = owner.Height;
            showScaleHeightAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(160 * speedFactor));
            Storyboard.SetTarget(showScaleHeightAnimation, owner);
            Storyboard.SetTargetProperty(showScaleHeightAnimation, "Height");

            storyboard.Children.Add(showElementAnimation);
            storyboard.Children.Add(showScaleWidthAnimation);
            storyboard.Children.Add(showScaleHeightAnimation);
        }

        internal static void SetNavigationBackgroundElementEndAnimation(Storyboard storyboard, FrameworkElement owner)
        {
            DoubleAnimation showScaleWidthAnimation = new DoubleAnimation();
            showScaleWidthAnimation.EnableDependentAnimation = true;
            showScaleWidthAnimation.From = owner.Width;
            showScaleWidthAnimation.To = 0;
            showScaleWidthAnimation.BeginTime = TimeSpan.FromMilliseconds(200 * speedFactor);
            showScaleWidthAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(160 * speedFactor));
            Storyboard.SetTarget(showScaleWidthAnimation, owner);
            Storyboard.SetTargetProperty(showScaleWidthAnimation, "Width");

            DoubleAnimation showScaleHeightAnimation = new DoubleAnimation();
            showScaleHeightAnimation.EnableDependentAnimation = true;
            showScaleHeightAnimation.From = owner.Height;
            showScaleHeightAnimation.To = 0;
            showScaleHeightAnimation.BeginTime = TimeSpan.FromMilliseconds(200 * speedFactor);
            showScaleHeightAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(160 * speedFactor));
            Storyboard.SetTarget(showScaleHeightAnimation, owner);
            Storyboard.SetTargetProperty(showScaleHeightAnimation, "Height");

            ObjectAnimationUsingKeyFrames collapseAnimation = new ObjectAnimationUsingKeyFrames();
            Storyboard.SetTarget(collapseAnimation, owner);
            Storyboard.SetTargetProperty(collapseAnimation, "Visibility");
            collapseAnimation.KeyFrames.Add(new DiscreteObjectKeyFrame { Value = Visibility.Collapsed, KeyTime = showScaleWidthAnimation.Duration.TimeSpan + showScaleWidthAnimation.BeginTime.Value });

            storyboard.Children.Add(showScaleWidthAnimation);
            storyboard.Children.Add(showScaleHeightAnimation);
            storyboard.Children.Add(collapseAnimation);
        }

        internal static void SetNavigateButtonsPanelStartAnimation(Storyboard storyboard, Panel owner)
        {
            DoubleAnimationUsingKeyFrames showScaleWidthAnimation = new DoubleAnimationUsingKeyFrames() { EnableDependentAnimation = RadControl.IsInTestMode };
            showScaleWidthAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(0), Value = 0 });
            showScaleWidthAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(110 * speedFactor), Value = 0.7 });
            showScaleWidthAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(160 * speedFactor), Value = 1 });

            Storyboard.SetTargetProperty(showScaleWidthAnimation, "(UIElement.RenderTransform).(CompositeTransform.ScaleY)");
            Storyboard.SetTarget(showScaleWidthAnimation, owner);

            DoubleAnimationUsingKeyFrames showScaleHeightAnimation = new DoubleAnimationUsingKeyFrames() { EnableDependentAnimation = RadControl.IsInTestMode };
            showScaleHeightAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(0), Value = 0 });
            showScaleHeightAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(110 * speedFactor), Value = 0.7 });
            showScaleHeightAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(160 * speedFactor), Value = 1 });

            Storyboard.SetTargetProperty(showScaleHeightAnimation, "(UIElement.RenderTransform).(CompositeTransform.ScaleX)");
            Storyboard.SetTarget(showScaleHeightAnimation, owner);

            DoubleAnimationUsingKeyFrames buttonsPanelOpacityAnimation = new DoubleAnimationUsingKeyFrames() { EnableDependentAnimation = RadControl.IsInTestMode };
            buttonsPanelOpacityAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(0), Value = 0 });
            buttonsPanelOpacityAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(160 * speedFactor), Value = 0 });
            buttonsPanelOpacityAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(320 * speedFactor), Value = 1 });

            Storyboard.SetTargetProperty(buttonsPanelOpacityAnimation, "Opacity");
            Storyboard.SetTarget(buttonsPanelOpacityAnimation, owner);

            DoubleAnimationUsingKeyFrames buttonsPanelStartAnimation = new DoubleAnimationUsingKeyFrames() { EnableDependentAnimation = RadControl.IsInTestMode };

            buttonsPanelStartAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(0), Value = -45 });
            buttonsPanelStartAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(160 * speedFactor), Value = -45 });
            buttonsPanelStartAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(320 * speedFactor), Value = 0 });

            Storyboard.SetTargetProperty(buttonsPanelStartAnimation, "(UIElement.RenderTransform).(CompositeTransform.Rotation)");
            Storyboard.SetTarget(buttonsPanelStartAnimation, owner);

            storyboard.Children.Add(buttonsPanelStartAnimation);
            storyboard.Children.Add(buttonsPanelOpacityAnimation);

            storyboard.Children.Add(showScaleWidthAnimation);
            storyboard.Children.Add(showScaleHeightAnimation);
        }

        internal static void SetNavigateButtonsPanelEndAnimation(Storyboard storyboard, Panel owner)
        {
            DoubleAnimationUsingKeyFrames buttonsPanelRotationAnimation = new DoubleAnimationUsingKeyFrames() { EnableDependentAnimation = RadControl.IsInTestMode };

            buttonsPanelRotationAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(0), Value = 0 });
            buttonsPanelRotationAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(160 * speedFactor), Value = -45 });

            Storyboard.SetTargetProperty(buttonsPanelRotationAnimation, "(UIElement.RenderTransform).(CompositeTransform.Rotation)");
            Storyboard.SetTarget(buttonsPanelRotationAnimation, owner);

            DoubleAnimationUsingKeyFrames showScaleWidthAnimation = new DoubleAnimationUsingKeyFrames() { EnableDependentAnimation = RadControl.IsInTestMode };
            showScaleWidthAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(0), Value = 1 });
            showScaleWidthAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(160 * speedFactor), Value = 0.9 });

            Storyboard.SetTargetProperty(showScaleWidthAnimation, "(UIElement.RenderTransform).(CompositeTransform.ScaleY)");
            Storyboard.SetTarget(showScaleWidthAnimation, owner);

            DoubleAnimationUsingKeyFrames showScaleHeightAnimation = new DoubleAnimationUsingKeyFrames() { EnableDependentAnimation = RadControl.IsInTestMode };
            showScaleHeightAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(0), Value = 1 });
            showScaleHeightAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(160 * speedFactor), Value = 0.9 });

            Storyboard.SetTargetProperty(showScaleHeightAnimation, "(UIElement.RenderTransform).(CompositeTransform.ScaleX)");
            Storyboard.SetTarget(showScaleHeightAnimation, owner);

            DoubleAnimationUsingKeyFrames buttonsPanelOpacityAnimation = new DoubleAnimationUsingKeyFrames() { EnableDependentAnimation = RadControl.IsInTestMode };
            buttonsPanelOpacityAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(0), Value = 1 });
            buttonsPanelOpacityAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(120 * speedFactor), Value = 0 });

            Storyboard.SetTargetProperty(buttonsPanelOpacityAnimation, "Opacity");
            Storyboard.SetTarget(buttonsPanelOpacityAnimation, owner);

            storyboard.Children.Add(buttonsPanelRotationAnimation);
            storyboard.Children.Add(buttonsPanelOpacityAnimation);
            storyboard.Children.Add(showScaleWidthAnimation);
            storyboard.Children.Add(showScaleHeightAnimation);
        }

        internal static void SetNavigateButtonsPanelNavigateFromAnimation(Storyboard storyboard, Panel owner)
        {
            DoubleAnimationUsingKeyFrames scaleXAnimation = new DoubleAnimationUsingKeyFrames() { EnableDependentAnimation = RadControl.IsInTestMode };
            scaleXAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0), Value = 1 });
            scaleXAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(160 * speedFactor), Value = 0.6 });
            Storyboard.SetTargetProperty(scaleXAnimation, "(UIElement.RenderTransform).(CompositeTransform.ScaleX)");
            Storyboard.SetTarget(scaleXAnimation, owner);

            DoubleAnimationUsingKeyFrames scaleYAnimation = new DoubleAnimationUsingKeyFrames() { EnableDependentAnimation = RadControl.IsInTestMode };
            scaleYAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0), Value = 1 });
            scaleYAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(160 * speedFactor), Value = 0.6 });

            Storyboard.SetTargetProperty(scaleYAnimation, "(UIElement.RenderTransform).(CompositeTransform.ScaleY)");
            Storyboard.SetTarget(scaleYAnimation, owner);

            storyboard.Children.Add(scaleXAnimation);
            storyboard.Children.Add(scaleYAnimation);
        }

        internal static void SetNavigateButtonsPanelNavigateToAnimation(Storyboard storyboard, Panel owner)
        {
            DoubleAnimationUsingKeyFrames scaleXAnimation = new DoubleAnimationUsingKeyFrames() { EnableDependentAnimation = RadControl.IsInTestMode };
            scaleXAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0), Value = 0.6 });
            scaleXAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(160 * speedFactor), Value = 1 });
            Storyboard.SetTargetProperty(scaleXAnimation, "(UIElement.RenderTransform).(CompositeTransform.ScaleX)");
            Storyboard.SetTarget(scaleXAnimation, owner);

            DoubleAnimationUsingKeyFrames scaleYAnimation = new DoubleAnimationUsingKeyFrames() { EnableDependentAnimation = RadControl.IsInTestMode };
            scaleYAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0), Value = 0.6 });
            scaleYAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(160 * speedFactor), Value = 1 });

            Storyboard.SetTargetProperty(scaleYAnimation, "(UIElement.RenderTransform).(CompositeTransform.ScaleY)");
            Storyboard.SetTarget(scaleYAnimation, owner);

            storyboard.Children.Add(scaleXAnimation);
            storyboard.Children.Add(scaleYAnimation);
        }
    }
}
