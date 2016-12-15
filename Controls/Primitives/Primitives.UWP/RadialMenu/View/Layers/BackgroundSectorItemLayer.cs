using System;
using System.Linq;
using Telerik.Core;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;

namespace Telerik.UI.Xaml.Controls.Primitives.Menu
{
    internal class BackgroundSectorItemLayer : ElementLayerBase<RadialSegment>
    {
        private Panel sectorVisualsPanel;
        private Storyboard showStoryBoard;
        private Storyboard hideStoryBoard;
        private Storyboard navigateFromStoryboard;
        private Storyboard navigateToStoryboard;

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
                if (this.sectorVisualsPanel == null)
                {
                    this.sectorVisualsPanel = new Grid()
                    {
                        Width = this.Model.OuterRadius * 2,
                        Height = this.Model.OuterRadius * 2,
                        Visibility = Visibility.Visible
                    };
                }
                return this.sectorVisualsPanel;
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
                Path sectorItem = BackgroundSectorItemLayer.GetSectorVisual(segment);
                segment.Visual = sectorItem;

                this.Visual.Children.Add(sectorItem);
            }
        }

        public override void UpdateVisual(RadialSegment segment, double startAngle)
        {
            if (segment != null && segment.LayoutSlot != RadialLayoutSlot.Invalid)
            {
                Path sectorVisual = segment.Visual as Path;

                if (sectorVisual != null)
                {
                    sectorVisual.Data = BackgroundSectorItemLayer.RenderArc(segment);
                    sectorVisual.Fill = segment.TargetItem.ContentSectorBackground;
                }
            }
        }

        public override void AttachToPanel(Panel panel)
        {
            base.AttachToPanel(panel);
            Canvas.SetZIndex(this.Visual, ZIndices.BackgroundSectorItemsZindex + 1);

            this.InitializeStartAnimation();
            this.InitializeEndAnimation();
            this.InitializeNavigateFromAnimation();
            this.InitializeNavigateToAnimation();

            this.Visual.RenderTransform = new CompositeTransform();
            this.Visual.RenderTransformOrigin = new Point(0.5, 0.5);
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

        private static RadPoint GetArcPoint(double angle, RadPoint center, double radius)
        {
            double angleInRad = angle * RadMath.DegToRadFactor;

            double x = center.X + (Math.Cos(angleInRad) * radius);
            double y = center.Y - (Math.Sin(angleInRad) * radius);

            return new RadPoint(x, y);
        }

        private static PathGeometry RenderArc(RadialSegment segment)
        {
            var layoutSlot = segment.LayoutSlot;

            var centerPoint = new RadPoint(layoutSlot.OuterRadius, layoutSlot.OuterRadius);

            PathFigure figure = new PathFigure();
            figure.IsClosed = true;
            figure.IsFilled = true;

            // Change the geometry if selected 
            double outerRadius = layoutSlot.OuterRadius;

            RadPoint startPoint = GetArcPoint(layoutSlot.StartAngle, centerPoint, outerRadius);
            figure.StartPoint = new Point(startPoint.X, startPoint.Y);

            ArcSegment firstArc = new ArcSegment();
            firstArc.Size = new Size(outerRadius, outerRadius);
            firstArc.IsLargeArc = layoutSlot.SweepAngle > 180;
            firstArc.SweepDirection = SweepDirection.Counterclockwise;
            var firstArcPoint = GetArcPoint(layoutSlot.StartAngle + layoutSlot.SweepAngle, centerPoint, outerRadius);
            firstArc.Point = new Point(firstArcPoint.X, firstArcPoint.Y);
            figure.Segments.Add(firstArc);

            LineSegment firstLine = new LineSegment();
            var firstLinePoint = GetArcPoint(layoutSlot.StartAngle + layoutSlot.SweepAngle, centerPoint, layoutSlot.InnerRadius);
            firstLine.Point = new Point(firstLinePoint.X, firstLinePoint.Y);
            figure.Segments.Add(firstLine);

            ArcSegment secondArc = new ArcSegment();
            secondArc.Size = new Size(layoutSlot.InnerRadius, layoutSlot.InnerRadius);
            secondArc.IsLargeArc = layoutSlot.SweepAngle > 180;
            secondArc.SweepDirection = SweepDirection.Clockwise;
            var secondArcPoint = GetArcPoint(layoutSlot.StartAngle, centerPoint, layoutSlot.InnerRadius);
            secondArc.Point = new Point(secondArcPoint.X, secondArcPoint.Y);
            figure.Segments.Add(secondArc);

            PathGeometry geometry = new PathGeometry();
            geometry.Figures.Add(figure);

            return geometry;
        }

        private static Path GetSectorVisual(RadialSegment segment)
        {
            Path sectorVisual = new Path();
            PathGeometry geometry = BackgroundSectorItemLayer.RenderArc(segment);

            sectorVisual.Data = geometry;
            sectorVisual.Fill = segment.TargetItem.ContentSectorBackground;
            return sectorVisual;
        }

        private void InitializeStartAnimation()
        {
            ObjectAnimationUsingKeyFrames showElementAnimation = new ObjectAnimationUsingKeyFrames();
            showElementAnimation.KeyFrames.Add(new DiscreteObjectKeyFrame { KeyTime = TimeSpan.Zero, Value = Visibility.Visible });

            Storyboard.SetTargetProperty(showElementAnimation, "(UIElement.Visibility)");
            Storyboard.SetTarget(showElementAnimation, this.Visual);

            this.showStoryBoard = new Storyboard();

            Animations.SetSectorElementsStartAnimation(this.showStoryBoard, this.Visual);

            this.showStoryBoard.Children.Add(showElementAnimation);
        }

        private void InitializeEndAnimation()
        {
            this.hideStoryBoard = new Storyboard();

            Animations.SetSectorElementsEndAnimation(this.hideStoryBoard, this.Visual);
        }

        private void InitializeNavigateFromAnimation()
        {
            this.navigateFromStoryboard = new Storyboard();

            Animations.SetSectorElementsNavigateFromAnimation(this.navigateFromStoryboard, this.Visual);
        }                                                                                   

        private void InitializeNavigateToAnimation()
        {
            this.navigateToStoryboard = new Storyboard();

            Animations.SetSectorElementsNavigateToAnimation(this.navigateToStoryboard, this.Visual);
        }
    }
}
