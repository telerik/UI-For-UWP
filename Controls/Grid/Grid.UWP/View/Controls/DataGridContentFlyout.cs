using System;
using Telerik.UI.Xaml.Controls.Grid.Commands;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    public sealed class DataGridContentFlyout : RadControl
    {
        /// <summary>
        /// Identifies the <see cref="IsOpen"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(DataGridContentFlyout), new PropertyMetadata(false, OnIsOpenChanged));

        /// <summary>
        /// Identifies the <see cref="IsOpen"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.Register("HorizontalOffset", typeof(double), typeof(DataGridContentFlyout), new PropertyMetadata(0d, OnHorizontalOffsetChanged));

        /// <summary>
        /// Identifies the <see cref="VerticalOffset"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.Register("VerticalOffset", typeof(double), typeof(DataGridContentFlyout), new PropertyMetadata(0d, OnVerticalOffsetChanged));

        /// <summary>
        /// Identifies the <see cref="Child"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty ChildProperty =
            DependencyProperty.Register("Child", typeof(FrameworkElement), typeof(DataGridContentFlyout), new PropertyMetadata(null, OnChildChanged));

        // Using a DependencyProperty as the backing store for Id.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty IdProperty = 
            DependencyProperty.Register("Id", typeof(DataGridFlyoutId), typeof(DataGridContentFlyout), new PropertyMetadata(DataGridFlyoutId.All, OnIdChanged));

        private Popup popup = new Popup() { ChildTransitions = new TransitionCollection(), IsLightDismissEnabled = false };   
        private Border content;
        private Storyboard opacityAnimationStoryboard = new Storyboard();
        private DoubleAnimation opacityAnimation;
        private TimeSpan AnimationDuration = TimeSpan.FromMilliseconds(150);

        private DoubleAnimation cellFlyoutHideTimeOutAnimation;
        private Storyboard cellFlyoutHideTimeOutAnimationBoard;
        private bool automaticallyHideFlyout = false;

        internal RadDataGrid Owner { get; set; }

        public event EventHandler<object> Closed;
        public event EventHandler<object> Opened;
        
        public DataGridContentFlyout()
        {
            this.DefaultStyleKey = typeof(DataGridContentFlyout);
        }

        public double HorizontalOffset
        {
            get { return (double)GetValue(HorizontalOffsetProperty); }
            set { SetValue(HorizontalOffsetProperty, value); }
        }

        public double VerticalOffset
        {
            get { return (double)GetValue(VerticalOffsetProperty); }
            set { SetValue(VerticalOffsetProperty, value); }
        }

        public FrameworkElement Child
        {
            get { return (FrameworkElement)GetValue(ChildProperty); }
            private set { SetValue(ChildProperty, value); }
        }

        public bool IsOpen
        {
            get
            {
                return (bool)GetValue(IsOpenProperty);
            }
            private set
            {
                SetValue(IsOpenProperty, value);
            }
        }

        internal DataGridFlyoutId Id
        {
            get { return (DataGridFlyoutId)GetValue(IdProperty); }
            set { SetValue(IdProperty, value); }
        }

        internal void Show(DataGridFlyoutId id, FrameworkElement content, bool automaticallyHideFlyout = false)
        {
            this.Hide(DataGridFlyoutId.All);
            this.automaticallyHideFlyout = automaticallyHideFlyout;
            this.Child = content;         
            this.Id = id;
            this.UpdateFlyoutPosition();
            this.IsOpen = true;
        }

        private void UpdateFlyoutPosition()
        {
            switch (this.Id)
            {
                case DataGridFlyoutId.Cell:
                    var cell = this.Child.DataContext as GridCellModel;
                    var groupPanelWidth = this.Owner.GroupPanelPosition == GroupPanelPosition.Left ? this.Owner.ServicePanel.ActualWidth - 2 : 0;

                    var offsetY = cell.LayoutSlot.Y + cell.Column.HeaderControl.ActualHeight - this.Owner.ScrollViewer.VerticalOffset;
                    var offsetX = cell.layoutSlot.X + groupPanelWidth - this.Owner.ScrollViewer.HorizontalOffset;

                    this.HorizontalOffset = offsetX;
                    this.VerticalOffset = offsetY;
                    break;
                case DataGridFlyoutId.ColumnChooser:
                    this.HorizontalOffset = this.Owner.ActualWidth - this.Child.ActualWidth - this.Owner.BorderThickness.Left;
                    this.VerticalOffset = 0;
                    break;
                case DataGridFlyoutId.ColumnHeader:
                    this.PositionColumnDataOperationsFlyout();
                    break;
                case DataGridFlyoutId.EditorLeft:
                    this.HorizontalOffset = 0 - this.Owner.BorderThickness.Left;
                    this.VerticalOffset = 0;
                    break;
                case DataGridFlyoutId.EditorRight:
                    this.HorizontalOffset = this.Owner.ActualWidth - this.Child.ActualWidth - this.Owner.BorderThickness.Left;
                    this.VerticalOffset = 0;
                    break;
                case DataGridFlyoutId.FilterButton:
                case DataGridFlyoutId.FlyoutFilterButton:
                    this.PositionFilterFlyout();
                    break;
                default:
                    break;
            }
        }

        internal void Hide(DataGridFlyoutId id)
        {
            if (this.Id == id || id == DataGridFlyoutId.All)
            {
                this.Child = null;
                this.IsOpen = false;
            }
        }

        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            this.content = this.GetTemplatePartField<Border>("PART_Content");
            applied = applied && this.content != null;

            return applied;
        }

        protected override void OnTemplateApplied()
        {
            base.OnTemplateApplied();

            this.content.Child = this.popup;
            this.popup.Closed += this.PopupClosed;
            this.popup.Opened += this.PopupOpened;
            this.Owner.SizeChanged += this.OwnerSizeChanged;

            this.opacityAnimation = new DoubleAnimation();
            this.opacityAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseInOut };
            this.opacityAnimation.From = 0;
            this.opacityAnimation.To = 1;
            this.opacityAnimation.Duration = this.AnimationDuration;
            this.opacityAnimation.FillBehavior = FillBehavior.Stop;
            Storyboard.SetTargetProperty(this.opacityAnimation, "Opacity");
            Storyboard.SetTarget(this.opacityAnimation, this.popup);
            this.opacityAnimationStoryboard.Children.Add(this.opacityAnimation);

            this.cellFlyoutHideTimeOutAnimation = new DoubleAnimation();
            this.cellFlyoutHideTimeOutAnimation.Duration = TimeSpan.FromSeconds(2);
            this.cellFlyoutHideTimeOutAnimationBoard = new Storyboard();
            Storyboard.SetTarget(this.cellFlyoutHideTimeOutAnimation, this.popup);
            Storyboard.SetTargetProperty(this.cellFlyoutHideTimeOutAnimation, "Opacity");
            this.cellFlyoutHideTimeOutAnimationBoard.Children.Add(this.cellFlyoutHideTimeOutAnimation);
        }

        private void CellTimeOutAnimationCompleted(object sender, object e)
        {
            this.cellFlyoutHideTimeOutAnimationBoard.Completed -= this.CellTimeOutAnimationCompleted;
            this.Hide(DataGridFlyoutId.All);
        }

        private void OwnerSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.Hide(DataGridFlyoutId.All);
        }

        protected override void UnapplyTemplateCore()
        {
            base.UnapplyTemplateCore();

            this.popup.Closed -= this.PopupClosed;
            this.popup.Opened -= this.PopupOpened;
            this.Owner.SizeChanged -= this.OwnerSizeChanged;
            this.content = null;
        }

        private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as DataGridContentFlyout;
            if (control != null)
            {
                if (!(bool)e.NewValue)
                {
                    control.automaticallyHideFlyout = false;
                    control.cellFlyoutHideTimeOutAnimationBoard.Completed -= control.CellTimeOutAnimationCompleted;
                    control.cellFlyoutHideTimeOutAnimationBoard.Stop();
                }
                else if (control.Id == DataGridFlyoutId.Cell) 
                {
                    control.opacityAnimationStoryboard.Stop();
                    control.opacityAnimationStoryboard.Begin();
                }
                control.popup.IsOpen = (bool)e.NewValue;
            }
        }

        private void DataOperationsStoryBoardCompleted(object sender, object e)
        {
            this.popup.IsOpen = false;
            (sender as Storyboard).Completed -= this.DataOperationsStoryBoardCompleted;
        }

        private static void OnIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as DataGridContentFlyout;

            if (control != null)
            {
                if ((DataGridFlyoutId)e.OldValue != DataGridFlyoutId.ColumnChooser)
                {
                    control.Owner.Columns.CollectionChanged -= control.GridColumnsCollectionChanged;
                }

                if ((DataGridFlyoutId)e.NewValue != DataGridFlyoutId.ColumnChooser)
                {
                    control.Owner.Columns.CollectionChanged += control.GridColumnsCollectionChanged;
                }
            }

            control.UpdateTransitions();
        }

        private void GridColumnsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.Hide(DataGridFlyoutId.All);
        }

        private static void OnHorizontalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as DataGridContentFlyout;
            if (control != null)
            {
                control.popup.HorizontalOffset = (double)e.NewValue;
            }
        }

        private static void OnVerticalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as DataGridContentFlyout;
            if (control != null)
            {
                control.popup.VerticalOffset = (double)e.NewValue;
            }
        }

        private static void OnChildChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as DataGridContentFlyout;
            if (control != null && e.NewValue != e.OldValue)
            {
                control.popup.Child = (FrameworkElement)e.NewValue;

                control.cellFlyoutHideTimeOutAnimationBoard.Completed -= control.CellTimeOutAnimationCompleted;
                control.cellFlyoutHideTimeOutAnimationBoard.Stop();

                if (control.Child != null)
                {
                    control.Child.SizeChanged += control.ChildSizeChanged;
                    if (control.automaticallyHideFlyout)
                    {
                        control.Child.PointerExited += control.Child_PointerExited;
                    }
                }

                if (e.OldValue is FrameworkElement)
                {
                    (e.OldValue as FrameworkElement).SizeChanged -= control.ChildSizeChanged;
                    (e.OldValue as FrameworkElement).PointerExited -= control.Child_PointerExited;
                }
            }
        }

        private void Child_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            this.Child.PointerExited -= this.Child_PointerExited;
            this.cellFlyoutHideTimeOutAnimationBoard.Completed += this.CellTimeOutAnimationCompleted;
            this.cellFlyoutHideTimeOutAnimationBoard.Begin();
        }

        private void ChildSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.UpdateFlyoutPosition();     
        }

        private void PositionFilterFlyout()
        {
            var filteringFlyout = this.Child as DataGridFilteringFlyout;

            var thickness = this.Owner.BorderThickness.Left;
            GeneralTransform transform;
            Rect rect;

            switch (filteringFlyout.DisplayMode)
            {
                case FilteringFlyoutDisplayMode.Inline:

                    var button = filteringFlyout.Context.Column.HeaderControl.FilterButton;

                    transform = button.TransformToVisual(this.Owner);
                    rect = transform.TransformBounds(new Rect(0, 0, filteringFlyout.ActualWidth, 0));

                    this.HorizontalOffset = rect.X - filteringFlyout.ActualWidth + button.ActualWidth - thickness;
                    this.VerticalOffset = 0;

                    filteringFlyout.FilterGlyphWidth = button.ActualWidth;
                    filteringFlyout.FilterGlyphHeight = button.ActualHeight;
                    break;
                case FilteringFlyoutDisplayMode.Flyout:

                    transform = filteringFlyout.Context.Column.HeaderControl.TransformToVisual(this.Owner);
                    rect = transform.TransformBounds(new Rect(0, 0, filteringFlyout.ActualWidth, 0));

                    this.HorizontalOffset = Math.Max(-thickness, filteringFlyout.Context.Column.HeaderControl.ActualWidth / 2 + rect.X - filteringFlyout.ActualWidth / 2 - thickness);
                    var width = filteringFlyout.ActualWidth;
                    if (filteringFlyout.ActualWidth > this.Owner.ActualWidth)
                    {
                        filteringFlyout.Width = this.Owner.ActualWidth;
                        width = Math.Max(filteringFlyout.MinWidth, filteringFlyout.Width);
                    }

                    this.HorizontalOffset = Math.Min(this.HorizontalOffset, this.Owner.ActualWidth - width - thickness);
                    this.VerticalOffset = 0;

                    break;
                case FilteringFlyoutDisplayMode.Fill:
                    transform = filteringFlyout.TransformToVisual(this.Owner);
                    rect = transform.TransformBounds(new Rect(0, 0, filteringFlyout.ActualWidth, 0));

                    this.HorizontalOffset = - thickness;
                    this.VerticalOffset = 0;

                    break;
            }
        }

        private void PositionColumnDataOperationsFlyout()
        {
            var context = this.Child.DataContext as ActionContext;
            var header = context.Column.HeaderControl;
            var transform2 = header.TransformToVisual(this.Owner);
            var layoutSlot = transform2.TransformPoint(new Point(0, 0));

            var offsetX = layoutSlot.X + header.ArrangeSize.Width / 2 - this.Child.ActualWidth / 2;
            var offsetY = layoutSlot.Y + header.ArrangeSize.Height;

            this.HorizontalOffset = offsetX;
            this.VerticalOffset = offsetY;


            if (this.popup.IsOpen)
            {
                var currentOffsetX = (this.Owner).TransformToVisual(Window.Current.Content).TransformPoint(new Point(0, 0)).X + this.HorizontalOffset;

                if (currentOffsetX < 0)
                {
                    this.HorizontalOffset += -currentOffsetX;
                }

                else if (currentOffsetX + this.Child.ActualWidth > Window.Current.Bounds.Width)
                {
                    this.HorizontalOffset -= (currentOffsetX + this.Child.ActualWidth - Window.Current.Bounds.Width);
                }

                if (this.HorizontalOffset + this.Child.ActualWidth > this.Owner.ActualWidth)
                {
                    this.HorizontalOffset = this.Owner.ActualWidth - this.Child.ActualWidth;
                }
                else if (this.HorizontalOffset < 0)
                {
                    this.HorizontalOffset = 0;
                }
            }

            this.HorizontalOffset -= this.Owner.BorderThickness.Left;
        }

        private void UpdateTransitions()
        {
            this.popup.ChildTransitions.Clear();

            switch (this.Id)
            {
                case DataGridFlyoutId.EditorLeft:
                    this.popup.ChildTransitions.Add(new PaneThemeTransition() { Edge = EdgeTransitionLocation.Left });
                    break;
                case DataGridFlyoutId.ColumnHeader:
                    this.popup.ChildTransitions.Add(new PaneThemeTransition() { Edge = EdgeTransitionLocation.Top });
                    break;
                case DataGridFlyoutId.FlyoutFilterButton:
                    this.popup.ChildTransitions.Add(new PaneThemeTransition() { Edge = EdgeTransitionLocation.Top });
                    break;
                case DataGridFlyoutId.ColumnChooser:
                    if (this.Owner != null && this.Owner.ActualHeight < this.Owner.ActualWidth)
                    {
                    this.popup.ChildTransitions.Add(new PaneThemeTransition() { Edge = EdgeTransitionLocation.Right });
                    }
                    else
                    {
                        this.popup.ChildTransitions.Add(new PaneThemeTransition() { Edge = EdgeTransitionLocation.Top });
                    }
                    break;
                case DataGridFlyoutId.EditorRight:
                    this.popup.ChildTransitions.Add(new PaneThemeTransition() { Edge = EdgeTransitionLocation.Right });
                    break;
                default:
                    break;
            }
        }

        private void PopupOpened(object sender, object e)
        {
            var handler = this.Opened;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void PopupClosed(object sender, object e)
        {
            var handler = this.Closed;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
