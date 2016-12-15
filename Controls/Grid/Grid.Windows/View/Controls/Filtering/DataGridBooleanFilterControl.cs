using System;
using Telerik.Data.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    /// <summary>
    /// Represents a concrete <see cref="DataGridFilterControlBase"/> that allows for filtering of Boolean data.
    /// </summary>
    [TemplatePart(Name = "PART_FalseButton", Type = typeof(RadioButton))]
    [TemplatePart(Name = "PART_TrueButton", Type = typeof(RadioButton))]
    public class DataGridBooleanFilterControl : DataGridFilterControlBase
    {
        private RadioButton falseButton;
        private RadioButton trueButton;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridBooleanFilterControl" /> class.
        /// </summary>
        public DataGridBooleanFilterControl()
        {
            this.DefaultStyleKey = typeof(DataGridBooleanFilterControl);
        }

        /// <summary>
        /// Gets or sets the name of the property that will be used in the underlying <see cref="NumericalFilterDescriptor"/>.
        /// </summary>
        public string PropertyName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the false button. Exposed for testing purposes only.
        /// </summary>
        internal RadioButton FalseButton
        {
            get
            {
                return this.falseButton;
            }
        }

        /// <summary>
        /// Gets the true button. Exposed for testing purposes only.
        /// </summary>
        internal RadioButton TrueButton
        {
            get
            {
                return this.trueButton;
            }
        }

        /// <summary>
        /// Builds the <see cref="FilterDescriptorBase" /> that describes the user input within this instance.
        /// </summary>
        public override FilterDescriptorBase BuildDescriptor()
        {
            BooleanFilterDescriptor descriptor = new BooleanFilterDescriptor();
            descriptor.PropertyName = this.PropertyName;
            if (this.trueButton.IsChecked.GetValueOrDefault())
            {
                descriptor.Value = true;
            }
            else
            {
                descriptor.Value = false;
            }

            return descriptor;
        }

        /// <summary>
        /// Initializes the control depending on the current <see cref="P:AssociatedDescriptor" /> value.
        /// </summary>
        protected override void Initialize()
        {
            var descriptor = this.ActualAssociatedDescriptor as BooleanFilterDescriptor;
            if (descriptor != null && (descriptor.Value is bool?))
            {
                bool? value = (bool?)descriptor.Value;
                if (value.GetValueOrDefault())
                {
                    this.trueButton.IsChecked = true;
                }
                else
                {
                    this.falseButton.IsChecked = true;
                }
            }
            else
            {
                this.trueButton.IsChecked = true;
            }
        }

        /// <summary>
        /// Called when the Framework <see cref="M:OnApplyTemplate" /> is called. Inheritors should override this method should they have some custom template-related logic.
        /// This is done to ensure that the <see cref="P:IsTemplateApplied" /> property is properly initialized.
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            this.falseButton = this.GetTemplatePartField<RadioButton>("PART_FalseButton");
            applied = applied && this.falseButton != null;

            this.trueButton = this.GetTemplatePartField<RadioButton>("PART_TrueButton");
            applied = applied && this.trueButton != null;

            return applied;
        }

        /// <summary>
        /// Occurs when the <see cref="M:OnApplyTemplate" /> method has been called and the template is already successfully applied.
        /// </summary>
        protected override void OnTemplateApplied()
        {
            this.trueButton.Content = GridLocalizationManager.Instance.GetString("IsTrue");
            this.falseButton.Content = GridLocalizationManager.Instance.GetString("IsFalse");

            base.OnTemplateApplied();
        }
    }
}
