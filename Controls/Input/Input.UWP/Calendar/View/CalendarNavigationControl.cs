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
        /// Identifies the <see cref="Header"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(nameof(Header), typeof(string), typeof(CalendarNavigationControl), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="NavigationArrowsVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NavigationArrowsVisibilityProperty =
            DependencyProperty.Register(nameof(NavigationArrowsVisibility), typeof(Visibility), typeof(CalendarNavigationControl), new PropertyMetadata(Visibility.Visible));

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

        private const string NextButtonPartName = "navigateToNextViewButton";
        private const string PreviousButtonPartName = "navigateToPreviousViewButton";
        private const string HeaderButtonPartName = "navigateToViewLevelButton";

        private Button nextButton;
        private Button previousButton;
        private Button headerButton;

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
        public string Header
        {
            get
            {
                return (string)this.GetValue(HeaderProperty);
            }
            set
            {
                this.SetValue(HeaderProperty, value);
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
            this.headerButton = this.GetTemplateChild(HeaderButtonPartName) as Button;

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

            if (this.headerButton != null)
            {
                this.headerButton.Click += this.OnHeaderButtonTapped;
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

            if (this.headerButton != null)
            {
                this.headerButton.Click -= this.OnHeaderButtonTapped;
            }
        }

        /// <inheritdoc/>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new CalendarNavigationControlAutomationPeer(this);
        }

        private void OnNextButtonTapped(object sender, RoutedEventArgs e)
        {
            if (this.Owner != null)
            {
                this.Owner.RaiseMoveToNextViewCommand();
            }
        }

        private void OnPrevButtonTapped(object sender, RoutedEventArgs e)
        {
            if (this.Owner != null)
            {
                this.Owner.RaiseMoveToPreviousViewCommand();
            }
        }

        private void OnHeaderButtonTapped(object sender, RoutedEventArgs e)
        {
            if (this.Owner != null)
            {
                this.Owner.RaiseMoveToUpperViewCommand();
            }
        }
    }
}