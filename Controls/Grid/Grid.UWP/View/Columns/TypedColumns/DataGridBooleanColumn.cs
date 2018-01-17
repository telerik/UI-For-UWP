using System;
using Telerik.UI.Xaml.Controls.Grid.Primitives;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Grid
{
    /// <summary>
    /// A special <see cref="DataGridTypedColumn"/> implementation that presents boolean data.
    /// </summary>
    public class DataGridBooleanColumn : DataGridTextColumn
    {
        private const string UncheckedGlyph = "\u2612";
        private const string CheckedGlyph = "\u2611";
        private const string IndeterminateGlyph = "\u25A3";

        private static Style defaultCellEditorStyle;
        private static Style defaultCellStyle;
        private static Type booleanType = typeof(bool);
        private static Type checkBoxType = typeof(CheckBox);

        internal override Style DefaultCellContentStyle
        {
            get
            {
                if (defaultCellStyle == null)
                {
                    defaultCellStyle = ResourceHelper.LoadEmbeddedResource(
                        typeof(DataGridTextColumn),
                        "Telerik.UI.Xaml.Controls.Grid.View.Columns.Resources.DefaultBooleanColumnStyle.xaml",
                        "DefaultColumnStyle") as Style;
                }
                return defaultCellStyle;
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
                        "Telerik.UI.Xaml.Controls.Grid.View.Columns.Resources.DefaultBooleanColumnEditorStyle.xaml",
                        "DefaultColumnEditorStyle") as Style;
                }
                return defaultCellEditorStyle;
            }
        }

        internal override bool SupportsCompositeFilter
        {
            get
            {
                return false;
            }
        }

        internal override bool CanEdit
        {
            get { return this.PropertyInfoInitialized && this.PropertyInfo.DataType == DataGridBooleanColumn.booleanType && this.CanUserEdit; }
        }

        /// <summary>
        /// Gets the type of the editor for the DataGridBooleanColumn that is visualized when entering in edit mode.
        /// </summary>
        /// <returns>The type of the editor.</returns>
        public override object GetEditorType(object item)
        {
            return this.CanEdit ? DataGridBooleanColumn.checkBoxType : DataGridBooleanColumn.TextBlockType;
        }

        /// <summary>
        /// Creates an instance of a Checkbox used by the column when entering edit mode.
        /// </summary>
        /// <returns>An instance of the editor.</returns>
        public override FrameworkElement CreateEditorContentVisual()
        {
            return new CheckBox();
        }

        /// <summary>
        /// Clears all bindings and content set to the CheckBox visualized when entering edit mode.
        /// </summary>
        /// <param name="editorContent">The editor itself.</param>
        public override void ClearEditorContentVisual(FrameworkElement editorContent)
        {
            editorContent.ClearValue(CheckBox.IsCheckedProperty);
        }

        /// <summary>
        /// Prepares all bindings and content set to the CheckBox visualized when entering edit mode.
        /// </summary>
        /// <param name="editorContent">The editor itself.</param>
        public override void PrepareEditorContentVisual(FrameworkElement editorContent, Binding binding)
        {
            editorContent.SetBinding(CheckBox.IsCheckedProperty, binding);
        }
        
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Windows.UI.Xaml.Controls.TextBlock.put_Text(System.String)")]
        public override void PrepareCell(object container, object value, object item)
        {
            base.PrepareCell(container, value, item);

            var textBlock = container as TextBlock;
            if (textBlock == null)
            {
                return;
            }

            bool? cellValue = (bool?)value;
            if (cellValue.HasValue)
            {
                textBlock.Text = cellValue.Value ? CheckedGlyph : UncheckedGlyph;
            }
            else
            {
                textBlock.Text = IndeterminateGlyph;
            }
        }

        /// <summary>
        /// Creates the <see cref="DataGridBooleanFilterControl" /> instance that allows filtering operation to be applied upon this column.
        /// </summary>
        protected internal override DataGridFilterControlBase CreateFilterControl()
        {
            return new DataGridBooleanFilterControl()
            {
                PropertyName = this.PropertyName
            };
        }
    }
}
