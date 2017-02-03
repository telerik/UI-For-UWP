using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Data.ListView
{
     [TemplatePart(Name = "PART_CheckBox", Type = typeof(CheckBox))]
    public class ItemCheckBoxControl : RadControl
    {

        public Style CheckBoxStyle
        {
            get { return (Style)GetValue(CheckBoxStyleProperty); }
            set { SetValue(CheckBoxStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CheckBoxStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CheckBoxStyleProperty =
            DependencyProperty.Register(nameof(CheckBoxStyle), typeof(Style), typeof(ItemCheckBoxControl), new PropertyMetadata(null));


        public CheckBoxPosition CheckBoxPosition
        {
            get { return (CheckBoxPosition)GetValue(CheckBoxPositionProperty); }
            set { SetValue(CheckBoxPositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CheckBoxPosition.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CheckBoxPositionProperty =
            DependencyProperty.Register(nameof(CheckBoxPosition), typeof(CheckBoxPosition), typeof(ItemCheckBoxControl), new PropertyMetadata(CheckBoxPosition.BeforeItem));

  
        internal void SetIsChecked(bool isChecked)
        {
            this.isChecked = isChecked;
            if (this.checkBox != null)
            {
                this.checkBox.IsChecked = isChecked;
            }
        }

        private bool isChecked;
        private EventHandler isCheckedChanged;

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

        internal CheckBox checkBox;
        public ItemCheckBoxControl()
        {
            this.DefaultStyleKey = typeof(ItemCheckBoxControl);
        }


        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            checkBox.IsChecked = this.isChecked;
            checkBox.Checked += checkBox_Checked;
            checkBox.Unchecked += checkBox_Checked;
        }

        protected override void UnapplyTemplateCore()
        {
            base.UnapplyTemplateCore();
            checkBox.Checked -= checkBox_Checked;
            checkBox.Unchecked -= checkBox_Checked;
        }
        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            var handler = this.isCheckedChanged;
            if (handler != null)
            {
                handler(sender, null);
            }
        }

        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            this.checkBox = this.GetTemplatePartField<CheckBox>("PART_CheckBox");
            applied = applied && this.checkBox != null;

            return applied;
        }
    }
}
