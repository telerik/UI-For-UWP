using Telerik.UI.Xaml.Controls.Data.ListView;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Data
{
    public partial class RadListView
    {
        /// <summary>
        /// Identifies the <see cref="IsActionOnSwipeEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsActionOnSwipeEnabledProperty =
            DependencyProperty.Register(nameof(IsActionOnSwipeEnabled), typeof(bool), typeof(RadListView), new PropertyMetadata(null, OnIsActionOnSwipeEnabled));

        /// <summary>
        /// Identifies the <see cref="SwipeActionContent"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SwipeActionContentProperty =
            DependencyProperty.Register(nameof(SwipeActionContent), typeof(object), typeof(RadListView), new PropertyMetadata(null, OnActionContentChanged));

        /// <summary>
        /// Identifies the <see cref="DragBehavior"/> dependency property.
        /// </summary>
        internal static readonly DependencyProperty DragBehaviorProperty =
            DependencyProperty.Register(nameof(DragBehavior), typeof(ListViewDragBehavior), typeof(RadListView), new PropertyMetadata(null, OnDragBehaviorChanged));

        private double itemSwipeThreshold = 40;
        private Thickness itemSwipeOffset = new Thickness(100);

        /// <summary>
        /// Gets or sets the item's swipe threshold.
        /// </summary>
        public double ItemSwipeThreshold
        {
            get
            {
                return this.itemSwipeThreshold;
            }
            set
            {
                if (this.itemSwipeThreshold != value)
                {
                    this.itemSwipeThreshold = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the item's swipe offset.
        /// </summary>
        public Thickness ItemSwipeOffset
        {
            get
            {
                return this.itemSwipeOffset;
            }
            set
            {
                if (this.itemSwipeOffset != value)
                {
                    this.itemSwipeOffset = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether swipe gesture will display the item actions.
        /// </summary>
        public bool IsActionOnSwipeEnabled
        {
            get
            {
                return this.isActionOnSwipeEnabledCache;
            }
            set
            {
                this.SetValue(IsActionOnSwipeEnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the content of the second item action control.
        /// </summary>
        public object SwipeActionContent
        {
            get
            {
                return this.GetValue(SwipeActionContentProperty);
            }
            set
            {
                this.SetValue(SwipeActionContentProperty, value);
            }
        }

        private static void OnActionContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var oldContent = e.OldValue;
            var newContent = e.NewValue;

            RadListView listView = d as RadListView;
            if (listView != null && listView.IsTemplateApplied)
            {
                listView.RebuildUI();
            }
        }

        private static void OnIsActionOnSwipeEnabled(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var listView = d as RadListView;
            listView.isActionOnSwipeEnabledCache = (bool)e.NewValue;
            listView.updateService.RegisterUpdate((int)UpdateFlags.AffectsContent);
        }

        private static void OnDragBehaviorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var oldBehavior = e.OldValue as ListViewDragBehavior;
            var newBehavior = e.NewValue as ListViewDragBehavior;

            if (oldBehavior != null)
            {
                oldBehavior.Owner = null;
            }

            if (newBehavior != null)
            {
                newBehavior.Owner = d as RadListView;
            }
        }
    }
}
