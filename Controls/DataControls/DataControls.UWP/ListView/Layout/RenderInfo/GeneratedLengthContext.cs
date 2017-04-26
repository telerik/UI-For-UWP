using System.Collections.Generic;

namespace Telerik.Data.Core.Layouts
{
    internal class StaggeredGeneratedLength : IGenerateLayoutLength
    {
        internal Dictionary<int, double> columnsLength = new Dictionary<int, double>();

        public StaggeredGeneratedLength(int columns)
        {
            this.StackCount = columns;

            for (int i = 1; i <= this.StackCount; i++)
            {
                this.columnsLength.Add(i, 0);
            }
        }

        public int StackCount { get; set; }

        public int GetShortestColumnKey()
        {
            int minKey = 1;
            double minValue = double.PositiveInfinity;
            for (int i = 1; i <= this.columnsLength.Count; i++)
            {
                if (this.columnsLength[i] < minValue)
                {
                    minKey = i;
                    minValue = this.columnsLength[i];
                }
            }

            return minKey;
        }

        public double GenerateLength(double length)
        {
            int minKey = this.GetShortestColumnKey();

            var minLength = this.columnsLength[minKey];

            this.columnsLength[minKey] += length;

            var newMinKey = this.GetShortestColumnKey();
            var newMinLength = this.columnsLength[newMinKey];

            var difference = newMinLength - minLength;

            return difference;
        }
    }
}
