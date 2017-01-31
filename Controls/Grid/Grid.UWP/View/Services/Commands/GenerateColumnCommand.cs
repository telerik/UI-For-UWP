using System;
using Telerik.Core;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Grid.Commands
{
    internal class GenerateColumnCommand : DataGridCommand
    {
        /// <summary>
        /// Determines whether the command can be executed against the provided parameter.
        /// </summary>
        public override bool CanExecute(object parameter)
        {
            return parameter is GenerateColumnContext;
        }

        /// <summary>
        /// Performs the core action given the provided parameter.
        /// </summary>
        public override void Execute(object parameter)
        {
            var context = parameter as GenerateColumnContext;
            if (context.FieldInfo == null)
            {
                return;
            }

            var dataType = context.FieldInfo.DataType;

            if (dataType == typeof(string))
            {
                context.Result = new DataGridTextColumn();
            }
            else if (dataType == typeof(bool) || dataType == typeof(bool?))
            {
                context.Result = new DataGridBooleanColumn();
            }
            else if (dataType == typeof(DateTime) || dataType == typeof(DateTimeOffset) || dataType == typeof(DateTime?) || dataType == typeof(DateTimeOffset?))
            {
                context.Result = new DataGridDateColumn();
            }
            else if (dataType == typeof(ImageSource) || dataType == typeof(byte[]))
            {
                context.Result = new DataGridImageColumn();
            }
            else if (NumericConverter.IsNumericType(dataType, true))
            {
                context.Result = new DataGridNumericalColumn();
            }
            else
            {
                // Default is TextColumn
                context.Result = new DataGridTextColumn();
            }
        }
    }
}