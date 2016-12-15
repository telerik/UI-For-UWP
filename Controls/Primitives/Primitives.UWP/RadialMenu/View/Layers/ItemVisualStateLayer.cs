using System;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Telerik.UI.Xaml.Controls.Primitives.Menu
{
    internal class ItemVisualStateLayer : ElementLayerBase<RadialSegment>
    {
        private Panel stateButtonsPanel;
        private Storyboard showStoryBoard;
        private Storyboard hideStoryBoard;
        private Storyboard navigateToStoryboard;
        private Storyboard navigateFromStoryboard;

        protected override Storyboard ShowLayerStoryboard
        {
            get { return this.showStoryBoard; }
        }

        protected override Storyboard HideLayerStoryboard
        {
            get { return this.hideStoryBoard; }
        }

        protected override Storyboard NavigateFromStoryboard
        {
            get { return this.navigateFromStoryboard; }
        }

        protected override Storyboard NavigateToStoryboard
        {
            get { return this.navigateToStoryboard; }
        }

        protected override Panel Visual
        {
            get
            {
                if (this.stateButtonsPanel == null)
                {
                    this.stateButtonsPanel = new Grid()
                    {
                        Width = this.Model.OuterRadius * 2,
                        Height = this.Model.OuterRadius * 2,
                        Visibility = Visibility.Visible
                    };

                    if (RadControl.IsInTestMode)
                    {
                        this.stateButtonsPanel.Name = "StateButtonsBackgroundPanel";
                    }
                }
                return this.stateButtonsPanel;
            }
        }

        public override void AttachToPanel(Panel panel)
        {
            base.AttachToPanel(panel);
            Canvas.SetZIndex(this.Visual, ZIndices.MenuStateItemsZINdex);

            this.InitializeStartAnimation();
            this.InitializeEndAnimation();
            this.InitializeNavigateFromAnimation();
            this.InitializeNavigateToAnimation();

            this.Visual.RenderTransform = new CompositeTransform();
            this.Visual.RenderTransformOrigin = new Point(0.5, 0.5);
        }

        public override void UpdateVisual(RadialSegment segment, double startAngle)
        {
            if (segment != null && segment.LayoutSlot != RadialLayoutSlot.Invalid)
            {
                DecorationItemButton stateButton = segment.Visual as DecorationItemButton;

                if (stateButton != null)
                {
                    stateButton.LayoutSlot.OuterRadius = this.Owner.outerRadiusCache;
                    stateButton.Update();
                }
            }
        }

        public override void ClearVisual(RadialSegment segment)
        {
            if (segment != null)
            {
                var element = segment.Visual as UIElement;

                if (element != null)
                {
                    this.Visual.Children.Remove(element);
                }

                segment.Visual = null;
            }
        }

        public override void ShowVisual(RadialSegment segment, double startAngle)
        {
            if (segment != null && segment.LayoutSlot != RadialLayoutSlot.Invalid)
            {
                DecorationItemButton stateButton = new DecorationItemButton();
                stateButton.Segment = segment;

                stateButton.LayoutSlot.OuterRadius = this.Owner.outerRadiusCache;
                segment.Visual = stateButton;
                this.Visual.Children.Add(stateButton);
            }
        }

        internal override void UpdateVisualPanel()
        {
            double newRadius = this.Model.OuterRadius * 2;

            this.Visual.Width = newRadius;
            this.Visual.Height = newRadius;

            foreach (FrameworkElement child in this.Visual.Children)
            {
                child.Width = newRadius;
                child.Height = newRadius;
            }

            this.InitializeStartAnimation();
            this.InitializeEndAnimation();
            this.InitializeNavigateFromAnimation();
            this.InitializeNavigateToAnimation();
        }

        private void InitializeStartAnimation()
        {
            ObjectAnimationUsingKeyFrames showElementAnimation = new ObjectAnimationUsingKeyFrames();
            showElementAnimation.KeyFrames.Add(new DiscreteObjectKeyFrame { KeyTime = TimeSpan.Zero, Value = Visibility.Visible });

            Storyboard.SetTargetProperty(showElementAnimation, "(UIElement.Visibility)");
            Storyboard.SetTarget(showElementAnimation, this.Visual);

            this.showStoryBoard = new Storyboard();

            Animations.SetStateButtonsPanelStartAnimation(this.showStoryBoard, this.Visual);

            this.showStoryBoard.Children.Add(showElementAnimation);
        }

        private void InitializeEndAnimation()
        {
            this.hideStoryBoard = new Storyboard();

            Animations.SetStateButtonsPanelEndAnimation(this.hideStoryBoard, this.Visual);
        }

        private void InitializeNavigateFromAnimation()
        {
            this.navigateFromStoryboard = new Storyboard();

            Animations.SetStateButtonsPanelNavigateFromAnimation(this.navigateFromStoryboard, this.Visual);
        }

        private void InitializeNavigateToAnimation()
        {
            this.navigateToStoryboard = new Storyboard();

            Animations.SetStateButtonsPanelNavigateToAnimation(this.navigateToStoryboard, this.Visual);
        }
    }
}
