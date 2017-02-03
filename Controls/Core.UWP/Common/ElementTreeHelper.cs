using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Telerik.Core
{
    /// <summary>
    /// Exposes helper methods for searching and traversing visual trees.
    /// </summary>
    public static class ElementTreeHelper
    {
        /// <summary>
        /// Finds a visual ancestor from a given type and based on a given
        /// condition that is in the parent chain of a given child element.
        /// </summary>
        /// <typeparam name="T">The type of the element to look for.</typeparam>
        /// <param name="child">The child.</param>
        /// <param name="condition">The condition.</param>
        /// <returns>An instance of the given type if found, otherwise null.</returns>
        public static T FindVisualAncestor<T>(DependencyObject child, Predicate<DependencyObject> condition = null) where T : class
        {
            if (child == null)
            {
                return null;
            }

            if (condition == null)
            {
                condition = IsInstanceOfType<T>;
            }

            DependencyObject parent = VisualTreeHelper.GetParent(child);
            while (parent != null)
            {
                if (condition(parent))
                {
                    return parent as T;
                }

                parent = VisualTreeHelper.GetParent(parent);
            }

            return null;
        }

        /// <summary>
        /// Finds the last visual ancestor of the provided type.
        /// </summary>
        /// <typeparam name="T">The type of the ancestor to search for.</typeparam>
        /// <param name="child">The from which to start the search.</param>
        /// <param name="condition">A condition to test each ancestor of the T type.</param>
        /// <returns>The last ancestor of the specified T type which matches the condition.</returns>
        public static T FindLastVisualAncestor<T>(DependencyObject child, Predicate<DependencyObject> condition = null) where T : class
        {
            if (child == null)
            {
                return null;
            }

            if (condition == null)
            {
                condition = IsInstanceOfType<T>;
            }

            DependencyObject parent = VisualTreeHelper.GetParent(child);
            DependencyObject prevParent = null;
            do
            {
                prevParent = parent;
                parent = FindVisualAncestor<T>(parent, condition) as DependencyObject;
            }
            while (parent != null);

            return prevParent as T;
        }

        /// <summary>
        /// Finds a visual descendant from a given type and based on a given
        /// condition that is in the hierarchy of a given parent element.
        /// </summary>
        /// <typeparam name="T">The type of the element to look for.</typeparam>
        /// <param name="parent">The parent.</param>
        /// <param name="condition">The condition.</param>
        /// <returns>An instance of the given type if found, otherwise null.</returns>
        public static T FindVisualDescendant<T>(DependencyObject parent, Predicate<DependencyObject> condition = null) where T : class
        {
            if (parent == null)
            {
                return null;
            }

            if (condition == null)
            {
                condition = IsInstanceOfType<T>;
            }

            IEnumerator<DependencyObject> enumerator = EnumVisualDescendants(parent, condition).GetEnumerator();
            if (enumerator.MoveNext())
            {
                return enumerator.Current as T;
            }

            return null;
        }
        
        /// <summary>
        /// Returns a lazily evaluated iterator that allows linear iteration
        /// over a tree of DependencyObjects.
        /// </summary>
        /// <param name="parent">The root element of the tree to iterate over.</param>
        /// <param name="condition">A predicate to filter what elements are returned.</param>
        /// <param name="mode">The tree traversal mode of the iteration algorithm.</param>
        /// <returns>
        /// A lazily evaluated iterator that allows linear iteration
        /// over a tree of DependencyObjects.
        /// </returns>
        public static IEnumerable<DependencyObject> EnumVisualDescendants(DependencyObject parent, Predicate<DependencyObject> condition = null, TreeTraversalMode mode = TreeTraversalMode.BreadthFirst)
        {
            if (parent == null)
            {
                return null;
            }

            if (condition == null)
            {
                condition = (unused) => { return true; };
            }

            if (mode == TreeTraversalMode.BreadthFirst)
            {
                return EnumVisualDescendantBreadthFirst(parent, condition);
            }

            return EnumVisualDescendantDepthFirst(parent, condition);
        }

        /// <summary>
        /// Determines whether the specified element is rendered on the visual tree.
        /// </summary>       
        public static bool IsElementRendered(FrameworkElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (element.Visibility == Visibility.Collapsed ||
                element.ActualWidth == 0 || element.ActualHeight == 0)
            {
                return false;
            }

            Size renderSize = element.RenderSize;
            return renderSize.Width > 0 && renderSize.Height > 0;
        }

        /// <summary>
        /// Transforms the specified point using the TransformToVisual routine while checking for valid transform conditions - e.g. elements are both loaded and rendered.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="point">The transform point.</param>
        public static Point SafeTransformPoint(FrameworkElement from, FrameworkElement to, Point point)
        {
            if (from == null)
            {
                throw new ArgumentNullException(nameof(from));
            }

            if (to == null)
            {
                throw new ArgumentNullException(nameof(to));
            }

            if (!IsElementRendered(from) || !IsElementRendered(to))
            {
                return point;
            }

            return from.TransformToVisual(to).TransformPoint(point);
        }

        /// <summary>
        /// Transforms the specified rect using the TransformToVisual routine while checking for valid transform conditions - e.g. elements are both loaded and rendered.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="bounds">The bounds.</param>
        public static Rect SafeTransformBounds(FrameworkElement from, FrameworkElement to, Rect bounds)
        {
            if (from == null)
            {
                throw new ArgumentNullException(nameof(from));
            }

            if (to == null)
            {
                throw new ArgumentNullException(nameof(to));
            }

            if (!IsElementRendered(from) || !IsElementRendered(to))
            {
                return bounds;
            }

            return from.TransformToVisual(to).TransformBounds(bounds);
        }

        /// <summary>
        /// Determines whether all the ancestors of the specified <see cref="UIElement"/> instance are hit-test visible.
        /// </summary>
        /// <param name="element">The element to start from.</param>
        /// <returns>True if all the ancestors are hit-test visible, false otherwise.</returns>
        public static bool IsParentChainHitTestVisible(UIElement element)
        {
            UIElement current = element;
            while (current != null)
            {
                if (!current.IsHitTestVisible)
                {
                    return false;
                }

                current = VisualTreeHelper.GetParent(current) as UIElement;
            }

            return true;
        }

        /// <summary>
        /// Transforms a rectangle according to the provided float direction. Since a <see cref="T:Rect"/>
        /// struct has its origin always at the top left point, to make a <see cref="T:Rect"/> usable
        /// in a RightToLeft scenario we need to offset its X coordinate with its width.
        /// </summary>
        /// <param name="source">The source rectangle to transform.</param>
        /// <param name="direction">The flow direction to transform against.</param>
        /// <returns>The transformed rectangle.</returns>
        public static Rect TransformRectForFlowDirection(Rect source, FlowDirection direction)
        {
            if (direction == FlowDirection.LeftToRight)
            {
                return source;
            }

            return new Rect(source.X - source.Width, source.Y, source.Width, source.Height);
        }

        private static IEnumerable<DependencyObject> EnumVisualDescendantBreadthFirst(DependencyObject root, Predicate<DependencyObject> condition)
        {
            Queue<DependencyObject> queue = new Queue<DependencyObject>();
            queue.Enqueue(root);

            while (queue.Count > 0)
            {
                DependencyObject current = queue.Dequeue();
                int childrenCount = VisualTreeHelper.GetChildrenCount(current);
                for (int i = 0; i < childrenCount; i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(current, i);
                    if (condition(child))
                    {
                        yield return child;
                    }

                    queue.Enqueue(child);
                }
            }
        }

        private static IEnumerable<DependencyObject> EnumVisualDescendantDepthFirst(DependencyObject root, Predicate<DependencyObject> condition)
        {
            int visualChildrenCount = VisualTreeHelper.GetChildrenCount(root);
            for (int i = 0; i < visualChildrenCount; ++i)
            {
                DependencyObject child = VisualTreeHelper.GetChild(root, i);
                if (condition(child))
                {
                    yield return child;
                }

                foreach (DependencyObject el in EnumVisualDescendants(child))
                {
                    if (condition(el))
                    {
                        yield return el;
                    }
                }
            }
        }

        private static bool IsInstanceOfType<T>(DependencyObject instance) where T : class
        {
            return instance is T;
        }
    }
}