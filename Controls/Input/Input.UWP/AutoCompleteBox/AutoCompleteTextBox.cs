using Telerik.UI.Automation.Peers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Telerik.UI.Xaml.Controls.Input.AutoCompleteBox
{
    /// <summary>
    /// Represents the TextBox part of the RadAutoCompleteBox. Exposed for easier styling via implicit style.
    /// </summary>
    [TemplatePart(Name = "PART_DeleteButton", Type = typeof(Button))]
    public class AutoCompleteTextBox : TextBox
    {
        private const string DeleteButtonPartName = "PART_DeleteButton";
        private Button deleteButton;

        internal bool isClearButtonVisible = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoCompleteTextBox" /> class.
        /// </summary>
        public AutoCompleteTextBox()
        {
            this.DefaultStyleKey = typeof(AutoCompleteTextBox);
        }

        internal void UpdateDeleteButtonVisibility()
        {
            if (string.IsNullOrEmpty(this.Text))
            {
                this.deleteButton.Visibility = Visibility.Collapsed;
                return;
            }

            this.deleteButton.Visibility = this.isClearButtonVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <inheritdoc />
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            this.UpdateDeleteButtonVisibility();
        }

        /// <inheritdoc />
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            this.UpdateDeleteButtonVisibility();
        }

        /// <inheritdoc />
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            RadAutoCompleteBox parent = ElementTreeHelper.FindVisualAncestor<RadAutoCompleteBox>(this);
            if (parent != null)
            {
                return new AutoCompleteTextBoxAutomationPeer(this, parent);
            }
            return base.OnCreateAutomationPeer();
        }

        /// <inheritdoc />
        protected override void OnApplyTemplate()
        {
            if (this.deleteButton != null)
            {
                this.TextChanged -= AutoCompleteTextBox_TextChanged;
                this.deleteButton.Click -= DeleteButton_Click;
            }

            base.OnApplyTemplate();
            this.deleteButton = (Button)this.GetTemplateChild(DeleteButtonPartName);
            this.deleteButton.Click += DeleteButton_Click;
            this.TextChanged += AutoCompleteTextBox_TextChanged;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            this.Text = string.Empty;
        }

        private void AutoCompleteTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.UpdateDeleteButtonVisibility();
        }
    }
}
