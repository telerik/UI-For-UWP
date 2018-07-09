using System;
using System.Collections.Generic;
using Telerik.Core;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;

namespace Telerik.UI.Xaml.Controls.Primitives.Menu
{
    internal class MenuItemLayer : ElementLayerBase<RadialSegment>
    {
        private Rectangle backgroundElement;
        private Panel panel;
        private Panel contentItemsPanel;

        private Storyboard showStoryBoard;
        private Storyboard hideStoryBoard;
        private Storyboard navigateFromStoryboard;
        private Storyboard navigateToStoryboard;
        private Queue<RadialMenuItemControl> recycledMenuItemControls = new Queue<RadialMenuItemControl>();

        internal Panel ContentItemsPanel
        {
            get
            {
                if (this.contentItemsPanel == null)
                {
                    this.contentItemsPanel = new Canvas()
                    {
                        Width = this.Model.OuterRadius * 2,
                        Height = this.Model.OuterRadius * 2,
                        Visibility = Visibility.Visible
                    };
                }
                return this.contentItemsPanel;
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
                        this.backgroundElement.Name = "MenuItemsBackgroundPanel";
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
                    // TODO: +2 offset ?.
                    this.panel = new Grid()
                    {
                        Width = (this.Model.OuterRadius * 2) + 2,
                        Height = (this.Model.OuterRadius * 2) + 2,
                        Visibility = Visibility.Collapsed
                    };
                }
                return this.panel;
            }
        }

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
            Canvas.SetZIndex(this.ContentItemsPanel, ZIndices.ContentMenuItemsZIndex);
            Canvas.SetZIndex(this.Visual, ZIndices.ContentMenuItemsZIndex);
            Canvas.SetZIndex(this.BackgroundElement, ZIndices.ContentMenuBackgroundVisualZIndex);

            base.AttachToPanel(assosiatedPanel);

            this.Visual.Children.Add(this.ContentItemsPanel);
            assosiatedPanel.Children.Add(this.BackgroundElement);

            this.InitializeStartAnimation();
            this.InitializeEndAnimation();
            this.InitializeNavigateFromAnimation();
            this.InitializeNavigateToAnimation();

            this.Visual.RenderTransform = new CompositeTransform();
            this.Visual.RenderTransformOrigin = new Point(0.5, 0.5);

            this.ContentItemsPanel.RenderTransform = new CompositeTransform();
            this.ContentItemsPanel.RenderTransformOrigin = new Point(0.5, 0.5);

            this.BackgroundElement.RenderTransform = new CompositeTransform();
            this.BackgroundElement.RenderTransformOrigin = new Point(0.5, 0.5);
        }

        public override void UpdateVisual(RadialSegment segment, double startAngle)
        {
            if (segment != null && segment.LayoutSlot != RadialLayoutSlot.Invalid)
            {
                var control = segment.Visual as RadialMenuItemControl;
                if (control != null)
                {
                    control.IsEnabled = segment.TargetItem.IsEnabled;
                    control.Update();

                    this.ArrangeMenuItemControl(segment);
                }
            }
        }

        public override void ShowVisual(RadialSegment segmentModel, double startAngle)
        {
            if (segmentModel != null && segmentModel.LayoutSlot != RadialLayoutSlot.Invalid)
            {
                RadialMenuItemControl menuItemControl;
                if (this.recycledMenuItemControls.Count > 0)
                {
                    menuItemControl = this.recycledMenuItemControls.Dequeue();
                }
                else
                {
                    menuItemControl = new RadialMenuItemControl();
                }

                this.ContentItemsPanel.Children.Add(menuItemControl);

                menuItemControl.IconContent = segmentModel.TargetItem.IconContent;
                menuItemControl.Header = segmentModel.TargetItem.Header;
                menuItemControl.Segment = segmentModel;
                menuItemControl.IsEnabled = segmentModel.TargetItem.IsEnabled;
                segmentModel.Visual = menuItemControl;

                this.ArrangeMenuItemControl(segmentModel);
            }
        }

        public override void ClearVisual(RadialSegment segment)
        {
            if (segment != null)
            {
                var element = segment.Visual as RadialMenuItemControl;
                if (element != null)
                {
                    this.recycledMenuItemControls.Enqueue(element);
                    this.ContentItemsPanel.Children.Remove(element);
                }

                segment.Visual = null;
                segment.LayoutSlot = RadialLayoutSlot.Invalid;
            }
        }

        internal override void UpdateVisualPanel()
        {
            this.BackgroundElement.Style = this.Owner.ContentMenuBackgroundStyle;

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

        private void ArrangeMenuItemControl(RadialSegment segmentModel)
        {
            var centerPointPolar = CoordinatesUtils.GetCenterPosition(segmentModel.LayoutSlot);
            var centerPoint = RadMath.ToCartesianCoordinates(centerPointPolar.Radius, centerPointPolar.Angle);
            var tranformedCenterPoint = new Point(centerPoint.X + this.Model.OuterRadius, this.Model.OuterRadius - centerPoint.Y);

            var element = segmentModel.Visual as RadialMenuItemControl;
            element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            var arrangeSlot = new Rect(tranformedCenterPoint.X - element.DesiredSize.Width / 2.0, tranformedCenterPoint.Y - element.DesiredSize.Height / 2.0, element.DesiredSize.Width, element.DesiredSize.Height);

            element.Arrange(arrangeSlot);
            Canvas.SetLeft(element, arrangeSlot.X);
            Canvas.SetTop(element, arrangeSlot.Y);
        }

        private void InitializeNavigateFromAnimation()
        {
            this.navigateFromStoryboard = new Storyboard();

            Animations.SetMenuBackgroundElementNavigateFromAnimation(this.navigateFromStoryboard, this.BackgroundElement);
            Animations.SetContentItemsPanelNavigateFromAnimation(this.navigateFromStoryboard, this.ContentItemsPanel);
        }

        private void InitializeNavigateToAnimation()
        {
            this.navigateToStoryboard = new Storyboard();

            Animations.SetMenuBackgroundElementNavigateToAnimation(this.navigateToStoryboard, this.BackgroundElement);
            Animations.SetContentItemsPanelNavigateToAnimation(this.navigateToStoryboard, this.ContentItemsPanel);
        }

        private void InitializeEndAnimation()
        {
            this.hideStoryBoard = new Storyboard();

            Animations.SetContentItemsPanelEndAnimation(this.hideStoryBoard, this.ContentItemsPanel);
            Animations.SetMenuBackgroundElementEndAnimation(this.hideStoryBoard, this.BackgroundElement);
        }

        private void InitializeStartAnimation()
        {
            ObjectAnimationUsingKeyFrames showElementAnimation = new ObjectAnimationUsingKeyFrames();
            showElementAnimation.KeyFrames.Add(new DiscreteObjectKeyFrame { KeyTime = TimeSpan.Zero, Value = Visibility.Visible });

            Storyboard.SetTargetProperty(showElementAnimation, "(UIElement.Visibility)");
            Storyboard.SetTarget(showElementAnimation, this.Visual);

            this.showStoryBoard = new Storyboard();

            Animations.SetContentItemsPanelStartAnimation(this.showStoryBoard, this.ContentItemsPanel);
            Animations.SetMenuBackgroundElementStartAnimation(this.showStoryBoard, this.BackgroundElement);
            this.showStoryBoard.Children.Add(showElementAnimation);
        }
    }
}
