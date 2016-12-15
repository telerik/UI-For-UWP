using System;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;

namespace Telerik.UI.Xaml.Controls.Primitives.Menu
{
    internal class NavigateItemLayer : ElementLayerBase<RadialSegment>
    {
        private Panel panel;
        private Rectangle backgroundElement;
        private Panel buttonsPanel;
        private NavigationItemButton[] navigationButtons = new NavigationItemButton[8];

        private Storyboard showStoryBoard;
        private Storyboard hideStoryBoard;
        private Storyboard navigateFromStoryboard;
        private Storyboard navigateToStoryboard;

        public NavigateItemLayer()
            : base()
        {
            for (int i = 0; i < this.navigationButtons.Length; i++)
            {
                this.navigationButtons[i] = new NavigationItemButton() { Visibility = Visibility.Collapsed };
                this.ButtonsPanel.Children.Add(this.navigationButtons[i]);
            }
        }

        public override RingModelBase Model
        {
            get
            {
                return base.Model;
            }
            set
            {
                base.Model = value;

                this.ButtonsPanel.Width = this.Model.OuterRadius * 2;
                this.ButtonsPanel.Height = this.Model.OuterRadius * 2;
            }
        }

        internal Panel ButtonsPanel
        {
            get
            {
                if (this.buttonsPanel == null)
                {
                    this.buttonsPanel = new Canvas();
                }
                return this.buttonsPanel;
            }
        }

        internal Rectangle BackgroundElement
        {
            get
            {
                if (this.backgroundElement == null)
                {
                    this.backgroundElement = new Rectangle()
                    {
                        RadiusX = this.Model.OuterRadius * 2,
                        RadiusY = this.Model.OuterRadius * 2,
                        Width = this.Model.OuterRadius * 2,
                        Height = this.Model.OuterRadius * 2,
                        Visibility = Visibility.Collapsed
                    };

                    if (RadControl.IsInTestMode)
                    {
                        this.backgroundElement.Name = "NavigationBackgroundPanel";
                    }
                }
                return this.backgroundElement;
            }
        }

        protected override Panel Visual
        {
            get
            {
                if (this.panel == null)
                {
                    this.panel = new Grid()
                    {
                        Width = this.Model.OuterRadius * 2,
                        Height = this.Model.OuterRadius * 2,
                        Visibility = Visibility.Collapsed
                    };
                }
                return this.panel;
            }
        }

        protected override Storyboard ShowLayerStoryboard
        {
            get
            {
                return this.showStoryBoard;
            }
        }

        protected override Storyboard HideLayerStoryboard
        {
            get
            {
                return this.hideStoryBoard;
            }
        }

        protected override Storyboard NavigateFromStoryboard
        {
            get
            {
                return this.navigateFromStoryboard;
            }
        }

        protected override Storyboard NavigateToStoryboard
        {
            get
            {
                return this.navigateToStoryboard;
            }
        }

        public override void DetachFromPanel(Panel assosiatedPanel)
        {
            base.DetachFromPanel(assosiatedPanel);

            if (this.panel != null)
            {
                this.Visual.Children.Clear();
            }
        }

        public override void AttachToPanel(Panel assosiatedPanel)
        {
            Canvas.SetZIndex(this.ButtonsPanel, ZIndices.NavigateMenuItemsZIndex);
            Canvas.SetZIndex(this.Visual, ZIndices.NavigateMenuItemsZIndex);
            Canvas.SetZIndex(this.BackgroundElement, ZIndices.NavigateBackgroundVisualZIndex);

            this.Visual.Children.Add(this.ButtonsPanel);
            assosiatedPanel.Children.Add(this.BackgroundElement);

            base.AttachToPanel(assosiatedPanel);

            this.InitializeStartAnimation();
            this.InitializeEndAnimation();
            this.InitializeNavigateFromAnimation();
            this.InitializeNavigateToAnimation();

            this.Visual.RenderTransform = new CompositeTransform();
            this.Visual.RenderTransformOrigin = new Point(0.5, 0.5);

            this.ButtonsPanel.RenderTransform = new CompositeTransform();
            this.ButtonsPanel.RenderTransformOrigin = new Point(0.5, 0.5);

            this.BackgroundElement.RenderTransform = new CompositeTransform();
            this.BackgroundElement.RenderTransformOrigin = new Point(0.5, 0.5);
        }

        public override void UpdateVisual(RadialSegment segment, double startAngle)
        {
            if (segment == null || !segment.TargetItem.CanNavigate || segment.LayoutSlot == RadialLayoutSlot.Invalid)
            {
                return;
            }

            NavigationItemButton navigationButton = this.navigationButtons[segment.TargetItem.Index];
            navigationButton.StartAngle = startAngle;
            navigationButton.UpdateVisualsState();
        }

        public override void ShowVisual(RadialSegment segmentModel, double startAngle)
        {
            if (segmentModel == null || !segmentModel.TargetItem.CanNavigate || segmentModel.LayoutSlot == RadialLayoutSlot.Invalid)
            {
                return;
            }

            NavigationItemButton navigationButton = this.navigationButtons[segmentModel.TargetItem.Index];
            navigationButton.StartAngle = startAngle;
            navigationButton.ResetVisualState();

            segmentModel.Visual = navigationButton;
            navigationButton.Model = segmentModel;
            navigationButton.Visibility = Visibility.Visible;
            segmentModel.Visual = navigationButton;

            navigationButton.UpdateVisualsState();
        }

        public override void ClearVisual(RadialSegment segment)
        {
            if (segment != null)
            {
                var element = segment.Visual as UIElement;

                if (element != null)
                {
                    element.Visibility = Visibility.Collapsed;
                }

                segment.Visual = null;
                segment.LayoutSlot = RadialLayoutSlot.Invalid;
            }
        }

        internal override void UpdateVisualPanel()
        {
            this.BackgroundElement.Style = this.Owner.NavigationMenuBackgroundStyle;

            double newRadius = this.Model.OuterRadius * 2;

            this.Visual.Width = newRadius;
            this.Visual.Height = newRadius;

            this.BackgroundElement.Width = newRadius;
            this.BackgroundElement.Height = newRadius;
            this.BackgroundElement.RadiusX = newRadius;
            this.BackgroundElement.RadiusY = newRadius;

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

        private void InitializeNavigateFromAnimation()
        {
            this.navigateFromStoryboard = new Storyboard();
            Animations.SetNavigateButtonsPanelNavigateFromAnimation(this.navigateFromStoryboard, this.ButtonsPanel);
        }

        private void InitializeNavigateToAnimation()
        {
            this.navigateToStoryboard = new Storyboard();
            Animations.SetNavigateButtonsPanelNavigateToAnimation(this.navigateToStoryboard, this.ButtonsPanel);
        }

        private void InitializeEndAnimation()
        {
            this.hideStoryBoard = new Storyboard();

            Animations.SetNavigateButtonsPanelEndAnimation(this.hideStoryBoard, this.ButtonsPanel);
            Animations.SetNavigationBackgroundElementEndAnimation(this.hideStoryBoard, this.BackgroundElement);
        }

        private void InitializeStartAnimation()
        {
            ObjectAnimationUsingKeyFrames showElementAnimation = new ObjectAnimationUsingKeyFrames();
            showElementAnimation.KeyFrames.Add(new DiscreteObjectKeyFrame { KeyTime = TimeSpan.Zero, Value = Visibility.Visible });

            Storyboard.SetTargetProperty(showElementAnimation, "(UIElement.Visibility)");
            Storyboard.SetTarget(showElementAnimation, this.Visual);

            this.showStoryBoard = new Storyboard();
            this.showStoryBoard.Children.Add(showElementAnimation);
            Animations.SetNavigationBackgroundElementStartAnimation(this.showStoryBoard, this.BackgroundElement);
            Animations.SetNavigateButtonsPanelStartAnimation(this.showStoryBoard, this.ButtonsPanel);
        }
    }
}
