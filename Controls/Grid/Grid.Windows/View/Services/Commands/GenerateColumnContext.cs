using System;
using Telerik.Data.Core.Fields;

namespace Telerik.UI.Xaml.Controls.Grid.Commands
{
    /// <summary>
    /// Represents a context, passed to a <see cref="CommandId.GenerateColumn"/> command.
    /// </summary>
    public class GenerateColumnContext
    {
        /// <summary>
        /// Gets or sets the generated column.
        /// </summary>
        public DataGridColumn Result
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the name of the property associated with the column.
        /// </summary>
        public string PropertyName
        {
            get
            {
                if (this.FieldInfo != null)
                {
                    return this.FieldInfo.Name;
                }

                return string.Empty;
            }
        }

        internal IDataFieldInfo FieldInfo
        {
            get;
            set;
        }
    }
}
