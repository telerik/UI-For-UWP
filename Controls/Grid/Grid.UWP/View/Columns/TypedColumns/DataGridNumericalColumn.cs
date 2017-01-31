using System;
using Telerik.UI.Xaml.Controls.Grid.Primitives;
using Telerik.UI.Xaml.Controls.Input;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Grid
{
    /// <summary>
    /// Represents an extended <see cref="DataGridTextColumn"/> that presents numerical data.
    /// The column will create a <see cref="DataGridNumericalFilterControl"/> upon triggering the filtering UI through user input.
    /// </summary>
    public class DataGridNumericalColumn : DataGridTextColumn
    {
        private static Style defaultCellEditorStyle;
        private static Type numericBoxType = typeof(RadNumericBox);

        internal override Style DefaultCellEditorStyle
        {
            get
            {
                if (defaultCellEditorStyle == null)
                {
                    defaultCellEditorStyle = ResourceHelper.LoadEmbeddedResource(
                        typeof(DataGridTextColumn),
                        "Telerik.UI.Xaml.Controls.Grid.View.Columns.Resources.DefaultNumericalColumnEditorStyle.xaml",
                        "DefaultColumnEditorStyle") as Style;
                }
                return defaultCellEditorStyle;
            }
        }

        internal override object GetEditorType(object item)
        {
            return this.CanEdit ? numericBoxType : DataGridNumericalColumn.TextBlockType;
        }

        internal override FrameworkElement CreateEditorContentVisual()
        {
            return new RadNumericBox();
        }

        internal override void PrepareEditorContentVisual(FrameworkElement editorContent, Binding binding)
        {
            editorContent.SetBinding(RadNumericBox.ValueProperty, binding);
        }

        internal override void ClearEditorContentVisual(FrameworkElement editorContent)
        {
            editorContent.ClearValue(RadNumericBox.ValueProperty);
        }

        /// <summary>
        /// Creates the <see cref="DataGridNumericalFilterControl" /> instance that allows filtering operation to be applied upon this column.
        /// </summary>
        protected internal override DataGridFilterControlBase CreateFilterControl()
        {
            return new DataGridNumericalFilterControl()
            {
                PropertyName = this.PropertyName
            };
        }
    }
}
