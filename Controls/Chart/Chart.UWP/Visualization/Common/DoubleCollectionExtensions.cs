using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Chart
{
    internal static class DoubleCollectionExtensions
    {
        public static DoubleCollection Clone(this DoubleCollection sourceCollection)
        {
            if (sourceCollection == null)
            {
                return null;
            }

            DoubleCollection clonedCollection = new DoubleCollection();
            foreach (double value in sourceCollection)
            {
                clonedCollection.Add(value);
            }

            return clonedCollection;
        }
    }
}
