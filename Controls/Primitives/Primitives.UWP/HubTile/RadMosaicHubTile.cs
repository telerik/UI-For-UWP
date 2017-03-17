using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.UI.Automation.Peers;
using Telerik.UI.Xaml.Controls.Primitives.HubTile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Defines a tile that mimics the WP OS's people hub tile.
    /// </summary>
    [TemplatePart(Name = "PART_LayoutRoot", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_FlipControl", Type = typeof(FlipControl))]
    [TemplatePart(Name = "PART_MosaicTileContainer", Type = typeof(Grid))]
    public class RadMosaicHubTile : PictureHubTile
    {
        /// <summary>
        /// Identifies the FlipMode dependency property.
        /// </summary>
        public static readonly DependencyProperty FlipModeProperty =
            DependencyProperty.Register(nameof(FlipMode), typeof(MosaicFlipMode), typeof(RadMosaicHubTile), new PropertyMetadata(MosaicFlipMode.Individual));

        /// <summary>
        /// Identifies the TileStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty TileStyleProperty =
            DependencyProperty.Register(nameof(TileStyle), typeof(Style), typeof(RadMosaicHubTile), new PropertyMetadata(null));

        private readonly int[][] cornersIndices = new int[][]
        {
           new int[] { 0, 1, 3, 4 }, // Upper left corner
           new int[] { 1, 2, 4, 5 }, // Upper right corner
           new int[] { 3, 4, 6, 7 }, // Lower left corner
           new int[] { 4, 5, 7, 8 }  // Lower right corner
        };

        private readonly Random randomGenerator = new Random();
        private readonly List<int> rowIndices = new List<int>();
        private readonly Dictionary<int, bool> pictureFrameFlipHistory = new Dictionary<int, bool>();

        private List<int> pictureFrameIndices = new List<int>();
        private Panel tilesPanel;
        private int pictureFrameCounter = 0;
        private ImageSource pictureCurrentFrameImage;
        private int updateCounter = 0;

        /// <summary>
        /// Initializes a new instance of the RadMosaicHubTile class.
        /// </summary>
        public RadMosaicHubTile()
        {
            this.DefaultStyleKey = typeof(RadMosaicHubTile);

            InteractionEffectManager.SetIsInteractionEnabled(this, false);
        }

        /// <summary>
        /// Gets or sets a style that is applied to each individual MosaicTile inside RadMosaicHubTile.
        /// </summary>
        public Style TileStyle
        {
            get
            {
                return (Style)this.GetValue(TileStyleProperty);
            }

            set
            {
                this.SetValue(TileStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value that determines how the cells of the mosaic tile are flipped.
        /// </summary>
        public MosaicFlipMode FlipMode
        {
            get
            {
                return (MosaicFlipMode)this.GetValue(RadMosaicHubTile.FlipModeProperty);
            }

            set
            {
                this.SetValue(RadMosaicHubTile.FlipModeProperty, value);
            }
        }

        /// <summary>
        /// A virtual callback that is called periodically when the tile is not frozen. It can be used to
        /// update the tile visual states or to perform other necessary operations.
        /// </summary>
        protected internal override void Update(bool animate, bool updateIsFlipped)
        {
            if (this.BackContent != null || this.BackContentTemplate != null)
            {
                this.updateCounter++;

                if (this.updateCounter == 5)
                {
                    base.Update(animate, updateIsFlipped);
                    this.updateCounter = 0;
                }
            }

            switch (this.FlipMode)
            {
                case MosaicFlipMode.Individual:
                    this.FlipIndividually();
                    break;
                default:
                    this.FlipRow();
                    break;
            }
        }

        /// <summary>
        /// Resolves the ControlTemplate parts.
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            this.tilesPanel = this.GetTemplatePartField<Panel>("PART_MosaicTileContainer");
            applied = applied && this.tilesPanel != null;

            return applied;
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RadMosaicHubTileAutomationPeer(this);
        }

        private void FlipIndividually()
        {
            if (this.pictureFrameCounter == 20 || this.pictureFrameIndices.Count > 0)
            {
                this.ProcessFrame();
                return;
            }

            this.ProcessIndividualTile();
        }

        private void ProcessFrame()
        {
            if (this.pictureFrameIndices.Count == 0)
            {
                int randomCorner = this.randomGenerator.Next(this.cornersIndices.Length / this.cornersIndices.Rank);
                this.pictureFrameIndices = this.cornersIndices[randomCorner].ToList<int>();
                this.pictureCurrentFrameImage = this.GetRandomImageSource();
                for (int i = 0; i < this.pictureFrameIndices.Count; ++i)
                {
                    this.pictureFrameFlipHistory[i] = false;
                }
            }

            int randomIndexIndex = this.randomGenerator.Next(this.pictureFrameIndices.Count);
            while (this.pictureFrameFlipHistory[randomIndexIndex])
            {
                randomIndexIndex = this.randomGenerator.Next(this.pictureFrameIndices.Count);
            }
            this.pictureFrameFlipHistory[randomIndexIndex] = true;

            int randomTileIndex = this.pictureFrameIndices[randomIndexIndex];

            CustomMosaicTilePart frameTile = this.tilesPanel.Children[randomTileIndex] as CustomMosaicTilePart;
            frameTile.SizeFactor = 2;
            frameTile.ImagePartIndex = randomIndexIndex;

            frameTile.Flip(this.pictureCurrentFrameImage);

            bool allFlipped = true;
            foreach (KeyValuePair<int, bool> pair in this.pictureFrameFlipHistory)
            {
                allFlipped = allFlipped && pair.Value;
            }
            if (allFlipped)
            {
                this.pictureFrameCounter = 0;
                this.pictureFrameFlipHistory.Clear();
                this.pictureFrameIndices.Clear();
            }
        }

        private void ProcessIndividualTile()
        {
            this.pictureFrameCounter++;

            int randomIndex = this.randomGenerator.Next(this.tilesPanel.Children.Count);
            CustomMosaicTilePart tile = this.tilesPanel.Children[randomIndex] as CustomMosaicTilePart;
            tile.ImagePartIndex = null;

            if (tile.IsContentDisplayed)
            {
                tile.Flip(null);
            }
            else
            {
                tile.Flip(this.GetRandomImageSource());
            }
        }

        private void FlipRow()
        {
            if (this.rowIndices.Count == 0)
            {
                this.FillRowIndicesCollection();
                this.pictureCurrentFrameImage = this.GetRandomImageSource();
            }

            int rowIndex = this.rowIndices[this.randomGenerator.Next(this.rowIndices.Count)];

            for (int i = 0; i < 3; ++i)
            {
                int imagePartIndex = (rowIndex * 3) + i;
                CustomMosaicTilePart tile = this.tilesPanel.Children[imagePartIndex] as CustomMosaicTilePart;
                tile.SizeFactor = 3;
                tile.ImagePartIndex = imagePartIndex;

                if (tile.IsContentDisplayed)
                {
                    tile.Flip(null);
                }
                else
                {
                    tile.Flip(this.pictureCurrentFrameImage);
                }
            }

            this.rowIndices.Remove(rowIndex);
        }

        private void FillRowIndicesCollection()
        {
            do
            {
                int nextIndex = this.randomGenerator.Next(3);
                while (this.rowIndices.Contains(nextIndex))
                {
                    nextIndex = this.randomGenerator.Next(3);
                }

                this.rowIndices.Add(nextIndex);
            }
            while (this.rowIndices.Count < 3);
        }
    }
}
