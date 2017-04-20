using System;
using Telerik.UI.Automation.Peers;
using Telerik.UI.Xaml.Controls.Primitives.HubTile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Defines a hub tile that simulates the pictures tile on WP OS's start screen.
    /// </summary>
    [TemplatePart(Name = "PART_FlipControl", Type = typeof(FlipControl))]
    [TemplatePart(Name = "PART_LayoutRoot", Type = typeof(Canvas))]
    [TemplatePart(Name = "PART_Panel", Type = typeof(StackPanel))]
    [TemplatePart(Name = "PART_FirstContent", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_FirstImage", Type = typeof(Image))]
    [TemplatePart(Name = "PART_SecondImage", Type = typeof(Image))]
    public class RadPictureRotatorHubTile : PictureHubTile
    {
        private UIElement panel;
        private FrameworkElement firstContentContainer;

        private Storyboard globalContentAnimation = new Storyboard();
        private DoubleAnimation moveUpGlobalAnimation;

        private Storyboard localContentAnimation = new Storyboard();
        private DoubleAnimation moveUpLocalAnimation;

        private Image firstImage;
        private Image secondImage;

        private int? lastPictureIndex;

        /// <summary>
        /// Initializes a new instance of the RadPictureRotatorHubTile class.
        /// </summary>
        public RadPictureRotatorHubTile()
        {
            this.DefaultStyleKey = typeof(RadPictureRotatorHubTile);

            this.moveUpGlobalAnimation = new DoubleAnimation();
            this.moveUpGlobalAnimation.Duration = TimeSpan.FromSeconds(0.4);
            this.moveUpGlobalAnimation.EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseInOut };
            this.moveUpGlobalAnimation.From = 0;
            Storyboard.SetTargetProperty(this.moveUpGlobalAnimation, "(Canvas.Top)");

            this.moveUpLocalAnimation = new DoubleAnimation();
            this.moveUpLocalAnimation.BeginTime = TimeSpan.FromSeconds(0.3);
            this.moveUpLocalAnimation.Duration = TimeSpan.FromSeconds(8);
            this.moveUpLocalAnimation.From = 0;
            Storyboard.SetTargetProperty(this.moveUpLocalAnimation, "(Canvas.Top)");

            this.localContentAnimation.Children.Add(this.moveUpLocalAnimation);

            this.globalContentAnimation.Children.Add(this.moveUpGlobalAnimation);
            this.globalContentAnimation.Completed += this.OnCurrentAnimationCompleted;
        }

        /// <summary>
        /// Gets a value of the secondImage displayed. Exposed for testing purposes.
        /// </summary>
        internal Image SecondImage
        {
            get
            {
                return this.secondImage;
            }
        }

        /// <summary>
        /// Gets a value of the firstImage displayed. Exposed for testing purposes.
        /// </summary>
        internal Image FirstImage
        {
            get
            {
                return this.firstImage;
            }
        }

        /// <summary>
        /// Gets a value indicating whether a rectangle clip is set on the LayoutRoot.
        /// </summary>
        protected override bool ShouldClip
        {
            get
            {
                return true;
            }
        }

        internal void UpdateImages()
        {
            if (!this.IsTemplateApplied || !this.IsLoaded)
            {
                return;
            }

            this.firstContentContainer.LayoutUpdated += this.OnFirstContentLayoutUpdated;

            Canvas.SetTop(this.panel, 0);
            Canvas.SetTop(this.firstContentContainer, 0);

            ImageSource tmp = this.secondImage.Source;

            this.secondImage.Source = null;
            this.firstImage.Source = tmp;

            this.secondImage.Source = this.GetRandomImageSource();
        }

        internal override void UpdateNextImageOnSourceChange()
        {
            if (!this.IsTemplateApplied || !this.IsLoaded)
            {
                return;
            }

            this.secondImage.Source = this.GetRandomImageSource();
        }

        /// <summary>
        /// A virtual callback that is called periodically when the tile is no frozen. It can be used to
        /// update the tile visual states or other necessary operations.
        /// </summary>
        protected internal override void Update(bool animate, bool updateIsFlipped)
        {
            if (this.BackContent != null || this.BackContentTemplate != null)
            {
                base.Update(animate, updateIsFlipped);
            }

            if (this.firstImage == null)
            {
                return;
            }

            // local moveup animation is running, stop it first
            if (this.localContentAnimation.GetCurrentState() == ClockState.Active)
            {
                this.localContentAnimation.SkipToFill();
            }

            this.moveUpGlobalAnimation.To = -this.Height;
            this.globalContentAnimation.Begin();
        }

        /// <summary>
        /// Retrieves the ControlTemplate parts.
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            this.panel = this.GetTemplatePartField<UIElement>("PART_Panel");
            applied = applied && this.panel != null;

            this.firstContentContainer = this.GetTemplatePartField<FrameworkElement>("PART_FirstContent");
            applied = applied && this.firstContentContainer != null;

            this.firstImage = this.GetTemplatePartField<Image>("PART_FirstImage");
            applied = applied && this.firstImage != null;

            this.secondImage = this.GetTemplatePartField<Image>("PART_SecondImage");
            applied = applied && this.secondImage != null;

            return applied;
        }

        /// <summary>
        /// Occurs when the <see cref="M:OnApplyTemplate"/> method has been called and the template is already successfully applied.
        /// </summary>
        protected override void OnTemplateApplied()
        {
            base.OnTemplateApplied();

            Storyboard.SetTarget(this.moveUpLocalAnimation, this.firstContentContainer);
            Storyboard.SetTarget(this.moveUpGlobalAnimation, this.panel);
        }

        /// <summary>
        /// Should be overridden in descendant classes to indicate if the same image can be displayed
        /// many times in a row.
        /// </summary>
        /// <param name="index">The index of the new image.</param>
        /// <returns>Returns true if the image can be repeated and false otherwise.</returns>
        protected override bool IsNewIndexValid(int index)
        {
            if (this.lastPictureIndex == null)
            {
                this.lastPictureIndex = index;
                return true;
            }

            return index != this.lastPictureIndex.Value;
        }

        /// <summary>
        /// Should be overridden in descendant classes to generate the new index from the picture collection.
        /// </summary>
        /// <param name="count">The length of the collection.</param>
        /// <returns>Returns new index different from previous.</returns>
        protected override int GetNewIndex(int count)
        {
            this.lastPictureIndex = base.GetNewIndex(count);
            return this.lastPictureIndex.Value;
        }

        /// <summary>
        /// Called within the handler of the <see cref="E:Loaded"/> event. Allows inheritors to provide their specific logic.
        /// </summary>
        protected override void LoadCore()
        {
            base.LoadCore();

            if (!this.IsTemplateApplied)
            {
                return;
            }

            this.firstImage.Source = this.GetRandomImageSource();
            this.secondImage.Source = this.GetRandomImageSource();
        }

        /// <inheritdoc />
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RadPictureRotatorHubTileAutomationPeer(this);
        }

        private void OnCurrentAnimationCompleted(object sender, object args)
        {
            this.UpdateImages();
        }

        private void OnFirstContentLayoutUpdated(object sender, object args)
        {
            this.firstContentContainer.LayoutUpdated -= this.OnFirstContentLayoutUpdated;

            double heightDiff = this.firstContentContainer.DesiredSize.Height - this.Height;
            if (heightDiff < 0)
            {
                return;
            }

            if (heightDiff > this.Height)
            {
                heightDiff = this.Height;
            }

            this.moveUpLocalAnimation.To = -heightDiff;
            this.localContentAnimation.Begin();
        }
    }
}
