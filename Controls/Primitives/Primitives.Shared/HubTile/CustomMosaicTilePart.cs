using System.ComponentModel;
using System.Windows;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Primitives.HubTile
{
    /// <summary>
    /// Used in RadMosaicHubTile to display the small flipping images.
    /// </summary>
    [TemplatePart(Name = "PART_BackCanvas", Type = typeof(Canvas))]
    [TemplatePart(Name = "PART_FrontCanvas", Type = typeof(Canvas))]
    [TemplateVisualState(Name = "Front", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Back", GroupName = "CommonStates")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class CustomMosaicTilePart : MosaicTilePart
    {
        private Canvas frontCanvas;
        private Canvas backCanvas;
        private Image frontImage;
        private Image backImage;
        private int? imagePartIndex = null;

        /// <summary>
        /// Initializes a new instance of the CustomMosaicTilePart class.
        /// </summary>
        public CustomMosaicTilePart()
        {
            this.DefaultStyleKey = typeof(CustomMosaicTilePart);
            this.SizeChanged += this.OnSizeChanged;
        }

        /// <summary>
        /// Gets or sets which part of an image the tile is displaying if it is a part
        /// of a picture frame inside the mosaic hub tile.
        /// </summary>
        public int? ImagePartIndex
        {
            get
            {
                return this.imagePartIndex;
            }
            set
            {
                int? previousValue = this.imagePartIndex;
                this.imagePartIndex = value;

                this.OnFramePartIndexChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value that is a factor of how much bigger the actual image is
        /// compared to this tile. 
        /// </summary>
        /// <remarks>
        /// For example if this tile is displaying one quarter
        /// of the whole image, the Size factor should be 2. If it is displaying one ninth
        /// of the whole image, the size factor should be 3.
        /// </remarks>
        public int SizeFactor { get; set; }

        /// <summary>
        /// Gets a value indicating whether the tile is currently displaying an image.
        /// </summary>
        public bool IsContentDisplayed
        {
            get
            {
                return (this.IsInFrontState && this.FrontContent != null) || (!this.IsInFrontState && this.BackContent != null);
            }
        }

        /// <summary>
        /// Flips the tile and displays the provided image on the new side.
        /// </summary>
        /// <param name="newSource">The new image.</param>
        public void Flip(ImageSource newSource)
        {
            if (this.IsInFrontState)
            {
                if (newSource == this.BackContent)
                {
                    this.Flip();
                }
                else
                {
                    this.BackContent = newSource;
                }
            }
            else
            {
                if (newSource == this.FrontContent)
                {
                    this.Flip();
                }
                else
                {
                    this.FrontContent = newSource;
                }
            }
        }

        internal void SetImagePosition(int sizeFactor)
        {
            int row = 0;
            int col = 0;

            if (this.imagePartIndex.HasValue)
            {
                row = this.imagePartIndex.Value / sizeFactor;
                col = this.imagePartIndex.Value % sizeFactor;
            }

            double imageWidth = this.ActualWidth * sizeFactor;
            double imageHeight = this.ActualHeight * sizeFactor;
            Image imageToOffset = null;

            if (this.IsInFrontState)
            {
                if (!this.imagePartIndex.HasValue)
                {
                    this.backImage.Width = this.ActualWidth;
                    this.backImage.Height = this.ActualHeight;
                    Canvas.SetLeft(this.backImage, 0);
                    Canvas.SetTop(this.backImage, 0);
                    return;
                }

                this.backImage.Width = imageWidth;
                this.backImage.Height = imageHeight;
                imageToOffset = this.backImage;
            }
            else
            {
                if (!this.imagePartIndex.HasValue)
                {
                    this.frontImage.Width = this.ActualWidth;
                    this.frontImage.Height = this.ActualHeight;

                    Canvas.SetLeft(this.frontImage, 0);
                    Canvas.SetTop(this.frontImage, 0);
                    return;
                }

                this.frontImage.Width = imageWidth;
                this.frontImage.Height = imageHeight;
                imageToOffset = this.frontImage;
            }

            Canvas.SetLeft(imageToOffset, -this.ActualWidth * col);
            Canvas.SetTop(imageToOffset, -this.ActualHeight * row);
        }

        /// <summary>
        /// Retrieves the ControlTemplate parts.
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            this.frontCanvas = this.GetTemplatePartField<Canvas>("PART_FrontCanvas");
            applied = applied && this.frontCanvas != null;

            this.backCanvas = this.GetTemplatePartField<Canvas>("PART_BackCanvas") as Canvas;
            applied = applied && this.backCanvas != null;

            return applied;
        }

        /// <summary>
        /// Occurs when the <see cref="M:OnApplyTemplate"/> method has been called and the template is already successfully applied.
        /// </summary>
        protected override void OnTemplateApplied()
        {
            base.OnTemplateApplied();

            this.frontImage = this.frontCanvas.Children[0] as Image;
            this.backImage = this.backCanvas.Children[0] as Image;
        }

        private void OnFramePartIndexChanged()
        {
            this.SetImagePosition(this.SizeFactor);
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var frontClip = new RectangleGeometry() { Rect = new Rect(new Point(), e.NewSize) };
            var backClip = new RectangleGeometry() { Rect = new Rect(new Point(), e.NewSize) };

            this.frontCanvas.Clip = frontClip;
            this.backCanvas.Clip = backClip;

            this.SetImagePosition(this.SizeFactor);
        }
    }
}
