using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Primitives.HubTile
{
    /// <summary>
    /// A small rectangle that has two sides and can flip between the two with a swivel animation.
    /// </summary>
    [TemplateVisualState(Name = "Front", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Back", GroupName = "CommonStates")]
    public class MosaicTilePart : RadControl
    {
        /// <summary>
        /// Identifies the FrontContent dependency property.
        /// </summary>
        public static readonly DependencyProperty FrontContentProperty =
            DependencyProperty.Register(nameof(FrontContent), typeof(object), typeof(MosaicTilePart), new PropertyMetadata(null, OnFrontContentChanged));

        /// <summary>
        /// Identifies the FrontContent dependency property.
        /// </summary>
        public static readonly DependencyProperty BackContentProperty =
            DependencyProperty.Register(nameof(BackContent), typeof(object), typeof(MosaicTilePart), new PropertyMetadata(null, OnBackContentChanged));

        /// <summary>
        /// Identifies the FrontContentTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty FrontContentTemplateProperty =
            DependencyProperty.Register(nameof(FrontContentTemplate), typeof(DataTemplate), typeof(MosaicTilePart), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the BackContentTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty BackContentTemplateProperty =
            DependencyProperty.Register(nameof(BackContentTemplate), typeof(DataTemplate), typeof(MosaicTilePart), new PropertyMetadata(null));

        private bool isInFrontState;

        /// <summary>
        /// Initializes a new instance of the <see cref="MosaicTilePart"/> class.
        /// </summary>
        public MosaicTilePart()
        {
            this.DefaultStyleKey = typeof(MosaicTilePart);
        }

        /// <summary>
        /// Gets or sets the content on the front side of the flip tile.
        /// </summary>
        public object FrontContent
        {
            get
            {
                return this.GetValue(MosaicTilePart.FrontContentProperty);
            }

            set
            {
                this.SetValue(MosaicTilePart.FrontContentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the content on the back side of the flip tile.
        /// </summary>
        public object BackContent
        {
            get
            {
                return this.GetValue(MosaicTilePart.BackContentProperty);
            }

            set
            {
                this.SetValue(MosaicTilePart.BackContentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the front content template.
        /// </summary>
        public DataTemplate FrontContentTemplate
        {
            get
            {
                return (DataTemplate)this.GetValue(MosaicTilePart.FrontContentTemplateProperty);
            }

            set
            {
                this.SetValue(MosaicTilePart.FrontContentTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the back content template.
        /// </summary>
        public DataTemplate BackContentTemplate
        {
            get
            {
                return (DataTemplate)this.GetValue(MosaicTilePart.BackContentTemplateProperty);
            }

            set
            {
                this.SetValue(MosaicTilePart.BackContentTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the flip tile is in its front state.
        /// </summary>
        public bool IsInFrontState
        {
            get
            {
                return this.isInFrontState;
            }
        }

        /// <summary>
        /// Flips the tile to its other side.
        /// </summary>
        public void Flip()
        {
            if (this.isInFrontState)
            {
                this.GoToBackState(true);
            }
            else
            {
                this.GoToFrontState(true);
            }
        }

        /// <summary>
        /// Occurs when the <see cref="M:OnApplyTemplate"/> method has been called and the template is already successfully applied.
        /// </summary>
        protected override void OnTemplateApplied()
        {
            base.OnTemplateApplied();

            this.GoToBackState(false);
        }

        /// <summary>
        /// A virtual callback that is invoked when the FrontContent property changes.
        /// </summary>
        /// <param name="newContent">The new FrontContent.</param>
        /// <param name="oldContent">The old FrontContent.</param>
        protected virtual void OnFrontContentChanged(object newContent, object oldContent)
        {
            this.GoToFrontState(true);
        }

        /// <summary>
        /// A virtual callback that is invoked when the BackContent property changes.
        /// </summary>
        /// <param name="newContent">The new BackContent.</param>
        /// <param name="oldContent">The old BackContent.</param>
        protected virtual void OnBackContentChanged(object newContent, object oldContent)
        {
            this.GoToBackState(true);
        }

        private static void OnFrontContentChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            MosaicTilePart tile = sender as MosaicTilePart;
            tile.OnFrontContentChanged(args.NewValue, args.OldValue);
        }

        private static void OnBackContentChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            MosaicTilePart tile = sender as MosaicTilePart;
            tile.OnBackContentChanged(args.NewValue, args.OldValue);
        }

        private static object CreateRandomlyDesaturatedBackColor()
        {
            return new Border() { Background = new SolidColorBrush(Colors.Blue) };
        }

        private void GoToFrontState(bool animate)
        {
            // this.Dispatcher.BeginInvoke(() => VisualStateManager.GoToState(this, "Front", animate));
            VisualStateManager.GoToState(this, "Front", animate);
            this.isInFrontState = true;
        }

        private void GoToBackState(bool animate)
        {
            // this.Dispatcher.BeginInvoke(() => VisualStateManager.GoToState(this, "Back", animate));
            VisualStateManager.GoToState(this, "Back", animate);
            this.isInFrontState = false;
        }
    }
}
