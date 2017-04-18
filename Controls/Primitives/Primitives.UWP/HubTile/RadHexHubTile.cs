using Telerik.UI.Automation.Peers;
using Telerik.UI.Xaml.Controls.Primitives.HubTile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Defines a hexagonal tile with a picture, title, message and notification count. Similar to Mail, Messaging or Internet Explorer.
    /// </summary>
    public class RadHexHubTile : HubTileBase
    {
        /// <summary>
        /// Identifies the <see cref="Length"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LengthProperty =
            DependencyProperty.Register(nameof(Length), typeof(double), typeof(RadHexHubTile), new PropertyMetadata(0d, OnLengthChanged));

        /// <summary>
        /// Identifies the <see cref="StrokeThickness"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register(nameof(StrokeThickness), typeof(double), typeof(RadHexHubTile), new PropertyMetadata(0d, OnStrokeThicknessChanged));

        /// <summary>
        /// Identifies the <see cref="Orientation"/> dependency property.
        /// </summary> 
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(nameof(Orientation), typeof(HexOrientation), typeof(RadHexHubTile), new PropertyMetadata(HexOrientation.Flat, OnOrientationChanged));

        /// <summary>
        /// Identifies the <see cref="ImageSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register(nameof(ImageSource), typeof(ImageSource), typeof(RadHexHubTile), new PropertyMetadata(null, OnImageSourceChanged));

        private const double WidthToHeightRatio = 0.866; // Math.Sqrt(3) / 2;

        private double lengthCache;
        private double oppositeLengthCache;
        private HexOrientation orientationCache;
        private bool updatingIsFlipped;
        private string currentVisualState = string.Empty;
        private double strokeThicknessCache;
        private ImageSource imageSourceCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadHexHubTile"/> class.
        /// </summary> 
        public RadHexHubTile()
        {
            this.DefaultStyleKey = typeof(RadHexHubTile);
            this.UpdateSize();
        }

        /// <summary>
        /// Gets or sets the longest dimension of the hexagon.
        /// </summary>
        public double Length
        {
            get
            {
                return this.lengthCache;
            }
            set
            {
                this.SetValue(LengthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the orientation of the hexagon.
        /// </summary>
        public HexOrientation Orientation
        {
            get
            {
                return this.orientationCache;
            }
            set
            {
                this.SetValue(OrientationProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the thickness of the hexagon border.
        /// </summary>
        public double StrokeThickness
        {
            get
            {
                return this.strokeThicknessCache;
            }
            set
            {
                this.SetValue(StrokeThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the source of the image.
        /// </summary>
        public ImageSource ImageSource
        {
            get
            {
                return this.imageSourceCache;
            }
            set
            {
                this.SetValue(ImageSourceProperty, value);
            }
        }

        internal IHexViewListener Owner { get; set; }

        /// <inheritdoc/>
        protected override bool ApplyTemplateCore()
        {
            return true;
        }

        /// <inheritdoc/>
        protected override void OnIsFlippedChanged(bool newValue, bool oldValue)
        {
            if (!this.IsInternalPropertyChange)
            {
                this.updatingIsFlipped = true;
                this.UpdateVisualState(this.IsLoaded);
                this.updatingIsFlipped = false;
            }
        }

        /// <inheritdoc/>
        protected override void OnBackContentChanged(object newBackContent, object oldBackContent)
        {
            if (this.IsFlipped)
            {
                if (newBackContent == null)
                {
                    this.OnBackStateDeactivated();
                }
                else
                {
                    this.OnBackStateActivated();
                }
            }
        }

        /// <inheritdoc/>
        protected override void UpdateIsFlipped(bool animate)
        {
            bool isInitialUpdate = string.IsNullOrEmpty(this.CurrentVisualState);

            if (this.BackContent == null && this.BackContentTemplate == null)
            {
                this.ChangeIsFlippedProperty(false);
            }
            else if (!isInitialUpdate)
            {
                // Toggle the IsFlipped property
                this.ChangeIsFlippedProperty(!this.IsFlipped);
            }
        }

        /// <inheritdoc/>
        protected override string ComposeVisualStateName()
        {
            return this.IsFlipped ? "Flipped" : "NotFlipped";
        }

        /// <inheritdoc/>
        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            if (this.Owner != null)
            {
                this.Owner.RaiseItemTap(this);
            }

            base.OnTapped(e);
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RadHexHubTileAutomationPeer(this);
        }

        private static void OnLengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tile = d as RadHexHubTile;
            var newLength = (double)e.NewValue;
            if (tile.lengthCache != newLength)
            {
                tile.lengthCache = newLength;
                tile.oppositeLengthCache = newLength * WidthToHeightRatio;
                tile.UpdateSize();
            }
        }

        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tile = d as RadHexHubTile;
            var newOrientation = (HexOrientation)e.NewValue;
            if (tile.orientationCache != newOrientation)
            {
                tile.orientationCache = newOrientation;
                tile.UpdateSize();
            }
        }

        private static void OnStrokeThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tile = d as RadHexHubTile;
            tile.strokeThicknessCache = (double)e.NewValue;
        }

        private static void OnImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tile = d as RadHexHubTile;
            tile.imageSourceCache = (ImageSource)e.NewValue;
        }

        private void UpdateSize()
        {
            if (this.Orientation == HexOrientation.Flat)
            {
                this.Width = this.lengthCache;
                this.Height = this.oppositeLengthCache;
            }
            else
            {
                this.Width = this.oppositeLengthCache;
                this.Height = this.lengthCache;
            }
        }

        private void ChangeIsFlippedProperty(bool value)
        {
            if (!this.updatingIsFlipped)
            {
                this.ChangePropertyInternally(RadHexHubTile.IsFlippedProperty, value);
            }
        }
    }
}
