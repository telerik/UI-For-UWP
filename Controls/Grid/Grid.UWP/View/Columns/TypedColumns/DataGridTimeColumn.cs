using System;
using System.Reflection;
using Telerik.UI.Xaml.Controls.Grid.Primitives;
using Telerik.UI.Xaml.Controls.Input;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Grid
{
    /// <summary>
    /// Represents an extended <see cref="DataGridTextColumn"/> that presents data of type <see cref="DateTime"/>. 
    /// The column will create a <see cref="DataGridTimeFilterControl"/> upon triggering the filtering UI through user input.
    /// </summary>
    public class DataGridTimeColumn : DataGridTextColumn
    {
        private static TypeInfo dateTypeInfo = typeof(DateTime).GetTypeInfo();
        private static Type timePickerType = typeof(RadTimePicker);

        private static Style defaultCellEditorStyle;

        internal override Style DefaultCellEditorStyle
        {
            get
            {
                if (defaultCellEditorStyle == null)
                {
                    defaultCellEditorStyle = ResourceHelper.LoadEmbeddedResource(
                        typeof(DataGridTextColumn),
                        "Telerik.UI.Xaml.Controls.Grid.View.Columns.Resources.DefaultTimeColumnEditorStyle.xaml",
                        "DefaultColumnEditorStyle") as Style;
                }
                return defaultCellEditorStyle;
            }
        }

        internal override bool CanEdit
        {
            get { return this.PropertyInfoInitialized && this.PropertyInfo.DataType.GetTypeInfo().IsAssignableFrom(dateTypeInfo) && this.CanUserEdit; }
        }

        /// <summary>
        /// Gets the type of the editor for the DataGridTimeColumn that is visualized when entering in edit mode.
        /// </summary>
        /// <returns>The type of the editor.</returns>
        public override object GetEditorType(object item)
        {
            return this.CanEdit ? timePickerType : DataGridTextColumn.TextBlockType;
        }

        /// <summary>
        /// Creates an instance of a RadTimePicker used by the column when entering edit mode.
        /// </summary>
        /// <returns>An instance of the editor.</returns>
        public override FrameworkElement CreateEditorContentVisual()
        {
            return new RadTimePicker();
        }

        /// <summary>
        /// Prepares all bindings and content set to the RadTimePicker visualized when entering edit mode.
        /// </summary>
        /// <param name="editorContent">The editor itself.</param>
        public override void PrepareEditorContentVisual(FrameworkElement editorContent, Binding binding)
        {
            editorContent.SetBinding(RadTimePicker.ValueProperty, binding);
        }

        /// <summary>
        /// Clears all bindings and content set to the RadTimePicker visualized when entering edit mode.
        /// </summary>
        /// <param name="editorContent">The editor itself.</param>
        public override void ClearEditorContentVisual(FrameworkElement editorContent)
        {
            editorContent.ClearValue(RadTimePicker.ValueProperty);
        }

        /// <summary>
        /// Creates the <see cref="DataGridTimeFilterControl" /> instance that allows filtering operation to be applied upon this column.
        /// </summary>
        protected internal override DataGridFilterControlBase CreateFilterControl()
        {
            return new DataGridTimeFilterControl()
            {
                PropertyName = this.PropertyName
            };
        }
    }
}
