using System;
using Telerik.Data.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    /// <summary>
    /// Represents a concrete <see cref="DataGridTypedFilterControl"/> that is used to filter data presented by a <see cref="DataGridTextColumn"/> instance.
    /// </summary> 
    [TemplatePart(Name = "PART_OperatorCombo", Type = typeof(DataGridFilterComboBox))]
    [TemplatePart(Name = "PART_ValueBox", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_CaseButton", Type = typeof(ToggleButton))]
    public class DataGridTextFilterControl : DataGridTypedFilterControl
    {
        /// <summary>
        /// Identifies the <see cref="IsCaseSensitive"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsCaseSensitiveProperty =
            DependencyProperty.Register(nameof(IsCaseSensitive), typeof(bool), typeof(DataGridTextFilterControl), new PropertyMetadata(true));

        private TextBox valueBox;
        private ToggleButton caseButton;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridTextFilterControl" /> class.
        /// </summary>
        public DataGridTextFilterControl()
        {
            this.DefaultStyleKey = typeof(DataGridTextFilterControl);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the corresponding <see cref="TextFilterDescriptor"/> will use a case-sensitive pattern to perform the match.
        /// </summary>
        public bool IsCaseSensitive
        {
            get
            {
                return (bool)this.GetValue(IsCaseSensitiveProperty);
            }
            set
            {
                this.SetValue(IsCaseSensitiveProperty, value);
            }
        }

        /// <summary>
        /// Gets the textbox. Exposed for testing purposes only.
        /// </summary>
        internal TextBox TextBox
        {
            get
            {
                return this.valueBox;
            }
        }

        /// <summary>
        /// Builds the <see cref="FilterDescriptorBase" /> that describes the user input within this instance.
        /// </summary>
        public override FilterDescriptorBase BuildDescriptor()
        {
            var descriptor = new TextFilterDescriptor();
            descriptor.PropertyName = this.PropertyName;
            descriptor.Operator = (TextOperator)this.OperatorCombo.SelectedIndex;
            descriptor.Value = this.valueBox.Text;
            descriptor.IsCaseSensitive = this.IsCaseSensitive;

            return descriptor;
        }

        /// <summary>
        /// Called when the Framework <see cref="M:OnApplyTemplate" /> is called. Inheritors should override this method should they have some custom template-related logic.
        /// This is done to ensure that the <see cref="P:IsTemplateApplied" /> property is properly initialized.
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            this.valueBox = this.GetTemplatePartField<TextBox>("PART_ValueBox");
            applied = applied && this.valueBox != null;

            this.caseButton = this.GetTemplatePartField<ToggleButton>("PART_CaseButton");
            applied = applied && this.caseButton != null;

            return applied;
        }

        /// <summary>
        /// Occurs when the <see cref="M:OnApplyTemplate" /> method has been called and the template is already successfully applied.
        /// </summary>
        protected override void OnTemplateApplied()
        {
            base.OnTemplateApplied();

            this.caseButton.Click += this.OnCaseButtonClick;
        }

        /// <inheritdoc/>
        protected override void UnapplyTemplateCore()
        {
            base.UnapplyTemplateCore();

            this.caseButton.Click -= this.OnCaseButtonClick;
        }

        /// <summary>
        /// Initializes the control depending on the current <see cref="P:AssociatedDescriptor" /> value.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            var textDescriptor = this.ActualAssociatedDescriptor as TextFilterDescriptor;
            string selectedItem;

            if (textDescriptor != null)
            {
                selectedItem = GridLocalizationManager.Instance.GetString(textDescriptor.Operator.ToString());
                if (textDescriptor.Value != null)
                {
                    this.valueBox.Text = textDescriptor.Value.ToString();
                }

                this.IsCaseSensitive = textDescriptor.IsCaseSensitive;
            }
            else
            {
                selectedItem = GridLocalizationManager.Instance.GetString(TextOperator.EqualsTo.ToString());
            }

            this.OperatorCombo.SelectedItem = selectedItem;
            this.UpdateCaseButtonTooltip();
        }

        private void OnCaseButtonClick(object sender, RoutedEventArgs e)
        {
            this.IsCaseSensitive = this.caseButton.IsChecked.GetValueOrDefault();
            this.UpdateCaseButtonTooltip();
        }

        private void UpdateCaseButtonTooltip()
        {
            string toolTip = GridLocalizationManager.Instance.GetString("CaseSensitiveMode") + " ";
            if (this.caseButton.IsChecked.GetValueOrDefault())
            {
                toolTip += GridLocalizationManager.Instance.GetString("On");
            }
            else
            {
                toolTip += GridLocalizationManager.Instance.GetString("Off");
            }
            ToolTipService.SetToolTip(this.caseButton, toolTip);
        }
    }
}
