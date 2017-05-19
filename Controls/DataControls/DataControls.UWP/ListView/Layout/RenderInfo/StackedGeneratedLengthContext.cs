namespace Telerik.Data.Core.Layouts
{
    internal class StackedGeneratedLengthContext : IGenerateLayoutLength
    {
        public StackedGeneratedLengthContext(int columns)
        {
            this.StackCount = columns;
        }

        public int StackCount { get; set; }
        public double GeneratedLength { get; set; }

        public double GenerateLength(double length)
        {
            return length;
        }
    }
}
