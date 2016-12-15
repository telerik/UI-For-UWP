using System;
using Telerik.Core;

namespace Telerik.Charting
{
    internal static class AnnotationHelper
    {
        /// <summary>
        /// Clips the <paramref name="rect"/> to the <paramref name="container"/>.
        /// </summary>
        /// <param name="rect">The <see cref="RadRect"/> to be clipped.</param>
        /// <param name="container">The container.</param>
        /// <param name="borderOverflow">The border (stroke thickness) of the <paramref name="rect"/>.</param>
        /// <param name="dashPatternLength">The length of the dash pattern of the <see cref="RadRect"/> stroke.</param>
        /// <returns>The clipped rectangle.</returns>
        internal static RadRect ClipRectangle(RadRect rect, RadRect container, double borderOverflow, double dashPatternLength)
        {
            // extend the container with the element border
            container.X -= borderOverflow;
            container.Y -= borderOverflow;
            container.Width += 2 * borderOverflow;
            container.Height += 2 * borderOverflow; 

            if (!rect.IntersectsWith(container))
            {
                return RadRect.Empty;
            }

            // calculate the clipped rectangle, extended with the element border
            double xFrom = RadMath.CoerceValue(rect.X, container.X, container.Right);
            double yFrom = RadMath.CoerceValue(rect.Y, container.Y, container.Bottom);
            double xTo = RadMath.CoerceValue(rect.Right, container.X, container.Right);
            double yTo = RadMath.CoerceValue(rect.Bottom, container.Y, container.Bottom);

            if (dashPatternLength == 0 || double.IsNaN(dashPatternLength) || double.IsInfinity(dashPatternLength))
            {
                dashPatternLength = 1;
            }
            
            // first check if "To" values are clipped, and if so, add the additional space needed by the dash array to be rendered correctly
            if (rect.Right != xTo)
            {
                xTo += (rect.Right - xTo) % dashPatternLength;
            }

            if (rect.Bottom != yTo)
            {
                yTo += (rect.Bottom - yTo) % dashPatternLength;
            }

            // check if "From" values are clipped, and if so, add the additional space needed by the dash array to be rendered correctly and change the rect coordinates
            if (rect.X != xFrom)
            {
                xFrom -= (xFrom - rect.X) % dashPatternLength;
                rect.X = xFrom;
            }

            if (rect.Y != yFrom)
            {
                yFrom -= (yFrom - rect.Y) % dashPatternLength;
                rect.Y = yFrom;
            }

            rect.Width = xTo - xFrom;
            rect.Height = yTo - yFrom;

            return rect;
        }

        /// <summary>
        /// Clips the <paramref name="line"/> to the <paramref name="container"/>.
        /// </summary>
        /// <param name="line">The line to be clipped.</param>
        /// <param name="container">The container.</param>
        /// <param name="borderOverflow">The border (stroke thickness) of the <paramref name="line"/>.</param>
        /// <param name="dashPatternLength">The length of the dash pattern of the line stroke.</param>
        /// <returns>The clipped line.</returns>
        internal static RadLine ClipLine(RadLine line, RadRect container, double borderOverflow, double dashPatternLength)
        {
            // extend the container with the element border
            container.X -= borderOverflow;
            container.Y -= borderOverflow;
            container.Width += 2 * borderOverflow;
            container.Height += 2 * borderOverflow; 
            
            bool firstPointInside = container.Contains(line.X1, line.Y1);
            bool secondPointInside = container.Contains(line.X2, line.Y2);

            if (firstPointInside && secondPointInside)
            {
                return line;
            }

            if (dashPatternLength == 0 || double.IsNaN(dashPatternLength) || double.IsInfinity(dashPatternLength))
            {
                dashPatternLength = 1;
            }

            // find intersectionns of the line with the sides of the container
            double topIntersectionX = RadMath.CalculateIntersectionX(line.X1, line.Y1, line.X2, line.Y2, container.Y);
            double bottomIntersectionX = RadMath.CalculateIntersectionX(line.X1, line.Y1, line.X2, line.Y2, container.Bottom);
            double leftIntersectionY = RadMath.CalculateIntersectionY(line.X1, line.Y1, line.X2, line.Y2, container.X);
            double rightIntersectionY = RadMath.CalculateIntersectionY(line.X1, line.Y1, line.X2, line.Y2, container.Right);

            // slope of the line: angle between the line ant the horizon (-pi/2, pi/2)
            var angle = Math.Atan((line.Y1 - line.Y2) / (line.X2 - line.X1));

            bool intersectsWithRect = false;

            // clip to container sides
            intersectsWithRect |= AnnotationHelper.TryClipToContainerTop(ref line, container, topIntersectionX, dashPatternLength, angle > 0 ? angle : Math.PI + angle);
            intersectsWithRect |= AnnotationHelper.TryClipToContainerBottom(ref line, container, bottomIntersectionX, dashPatternLength, angle > 0 ? angle : Math.PI + angle);
            intersectsWithRect |= AnnotationHelper.TryClipToContainerLeft(ref line, container, leftIntersectionY, dashPatternLength, angle);
            intersectsWithRect |= AnnotationHelper.TryClipToContainerRight(ref line, container, rightIntersectionY, dashPatternLength, angle);

            if (!intersectsWithRect)
            {
                line = new RadLine();
            }

            return line;
        }

        /// <summary>
        /// Clips vertical or horizontal grid line to rectangle area.
        /// </summary>
        /// <param name="line">The line to be clipped.</param>
        /// <param name="container">The container.</param>
        /// <param name="borderOverflow">The border (stroke thickness) of the <paramref name="line"/>.</param>
        /// <param name="dashPatternLength">The length of the dash pattern of the line stroke.</param>
        /// <returns>The clipped line.</returns>
        internal static RadLine ClipGridLine(RadLine line, RadRect container, double borderOverflow, double dashPatternLength)
        {
            // extend the container with the element border
            container.X -= borderOverflow;
            container.Y -= borderOverflow;
            container.Width += 2 * borderOverflow;
            container.Height += 2 * borderOverflow;

            if (dashPatternLength == 0 || double.IsNaN(dashPatternLength) || double.IsInfinity(dashPatternLength))
            {
                dashPatternLength = 1;
            }

            if (line.X1 == line.X2)
            {
                if (line.X1 < container.X || line.X1 > container.Right)
                {
                    return new RadLine();
                }

                line.Y1 = container.Y - (container.Y - line.Y1) % dashPatternLength;
                line.Y2 = container.Bottom;
            }
            else if (line.Y1 == line.Y2)
            {
                if (line.Y1 < container.Y || line.Y1 > container.Bottom)
                {
                    return new RadLine();
                }

                line.X1 = container.X - (container.X - line.X1) % dashPatternLength;
                line.X2 = container.Right;
            }

            return line;
        }

        private static bool TryClipToContainerRight(ref RadLine line, RadRect container, double rightIntersectionY, double dashPatternLength, double angle)
        {
            bool intersectsWithRect = false;

            if (container.Y <= rightIntersectionY && rightIntersectionY <= container.Bottom)
            {
                if (line.X1 < line.X2 && line.X2 > container.Right)
                {
                    intersectsWithRect = true;
                    line.Y2 = rightIntersectionY;
                    line.X2 = container.Right;
                }
                else if (line.X2 < line.X1 && line.X1 > container.Right)
                {
                    intersectsWithRect = true;
                    var lengthToAdd = RadMath.GetPointDistance(container.Right, line.X1, rightIntersectionY, line.Y1) % dashPatternLength;

                    line.Y1 = rightIntersectionY - lengthToAdd * Math.Sin(angle);
                    line.X1 = container.Right + lengthToAdd * Math.Cos(angle);
                }
            }

            return intersectsWithRect;
        }

        private static bool TryClipToContainerLeft(ref RadLine line, RadRect container, double leftIntersectionY, double dashPatternLength, double angle)
        {
            bool intersectsWithRect = false;

            if (container.Y <= leftIntersectionY && leftIntersectionY <= container.Bottom)
            {
                if (line.X1 < line.X2 && line.X1 < container.X)
                {
                    intersectsWithRect = true;
                    var lengthToAdd = RadMath.GetPointDistance(container.X, line.X1, leftIntersectionY, line.Y1) % dashPatternLength;

                    line.Y1 = leftIntersectionY + lengthToAdd * Math.Sin(angle);
                    line.X1 = container.X - lengthToAdd * Math.Cos(angle);
                }
                else if (line.X2 < line.X1 && line.X2 < container.X)
                {
                    intersectsWithRect = true;
                    line.Y2 = leftIntersectionY;
                    line.X2 = container.X;
                }
            }

            return intersectsWithRect;
        }

        private static bool TryClipToContainerBottom(ref RadLine line, RadRect container, double bottomIntersectionX, double dashPatternLength, double angle)
        {
            bool intersectsWithRect = false;

            if (container.X <= bottomIntersectionX && bottomIntersectionX <= container.Right)
            {
                if (line.Y1 < line.Y2 && line.Y2 > container.Bottom)
                {
                    intersectsWithRect = true;
                    line.X2 = bottomIntersectionX;
                    line.Y2 = container.Bottom;
                }
                else if (line.Y2 < line.Y1 && line.Y1 > container.Bottom)
                {
                    intersectsWithRect = true;
                    var lengthToAdd = RadMath.GetPointDistance(bottomIntersectionX, line.X1, container.Bottom, line.Y1) % dashPatternLength;

                    line.X1 = bottomIntersectionX - lengthToAdd * System.Math.Cos(angle);
                    line.Y1 = container.Bottom + lengthToAdd * System.Math.Sin(angle);
                }
            }

            return intersectsWithRect;
        }

        private static bool TryClipToContainerTop(ref RadLine line, RadRect container, double topIntersectionX, double dashPatternLength, double angle)
        {
            bool intersectsWithRect = false;

            if (container.X <= topIntersectionX && topIntersectionX <= container.Right)
            {
                if (line.Y1 < line.Y2 && line.Y1 < container.Y)
                {
                    intersectsWithRect = true;
                    var lengthToAdd = RadMath.GetPointDistance(topIntersectionX, line.X1, container.Y, line.Y1) % dashPatternLength;

                    line.X1 = topIntersectionX + lengthToAdd * Math.Cos(angle);
                    line.Y1 = container.Y - lengthToAdd * Math.Sin(angle);
                }
                else if (line.Y2 < line.Y1 && line.Y2 < container.Y)
                {
                    intersectsWithRect = true;
                    line.X2 = topIntersectionX;
                    line.Y2 = container.Y;
                }
            }

            return intersectsWithRect;
        }
    }
}