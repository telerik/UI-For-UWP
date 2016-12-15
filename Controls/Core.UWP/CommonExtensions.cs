using System;
using System.Collections.Generic;
using Windows.Foundation;

namespace Telerik.Core
{
    internal static class CommonExtensions
    {
        public static int IndexOf<T>(this IReadOnlyList<T> list, T item)
        {
            int index = -1;

            foreach (var listItem in list)
            {
                index++;

                if (listItem.Equals(item))
                {
                    return index;
                }
            }

            return ~index;
        }

        public static Rect ToRect(this RadRect rect)
        {
            return new Rect(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public static RadRect ToRadRect(this Rect rect)
        {
            return new RadRect(rect.Left, rect.Top, rect.Width, rect.Height);
        }

        public static RadPoint ToRadPoint(this Point point)
        {
            return new RadPoint(point.X, point.Y);
        }

        public static Point ToPoint(this RadPoint point)
        {
            return new Point(point.X, point.Y);
        }

        public static Size ToSize(this RadSize size)
        {
            return new Size(size.Width, size.Height);
        }

        public static RadSize ToRadSize(this Size size)
        {
            return new RadSize(size.Width, size.Height);
        }
    }
}