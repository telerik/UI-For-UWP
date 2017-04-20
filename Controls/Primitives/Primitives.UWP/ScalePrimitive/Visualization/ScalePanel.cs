using System;
using System.Collections.Generic;
using Telerik.Core;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;

namespace Telerik.UI.Xaml.Controls.Primitives.Scale
{
    /// <summary>
    /// Represents a custom panel which hosts scale control ticks and axis. It is used to arrange and measure all its children in a specific way.
    /// </summary>
    public sealed class ScalePanel : Panel, IView
    {
        private static readonly Size InfinitySize = new Size(double.PositiveInfinity, double.PositiveInfinity);

        private List<FrameworkElement> tickVisuals;
        private List<FrameworkElement> labelVisuals;
        private bool measuring;
        private RadSize lastModelDesiredSize;
        private Size availableSize;

        private NumericalAxisModel model;
        private ScalePrimitive owner;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScalePanel"/> class.
        /// </summary>
        public ScalePanel()
        {
            this.tickVisuals = new List<FrameworkElement>();
            this.labelVisuals = new List<FrameworkElement>();

            this.model = new NumericalAxisModel();
            this.model.presenter = this;
            this.model.View = this;
        }

        double IView.ViewportWidth
        {
            get
            {
                return this.availableSize.Width;
            }
        }

        double IView.ViewportHeight
        {
            get
            {
                return this.availableSize.Height;
            }
        }

        bool IElementPresenter.IsVisible
        {
            get
            {
                return this.Visibility == Visibility.Visible;
            }
        }

        internal ScalePrimitive Owner
        {
            get
            {
                return this.owner;
            }
            set
            {
                this.owner = value;
                this.AttachToOwner();
            }
        }

        internal NumericalAxisModel Model
        {
            get
            {
                return this.model;
            }
        }

        void IElementPresenter.RefreshNode(object node)
        {
            if (!this.measuring)
            {
                this.InvalidateMeasure();
            }
        }

        RadSize IElementPresenter.MeasureContent(object ownerModel, object content)
        {
            var labelModel = ownerModel as AxisLabelModel;
            if (labelModel != null)
            {
                return this.MeasureLabel(labelModel);
            }

            var tick = ownerModel as AxisTickModel;
            if (tick != null)
            {
                return this.MeasureTick(tick);
            }

            return RadSize.Empty;
        }

        internal void ClearVisuals()
        {
            this.ClearVisuals(this.tickVisuals);
            this.ClearVisuals(this.labelVisuals);
        }

        /// <summary>
        /// Called in the measure layout pass to determine the desired size.
        /// </summary>
        /// <param name="availableSize">The available size that was given by the layout system.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "availableSize")]
        protected override Size MeasureOverride(Size availableSize)
        {
            if (this.owner == null)
            {
                return availableSize;
            }

            this.measuring = true;

            this.availableSize = this.NormalizeAvailableSize(availableSize);

            this.model.LineThickness = this.owner.Line.StrokeThickness;

            this.model.Measure(new RadSize(this.availableSize.Width, this.availableSize.Height));
            this.lastModelDesiredSize = this.model.DesiredSize;

            this.UpdateLabels();
            this.UpdateTicks();

            Size desiredSize = new Size();

            if (this.owner.Orientation == Orientation.Horizontal)
            {
                double desiredWidth = this.lastModelDesiredSize.Width;
                desiredSize = new Size(desiredWidth, this.lastModelDesiredSize.Height);
            }
            else
            {
                double desiredHeight = this.lastModelDesiredSize.Height;
                desiredSize = new Size(this.lastModelDesiredSize.Width, desiredHeight);
            }

            this.owner.Line.Measure(availableSize);
            this.owner.ChangePropertyInternally(ScalePrimitive.AxisLineOffsetProperty, this.model.AxisLineOffset);

            this.measuring = false;

            return desiredSize;
        }

        /// <summary>
        /// Provides the behavior for the Arrange pass of layout. Classes can override this method to define their own Arrange pass behavior.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this object should use to arrange itself and its children.</param>
        /// <returns>
        /// The actual size that is used after the element is arranged in layout.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (this.owner == null)
            {
                return finalSize;
            }

            this.availableSize = base.ArrangeOverride(finalSize);

            this.model.Arrange(new RadRect(this.availableSize.Width, this.availableSize.Height));

            int index = 0;
            foreach (var tick in this.model.Ticks)
            {
                var visual = this.GetTickVisual(tick, index);
                visual.Arrange(new Rect(tick.layoutSlot.X, tick.layoutSlot.Y, tick.layoutSlot.Width, tick.layoutSlot.Height));
                index++;
            }

            index = 0;
            foreach (var label in this.model.Labels)
            {
                var visual = this.GetLabelVisual(label, index, false);
                visual.Arrange(new Rect(label.layoutSlot.X, label.layoutSlot.Y, label.layoutSlot.Width, label.layoutSlot.Height));

                index++;
            }

            var line = this.owner.Line;

            if (this.owner.Orientation == Orientation.Horizontal)
            {
                line.Arrange(new Rect(this.model.Line.X1, this.model.Line.Y1 - this.model.LineThickness / 2, this.model.Line.X2 - this.model.Line.X1, this.model.LineThickness));
            }
            else
            {
                line.Arrange(new Rect(this.model.Line.X1 - this.model.LineThickness / 2, this.model.Line.Y1, this.model.LineThickness, this.model.Line.Y2 - this.model.Line.Y1));
            }
            return finalSize;
        }

        private static ContentPresenter CreateContentPresenter(object content, DataTemplate template)
        {
            ContentPresenter presenter = new ContentPresenter();
            presenter.Content = content;
            presenter.ContentTemplate = template;

            return presenter;
        }

        private static void SetLabelContent(FrameworkElement presenter, AxisLabelModel label)
        {
            var contentPresetner = presenter as ContentPresenter;
            var textBlock = presenter as TextBlock;

            if (contentPresetner != null)
            {
                contentPresetner.Content = label.Content;
            }
            else if (textBlock != null)
            {
                textBlock.Text = label.Content == null ? string.Empty : label.Content.ToString();
            }
            else
            {
                // TODO: consider throwing exception
            }
        }

        private RadSize MeasureLabel(AxisLabelModel label)
        {
            FrameworkElement visual = this.GetLabelVisual(label, label.CollectionIndex, true);
            visual.Measure(InfinitySize);

            return new RadSize(visual.DesiredSize.Width, visual.DesiredSize.Height);
        }

        private RadSize MeasureTick(AxisTickModel tick)
        {
            FrameworkElement visual = this.GetTickVisual(tick, tick.CollectionIndex);
            visual.Measure(InfinitySize);

            if (visual is ContentPresenter)
            {
                return new RadSize(visual.DesiredSize.Width, visual.DesiredSize.Height);
            }

            if (this.model.Orientation == Orientation.Horizontal)
            {
                return new RadSize(this.owner.TickThickness, this.owner.TickLength);
            }

            return new RadSize(this.owner.TickLength, this.owner.TickThickness);
        }

        private void UpdateTicks()
        {
            int visibleTicks = this.model.Ticks.Count;

            // collapse unnecessary ticks
            while (visibleTicks < this.tickVisuals.Count)
            {
                this.tickVisuals[visibleTicks].Visibility = Visibility.Collapsed;
                visibleTicks++;
            }
        }

        private void UpdateLabels()
        {
            int visibleLabels = this.model.Labels.Count;

            // collapse unnecessary labels
            while (visibleLabels < this.labelVisuals.Count)
            {
                this.labelVisuals[visibleLabels].Visibility = Visibility.Collapsed;
                visibleLabels++;
            }
        }

        private FrameworkElement GetTickVisual(AxisTickModel tick, int index)
        {
            FrameworkElement visual;

            if (index >= this.tickVisuals.Count)
            {
                visual = this.CreateTickVisual(tick, this.owner.TickTemplate);
            }
            else
            {
                visual = this.tickVisuals[index];
                visual.Visibility = Visibility.Visible;
            }

            return visual;
        }

        private FrameworkElement CreateTickVisual(AxisTickModel tick, DataTemplate template)
        {
            FrameworkElement visual;
            if (template == null)
            {
                visual = this.CreateTickRectangle();
            }
            else
            {
                visual = CreateContentPresenter(tick, template);
            }

            this.Children.Add(visual);
            this.tickVisuals.Add(visual);

            return visual;
        }

        private Rectangle CreateTickRectangle()
        {
            Rectangle visual = new Rectangle();
            visual.Style = this.owner.TickStyle;

            return visual;
        }

        private void ClearVisuals(IList<FrameworkElement> elements)
        {
            for (int i = elements.Count - 1; i >= 0; i--)
            {
                this.Children.Remove(elements[i]);
                elements.RemoveAt(i);
            }

            this.InvalidateMeasure();
        }

        private FrameworkElement GetLabelVisual(AxisLabelModel label, int index, bool setContent)
        {
            FrameworkElement visual;
            if (index >= this.labelVisuals.Count)
            {
                visual = this.CreateLabelVisual(label);
            }
            else
            {
                visual = this.labelVisuals[index];
                visual.Visibility = Visibility.Visible;
            }

            if (setContent)
            {
                SetLabelContent(visual, label);
            }

            return visual;
        }

        private FrameworkElement CreateLabelVisual(AxisLabelModel label)
        {
            DataTemplate template = null;
            if (this.owner.LabelTemplate != null)
            {
                template = this.owner.LabelTemplate;
            }

            FrameworkElement visual;
            if (template == null)
            {
                // creating a TextBlock directly gives huge performance boost - about 10 frames per second!!!
                visual = this.CreateLabelTextBlock();
            }
            else
            {
                visual = CreateContentPresenter(label.Content, template);
            }

            this.Children.Add(visual);
            this.labelVisuals.Add(visual);

            return visual;
        }

        private TextBlock CreateLabelTextBlock()
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Style = this.owner.LabelStyle;

            return textBlock;
        }

        private Size NormalizeAvailableSize(Size currentAvailableSize)
        {
            double width = currentAvailableSize.Width;
            if (double.IsInfinity(width))
            {
                width = this.MinWidth;
            }

            double height = currentAvailableSize.Height;
            if (double.IsInfinity(height))
            {
                height = this.MinHeight;
            }

            width = Math.Max(this.MinWidth, width);
            height = Math.Max(this.MinHeight, height);

            return new Size(width, height);
        }

        private void AttachToOwner()
        {
            this.model.Minimum = this.owner.Minimum;
            this.model.Maximum = this.owner.Maximum;
            this.model.TickPlacement = this.owner.TickPlacement;
            this.model.LabelPlacement = this.owner.LabelPlacement;
            this.model.LabelFormat = this.owner.LabelFormat;
            this.model.Orientation = this.owner.Orientation;
            this.model.TickFrequency = this.owner.TickFrequency;
            this.model.TickLength = this.owner.TickLength;
            this.model.TickThickness = this.owner.TickThickness;
            this.model.Load(this.model);
        }
    }
}
