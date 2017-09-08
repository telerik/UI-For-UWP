using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Telerik.Charting;
using Telerik.Core;
using Windows.Foundation;
using Windows.UI.Composition;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Base class for all <see cref="Control"/> instances that represent charting engine logical models.
    /// </summary>
    ////[TemplatePart(Name = RenderSurfacePartName, Type = typeof(Canvas))]
    public abstract class PresenterBase : RadControl, IChartElementPresenter
    {
        /// <summary>
        /// Represents a <see cref="Windows.Foundation.Size(double, double)"/> structure, which Width and Height members are set to double.PositiveInfinity.
        /// </summary>
        public static readonly Size InfinitySize = new Size(double.PositiveInfinity, double.PositiveInfinity);

        /// <summary>
        /// Represents a <see cref="Windows.Foundation.Point(double, double)"/> structure, which Width and Height members are set to double.PositiveInfinity.
        /// </summary>
        public static readonly Point InfinityPoint = new Point(double.PositiveInfinity, double.PositiveInfinity);
        
        internal Canvas renderSurface;
        internal ChartLayoutContext lastLayoutContext;
        internal bool isPaletteApplied;
        internal bool invalidatePaletteScheduled;
        internal bool drawWithComposition;

        private const string RenderSurfacePartName = "PART_RenderSurface";

        private Compositor compositor;
        private ContainerVisual containerVisualRoot;

        /// <summary>
        /// Initializes a new instance of the <see cref="PresenterBase"/> class.
        /// </summary>
        protected PresenterBase()
        {
        }
        
        /// <summary>
        /// Gets a value indicating whether this instance is visible.
        /// </summary>
        /// <remarks>
        /// This property supports the RadChart infrastructure and is not intended to be used directly from your code.
        /// </remarks>
        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        bool IElementPresenter.IsVisible
        {
            get
            {
                return this.Visibility == Visibility.Visible;
            }
        }

        internal bool IsInvalidated
        {
            get
            {
                return this.lastLayoutContext.AvailableSize == RadChartBase.InfinitySize;
            }
        }

        /// <summary>
        /// Gets the <see cref="Canvas"/> instance used as a visual parent for all the child UI elements.
        /// </summary>
        protected Canvas RenderSurface
        {
            get
            {
                return this.renderSurface;
            }
        }

        /// <summary>
        /// Gets the <see cref="Windows.UI.Composition.Compositor"/> instance used for the creation of Composition visuals.
        /// </summary>
        protected Compositor Compositor
        {
            get
            {
                return this.compositor;
            }
        }

        /// <summary>
        /// Gets the <see cref="Windows.UI.Composition.ContainerVisual"/> instance used as a container for the visual elements drawn by the Composition.
        /// </summary>
        protected ContainerVisual ContainerVisualRoot
        {
            get
            {
                return this.containerVisualRoot;
            }
        }

        /// <summary>
        /// Retrieves the desired size for the specified node's content.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        RadSize IElementPresenter.MeasureContent(object owner, object content)
        {
            if (content == null)
            {
                return RadSize.Empty;
            }

            return this.MeasureNodeOverride(owner as Node, content);
        }

        /// <summary>
        /// Invalidates the visual representation of the specified logical node.
        /// </summary>
        public void RefreshNode(object node)
        {
            this.RefreshNodeCore(node as Node);
        }

        /// <summary>
        /// Re-applies the owning chart's palette.
        /// </summary>
        public void InvalidatePalette()
        {
            if (this.invalidatePaletteScheduled || !this.IsTemplateApplied)
            {
                return;
            }

            if (!this.InvokeAsync(this.OnPaletteInvalidated))
            {
                return;
            }

            this.invalidatePaletteScheduled = true;
            this.isPaletteApplied = false;
        }

        internal static RadSize MeasureVisual(FrameworkElement visual)
        {
            // measure the label synchronously to determine its within the corresponding segment
            visual.ClearValue(FrameworkElement.WidthProperty);
            visual.ClearValue(FrameworkElement.HeightProperty);
            visual.Measure(RadChartBase.InfinitySize);

            return GetVisualDesiredSize(visual);
        }

        internal static RadSize GetVisualDesiredSize(FrameworkElement visual)
        {
            RadSize visualSize = RadSize.Empty;
            visualSize = new RadSize(visual.DesiredSize.Width, visual.DesiredSize.Height);

            return visualSize;
        }

        internal void UpdateUI(ChartLayoutContext context)
        {
            context.Flags = this.GetLayoutFlags(context);

            if ((context.Flags & ChartLayoutFlags.Size) == ChartLayoutFlags.Size ||
                (context.Flags & ChartLayoutFlags.Zoom) == ChartLayoutFlags.Zoom)
            {
                this.UpdateUICore(context);
            }
            else if ((context.Flags & ChartLayoutFlags.Pan) == ChartLayoutFlags.Pan)
            {
                this.OnPlotOriginChanged(context);
            }

            // update the palette after UI has been updated
            this.isPaletteApplied = false;
            this.ApplyPalette();

            this.lastLayoutContext = context;
            this.OnUIUpdated();
        }

        internal virtual void OnUIUpdated()
        {
        }

        internal virtual bool CanApplyPalette()
        {
            return this.renderSurface != null;
        }

        /// <summary>
        /// Core entry point for applying owning chart's palette.
        /// </summary>
        internal virtual void ApplyPaletteCore()
        {
        }

        internal virtual void OnPlotOriginChanged(ChartLayoutContext context)
        {
        }

        internal virtual ChartLayoutFlags GetLayoutFlags(ChartLayoutContext context)
        {
            // check whether last layout context is valid
            if (this.lastLayoutContext.AvailableSize == InfinitySize)
            {
                return ChartLayoutFlags.Size;
            }

            ChartLayoutFlags flags = ChartLayoutFlags.None;
            if (this.lastLayoutContext.AvailableSize != context.AvailableSize)
            {
                flags |= ChartLayoutFlags.Size;
            }
            if (this.lastLayoutContext.Scale != context.Scale)
            {
                flags |= ChartLayoutFlags.Zoom;
            }
            if (this.lastLayoutContext.PlotOrigin != context.PlotOrigin)
            {
                flags |= ChartLayoutFlags.Pan;
            }

            return flags;
        }

        internal virtual void OnPaletteInvalidated()
        {
            this.ApplyPalette();
            this.invalidatePaletteScheduled = false;
        }

        /// <summary>
        /// Updates of all of the chart elements presented by this instance.
        /// </summary>
        internal virtual void UpdateUICore(ChartLayoutContext context)
        {
        }

        internal virtual void InvalidateCore()
        {
            this.lastLayoutContext = ChartLayoutContext.Invalid;

            // TODO: Check carefully for any performance impact
            this.isPaletteApplied = false;
        }

        /// <summary>
        /// Updates the layout slot of the specified logical node, presented by the provided <see cref="FrameworkElement"/>.
        /// </summary>
        internal virtual void ArrangeUIElement(FrameworkElement presenter, RadRect layoutSlot, bool setSize = true)
        {
            if (presenter == null)
            {
                return;
            }

            presenter.AddTransform(new TranslateTransform() { X = layoutSlot.X, Y = layoutSlot.Y });

            if (setSize)
            {
                // We can have custom Canvas and to skip the Width/Height setting
                if (presenter.Width != layoutSlot.Width)
                {
                    presenter.Width = layoutSlot.Width;
                }
                    
                if (presenter.Height != layoutSlot.Height)
                {
                    presenter.Height = layoutSlot.Height;
                }
            }
        }

        internal void ClearPresenters(IList elements)
        {
            if (this.renderSurface == null)
            {
                elements.Clear();
                return;
            }

            for (int i = elements.Count - 1; i >= 0; i--)
            {
                this.renderSurface.Children.Remove(elements[i] as UIElement);
                elements.RemoveAt(i);
            }

            this.InvalidateCore();
        }

        internal void UpdatePalette(bool force)
        {
            if (force)
            {
                this.isPaletteApplied = false;
            }
            this.ApplyPalette();
        }

        /// <summary>
        /// Core entry point for calculating the size of a node's content.
        /// </summary>
        protected internal virtual RadSize MeasureNodeOverride(Node node, object content)
        {
            return RadSize.Empty;
        }

        /// <summary>
        /// Initializes the render surface template part.
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            base.ApplyTemplateCore();

            this.renderSurface = this.GetTemplatePartField<Canvas>(RenderSurfacePartName);

            if (this.renderSurface != null)
            {
                this.containerVisualRoot = this.GetContainerVisual(this.renderSurface);
                if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
                {
                    this.compositor = this.containerVisualRoot.Compositor;
                }
               
                return true;
            }

            return false;
        }

        /// <summary>
        /// Performs the core logic that invalidates the visual representation of the specified logical node.
        /// </summary>
        protected virtual void RefreshNodeCore(Node node)
        {
            this.InvalidateCore();
        }

        /// <summary>
        /// Creates a <see cref="ContentPresenter"/> associated with the specified content and template.
        /// </summary>
        protected ContentPresenter CreateContentPresenter(object content, DataTemplate template)
        {
            ContentPresenter presenter = new ContentPresenter();
            presenter.Content = content;
            presenter.ContentTemplate = template;
            this.renderSurface.Children.Add(presenter);

            return presenter;
        }

        private ContainerVisual GetContainerVisual(UIElement element)
        {
            var hostVisual = ElementCompositionPreview.GetElementVisual(element);
            var root = hostVisual.Compositor.CreateContainerVisual();
            ElementCompositionPreview.SetElementChildVisual(element, root);

            return root;
        }

        private void ApplyPalette()
        {
            if (this.isPaletteApplied || !this.CanApplyPalette())
            {
                return;
            }

            this.ApplyPaletteCore();
            this.isPaletteApplied = true;
        }
    }
}
