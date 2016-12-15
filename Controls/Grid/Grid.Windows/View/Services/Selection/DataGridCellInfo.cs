using System;
using Telerik.Data.Core.Layouts;

namespace Telerik.UI.Xaml.Controls.Grid
{
    /// <summary>
    /// Defines the abstraction of a cell within a <see cref="RadDataGrid"/> row.
    /// A grid cell is generally the intersection of a grid row and grid column.
    /// </summary>
    public class DataGridCellInfo
    {
        private ItemInfo rowItemInfo = ItemInfo.Invalid;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridCellInfo" /> class.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="column">The column.</param>
        public DataGridCellInfo(object item, DataGridColumn column)
        {
            this.Item = item;
            this.Column = column;
        }

        internal DataGridCellInfo(ItemInfo info, DataGridColumn column)
        {
            this.RowItemInfo = info;
            this.Column = column;
        }

        internal DataGridCellInfo(GridCellModel cellModel)
        {
            this.RowItemInfo = cellModel.ParentRow.ItemInfo;
            this.Column = cellModel.Column;
            this.Cell = cellModel;
        }

        /// <summary>
        /// Gets or sets the <see cref="DataGridColumn"/> instance associated with the cell.
        /// </summary>
        public DataGridColumn Column
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the data item that represents the grid row.
        /// </summary>
        public object Item
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the current value represented by the corresponding grid cell.
        /// This is a shortcut to the <see cref="DataGridColumn.GetValueForInstance"/> method.
        /// </summary>
        public object Value
        {
            get
            {
                if (this.Column != null && this.Item != null)
                {
                    return this.Column.GetValueForInstance(this.Item);
                }

                return null;
            }
        }

        internal GridCellModel Cell
        {
            get;
            set;
        }

        internal ItemInfo RowItemInfo
        {
            get
            {
                return this.rowItemInfo;
            }
            set
            {
                this.rowItemInfo = value;
                this.Item = this.rowItemInfo.Equals(ItemInfo.Invalid) ? null : this.rowItemInfo.Item;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var info = obj as DataGridCellInfo;

            if (info == null)
            {
                return false;
            }

            return this.Item == info.Item && this.Column == info.Column;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            if (this.Item != null && this.Column != null)
            {
                return this.Item.GetHashCode() ^ this.Column.GetHashCode();
            }

            return base.GetHashCode();
        }
    }
}
