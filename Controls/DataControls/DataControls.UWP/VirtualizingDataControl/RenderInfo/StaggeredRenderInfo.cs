using System.Collections.Generic;
using System.Linq;
using Telerik.Core.Data;

namespace Telerik.UI.Xaml.Controls.Data
{
    internal class StaggeredRenderInfo
    {
        // item per hashcode with its row/column and position
        private readonly SortedDictionary<IDataSourceItem, ItemLayoutInfo> itemSlotsRepository;

        private int columnCount;

        public StaggeredRenderInfo(int columnCount)
        {
            this.columnCount = columnCount;
            this.itemSlotsRepository = new SortedDictionary<IDataSourceItem, ItemLayoutInfo>(new DataSourceItemComparer());
        }

        public ItemLayoutInfo GetFreeSpot(IDataSourceItem item, bool forward)
        {
            if (forward && item.Previous == null || !forward && item.Next == null)
            {
                return new ItemLayoutInfo();
            }

            // TODO Implement for backwards
            double[] columnsLength = forward ? new double[this.columnCount] : Enumerable.Repeat(double.MaxValue, this.columnCount).ToArray();

            var pivotItem = forward ? item.Previous : item.Next;
            ItemLayoutInfo itemInfo;

            List<int> foundColumns = new List<int>();

            while (foundColumns.Count < this.columnCount)
            {
                if (pivotItem != null)
                {
                    itemInfo = this.GetRenderInfo(pivotItem);

                    if (itemInfo != ItemLayoutInfo.Invalid && !foundColumns.Contains(itemInfo.ColumnIndex))
                    {
                        // Implement backward
                        foundColumns.Add(itemInfo.ColumnIndex);
                        columnsLength[itemInfo.ColumnIndex] = itemInfo.Position + itemInfo.Length;
                    }

                    pivotItem = forward ? item.Previous : item.Next;
                }
            }

            var min = columnsLength.Min();
            ItemLayoutInfo result = ItemLayoutInfo.Invalid;

            for (int i = 0; i < columnsLength.Length; i++)
            {
                if (columnsLength[i] == min)
                {
                    result = new ItemLayoutInfo(columnsLength[i], 0, i);
                }
            }

            return result;
        }

        public void SetRenderInfo(IDataSourceItem item, ItemLayoutInfo info)
        {
            if (info != ItemLayoutInfo.Invalid)
            {
                this.itemSlotsRepository[item] = info;
            }
        }

        public void Clear()
        {
            this.itemSlotsRepository.Clear();
        }

        public ItemLayoutInfo GetRenderInfo(IDataSourceItem item)
        {
            ItemLayoutInfo info;

            if (this.itemSlotsRepository.TryGetValue(item, out info))
            {
                return info;
            }

            return ItemLayoutInfo.Invalid;
        }

        public void EnsureCorrectLayout(bool forward)
        {
            double[] columnsLength = forward ? new double[this.columnCount] : Enumerable.Repeat(double.MaxValue, this.columnCount).ToArray();

            var source = forward ? this.itemSlotsRepository : this.itemSlotsRepository.Reverse();

            foreach (var item in this.itemSlotsRepository)
            {
                var columnIndex = item.Value.ColumnIndex;
                var arrangePosition = forward ? item.Value.Position : item.Value.Position + item.Value.Length;
                var freeColumnLength = forward ? columnsLength.Min() : columnsLength.Max();

                if ((forward && freeColumnLength < arrangePosition) || (!forward && freeColumnLength > arrangePosition))
                {
                    for (int i = 0; i < columnsLength.Length; i++)
                    {
                        if (columnsLength[i] == freeColumnLength)
                        {
                            columnIndex = i;
                            this.itemSlotsRepository[item.Key] = new ItemLayoutInfo { ColumnIndex = columnIndex, Position = freeColumnLength, Length = item.Value.Length };
                            break;
                        }
                    }
                }

                if (forward)
                {
                    columnsLength[columnIndex] += item.Value.Length;
                }
                else
                {
                    columnsLength[columnIndex] -= item.Value.Length;

                    if (columnsLength[columnIndex] < 0)
                    {
                        this.EnsureCorrectLayout(true);
                        break;
                    }
                }
            }
        }

        internal struct ItemLayoutInfo
        {
            public static ItemLayoutInfo Invalid = new ItemLayoutInfo();

            public ItemLayoutInfo(double position, double length, int columnIndex)
            {
                this.ColumnIndex = columnIndex;
                this.Length = length;
                this.Position = position;
            }

            public int ColumnIndex { get; set; }
            public double Length { get; set; }

            public double Position { get; set; }

            public static bool operator ==(ItemLayoutInfo info1, ItemLayoutInfo info2)
            {
                return info1.ColumnIndex == info2.ColumnIndex && info1.Length == info2.Length && info1.Position == info2.Position;
            }

            /// <summary>
            /// Determines whether two RadRect structures are not equal.
            /// </summary>
            public static bool operator !=(ItemLayoutInfo rect1, ItemLayoutInfo rect2)
            {
                return !(rect1 == rect2);
            }

            public override bool Equals(object obj)
            {
                if (obj is ItemLayoutInfo)
                {
                    return (ItemLayoutInfo)obj == this;
                }

                return false;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        private class DataSourceItemComparer : IComparer<IDataSourceItem>
        {
            public int Compare(IDataSourceItem x, IDataSourceItem y)
            {
                return x.Index.CompareTo(y.Index);
            }
        }
    }
}
