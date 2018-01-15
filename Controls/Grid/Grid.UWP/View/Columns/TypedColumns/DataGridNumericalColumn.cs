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

        /// <summary>
        /// Gets the type of the editor for the DataGridNumericalColumn that is visualized when entering in edit mode.
        /// </summary>
        /// <returns>The type of the editor.</returns>
        public override object GetEditorType(object item)
        {
            return this.CanEdit ? numericBoxType : DataGridNumericalColumn.TextBlockType;
        }

        /// <summary>
        /// Creates an instance of a RadNumericBox used by the column when entering edit mode.
        /// </summary>
        /// <returns>An instance of the editor.</returns>
        public override FrameworkElement CreateEditorContentVisual()
        {
            return new RadNumericBox();
        }

        /// <summary>
        /// Prepares all bindings and content set to the RadNumericBox visualized when entering edit mode.
        /// </summary>
        /// <param name="editorContent">The editor itself.</param>
        public override void PrepareEditorContentVisual(FrameworkElement editorContent, Binding binding)
        {
            editorContent.SetBinding(RadNumericBox.ValueProperty, binding);
        }

        /// <summary>
        /// Clears all bindings and content set to the RadNumericBox visualized when entering edit mode.
        /// </summary>
        /// <param name="editorContent">The editor itself.</param>
        public override void ClearEditorContentVisual(FrameworkElement editorContent)
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
