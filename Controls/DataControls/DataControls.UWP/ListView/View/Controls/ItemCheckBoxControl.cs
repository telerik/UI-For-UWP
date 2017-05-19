using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Data.ListView
{
    /// <summary>
    /// Represents an ItemCheckBoxControl control.
    /// </summary>
    [TemplatePart(Name = "PART_CheckBox", Type = typeof(CheckBox))]
    public class ItemCheckBoxControl : RadControl
    {
        /// <summary>
        /// Identifies the <see cref="CheckBoxStyle"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty CheckBoxStyleProperty =
            DependencyProperty.Register(nameof(CheckBoxStyle), typeof(Style), typeof(ItemCheckBoxControl), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="CheckBoxPosition"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty CheckBoxPositionProperty =
            DependencyProperty.Register(nameof(CheckBoxPosition), typeof(CheckBoxPosition), typeof(ItemCheckBoxControl), new PropertyMetadata(CheckBoxPosition.BeforeItem));
        
        internal CheckBox checkBox;
        private EventHandler isCheckedChanged;
        private bool isChecked;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemCheckBoxControl"/> class.
        /// </summary>
        public ItemCheckBoxControl()
        {
            this.DefaultStyleKey = typeof(ItemCheckBoxControl);
        }
        
        /// <summary>
        /// Occurs when the <see cref="CheckBox"/> is checked.
        /// </summary>
        public event EventHandler IsCheckedChanged
        {
            add
            {
                this.isCheckedChanged += value;
            }
            remove
            {
                this.isCheckedChanged -= value;
            }
        }

        /// <summary>
        /// Gets or sets the style for the <see cref="CheckBox"/>.
        /// </summary>
        public Style CheckBoxStyle
        {
            get { return (Style)GetValue(CheckBoxStyleProperty); }
            set { this.SetValue(CheckBoxStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the the position of the CheckBox.
        /// </summary>
        public CheckBoxPosition CheckBoxPosition
        {
            get { return (CheckBoxPosition)GetValue(CheckBoxPositionProperty); }
            set { this.SetValue(CheckBoxPositionProperty, value); }
        }

                internal void SetIsChecked(bool isChecked)
        {
            this.isChecked = isChecked;
            if (this.checkBox != null)
            {
                this.checkBox.IsChecked = isChecked;
            }
        }

        /// <summary>
        /// Called when the Framework <see cref="M:OnApplyTemplate" /> is called. Inheritors should override this method should they have some custom template-related logic.
        /// This is done to ensure that the <see cref="P:IsTemplateApplied" /> property is properly initialized.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.checkBox.IsChecked = this.isChecked;
            this.checkBox.Checked += this.checkBox_Checked;
            this.checkBox.Unchecked += this.checkBox_Checked;
        }

        /// <inheritdoc/>
        protected override void UnapplyTemplateCore()
        {
            base.UnapplyTemplateCore();
            this.checkBox.Checked -= this.checkBox_Checked;
            this.checkBox.Unchecked -= this.checkBox_Checked;
        }

        /// <inheritdoc/>
        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            this.checkBox = this.GetTemplatePartField<CheckBox>("PART_CheckBox");
            applied = applied && this.checkBox != null;

            return applied;
        }

        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            var handler = this.isCheckedChanged;
            if (handler != null)
            {
                handler(sender, null);
            }
        }
    }
}
