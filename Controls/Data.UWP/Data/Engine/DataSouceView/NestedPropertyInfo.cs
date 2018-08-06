namespace Telerik.Data.Core
{
    internal class NestedPropertyInfo
    {
        internal readonly object rootItem;
        internal readonly string nestedPropertyPath;

        internal NestedPropertyInfo(object rootItem, string nestedPropertyPath)
        {
            this.rootItem = rootItem;
            this.nestedPropertyPath = nestedPropertyPath;
        }
    }
}
