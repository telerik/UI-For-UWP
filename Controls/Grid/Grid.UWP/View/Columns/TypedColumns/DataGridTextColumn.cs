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
            DependencyProperty.Register(nameof(CellContentFormat), typeof(string), typeof(DataGridTextColumn), new PropertyMetadata(null, OnCellContentFormatChanged));

        internal static Type TextBlockType = typeof(TextBlock);

        private static Type textBoxType = typeof(TextBox);

        private Style defaultTextCellStyle;
        private Style defaultCellEditorStyle;
        private Style defaultCellFlyoutContentStyle;
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
                if (this.defaultCellEditorStyle == null)
                {
                    this.defaultCellEditorStyle = ResourceHelper.LoadEmbeddedResource(
                        typeof(DataGridTextColumn),
                        "Telerik.UI.Xaml.Controls.Grid.View.Columns.Resources.DefaultTextColumnEditorStyle.xaml",
                        "DefaultColumnEditorStyle") as Style;
                }
                return this.defaultCellEditorStyle;
            }
        }

        internal override Style DefaultCellContentStyle
        {
            get
            {
                if (this.defaultTextCellStyle == null)
                {
                    this.defaultTextCellStyle = ResourceHelper.LoadEmbeddedResource(
                        typeof(DataGridTextColumn),
                        "Telerik.UI.Xaml.Controls.Grid.View.Columns.Resources.DefaultTextColumnStyle.xaml",
                        "DefaultColumnStyle") as Style;
                }
                return this.defaultTextCellStyle;
            }
        }

        internal override Style DefaultCellFlyoutContentStyle
        {
            get
            {
                if (this.defaultCellFlyoutContentStyle == null)
                {
                    this.defaultCellFlyoutContentStyle = ResourceHelper.LoadEmbeddedResource(
                        typeof(DataGridTextColumn),
                        "Telerik.UI.Xaml.Controls.Grid.View.Columns.Resources.DefaultTextColumnFlyoutContentStyle.xaml",
                        "DefaultColumnFlyoutStyle") as Style;
                }
                return this.defaultCellFlyoutContentStyle;
            }
        }

        internal override bool CanEdit
        {
            get
            {
                return !string.IsNullOrEmpty(this.PropertyName) && this.PropertyInfo != null && this.CanUserEdit;
            }
        }

        /// <summary>
        /// Gets the type of the editor for the DataGridTextColumn that is visualized when entering in edit mode.
        /// </summary>
        /// <returns>The type of the editor.</returns>
        public override object GetEditorType(object item)
        {
            return this.CanEdit ? textBoxType : TextBlockType;
        }

        /// <summary>
        /// Gets the type of the control visualized when the text column is not currently edited.
        /// </summary>
        /// <returns>The type of the control.</returns>
        public override object GetContainerType(object rowItem)
        {
            return TextBlockType;
        }

        /// <summary>
        /// Creates an instance of a TextBlock visualized when the column is not edited.
        /// </summary>
        /// <returns>An instance of the control.</returns>
        public override object CreateContainer(object rowItem)
        {
            return new TextBlock();
        }

        /// <summary>
        /// Creates an instance of a TextBox used by the column when entering edit mode.
        /// </summary>
        /// <returns>An instance of the editor.</returns>
        public override FrameworkElement CreateEditorContentVisual()
        {
            TextBox textbox = new TextBox();
            textbox.TextChanged += this.Textbox_TextChanged;

            return textbox;
        }

        /// <summary>
        /// Prepares all bindings and content set to the TextBox visualized when entering edit mode.
        /// </summary>
        /// <param name="editorContent">The editor itself.</param>
        /// <param name="binding">The binding set to the editor of the cell.</param>
        public override void PrepareEditorContentVisual(FrameworkElement editorContent, Binding binding)
        {
            editorContent.SetBinding(TextBox.TextProperty, binding);
        }

        /// <summary>
        /// Clears all bindings and content set to the TextBox visualized when entering edit mode.
        /// </summary>
        /// <param name="editorContent">The editor itself.</param>
        public override void ClearEditorContentVisual(FrameworkElement editorContent)
        {
            editorContent.ClearValue(TextBox.TextProperty);
        }

        /// <inheritdoc/>
        public override void PrepareCell(object container, object value, object item)
        {
            base.PrepareCell(container, value, item);

            TextBlock tb = container as TextBlock;
            if (tb == null)
            {
                return;
            }

            if (value == null)
            {
                tb.ClearValue(TextBlock.TextProperty);
                return;
            }

            string text;
            if (!string.IsNullOrEmpty(this.cellContentFormatCache))
            {
                text = string.Format(CultureInfo.CurrentUICulture, this.cellContentFormatCache, value);
            }
            else
            {
                text = Convert.ToString(value, CultureInfo.CurrentUICulture);
            }

            if (tb.Text != text)
            {
                tb.Text = text;
            }
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

            column.OnPropertyChange(UpdateFlags.AllButData);
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