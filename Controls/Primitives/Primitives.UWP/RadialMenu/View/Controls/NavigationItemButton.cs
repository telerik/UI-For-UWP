using System;
using System.Linq;
using Telerik.Core;
using Telerik.UI.Automation.Peers;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Telerik.UI.Xaml.Controls.Primitives.Menu
{
    /// <summary>
    /// Represents custom control that is used to navigate to the specific <see cref="RadialMenuItem" /> children.
    /// </summary>
    public class NavigationItemButton : RadControl
    {
        /// <summary>
        /// Identifies the <see cref="ContentGlyph"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentGlyphProperty =
            DependencyProperty.Register(nameof(ContentGlyph), typeof(string), typeof(NavigationItemButton), new PropertyMetadata(null));

        internal double startAngle;
        internal TextBlock arrowGlyph;
        internal Path itemPath;
        internal RadialSegment model;
        internal bool isPointerOver;

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationItemButton"/> class.
        /// </summary>
        public NavigationItemButton()
        {
            this.DefaultStyleKey = typeof(NavigationItemButton);
        }

        /// <summary>
        /// Gets or sets the text inside <see cref="NavigationItemButton"/>.
        /// </summary>
        /// <example>
        /// <para>This example demonstrates how to style the <see cref="NavigationItemButton"/> of a <see cref="RadRadialMenu"/> using an implicit style.</para>
        /// <para>You will need to add the following namespace: <c>xmlns:telerikPrimitivesMenu="using:Telerik.UI.Xaml.Controls.Primitives.Menu"</c></para>
        /// <code language="xaml">
        /// &lt;telerikPrimitives:RadRadialMenu.Resources&gt;
        ///     &lt;Style TargetType="telerikPrimitivesMenu:NavigationItemButton"&gt;
        ///         &lt;Setter Property="ContentGlyph" Value="select"/&gt;
        ///     &lt;/Style&gt;
        /// &lt;/telerikPrimitives:RadRadialMenu.Resources&gt;
        /// </code>
        /// </example>
        public string ContentGlyph
        {
            get
            {
                return (string)this.GetValue(ContentGlyphProperty);
            }
            set
            {
                this.SetValue(ContentGlyphProperty, value);
            }
        }

        internal double StartAngle
        {
            get
            {
                return this.startAngle;
            }

            set
            {
                this.startAngle = value;
            }
        }

        internal RadialSegment Model
        {
            get
            {
                return this.model;
            }
            set
            {
                this.model = value;
            }
        }

        internal static RadPoint GetArcPoint(double angle, RadPoint center, double radius)
        {
            double angleInRad = angle * RadMath.DegToRadFactor;

            double x = center.X + (Math.Cos(angleInRad) * radius);
            double y = center.Y - (Math.Sin(angleInRad) * radius);

            return new RadPoint(x, y);
        }

        internal void UpdateVisualsState()
        {
            if (!this.IsTemplateApplied)
            {
                return;
            }
            this.itemPath.Data = NavigationItemButton.GetNavigationButtonArc(this.Model);

            this.arrowGlyph.RenderTransform = new RotateTransform()
            {
                Angle = (90 - this.model.LayoutSlot.SweepAngle / 2.0 - this.StartAngle) - this.Model.TargetItem.Index * 45
            };

            this.ArrangeArrowGlyph();
        }

        internal void ResetVisualState()
        {
            this.isPointerOver = false;
            this.UpdateVisualState(true);
        }

        internal void ExecuteNavigation()
        {
            var navigateItem = this.Model as RadialNavigateItem;
            if (navigateItem != null && navigateItem.TargetItem.CanNavigate)
            {
                var radialMenuModel = navigateItem.TargetItem.Owner as RadialMenuModel;
                if (radialMenuModel != null)
                {
                    var radialMenu = radialMenuModel.Owner as RadRadialMenu;
                    if (radialMenu != null)
                    {
                        radialMenu.RaiseNavigateCommand(navigateItem.TargetItem, radialMenu.model.viewState.MenuLevels.FirstOrDefault(), navigateItem.LayoutSlot.StartAngle);
                    }
                }
            }
        }

        /// <summary>
        /// Called when the Framework <see cref="M:OnApplyTemplate" /> is called. Inheritors should override this method should they have some custom template-related logic.
        /// This is done to ensure that the <see cref="P:IsTemplateApplied" /> property is properly initialized.
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            this.itemPath = this.GetTemplatePartField<Path>("PART_PathElement");
            this.arrowGlyph = this.GetTemplatePartField<TextBlock>("PART_ArrowGlyph");
            return base.ApplyTemplateCore() && this.itemPath != null && this.arrowGlyph != null;
        }

        /// <summary>
        /// Occurs when the <see cref="M:OnApplyTemplate" /> method has been called and the template is already successfully applied.
        /// </summary>
        protected override void OnTemplateApplied()
        {
            this.arrowGlyph.RenderTransformOrigin = new Point(0, 0);
            this.arrowGlyph.RenderTransform = new RotateTransform()
            {
                Angle = ((90 - this.model.LayoutSlot.SweepAngle / 2) - this.StartAngle) - this.Model.TargetItem.Index * 45
            };

            this.itemPath.Data = NavigationItemButton.GetNavigationButtonArc(this.Model);
            this.ArrangeArrowGlyph();

            base.OnTemplateApplied();
        }

        /// <summary>
        /// Called before the PointerEntered event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnPointerEntered(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            this.isPointerOver = true;
            this.UpdateVisualState(false);

            base.OnPointerEntered(e);
        }

        /// <summary>
        /// Called before the PointerExited event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnPointerExited(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            this.isPointerOver = false;
            this.UpdateVisualState(false);

            base.OnPointerExited(e);
        }

        /// <summary>
        /// Builds the current visual state for this instance.
        /// </summary>
        protected override string ComposeVisualStateName()
        {
            if (this.isPointerOver && this.IsEnabled)
            {
                return "PointerOver";
            }
            else
            {
                return base.ComposeVisualStateName();
            }
        }

        /// <summary>
        /// Called before the KeyDown event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            if (e.Handled)
            {
                return;
            }

            e.Handled = this.HandleKeyDown(e.Key);

            base.OnKeyDown(e);
        }

        /// <inheritdoc />
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new NavigationItemButtonAutomationPeer(this);
        }

        private static PathGeometry GetNavigationButtonArc(RadialSegment segmentModel)
        {
            var layoutSlot = segmentModel.LayoutSlot;

            var centerPoint = new RadPoint(layoutSlot.OuterRadius, layoutSlot.OuterRadius);
            PathFigure figure = new PathFigure();
            figure.IsClosed = true;
            figure.IsFilled = true;

            RadPoint startPoint = GetArcPoint(layoutSlot.StartAngle, centerPoint, layoutSlot.OuterRadius);
            figure.StartPoint = new Point(startPoint.X, startPoint.Y);

            ArcSegment firstArc = new ArcSegment();
            firstArc.Size = new Size(layoutSlot.OuterRadius, layoutSlot.OuterRadius);
            firstArc.IsLargeArc = layoutSlot.SweepAngle > 180;
            firstArc.SweepDirection = SweepDirection.Counterclockwise;
            var firstArcPoint = GetArcPoint(layoutSlot.StartAngle + layoutSlot.SweepAngle, centerPoint, layoutSlot.OuterRadius);
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

        private bool HandleKeyDown(VirtualKey key)
        {
            if (key == VirtualKey.Enter)
            {
                this.ExecuteNavigation();
                return true;
            }

            return false;
        }

        private void ArrangeArrowGlyph()
        {
            this.arrowGlyph.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            var centerPoint = new RadPoint(this.Model.LayoutSlot.OuterRadius - 1, this.Model.LayoutSlot.OuterRadius - 1);

            var middleRadius = (this.Model.LayoutSlot.OuterRadius - this.Model.LayoutSlot.InnerRadius) / 2 + this.Model.LayoutSlot.InnerRadius + this.arrowGlyph.ActualHeight / 2;

            var atangAngle = Math.Atan(this.arrowGlyph.ActualWidth / middleRadius) * 180 / Math.PI;
            var itemCenterAngle = (this.Model.LayoutSlot.SweepAngle / 2 + this.model.LayoutSlot.StartAngle) + atangAngle / 2;

            RadPoint startPoint = GetArcPoint(itemCenterAngle, centerPoint, middleRadius);

            this.arrowGlyph.Margin = new Thickness(startPoint.X, startPoint.Y, 0, 0);
        }
    }
}
