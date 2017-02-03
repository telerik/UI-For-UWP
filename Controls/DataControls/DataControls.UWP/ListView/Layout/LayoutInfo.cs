namespace Telerik.Data.Core.Layouts
{
    internal struct LayoutInfo
    {
        public int Line { get; set; }
        public int ChildLine { get; set; } // This is the index of the line within the Children collection of the owning group (if any)
        public int LineSpan { get; set; }
        public int Level { get; set; }
        public int LevelSpan { get; set; }
        public int Indent { get; set; }
        public bool SpansThroughCells { get; set; }
    }
}