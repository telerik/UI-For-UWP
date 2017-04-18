using Telerik.UI.Automation.Peers;
using Telerik.UI.Xaml.Controls.Primitives.Pagination;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Represents a control that gives visual means for tracking the current position within a <see cref="Selector"/> instance.
    /// </summary>
    [TemplatePart(Name = "PART_IndexLabelControl", Type = typeof(PaginationIndexLabelControl))]
    [TemplatePart(Name = "PART_ThumbnailList", Type = typeof(PaginationListControl))]
    [TemplatePart(Name = "PART_LeftArrow", Type = typeof(PaginationButton))]
    [TemplatePart(Name = "PART_RightArrow", Type = typeof(PaginationButton))]
    [TemplatePart(Name = "PART_LayoutRoot", Type = typeof(Grid))]
    public class RadPaginationControl : RadControl
    {
        /// <summary>
        /// Identifies the <see cref="ListItemTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ListItemTemplateProperty =
            DependencyProperty.Register(nameof(ListItemTemplate), typeof(DataTemplate), typeof(RadPaginationControl), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="PageProvider"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PageProviderProperty =
            DependencyProperty.Register(nameof(PageProvider), typeof(Selector), typeof(RadPaginationControl), new PropertyMetadata(null, OnPageProviderChanged));

        /// <summary>
        /// Identifies the <see cref="DisplayMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayModeProperty =
            DependencyProperty.Register(nameof(DisplayMode), typeof(PaginationControlDisplayMode), typeof(RadPaginationControl), new PropertyMetadata(PaginationControlDisplayMode.All, OnDisplayModeChanged));

        /// <summary>
        /// Identifies the <see cref="LeftArrowTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LeftArrowTemplateProperty =
            DependencyProperty.Register(nameof(LeftArrowTemplate), typeof(DataTemplate), typeof(RadPaginationControl), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="RightArrowTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RightArrowTemplateProperty =
            DependencyProperty.Register(nameof(RightArrowTemplate), typeof(DataTemplate), typeof(RadPaginationControl), new PropertyMetadata(null));

        private const string LayoutRootName = "PART_LayoutRoot";
        private const string ThumbnailListName = "PART_ThumbnailList";
        private const string IndexLabelControlName = "PART_IndexLabelControl";
        private const string LeftArrowControlName = "PART_LeftArrow";
        private const string RightArrowControlName = "PART_RightArrow";

        private Selector pageProviderCache;

        private PaginationListControl thumbnailList;
        private PaginationButton leftArrowPresenter;
        private PaginationButton rightArrowPresenter;
        private PaginationIndexLabelControl indexLabelControl;
        private Grid layoutRoot;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadPaginationControl"/> class.
        /// </summary>
        public RadPaginationControl()
        {
            this.DefaultStyleKey = typeof(RadPaginationControl);

            this.IsEnabledChanged += this.OnIsEnabledChanged;
        }

        /// <summary>
        /// Gets or sets a value indication which parts of the control are currently visible.
        /// </summary>
        public PaginationControlDisplayMode DisplayMode
        {
            get
            {
                return (PaginationControlDisplayMode)this.GetValue(DisplayModeProperty);
            }
            set
            {
                this.SetValue(DisplayModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> instance that defines the appearance of the items within of the list control.
        /// </summary>
        public DataTemplate ListItemTemplate
        {
            get
            {
                return this.GetValue(ListItemTemplateProperty) as DataTemplate;
            }
            set
            {
                this.SetValue(ListItemTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> instance that defines the appearance of the left arrow of the control.
        /// </summary>
        public DataTemplate LeftArrowTemplate
        {
            get
            {
                return this.GetValue(LeftArrowTemplateProperty) as DataTemplate;
            }
            set
            {
                this.SetValue(LeftArrowTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> instance that defines the appearance of the right arrow of the control.
        /// </summary>
        public DataTemplate RightArrowTemplate
        {
            get
            {
                return this.GetValue(RightArrowTemplateProperty) as DataTemplate;
            }
            set
            {
                this.SetValue(RightArrowTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the current <see cref="Selector"/> instance targeted by this pagination control.
        /// </summary>
        public Selector PageProvider
        {
            get
            {
                return this.pageProviderCache as Selector;
            }
            set
            {
                this.SetValue(PageProviderProperty, value);
            }
        }

        internal ItemCollection ListSource
        {
            get
            {
                if (this.PageProvider == null)
                {
                    return null;
                }

                return this.PageProvider.Items;
            }
        }

        /// <summary>
        /// Gets the PART_LayoutRoot visual of the control template. Exposed for testing purposes.
        /// </summary>
        internal Grid LayoutRoot
        {
            get
            {
                return this.layoutRoot;
            }
        }

        /// <summary>
        /// Gets the PART_ThumbnailList visual of the control template. Exposed for testing purposes.
        /// </summary>
        internal PaginationListControl ThumbnailList
        {
            get
            {
                return this.thumbnailList;
            }
        }

        /// <summary>
        /// Gets the PART_IndexLabelControl visual of the control template. Exposed for testing purposes.
        /// </summary>
        internal PaginationIndexLabelControl IndexLabelControl
        {
            get
            {
                return this.indexLabelControl;
            }
        }

        /// <summary>
        /// Gets the PART_LeftArrow visual of the control template. Exposed for testing purposes.
        /// </summary>
        internal PaginationButton LeftArrowPresenter
        {
            get
            {
                return this.leftArrowPresenter;
            }
        }

        /// <summary>
        /// Gets the PART_RightArrow visual of the control template. Exposed for testing purposes.
        /// </summary>
        internal PaginationButton RightArrowPresenter
        {
            get
            {
                return this.rightArrowPresenter;
            }
        }

        /// <summary>
        /// Called when the Framework OnApplyTemplate is called. Inheritors should override this method should they have some custom template-related logic.
        /// This is done to ensure that the <see cref="P:IsTemplateApplied"/> property is properly initialized.
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            var applied = base.ApplyTemplateCore();

            if (this.leftArrowPresenter != null)
            {
                this.leftArrowPresenter.Click -= this.OnLeftArrowPressed;
            }
            if (this.rightArrowPresenter != null)
            {
                this.rightArrowPresenter.Click -= this.OnRightArrowPressed;
            }

            this.layoutRoot = this.GetTemplatePartField<Grid>(LayoutRootName);
            applied = applied && this.layoutRoot != null;

            this.thumbnailList = this.GetTemplatePartField<PaginationListControl>(ThumbnailListName);
            applied = applied && this.thumbnailList != null;

            this.thumbnailList.Attach(this);

            this.indexLabelControl = this.GetTemplatePartField<PaginationIndexLabelControl>(IndexLabelControlName);
            applied = applied && this.indexLabelControl != null;

            this.leftArrowPresenter = this.GetTemplatePartField<PaginationButton>(LeftArrowControlName);
            applied = applied && this.leftArrowPresenter != null;

            this.leftArrowPresenter.Click += this.OnLeftArrowPressed;

            this.rightArrowPresenter = this.GetTemplatePartField<PaginationButton>(RightArrowControlName);
            applied = applied && this.rightArrowPresenter != null;

            this.rightArrowPresenter.Click += this.OnRightArrowPressed;

            return applied;
        }

        /// <summary>
        /// Occurs when the OnApplyTemplate() method has been called and the template is already successfully applied.
        /// </summary>
        protected override void OnTemplateApplied()
        {
            base.OnTemplateApplied();

            this.UpdateIndexControl();
            this.UpdateDisplayMode(this.DisplayMode);
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RadPaginationControlAutomationPeer(this);
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            if (e.OriginalSource is RadPaginationControl)
            {
                var peer = FrameworkElementAutomationPeer.FromElement(this) as RadPaginationControlAutomationPeer;
                if (peer != null)
                {
                    peer.RaiseAutomationEvent(AutomationEvents.LiveRegionChanged);
                }
            }
            base.OnGotFocus(e);
        }

        private static void OnPageProviderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadPaginationControl control = d as RadPaginationControl;
            control.ChangeProvider(e.NewValue as Selector);
        }

        private static void OnDisplayModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadPaginationControl control = d as RadPaginationControl;
            if (!control.IsTemplateApplied)
            {
                return;
            }

            control.UpdateDisplayMode((PaginationControlDisplayMode)e.NewValue);
        }

        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.UpdateArrowsIsEnabledState();
        }

        private void UpdateDisplayMode(PaginationControlDisplayMode parts)
        {
            if ((parts & PaginationControlDisplayMode.Arrows) == PaginationControlDisplayMode.Arrows)
            {
                this.leftArrowPresenter.Visibility = Visibility.Visible;
                this.rightArrowPresenter.Visibility = Visibility.Visible;
            }
            else
            {
                this.leftArrowPresenter.Visibility = Visibility.Collapsed;
                this.rightArrowPresenter.Visibility = Visibility.Collapsed;
            }

            if ((parts & PaginationControlDisplayMode.Thumbnails) == PaginationControlDisplayMode.Thumbnails)
            {
                this.thumbnailList.Visibility = Visibility.Visible;
            }
            else
            {
                this.thumbnailList.Visibility = Visibility.Collapsed;
            }

            if ((parts & PaginationControlDisplayMode.IndexLabel) == PaginationControlDisplayMode.IndexLabel)
            {
                this.indexLabelControl.Visibility = Visibility.Visible;
            }
            else
            {
                this.indexLabelControl.Visibility = Visibility.Collapsed;
            }
        }

        private void ChangeProvider(Selector provider)
        {
            this.DetachFromProvider(this.PageProvider);
            this.pageProviderCache = provider;
            this.AttachToProvider(this.PageProvider);

            if (this.IsTemplateApplied)
            {
                this.UpdateIndexControl();
            }
        }

        private void AttachToProvider(Selector provider)
        {
            if (provider != null)
            {
                provider.SelectionChanged += this.OnSelectionChanged;
                provider.Items.VectorChanged += this.OnSelectorItemsVectorChanged;
            }
        }

        private void DetachFromProvider(Selector provider)
        {
            if (provider != null)
            {
                provider.SelectionChanged -= this.OnSelectionChanged;

                if (provider.Items != null)
                {
                    provider.Items.VectorChanged -= this.OnSelectorItemsVectorChanged;
                }
            }
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.UpdateIndexControl();
        }

        private void OnSelectorItemsVectorChanged(IObservableVector<object> sender, IVectorChangedEventArgs args)
        {
            if (this.thumbnailList != null)
            {
                this.thumbnailList.OnOwnerSourceCollectionChanged(args);
            }
            this.UpdateIndexControl();
            ////TODO: update selectedIndex if needed. and count(verify that this is needed.)
        }

        private void UpdateIndexControl()
        {
            if (this.indexLabelControl == null)
            {
                return;
            }

            if (this.pageProviderCache != null)
            {
                this.indexLabelControl.SetValues(this.pageProviderCache.Items.Count, this.pageProviderCache.SelectedIndex + 1);

                this.thumbnailList.ItemsSource = this.ListSource;
                this.thumbnailList.SelectedIndex = this.pageProviderCache.SelectedIndex;

                this.UpdateArrowsIsEnabledState();
            }
            else
            {
                this.indexLabelControl.SetValues(0, 0);
            }
        }

        private void UpdateArrowsIsEnabledState()
        {
            if (this.pageProviderCache.SelectedIndex + 1 == this.pageProviderCache.Items.Count)
            {
                this.RightArrowPresenter.IsEnabled = false;
            }
            else
            {
                this.RightArrowPresenter.IsEnabled = this.IsEnabled;
            }

            if (this.pageProviderCache.SelectedIndex == 0)
            {
                this.LeftArrowPresenter.IsEnabled = false;
            }
            else
            {
                this.LeftArrowPresenter.IsEnabled = this.IsEnabled;
            }
        }

        private void OnLeftArrowPressed(object sender, RoutedEventArgs e)
        {
            if (this.pageProviderCache != null && this.pageProviderCache.SelectedIndex > 0)
            {
                this.pageProviderCache.SelectedIndex--;
            }
        }

        private void OnRightArrowPressed(object sender, RoutedEventArgs e)
        {
            if (this.pageProviderCache != null && this.pageProviderCache.SelectedIndex < this.pageProviderCache.Items.Count - 1)
            {
                this.pageProviderCache.SelectedIndex++;
            }
        }
    }
}
