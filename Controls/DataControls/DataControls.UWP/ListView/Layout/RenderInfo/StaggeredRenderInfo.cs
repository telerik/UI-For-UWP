using System.Collections.Generic;

namespace Telerik.Data.Core.Layouts
{
    internal class StaggeredRenderInfo
    {
        private double averageItemlength;

        private List<double> columnsLength = new List<double>();
        private Dictionary<int, List<int>> slots = new Dictionary<int, List<int>>();
        private List<double> itemLength = new List<double>();

        internal StaggeredRenderInfo(int stackCount)
        {
            for (int i = 0; i < stackCount; i++)
            {
                this.columnsLength.Add(0);
            }
        }

        internal void UpdateAverageLength(double averageLength, double size)
        {
            this.averageItemlength = averageLength;
            for (int i = this.itemLength.Count; i < size; i++)
            {
                this.Update(i, this.averageItemlength);
            }
        }
        
        internal void Update(int id, double length)
        {
            if (this.itemLength.Count > id)
            {
                var oldvalue = this.itemLength[id];
                if (oldvalue != length)
                {
                    this.itemLength[id] = length;
                    this.RefreshColumns(id);
                }
            }
            else
            {
                this.Insert(id, length);
            }
        }

        internal int SlotFromPhysicalOffset(double offset)
        {
            List<int> slotsPerOffset = new List<int>();
            foreach (var pair in this.slots)
            {
                double generatedLength = 0;
                foreach (int id in pair.Value)
                {
                    generatedLength += this.itemLength[id];
                    if (generatedLength >= offset)
                    {
                        slotsPerOffset.Add(id);
                        break;
                    }
                }
            }

            slotsPerOffset.Sort();

            return slotsPerOffset.Count > 0 ? slotsPerOffset[0] : 0;
        }

        internal int GetShortestColumnKey()
        {
            int minKey = 0;
            double minValue = double.PositiveInfinity;
            for (int i = 0; i <= this.columnsLength.Count - 1; i++)
            {
                if (this.columnsLength[i] < minValue)
                {
                    minKey = i;
                    minValue = this.columnsLength[i];
                }
            }

            return minKey;
        }

        internal void RefreshColumns(int startIndex)
        {
            foreach (var columnKey in this.slots)
            {
                double currentColumnLength = 0;
                var indexList = columnKey.Value;
                indexList.Sort();

                for (int i = 0; i < indexList.Count; i++)
                {
                    int currentId = columnKey.Value[i];
                    if (currentId < startIndex)
                    {
                        currentColumnLength += this.itemLength[currentId];
                    }
                    else
                    {
                        this.slots[columnKey.Key].Remove(currentId);
                        i--;
                    }
                }

                this.columnsLength[columnKey.Key] = currentColumnLength;
            }

            for (int i = startIndex; i < this.itemLength.Count; i++)
            {
                this.Insert(i, this.itemLength[i]);
            }
        }

        internal double PhysicalOffsetFromSlot(int slot)
        {
            int column = 1;
            foreach (var slotPair in this.slots)
            {
                if (slotPair.Value.Contains(slot))
                {
                    column = slotPair.Key;
                    break;
                }
            }

            double offset = 0;
            var slotColl = this.slots[column];
            slotColl.Sort();
            for (int i = 0; i < slotColl.Count - 1; i++)
            {
                if (slotColl[i] < slot)
                {
                    offset += this.itemLength[slotColl[i]];
                }
            }

            return offset;
        }

        internal int GetColumnForId(int id)
        {
            foreach (var pair in this.slots)
            {
                if (pair.Value.Contains(id))
                {
                    return pair.Key;
                }
            }

            return 0;
        }

        internal double GetMaxColumnLength()
        {
            double maxLength = 0;

            foreach (var length in this.columnsLength)
            {
                if (length > maxLength)
                {
                    maxLength = length;
                }
            }
            return maxLength;
        }

        internal double GetLengthForSlot(int slot)
        {
            if (this.itemLength.Count > slot)
            {
                return this.itemLength[slot];
            }

            return 0;
        }

        private void Insert(int id, double length)
        {
            if (this.itemLength.Count > id)
            {
                this.itemLength[id] = length;
            }
            else
            {
                for (int i = this.itemLength.Count; i < id; i++)
                {
                    this.itemLength.Add(this.averageItemlength);
                    this.ArrangeItem(i, this.averageItemlength);
                }

                this.itemLength.Add(length);
            }

            this.ArrangeItem(id, length);
        }

        private void ArrangeItem(int id, double length)
        {
            int columnKey = this.GetShortestColumnKey();
            if (!this.slots.ContainsKey(columnKey))
            {
                this.slots.Add(columnKey, new List<int>() { id });
            }
            else
            {
                this.slots[columnKey].Add(id);
            }

            this.columnsLength[columnKey] += length;
        }
    }
}
