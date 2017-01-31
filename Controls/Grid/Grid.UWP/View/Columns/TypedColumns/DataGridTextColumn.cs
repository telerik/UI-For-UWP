using System;
using System.Globalization;
using Telerik.UI.Xaml.Controls.Grid.Primitives;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Grid
{
    /// <summary>
    /// Represents a <see cref="DataGridTypedColumn"/> that converts the content of each associated cell to a <see cref="System.String"/> object.
    /// </summary>
    public class DataGridTextColumn : DataGridTypedColumn
    {
        /// <summary>
        /// Identifies the <see cref="CellContentFormat"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CellContentFormatProperty =
            DependencyProperty.Register("CellContentFormat", typeof(string), typeof(DataGridTextColumn), new PropertyMetadata(null, OnCellContentFormatChanged));

        internal static Type TextBlockType = typeof(TextBlock);
        private static Style defaultTextCellStyle;
        private static Style defaultCellEditorStyle;
        private static Style defaultCellFlyoutContentStyle;

        private static Type textBoxType = typeof(TextBox);

        private string cellContentFormatCache;

        /// <summary>
        /// Gets or sets the custom format for each cell value. The String.Format routine is used and the format passed should be in the form required by this method.
        /// </summary>
        public string CellContentFormat
        {
            get
            {
                return this.cellContentFormatCache;
            }
            set
            {
                this.SetValue(CellContentFormatProperty, value);
            }
        }

        internal override Style DefaultCellEditorStyle
        {
            get
            {
                if (defaultCellEditorStyle == null)
                {
                    defaultCellEditorStyle = ResourceHelper.LoadEmbeddedResource(
                        typeof(DataGridTextColumn),
                        "Telerik.UI.Xaml.Controls.Grid.View.Columns.Resources.DefaultTextColumnEditorStyle.xaml",
                        "DefaultColumnEditorStyle") as Style;
                }
                return defaultCellEditorStyle;
            }
        }

        internal override Style DefaultCellContentStyle
        {
            get
            {
                if (defaultTextCellStyle == null)
                {
                    defaultTextCellStyle = ResourceHelper.LoadEmbeddedResource(
                        typeof(DataGridTextColumn),
                        "Telerik.UI.Xaml.Controls.Grid.View.Columns.Resources.DefaultTextColumnStyle.xaml",
                        "DefaultColumnStyle") as Style;
                }
                return defaultTextCellStyle;
            }
        }

        internal override Style DefaultCellFlyoutContentStyle
        {
            get
            {
                if (defaultCellFlyoutContentStyle == null)
                {
                    defaultCellFlyoutContentStyle = ResourceHelper.LoadEmbeddedResource(
                        typeof(DataGridTextColumn),
                        "Telerik.UI.Xaml.Controls.Grid.View.Columns.Resources.DefaultTextColumnFlyoutContentStyle.xaml",
                        "DefaultColumnFlyoutStyle") as Style;
                }
                return defaultCellFlyoutContentStyle;
            }
        }

        internal override bool CanEdit
        {
            get
            {
                return !string.IsNullOrEmpty(this.PropertyName) && this.PropertyInfo != null && this.CanUserEdit;
            }
        }

        internal override object GetEditorType(object item)
        {
            return this.CanEdit ? textBoxType : TextBlockType;
        }

        internal override object GetContainerType(object rowItem)
        {
            return TextBlockType;
        }

        internal override void PrepareCell(GridCellModel cell)
        {
            base.PrepareCell(cell);

            TextBlock tb = cell.Container as TextBlock;
            if (tb == null)
            {
                return;
            }

            if (cell.Value == null)
            {
                tb.ClearValue(TextBlock.TextProperty);
                return;
            }

            string text;
            if (!string.IsNullOrEmpty(this.cellContentFormatCache))
            {
                text = string.Format(CultureInfo.CurrentUICulture, this.cellContentFormatCache, cell.Value);
            }
            else
            {
                text = Convert.ToString(cell.Value, CultureInfo.CurrentUICulture);
            }

            if (tb.Text != text)
            {
                tb.Text = text;
            }
        }

        internal override object CreateContainer(object rowItem)
        {
            return new TextBlock();
        }

        internal override FrameworkElement CreateEditorContentVisual()
        {
            TextBox textbox = new TextBox();
            textbox.TextChanged += this.Textbox_TextChanged;

            return textbox;
        }

        internal override void PrepareEditorContentVisual(FrameworkElement editorContent, Binding binding)
        {
            editorContent.SetBinding(TextBox.TextProperty, binding);
        }

        internal override void ClearEditorContentVisual(FrameworkElement editorContent)
        {
            editorContent.ClearValue(TextBox.TextProperty);
        }

        /// <summary>
        /// Creates the <see cref="DataGridTextFilterControl"/> instance that allows filtering operation to be applied upon this column.
        /// </summary>
        protected internal override DataGridFilterControlBase CreateFilterControl()
        {
            return new DataGridTextFilterControl()
            {
                PropertyName = this.PropertyName
            };
        }

        private static void OnCellContentFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var column = d as DataGridTextColumn;
            column.cellContentFormatCache = (string)e.NewValue;

            column.OnProperyChange(UpdateFlags.AllButData);
        }

        private void Textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            var expression = textBox.GetBindingExpression(TextBox.TextProperty);
            if (expression != null)
            {
                expression.UpdateSource();
            }
        }
    }
}