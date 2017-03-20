using System;
using System.Globalization;
using Telerik.Core;
using Telerik.UI.Automation.Peers;
using Telerik.UI.Xaml.Controls.Grid.Commands;
using Telerik.UI.Xaml.Controls.Primitives.Common;
using Telerik.UI.Xaml.Controls.Primitives.DragDrop;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    /// <summary>
    /// Defines the visual representation of a column header within a <see cref="RadDataGrid"/> control.
    /// </summary>
    [TemplatePart(Name = "PART_FilterButton", Type = typeof(InlineButton))]
    [TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Filtered", GroupName = "CommonStates")]
    public sealed partial class DataGridColumnHeader : RadControl
    {
        /// <summary>
        /// Identifies the <see cref="Content"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(nameof(Content), typeof(object), typeof(DataGridColumnHeader), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="FilterGlyphVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FilterGlyphVisibilityProperty =
            DependencyProperty.Register(nameof(FilterGlyphVisibility), typeof(Visibility), typeof(DataGridColumnHeader), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Identifies the <see cref="FilterGlyphVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ResizeHandleVisiblityProperty =
            DependencyProperty.Register(nameof(ResizeHandleVisiblity), typeof(Visibility), typeof(DataGridColumnHeader), new PropertyMetadata(Visibility.Collapsed, OnResizeHandleVisiblityChanged));

        /// <summary>
        /// Identifies the <see cref="SortDirection"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SortDirectionProperty =
            DependencyProperty.Register(nameof(SortDirection), typeof(SortDirection), typeof(DataGridColumnHeader), new PropertyMetadata(SortDirection.None));
        private Button filterButton;
        private double initialColumnResizeWidth;
        private bool isFiltered;
        private bool isResizing;
        private bool isSelected;
        private Size lastValidArrangeSize = new Size(0, 0);
        private Thumb thumb;
        private double totalResizeDragDelta;
        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridColumnHeader" /> class.
        /// </summary>
        public DataGridColumnHeader()
        {
            this.DefaultStyleKey = typeof(DataGridColumnHeader);
            DragDrop.SetAllowDrag(this, true);
        }

        /// <summary>
        /// Gets or sets the arbitrary content displayed by the header.
        /// </summary>
        public object Content
        {
            get
            {
                return this.GetValue(ContentProperty);
            }
            set
            {
                this.SetValue(ContentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the visibility of the filter glyph displayed on the right of the header.
        /// </summary>
        public Visibility FilterGlyphVisibility
        {
            get
            {
                return (Visibility)this.GetValue(FilterGlyphVisibilityProperty);
            }
            set
            {
                this.SetValue(FilterGlyphVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the visibility of the resize handle displayed on the right of the header.
        /// </summary>
        public Visibility ResizeHandleVisiblity
        {
            get
            {
                return (Visibility)this.GetValue(ResizeHandleVisiblityProperty);
            }
            set
            {
                this.SetValue(ResizeHandleVisiblityProperty, value);
            }
        }
        /// <summary>
        /// Gets or sets the <see cref="SortDirection"/> value for the header. Typically this value is updated by the owning <see cref="DataGridColumn"/>.
        /// </summary>
        public SortDirection SortDirection
        {
            get
            {
                return (SortDirection)this.GetValue(SortDirectionProperty);
            }
            set
            {
                this.SetValue(SortDirectionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the base Arrange routine may be used.
        /// Since we perform Arrange outside the standard Layout pass,
        /// there is a corner case where the header is sized to its DesiredSize first and then sized to a larger size, defined by the layouts.
        /// </summary>
        internal bool AllowArrange
        {
            get;
            set;
        }

        internal Size ArrangeSize
        {
            get
            {
                return this.lastValidArrangeSize;
            }
        }

        internal DataGridColumn Column
        {
            get;
            set;
        }

        internal Button FilterButton
        {
            get
            {
                return this.filterButton;
            }
        }

        internal bool IsFiltered
        {
            get
            {
                return this.isFiltered;
            }
            set
            {
                this.isFiltered = value;
                this.UpdateVisualState(this.IsTemplateApplied);
            }
        }

        internal bool IsResizing
        {
            get
            {
                return this.isResizing;
            }
        }

        internal bool IsSelected
        {
            get
            {
                return this.isSelected;
            }
            set
            {
                this.isSelected = value;

                this.UpdateResizeThumbVisibility();

                if (this.Owner != null)
                {
                    this.Owner.OnColumnHeaderSelectionChanged(this);
                }

                this.UpdateVisualState(this.IsTemplateApplied);
            }
        }

        internal RadDataGrid Owner
        {
            get;
            set;
        }
        /// <summary>
        /// Called when the Framework <see cref="M:OnApplyTemplate" /> is called. Inheritors should override this method should they have some custom template-related logic.
        /// This is done to ensure that the <see cref="P:IsTemplateApplied" /> property is properly initialized.
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            this.filterButton = this.GetTemplatePartField<Button>("PART_FilterButton");
            this.thumb = this.GetTemplatePartField<Thumb>("PART_ResizeHandle");

            applied = applied && this.filterButton != null && this.thumb != null;

            return applied;
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new DataGridColumnHeaderAutomationPeer(this);
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
            // A HACK to overcome the differences in the DesiredSize of the header and the ArrangeSize, coming from the NodePool.
            if (!this.AllowArrange)
            {
                return new Size(0, 0);
            }

            this.lastValidArrangeSize = base.ArrangeOverride(this.ArrangeRestriction);

            return this.lastValidArrangeSize;
        }

        /// <summary>
        /// Builds the current visual state for this instance.
        /// </summary>
        protected override string ComposeVisualStateName()
        {
            var visualStates = base.ComposeVisualStateName();

            if (this.isFiltered)
            {
                visualStates = "Filtered";
            }


            if (this.IsSelected)
            {
                visualStates += RadControl.VisualStateDelimiter + "Selected";
            }
            else
            {
                visualStates += RadControl.VisualStateDelimiter + "Unselected";
            }

            return visualStates;
        }

        /// <summary>
        /// Called before the Tapped event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            base.OnTapped(e);

            if (this.Owner != null)
            {
                this.Owner.OnColumnHeaderTap(this, e);
            }
        }

        /// <summary>
        /// Occurs when the <see cref="M:OnApplyTemplate" /> method has been called and the template is already successfully applied.
        /// </summary>
        protected override void OnTemplateApplied()
        {
            base.OnTemplateApplied();

            this.filterButton.Tapped += this.OnFilterButtonTapped;
            this.filterButton.Click += this.OnFilterButtonClicked;

            this.thumb.PointerPressed += Thumb_PointerPressed;
            this.thumb.IsDoubleTapEnabled = true;

            this.thumb.DragStarted += this.Thumb_DragStarted;
            this.thumb.DragDelta += this.Thumb_DragDelta;
            this.thumb.DragCompleted += this.Thumb_DragCompleted;
            this.thumb.Tapped += this.Thumb_Tapped;
            this.thumb.DoubleTapped += this.Thumb_DoubleTapped;

        }

        /// <inheritdoc/>
        protected override void UnapplyTemplateCore()
        {
            base.UnapplyTemplateCore();

            this.filterButton.Tapped -= this.OnFilterButtonTapped;
            this.filterButton.Click -= this.OnFilterButtonClicked;

            this.thumb.DragStarted -= this.Thumb_DragStarted;
            this.thumb.DragDelta -= this.Thumb_DragDelta;
            this.thumb.DragCompleted -= this.Thumb_DragCompleted;
            this.thumb.Tapped -= this.Thumb_Tapped;
            this.thumb.DoubleTapped -= this.Thumb_DoubleTapped;
        }

        private static void OnResizeHandleVisiblityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var header = d as DataGridColumnHeader;

            header.UpdateVisualState(true);
        }

        private void OnFilterButtonTapped(object sender, TappedRoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void OnFilterButtonClicked(object sender, RoutedEventArgs e)
        {
            if (this.Owner != null)
            {
                this.Owner.OnFilterButtonTap(this);
            }
        }

        private void Thumb_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if (this.Owner != null)
            {
                this.Owner.DragBehavior.OnColumnResizeHandleDoubleTapped(this.Column);
            }
        }

        private void Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (this.Owner != null)
            {
                this.isResizing = false;
                this.Owner.DragBehavior.OnColumnResizeEnded(this.Column, e.HorizontalChange);
            }
        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            totalResizeDragDelta += e.HorizontalChange;

            this.Owner.DragBehavior.OnColumnResizing(this.Column, this.initialColumnResizeWidth, totalResizeDragDelta);
        }

        private void Thumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            var id = this.Owner.ContentFlyout.Id;
            if (id != DataGridFlyoutId.ColumnHeader)
            {
                this.Owner.ContentFlyout.Hide(DataGridFlyoutId.All);
            }

            this.totalResizeDragDelta = 0;
            this.initialColumnResizeWidth = this.Column.ActualWidth;

            if (this.Owner != null)
            {
                if (!this.Owner.DragBehavior.CanStartResize(this.Column))
                {
                    this.thumb.CancelDrag();
                }

                this.Owner.DragBehavior.OnColumnResizeStarted(this.Column);
            }
        }

        private void Thumb_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            this.isResizing = true;
        }

        private void Thumb_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Owner.ContentFlyout.Hide(DataGridFlyoutId.All);

            e.Handled = true;
        }

        private void UpdateResizeThumbVisibility()
        {
            if (this.Owner != null && this.Owner.ColumnResizeHandleDisplayMode == DataGridColumnResizeHandleDisplayMode.OnColumnHeaderActionFlyoutOpen && !this.IsResizing)
            {
                this.ResizeHandleVisiblity = this.IsSelected ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }
}