using System;
using System.Collections.Generic;

namespace Telerik.UI.Xaml.Controls.Grid
{
    internal class ArrangeDataForDecorations
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Will be used for Cell Selection.")]
        private IDictionary<int, double> cellsData;
        private List<double> groupLevelPositions;
        private Dictionary<int, Tuple<double, double>> cellsBySlots;

        public ArrangeDataForDecorations(IDictionary<int, double> cellsData, IEnumerable<double> groupSizes, double groupsLength, double slotsLenght)
        {
            this.cellsData = cellsData;
            this.GroupsLenght = groupsLength;
            this.SlotsLenght = slotsLenght;
            this.ScrollBarEdge = this.GroupsLenght + this.SlotsLenght;

            // Calculate header positions
            this.groupLevelPositions = new List<double>();
            double groupsLenght = 0;
            foreach (var lenght in groupSizes)
            {
                this.groupLevelPositions.Add(groupsLenght);
                groupsLenght += lenght;
            }

            this.groupLevelPositions.Add(groupsLenght);

            // Calculate cell positions
            IList<double> cellPositions = new List<double>();
            this.cellsBySlots = new Dictionary<int, Tuple<double, double>>();
            this.SlotsStart = this.SlotsEnd = this.GroupsLenght;
            this.SlotsStartLine = this.SlotsEndLine = -1;

            foreach (var slotPair in cellsData)
            {
                if (this.SlotsStartLine == -1)
                {
                    this.SlotsStartLine = slotPair.Key;
                }

                cellPositions.Add(this.SlotsEnd);
                int slot = slotPair.Key;
                double lenght = slotPair.Value;
                double nextSlotsEnd = this.SlotsEnd + lenght;
                this.cellsBySlots.Add(slot, new Tuple<double, double>(this.SlotsEnd, nextSlotsEnd));
                this.SlotsEnd += lenght;

                this.SlotsEndLine = slotPair.Key;
            }

            cellPositions.Add(this.SlotsEnd);
        }

        public double GroupsLenght { get; private set; }
        public double SlotsLenght { get; private set; }
        public double ScrollBarEdge { get; private set; }

        public double SlotsStart { get; private set; }
        public double SlotsEnd { get; private set; }

        public int SlotsStartLine { get; private set; }
        public int SlotsEndLine { get; private set; }

        public double GetStartOfLevel(int level)
        {
            return this.groupLevelPositions[level];
        }

        public double GetEndOfLevel(int level)
        {
            return this.groupLevelPositions[level + 1];
        }

        public double GetStartOfSlot(int slot)
        {
            double result;
            if (this.TryGetStartOfSlot(slot, out result))
            {
                return result;
            }

            return this.SlotsStart;
        }

        public double GetEndOfSlot(int slot)
        {
            double result;
            if (this.TryGetEndOfSlot(slot, out result))
            {
                return result;
            }

            return this.SlotsEnd;
        }

        public bool TryGetStartOfSlot(int slot, out double result)
        {
            result = -1;
            Tuple<double, double> tuple = null;
            if (this.cellsBySlots.TryGetValue(slot, out tuple))
            {
                result = tuple.Item1;
                return true;
            }

            return false;
        }

        public bool TryGetEndOfSlot(int slot, out double result)
        {
            result = this.SlotsEnd;
            Tuple<double, double> tuple = null;
            if (this.cellsBySlots.TryGetValue(slot, out tuple))
            {
                result = tuple.Item2;
                return true;
            }

            return false;
        }
    }
}