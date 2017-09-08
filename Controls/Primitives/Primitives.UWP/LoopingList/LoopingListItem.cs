using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Primitives.LoopingList
{
    /// <summary>
    /// Represents a visual item that resides within a <see cref="LoopingPanel"/> instance.
    /// </summary>
    public class LoopingListItem : RadContentControl
    {
        /// <summary>
        /// Specifies the <see cref="LoopingListItem.IsSelected"/> property.
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(LoopingListItem), new PropertyMetadata(false, OnIsSelectedChanged));

        internal TranslateTransform translateTransform;
        internal Point translation;
        internal bool isHidden;

        private bool isEmpty = false;
        private LoopingPanel panel;
        private Rect arrangeRect;
        private bool isSelected;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoopingListItem"/> class.
        /// </summary>
        public LoopingListItem()
        {
            this.DefaultStyleKey = typeof(LoopingListItem);

            this.IsEnabledChanged += this.OnIsEnabledChanged;

            this.LogicalIndex = -1;
            this.VisualIndex = -1;
            
            this.translateTransform = new TranslateTransform();
            this.RenderTransform = this.translateTransform;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the visual item is currently selected.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return this.isSelected;
            }
            set
            {
                this.SetValue(IsSelectedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the logical index in the owning LoopingPanel, represented by this item. 
        /// </summary>
        public int LogicalIndex { get; protected internal set; }

        /// <summary>
        /// Gets the Rect used by the owning LoopingPanel to arrange this item.
        /// </summary>
        public Rect ArrangeRect
        {
            get
            {
                return this.arrangeRect;
            }
            internal set
            {
                this.arrangeRect = value;
            }
        }

        /// <summary>
        /// Gets the amount of pixels this item is translated vertically relative to its owning panel.
        /// </summary>
        public double VerticalOffset
        {
            get
            {
                return this.arrangeRect.Y + this.translation.Y;
            }
        }

        /// <summary>
        /// Gets the amount of pixels this item is translated horizontally relative to its owning panel.
        /// </summary>
        public double HorizontalOffset
        {
            get
            {
                return this.arrangeRect.X + this.translation.X;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the item is currently expanded (its owning list is in Expanded state).
        /// </summary>
        public bool IsExpanded
        {
            get
            {
                if (this.panel != null && this.panel.Owner != null)
                {
                    return this.panel.Owner.IsExpanded;
                }

                return true;
            }
        }

        internal int VisualIndex { get; set; }

        internal bool IsOwnerFocused
        {
            get
            {
                return this.panel != null && this.panel.Owner != null && this.panel.Owner.FocusState != FocusState.Unfocused;
            }
        }

        /// <summary>
        /// Gets the <see cref="LoopingPanel"/> instance where this item resides.
        /// </summary>
        internal LoopingPanel Panel
        {
            get
            {
                return this.panel;
            }
        }

        internal bool IsEmpty
        {
            get
            {
                return this.isEmpty;
            }
        }

        internal void SetIsEmpty(bool empty)
        {
            this.isEmpty = empty;
            this.Visibility = empty ? Visibility.Collapsed : Visibility.Visible;
        }

        internal virtual void SetIsHidden(bool hidden)
        {
            if (hidden == this.isHidden)
            {
                return;
            }

            this.isHidden = hidden;
            this.Opacity = hidden ? 0 : 1;
        }

        internal virtual void Attach(LoopingPanel owner)
        {
            this.panel = owner;
            this.UpdateVisualState(false);
        }

        /// <summary>
        /// Applies the desired vertical offset by setting a TranslateTransform.Y value.
        /// </summary>
        internal void SetVerticalOffset(double offset)
        {
            this.translation.Y = offset;
            this.translateTransform.Y = offset;
        }

        /// <summary>
        /// Applies the desired horizontal offset by setting a TranslateTransform.X value.
        /// </summary>
        internal void SetHorizontalOffset(double offset)
        {
            this.translation.X = offset;
            this.translateTransform.X = offset;
        }

        /// <summary>
        /// Builds the current visual state that is valid for the item.
        /// </summary>
        protected override string ComposeVisualStateName()
        {
            if (this.isSelected)
            {
                return this.IsOwnerFocused ? "Selected,Focused" : "Selected,NotFocused";
            }

            if (this.IsExpanded)
            {
                return this.IsEnabled ? "Expanded,NotFocused" : "Disabled,NotFocused";
            }

            return "Collapsed";
        }

        /// <summary>
        /// Applies the specified visual state as current.
        /// </summary>
        /// <param name="state">The new visual state.</param>
        /// <param name="animate">True to use transitions, false otherwise.</param>
        protected override void SetVisualState(string state, bool animate)
        {
            if (this.isSelected)
            {
                animate = false;
            }

            base.SetVisualState(state, animate);
        }

        /// <inheritdoc />
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new Automation.Peers.LoopingListItemAutomationPeer(this);
        }

        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LoopingListItem item = d as LoopingListItem;
            item.isSelected = (bool)e.NewValue;
            item.UpdateVisualState(false);
        }

        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.UpdateVisualState(false);
        }
    }
}
