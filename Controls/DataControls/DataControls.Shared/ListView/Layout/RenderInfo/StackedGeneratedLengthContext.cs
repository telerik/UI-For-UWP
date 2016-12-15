using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.Data.Core.Layouts
{
    internal class StackedGeneratedLengthContext : IGenerateLayoutLength
    {
        public int StackCount { get; set; }
        public double GeneratedLength { get; set; }
        public StackedGeneratedLengthContext(int columns)
        {
            this.StackCount = columns;
        }

        public double GenerateLength(double length)
        {
            return length;
        }
    }
}
