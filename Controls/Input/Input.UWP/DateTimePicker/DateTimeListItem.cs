using Telerik.UI.Xaml.Controls.Primitives.LoopingList;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Input.DateTimePickers
{
    /// <summary>
    /// Represents a special looping list item that is created within a date-time list, used internally by <see cref="RadDatePicker"/> and <see cref="RadTimePicker"/>.
    /// </summary>
    [TemplateVisualState(Name = "Collapsed", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Expanded", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Selected", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "NotFocused", GroupName = "FocusedStates")]
    [TemplateVisualState(Name = "Focused", GroupName = "FocusedStates")]
    public class DateTimeListItem : LoopingListItem
    {
        /// <summary>
        /// Identifies the <see cref="SelectedBackground"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty SelectedBackgroundProperty =
            DependencyProperty.Register("SelectedBackground", typeof(Brush), typeof(DateTimeListItem), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="SelectedForeground"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty SelectedForegroundProperty =
            DependencyProperty.Register("SelectedForeground", typeof(Brush), typeof(DateTimeListItem), new PropertyMetadata(null));

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeListItem"/> class.
        /// </summary>
        public DateTimeListItem()
        {
            this.DefaultStyleKey = typeof(DateTimeListItem);
        }

        /// <summary>
        /// Gets or sets the Background of the rectangle area of the <see cref="DateTimeListItem"/> when the item is selected.
        /// </summary>
        public Brush SelectedBackground
        {
            get { return (Brush)GetValue(SelectedBackgroundProperty); }
            set { this.SetValue(SelectedBackgroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Foreground of the rectangle area of the <see cref="DateTimeListItem"/> when the item is selected.
        /// </summary>
        public Brush SelectedForeground
        {
            get { return (Brush)GetValue(SelectedForegroundProperty); }
            set { this.SetValue(SelectedForegroundProperty, value); }
        }

        /// <inheritdoc />
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new Automation.Peers.DateTimeListItemAutomationPeer(this);
        }
    }
}