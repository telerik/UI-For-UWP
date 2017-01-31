using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Core;
using Telerik.Data.Core.Layouts;

namespace Telerik.UI.Xaml.Controls.Grid
{
    internal class GridCellEditorModel : GridCellModel
    {
        internal static readonly int OriginalValuePropertyKey = PropertyKeys.Register(typeof(GridCellModel), "OriginalValue");

        /// <summary>
        /// Gets or sets the original content associated with the node.
        /// </summary>
        public object OriginalValue
        {
            get
            {
                return this.GetValue(OriginalValuePropertyKey);
            }
            set
            {
                this.DesiredSize = RadSize.Invalid;
                this.SetValue(OriginalValuePropertyKey, value);
            }
        }

        public bool IsFrozen
        {
            get;
            set;
        }

        public object EditorHost { get; set; }
    }
}
