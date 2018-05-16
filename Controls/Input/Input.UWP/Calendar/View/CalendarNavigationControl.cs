using Telerik.UI.Automation.Peers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    /// <summary>
    /// Represents the header panel of the calendar control that is responsible for the control navigation.
    /// </summary>
    public class CalendarNavigationControl : RadControl
    {
        /// <summary>
        /// Identifies the <see cref="HeaderContent"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderContentProperty =
            DependencyProperty.Register(nameof(HeaderContent), typeof(object), typeof(CalendarNavigationControl), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="HeaderContentTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderContentTemplateProperty =
            DependencyProperty.Register(nameof(HeaderContentTemplate), typeof(DataTemplate), typeof(CalendarNavigationControl), new PropertyMetadata(null));
        
        /// <summary>
        /// Identifies the <see cref="NavigationArrowsVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NavigationArrowsVisibilityProperty =
            DependencyProperty.Register(nameof(NavigationArrowsVisibility), typeof(Visibility), typeof(CalendarNavigationControl), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Identifies the <c cref="NavigationControlBorderStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NavigationControlBorderStyleProperty =
            DependencyProperty.Register(nameof(NavigationControlBorderStyle), typeof(Style), typeof(CalendarNavigationControl), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="IsNavigationToPreviousViewEnabled"/> dependency property.
        /// </summary>
        internal static readonly DependencyProperty IsNavigationToPreviousViewEnabledProperty =
            DependencyProperty.Register(nameof(IsNavigationToPreviousViewEnabled), typeof(bool), typeof(CalendarNavigationControl), new PropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="IsNavigationToNextViewEnabled"/> dependency property.
        /// </summary>
        internal static readonly DependencyProperty IsNavigationToNextViewEnabledProperty =
            DependencyProperty.Register(nameof(IsNavigationToNextViewEnabled), typeof(bool), typeof(CalendarNavigationControl), new PropertyMetadata(true));

        internal Button nextButton;
        internal Button previousButton;

        private const string NextButtonPartName = "navigateToNextViewButton";
        private const string PreviousButtonPartName = "navigateToPreviousViewButton";
        private const string HeaderPresenterPartName = "navigateToViewLevelContentPresenter";

        private ContentPresenter headerPresenter;
        private bool isPointerOverHeader;

        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarNavigationControl"/> class.
        /// </summary>
        public CalendarNavigationControl()
        {
            this.DefaultStyleKey = typeof(CalendarNavigationControl);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the navigation arrows are visible.
        /// </summary>
        /// <remarks>
        /// The navigation arrows are visible by default.
        /// </remarks>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadCalendar NavigationArrowsVisibility="Visible"/&gt;
        /// </code>
        /// </example>
        public Visibility NavigationArrowsVisibility
        {
            get
            {
                return (Visibility)this.GetValue(NavigationArrowsVisibilityProperty);
            }
            set
            {
                this.SetValue(NavigationArrowsVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the content of the navigation header.
        /// </summary>
        public object HeaderContent
        {
            get
            {
                return (object)this.GetValue(HeaderContentProperty);
            }
            set
            {
                this.SetValue(HeaderContentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Style the content of the navigation header.
        /// </summary>
        public DataTemplate HeaderContentTemplate
        {
            get
            {
                return (DataTemplate)GetValue(HeaderContentTemplateProperty);
            }
            set
            {
                this.SetValue(HeaderContentTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Style for the Border of the Navigation control.
        /// </summary>
        public Style NavigationControlBorderStyle
        {
            get
            {
                return (Style)this.GetValue(NavigationControlBorderStyleProperty);
            }
            set
            {
                this.SetValue(NavigationControlBorderStyleProperty, value);
            }
        }

        internal bool IsNavigationToPreviousViewEnabled
        {
            get
            {
                return (bool)this.GetValue(IsNavigationToPreviousViewEnabledProperty);
            }
            set
            {
                this.SetValue(IsNavigationToPreviousViewEnabledProperty, value);
            }
        }

        internal bool IsNavigationToNextViewEnabled
        {
            get
            {
                return (bool)this.GetValue(IsNavigationToNextViewEnabledProperty);
            }
            set
            {
                this.SetValue(IsNavigationToNextViewEnabledProperty, value);
            }
        }

        internal RadCalendar Owner { get; set; }

        /// <summary>
        /// Called when the Framework <see cref="M:OnApplyTemplate" /> is called. Inheritors should override this method should they have some custom template-related logic.
        /// This is done to ensure that the <see cref="P:IsTemplateApplied" /> property is properly initialized.
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            this.nextButton = this.GetTemplateChild(NextButtonPartName) as Button;
            this.previousButton = this.GetTemplateChild(PreviousButtonPartName) as Button;
            this.headerPresenter = this.GetTemplateChild(HeaderPresenterPartName) as ContentPresenter;

            return applied;
        }

        /// <summary>
        /// Occurs when the <see cref="M:OnApplyTemplate" /> method has been called and the template is already successfully applied.
        /// </summary>
        protected override void OnTemplateApplied()
        {
            base.OnTemplateApplied();

            // NOTE: Tapped event not raised on keyboard enter press.
            if (this.previousButton != null)
            {
                this.previousButton.Click += this.OnPrevButtonTapped;

                Binding binding = new Binding();
                binding.Source = this;
                binding.Path = new PropertyPath("IsNavigationToPreviousViewEnabled");

                this.previousButton.SetBinding(Control.IsEnabledProperty, binding);
            }

            if (this.nextButton != null)
            {
                this.nextButton.Click += this.OnNextButtonTapped;

                Binding binding = new Binding();
                binding.Source = this;
                binding.Path = new PropertyPath("IsNavigationToNextViewEnabled");

                this.nextButton.SetBinding(Control.IsEnabledProperty, binding);
            }

            if (this.headerPresenter != null)
            {
                this.headerPresenter.PointerPressed += this.OnHeaderPresenterPointerPressed;
                this.headerPresenter.PointerReleased += this.OnHeaderPresenterPointerReleased;
                this.headerPresenter.PointerExited += this.OnHeaderPresenterPointerExited;
                this.headerPresenter.PointerEntered += this.OnHeaderPresenterPointerEntered;
                this.headerPresenter.PointerCaptureLost += this.OnHeaderPresenterPointerCaptureLost;
            }

            if (this.Owner.DisplayMode == CalendarDisplayMode.MultiDayView)
            {
                if (this.previousButton != null && this.nextButton != null)
                {
                    this.previousButton.Content = RadCalendar.DefaultMultiDayViewPreviousButtonContent;
                    this.nextButton.Content = RadCalendar.DefaultMultiDayViewNextButtonContent;
                }
            }
            else if (this.previousButton != null && this.nextButton != null)
            {
                this.previousButton.Content = RadCalendar.DefaultPreviousButtonContent;
                this.nextButton.Content = RadCalendar.DefaultNextButtonContent;
            }
        }

        /// <inheritdoc/>
        protected override void UnapplyTemplateCore()
        {
            base.UnapplyTemplateCore();

            if (this.nextButton != null)
            {
                this.nextButton.Click -= this.OnNextButtonTapped;
            }

            if (this.previousButton != null)
            {
                this.previousButton.Click -= this.OnPrevButtonTapped;
            }

            if (this.headerPresenter != null)
            {
                this.headerPresenter.PointerPressed -= this.OnHeaderPresenterPointerPressed;
                this.headerPresenter.PointerReleased -= this.OnHeaderPresenterPointerReleased;
                this.headerPresenter.PointerExited -= this.OnHeaderPresenterPointerExited;
                this.headerPresenter.PointerEntered -= this.OnHeaderPresenterPointerEntered;
                this.headerPresenter.PointerCaptureLost -= this.OnHeaderPresenterPointerCaptureLost;
            }
        }

        /// <inheritdoc/>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new CalendarNavigationControlAutomationPeer(this);
        }

        private void OnNextButtonTapped(object sender, RoutedEventArgs e)
        {
            RadCalendar calendar = this.Owner;
            if (calendar != null)
            {
                int navigationStep = 1;
                if (calendar.DisplayMode == CalendarDisplayMode.MultiDayView)
                {
                    navigationStep = calendar.MultiDayViewSettings.VisibleDays;
                }

                calendar.RaiseMoveToNextViewCommand(navigationStep);
            }
        }

        private void OnPrevButtonTapped(object sender, RoutedEventArgs e)
        {
            RadCalendar calendar = this.Owner;
            if (calendar != null)
            {
                int navigationStep = 1;
                if (calendar.DisplayMode == CalendarDisplayMode.MultiDayView)
                {
                    navigationStep = calendar.MultiDayViewSettings.VisibleDays;
                }

                this.Owner.RaiseMoveToPreviousViewCommand(navigationStep);
            }
        }
        
        private void OnHeaderPresenterPointerCaptureLost(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            this.headerPresenter.ReleasePointerCaptures();
        }

        private void OnHeaderPresenterPointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (this.headerPresenter.PointerCaptures != null && this.headerPresenter.PointerCaptures.Count > 0)
            {
                VisualStateManager.GoToState(this, "Pressed", false);
            }

            this.isPointerOverHeader = true;
        }

        private void OnHeaderPresenterPointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Released", false);
            this.isPointerOverHeader = false;
        }

        private void OnHeaderPresenterPointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (this.headerPresenter.PointerCaptures != null && this.headerPresenter.PointerCaptures.Count > 0 && 
                this.isPointerOverHeader && this.Owner != null)
            {
                this.Owner.RaiseMoveToUpperViewCommand();
            }
        }

        private void OnHeaderPresenterPointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            this.headerPresenter.CapturePointer(e.Pointer);
            VisualStateManager.GoToState(this, "Pressed", false);
        }
    }
}