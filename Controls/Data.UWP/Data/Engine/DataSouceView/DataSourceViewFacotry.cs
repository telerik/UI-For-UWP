using System;
using System.Collections;
using Windows.UI.Xaml.Data;

namespace Telerik.Data.Core
{
    internal static class DataSourceViewFacotry
    {
        public static IDataSourceView CreateDataSourceView(object dataSource)
        {
            var concreteDataSourceView = TryGetDataSourceViewForEnumerable(dataSource);

            if (concreteDataSourceView == null)
            {
                throw new InvalidOperationException("Specified data source not supported");
            }

            return concreteDataSourceView;
        }

        private static IDataSourceView TryGetDataSourceViewForEnumerable(object dataSource)
        {
            var collectionView = dataSource as ICollectionView;

            if (collectionView != null)
            {
                return new CollectionViewDataSourceView(collectionView);
            }

            var enumerable = dataSource as IEnumerable;

            if (enumerable != null)
            {
                return new EnumerableDataSourceView(enumerable);
            }

            return null;
        }
    }
}