using System;
using System.Collections.Generic;

namespace Telerik.UI.Xaml.Controls.Grid.Tests
{
    public static class IEnumerableExtensions
    {
        public static bool ItemsEqual<T>(this IEnumerable<T> first, IEnumerable<T> second)
        {
            if (first == null && second == null)
            {
                return true;
            }
            else if (first == null || second == null)
            {
                return false;
            }

            var enumerator1 = first.GetEnumerator();
            var enumerator2 = second.GetEnumerator();

            bool firstMove = enumerator1.MoveNext();
            bool secondMove = enumerator2.MoveNext();
            while (firstMove && secondMove)
            {
                if (!Object.Equals(enumerator1.Current, enumerator2.Current))
                {
                    return false;
                }
                firstMove = enumerator1.MoveNext();
                secondMove = enumerator2.MoveNext();
            }
            return !firstMove && !secondMove;
        }

        public static bool DoublesAreClose(this IEnumerable<double?> first, IEnumerable<double?> second, double epsilon = 0.01)
        {
            if (first == null && second == null)
            {
                return true;
            }
            else if (first == null || second == null)
            {
                return false;
            }

            var enumerator1 = first.GetEnumerator();
            var enumerator2 = second.GetEnumerator();

            bool firstMove = enumerator1.MoveNext();
            bool secondMove = enumerator2.MoveNext();
            while (firstMove && secondMove)
            {
                double? d1 = enumerator1.Current;
                double? d2 = enumerator2.Current;
                if (d1.HasValue != d2.HasValue || (d1.HasValue &&
                    (!(double.IsPositiveInfinity(d1.Value) && double.IsPositiveInfinity(d2.Value)) &&
                    !(double.IsNegativeInfinity(d1.Value) && double.IsNegativeInfinity(d2.Value)) &&
                    !(double.IsNaN(d1.Value) && double.IsNaN(d2.Value)) &&
                    !(Math.Abs(d1.Value - d2.Value) <= epsilon))))
                {
                    return false;
                }
                firstMove = enumerator1.MoveNext();
                secondMove = enumerator2.MoveNext();
            }
            return !firstMove && !secondMove;
        }
    }
}
