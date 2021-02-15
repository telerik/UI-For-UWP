using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Shapes;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Represents a control that enables a user to show a shadow around another view.
    /// </summary>
    [TemplatePart(Name = "PART_Shadow", Type = typeof(Canvas))]
    [ContentProperty(Name = nameof(Content))]
    public class RadShadow : RadControl
    {
        /// <summary>
        /// Identifies the <see cref="Color"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register(nameof(Color), typeof(Color), typeof(RadShadow), new PropertyMetadata(Colors.Black, new PropertyChangedCallback((d, e) => ((RadShadow)d).OnColorPropertyChanged((Color)e.NewValue))));

        /// <summary>
        /// Identifies the <see cref="OffsetX"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OffsetXProperty =
            DependencyProperty.Register(nameof(OffsetX), typeof(double), typeof(RadShadow), new PropertyMetadata(0.0, new PropertyChangedCallback((d, e) => ((RadShadow)d).OnOffsetPropertyChanged())));

        /// <summary>
        /// Identifies the <see cref="OffsetY"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OffsetYProperty =
            DependencyProperty.Register(nameof(OffsetY), typeof(double), typeof(RadShadow), new PropertyMetadata(0.0, new PropertyChangedCallback((d, e) => ((RadShadow)d).OnOffsetPropertyChanged())));

        /// <summary>
        /// Identifies the <see cref="BlurRadius"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BlurRadiusProperty =
            DependencyProperty.Register(nameof(BlurRadius), typeof(double), typeof(RadShadow), new PropertyMetadata(9.0, new PropertyChangedCallback((d, e) => ((RadShadow)d).OnBlurRadiusPropertyChanged((double)e.NewValue))));

        /// <summary>
        /// Identifies the <see cref="ShadowOpacity"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShadowOpacityProperty =
            DependencyProperty.Register(nameof(ShadowOpacity), typeof(double), typeof(RadShadow), new PropertyMetadata(1.0, new PropertyChangedCallback((d, e) => ((RadShadow)d).OnShadowOpacityPropertyChanged((double)e.NewValue))));

        /// <summary>
        /// Identifies the <see cref="Content"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(nameof(Content), typeof(object), typeof(RadShadow), new PropertyMetadata(null, new PropertyChangedCallback((d, e) => ((RadShadow)d).OnContentPropertyChanged())));

        private const string PartShadowName = "PART_Shadow";

        private bool invalidateShadowMask;
        private SpriteVisual shadowVisual;
        private DropShadow dropShadow;
        private Canvas shadowView;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadShadow"/> class.
        /// </summary>
        public RadShadow()
        {
            this.DefaultStyleKey = typeof(RadShadow);
        }

        /// <summary>
        /// Gets or sets the color of the shadow.
        /// </summary>
        public Color Color
        {
            get { return (Color)this.GetValue(ColorProperty); }
            set { this.SetValue(ColorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the X offset of the shadow from its content.
        /// </summary>
        public double OffsetX
        {
            get { return (double)this.GetValue(OffsetXProperty); }
            set { this.SetValue(OffsetXProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Y offset of the shadow from its content.
        /// </summary>
        public double OffsetY
        {
            get { return (double)this.GetValue(OffsetYProperty); }
            set { this.SetValue(OffsetYProperty, value); }
        }

        /// <summary>
        /// Gets or sets the blur radius of the shadow.
        /// </summary>
        public double BlurRadius
        {
            get { return (double)this.GetValue(BlurRadiusProperty); }
            set { this.SetValue(BlurRadiusProperty, value); }
        }

        /// <summary>
        /// Gets or sets the opacity of the shadow.
        /// </summary>
        public double ShadowOpacity
        {
            get { return (double)this.GetValue(ShadowOpacityProperty); }
            set { this.SetValue(ShadowOpacityProperty, value); }
        }

        /// <summary>
        /// Gets or sets the content of the RadShadow.
        /// </summary>
        public object Content
        {
            get { return (object)this.GetValue(ContentProperty); }
            set { this.SetValue(ContentProperty, value); }
        }

        /// <inheritdoc />
        protected override Size ArrangeOverride(Size finalSize)
        {
            var size = base.ArrangeOverride(finalSize);

            var content = this.GetVisualContent();
            if (content != null)
            {
                this.ApplyShadowMaskIfNeeded(content);

                var contentPosition = content.TransformToVisual(this);
                var offset = contentPosition.TransformPoint(new Point(0, 0));

                this.shadowVisual.Offset = new Vector3((float)offset.X - (float)this.BorderThickness.Left, (float)offset.Y - (float)this.BorderThickness.Top, 0);
                this.shadowVisual.Size = new Vector2((float)content.ActualWidth, (float)content.ActualHeight);
            }

            return size;
        }

        /// <inheritdoc />
        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            this.shadowView = this.GetTemplatePartField<Canvas>(PartShadowName);
            applied = applied && this.shadowView != null;

            if (applied)
            {
                this.InitializeDropShadow();
            }

            return applied;
        }

        private void InitializeDropShadow()
        {
            var compositor = ElementCompositionPreview.GetElementVisual(this.shadowView).Compositor;
            this.shadowVisual = compositor.CreateSpriteVisual();
            this.dropShadow = compositor.CreateDropShadow();

            this.OnColorPropertyChanged(this.Color);
            this.OnOffsetPropertyChanged();
            this.OnBlurRadiusPropertyChanged(this.BlurRadius);
            this.OnShadowOpacityPropertyChanged(this.ShadowOpacity);

            this.shadowVisual.Shadow = this.dropShadow;

            ElementCompositionPreview.SetElementChildVisual(this.shadowView, this.shadowVisual);
        }

        private void ApplyShadowMaskIfNeeded(FrameworkElement content)
        {
            if (!this.invalidateShadowMask)
            {
                return;
            }

            this.invalidateShadowMask = false;

            this.dropShadow.Mask = null;

            var shape = content as Shape;
            if (shape != null)
            {
                this.dropShadow.Mask = shape.GetAlphaMask();
                return;
            }

            var textBlock = content as TextBlock;
            if (textBlock != null)
            {
                this.dropShadow.Mask = textBlock.GetAlphaMask();
                return;
            }

            var image = content as Image;
            if (image != null)
            {
                this.dropShadow.Mask = image.GetAlphaMask();
            }
        }

        private FrameworkElement GetVisualContent()
        {
            var content = this.Content;
            if (content == null)
            {
                return null;
            }

            FrameworkElement visualContent = this.Content as FrameworkElement;
            if (visualContent == null)
            {
                visualContent = ElementTreeHelper.FindVisualDescendant<TextBlock>(this);
            }

            return visualContent;
        }

        private void OnColorPropertyChanged(Color color)
        {
            if (this.dropShadow != null)
            {
                this.dropShadow.Color = color;
            }
        }

        private void OnOffsetPropertyChanged()
        {
            if (this.dropShadow != null)
            {
                this.dropShadow.Offset = new Vector3((float)this.OffsetX, (float)this.OffsetY, 0f);
            }
        }

        private void OnBlurRadiusPropertyChanged(double blurRadius)
        {
            if (this.dropShadow != null)
            {
                this.dropShadow.BlurRadius = (float)blurRadius;
            }
        }

        private void OnShadowOpacityPropertyChanged(double opacity)
        {
            if (this.dropShadow != null)
            {
                this.dropShadow.Opacity = (float)opacity;
            }
        }

        private void OnContentPropertyChanged()
        {
            this.invalidateShadowMask = true;
            this.InvalidateArrange();
        }
    }
}
