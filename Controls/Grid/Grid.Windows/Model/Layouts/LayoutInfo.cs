namespace Telerik.Data.Core.Layouts
{
    internal struct LayoutInfo
    {
        public int Line;
        public int ChildLine; // This is the index of the line within the Children collection of the owning group (if any)
        public int LineSpan;
        public int Level;
        public int LevelSpan;
        public int Indent;
        public bool SpansThroughCells;
    }
}