using System;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Telerik.Core
{
    /// <summary>
    /// Processes user manipulations on a <see cref="FrameworkElement"/>.
    /// </summary>
    public class ManipulationInputProcessor
    {
        private readonly FrameworkElement element;
        private readonly GestureRecognizer gestureRecognizer;
        private readonly FrameworkElement reference;

        private Rect elementOriginalRect;
        private double maxZoom;
        private double minZoom;
        private Thickness scaleOffsets;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManipulationInputProcessor" /> class.
        /// </summary>
        /// <param name="target">The target element.</param>
        /// <param name="referenceFrame">The element that will be used as a reference frame.</param>
        /// <param name="gestureSettings">The gesture recognizer settings.</param>
        public ManipulationInputProcessor(FrameworkElement target, FrameworkElement referenceFrame, GestureSettings gestureSettings)
        {
            this.gestureRecognizer = new GestureRecognizer { GestureSettings = gestureSettings };
            this.element = target;
            this.reference = referenceFrame;

            // Set up pointer event handlers. These receive input events that are used by the gesture recognizer.
            this.reference.PointerCanceled += this.OnPointerCanceled;
            this.reference.PointerPressed += this.OnPointerPressed;
            this.reference.PointerReleased += this.OnPointerReleased;
            this.reference.PointerMoved += this.OnPointerMoved;
            this.reference.DoubleTapped += this.OnDoubleTapped;

            // Set up event handlers to respond to gesture recognizer output
            ////this.gestureRecognizer.Tapped += this.OnTapped;
            ////this.gestureRecognizer.RightTapped += this.OnRightTapped;
            ////this.gestureRecognizer.ManipulationStarted += this.OnManipulationStarted;
            ////this.gestureRecognizer.ManipulationCompleted += this.OnManipulationCompleted;

            this.gestureRecognizer.ManipulationUpdated += this.OnManipulationUpdated;
        }

        /// <summary>
        /// Gets the element original rect.
        /// </summary>
        public Rect ElementOriginalRect
        {
            get
            {
                return this.elementOriginalRect;
            }
        }

        /// <summary>
        /// Gets or sets the settings of the gesture recognizer.
        /// </summary>
        public GestureSettings GestureSettings
        {
            get
            {
                return this.gestureRecognizer.GestureSettings;
            }
            set
            {
                this.gestureRecognizer.GestureSettings = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum zoom factor.
        /// </summary>
        public double MaxZoom
        {
            get
            {
                return this.maxZoom;
            }
            set
            {
                if (this.maxZoom != value)
                {
                    this.maxZoom = value;

                    if (this.GetZoom() > this.maxZoom)
                    {
                        this.SetAbsoluteZoom(this.maxZoom);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the minimum zoom factor.
        /// </summary>
        public double MinZoom
        {
            get
            {
                return this.minZoom;
            }
            set
            {
                if (this.minZoom != value)
                {
                    this.minZoom = value;

                    if (this.GetZoom() < this.minZoom)
                    {
                        this.SetAbsoluteZoom(this.minZoom);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the image manipulations will be the reset on double tap.
        /// </summary>
        public bool ResetOnDoubleTap { get; set; }

        /// <summary>
        /// Gets or sets the scale offset.
        /// </summary>
        public Thickness ScaleOffsets
        {
            get
            {
                return this.scaleOffsets;
            }
            set
            {
                if (this.scaleOffsets != value)
                {
                    this.scaleOffsets = value;
                    CorrectBounds(this.elementOriginalRect, this.reference.RenderSize, this.scaleOffsets, this.element.RenderTransform as TransformGroup);
                }
            }
        }

        /// <summary>
        /// Gets or sets the matrix of the currently applied transformation.
        /// </summary>
        public Matrix TransformMatrix
        {
            get
            {
                return (this.element.RenderTransform as TransformGroup).Value;
            }
            set
            {
                var transform = new TransformGroup();
                transform.Children.Add(new MatrixTransform { Matrix = value });

                //// TODO: add zoom coercion

                CorrectBounds(this.elementOriginalRect, this.reference.RenderSize, this.scaleOffsets, transform);
                this.element.RenderTransform = transform;
            }
        }

        /// <summary>
        /// Calculates the current element rotation angle.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public double GetRotationAngle()
        {
            var matrix = this.TransformMatrix;
            return -Math.Atan2(matrix.M21, matrix.M22);
        }

        /// <summary>
        /// Calculates the transformed element bounds.
        /// </summary>
        /// <returns>The bounds.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public Rect GetTransformedElementBounds()
        {
            return this.element.RenderTransform.TransformBounds(this.elementOriginalRect);
        }

        /// <summary>
        /// Calculates the position of the transformed element center.
        /// </summary>
        /// <returns>The center.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public Point GetTransformedElementCenter()
        {
            var bounds = this.GetTransformedElementBounds();
            return new Point((bounds.Left + bounds.Right) / 2, (bounds.Top + bounds.Bottom) / 2);
        }

        /// <summary>
        /// Calculates the current element translation.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public Point GetTranslation()
        {
            var originalCenter = new Point((this.elementOriginalRect.Right + this.elementOriginalRect.Left) / 2, (this.elementOriginalRect.Top + this.elementOriginalRect.Bottom) / 2);
            var transformedCenter = this.GetTransformedElementCenter();

            return new Point(transformedCenter.X - originalCenter.X, transformedCenter.Y - originalCenter.Y);
        }

        /// <summary>
        /// Calculates the current zoom factor of the element.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public double GetZoom()
        {
            var matrix = this.TransformMatrix;
            return Math.Sqrt(matrix.M11 * matrix.M11 + matrix.M12 * matrix.M12);
        }

        /// <summary>
        /// Initializes the transforms.
        /// </summary>
        /// <param name="minimumZoom">The min zoom.</param>
        /// <param name="maximumZoom">The max zoom.</param>
        /// <param name="offsetMarginScale">The offset margin scale.</param>
        /// <param name="resetOnDoubleTap">The reset on double tap.</param>
        public void InitializeTransforms(double minimumZoom, double maximumZoom, Thickness offsetMarginScale, bool resetOnDoubleTap = true)
        {
            this.minZoom = minimumZoom;
            this.maxZoom = maximumZoom;
            this.scaleOffsets = offsetMarginScale;
            this.ResetOnDoubleTap = resetOnDoubleTap;

            var availableSize = this.reference.RenderSize;

            this.elementOriginalRect = new Rect(
                (availableSize.Width - this.element.RenderSize.Width) / 2,
                (availableSize.Height - this.element.RenderSize.Height) / 2,
                this.element.RenderSize.Width,
                this.element.RenderSize.Height);

            var centerX = (1 - availableSize.Width / this.elementOriginalRect.Width) / 2;
            var centerY = (1 - availableSize.Height / this.elementOriginalRect.Height) / 2;

            this.element.RenderTransformOrigin = new Point(centerX, centerY);

            var transform = new TransformGroup();
            CorrectBounds(this.elementOriginalRect, this.reference.RenderSize, offsetMarginScale, transform);

            this.element.RenderTransform = transform;
        }

        /// <summary>
        /// Sets the absolute rotation angle. The rotation center will be the center of the transformed element.
        /// </summary>
        /// <param name="angle">The angle.</param>
        public void SetAbsoluteRotation(double angle)
        {
            var center = this.GetTransformedElementCenter();
            this.SetAbsoluteRotation(angle, center.X, center.Y);
        }

        /// <summary>
        /// Sets the absolute rotation angle.
        /// </summary>
        /// <param name="angle">The angle.</param>
        /// <param name="centerX">The center X.</param>
        /// <param name="centerY">The center Y.</param>
        public void SetAbsoluteRotation(double angle, double centerX, double centerY)
        {
            var matrix = this.TransformMatrix;
            var transform = new TransformGroup();
            transform.Children.Add(new MatrixTransform { Matrix = matrix });

            var currentAngle = -Math.Atan2(matrix.M21, matrix.M22) * 180 / Math.PI;
            var newAngle = angle - currentAngle;

            transform.Children.Add(new RotateTransform { Angle = newAngle, CenterX = centerX, CenterY = centerY });
            CorrectBounds(this.elementOriginalRect, this.reference.RenderSize, this.scaleOffsets, transform);

            this.element.RenderTransform = transform;
        }

        /// <summary>
        /// Sets the absolute translation.
        /// </summary>
        /// <param name="offset">The offset.</param>
        public void SetAbsoluteTranslation(Point offset)
        {
            var transform = new TransformGroup();

            transform.Children.Add(new MatrixTransform { Matrix = this.TransformMatrix });

            var originalCenter = new Point((this.elementOriginalRect.Right + this.elementOriginalRect.Left) / 2, (this.elementOriginalRect.Top + this.elementOriginalRect.Bottom) / 2);
            var transformedCenter = this.GetTransformedElementCenter();

            transform.Children.Add(new TranslateTransform { X = originalCenter.X - transformedCenter.X + offset.X, Y = originalCenter.Y - transformedCenter.Y + offset.Y });

            this.element.RenderTransform = transform;
        }

        /// <summary>
        /// Sets the absolute zoom factor of the element. The zoom center will be the center of the transformed element.
        /// </summary>
        /// <param name="zoom">The zoom factor.</param>
        public void SetAbsoluteZoom(double zoom)
        {
            var center = this.GetTransformedElementCenter();
            this.SetAbsoluteZoom(zoom, center.X, center.Y);
        }

        /// <summary>
        /// Sets the absolute zoom factor of the element.
        /// </summary>
        /// <param name="zoom">The zoom factor.</param>
        /// <param name="centerX">The X coordinate of the manipulation center.</param>
        /// <param name="centerY">The Y coordinate of the manipulation center.</param>
        public void SetAbsoluteZoom(double zoom, double centerX, double centerY)
        {
            var matrix = this.TransformMatrix;
            var transform = new TransformGroup();
            transform.Children.Add(new MatrixTransform { Matrix = matrix });

            zoom = this.CoerceZoom(zoom);

            var currentScale = Math.Sqrt(matrix.M11 * matrix.M11 + matrix.M12 * matrix.M12);
            var newScale = zoom / currentScale;

            transform.Children.Add(new ScaleTransform { ScaleX = newScale, ScaleY = newScale, CenterX = centerX, CenterY = centerY });
            CorrectBounds(this.elementOriginalRect, this.reference.RenderSize, this.scaleOffsets, transform);

            this.element.RenderTransform = transform;
        }

        /// <summary>
        /// Sets the delta rotation angle. The rotation center will be the center of the transformed element.
        /// </summary>
        /// <param name="angle">The angle.</param>
        public void SetDeltaRotation(double angle)
        {
            var center = this.GetTransformedElementCenter();
            this.SetDeltaRotation(angle, center.X, center.Y);
        }

        /// <summary>
        /// Sets the delta rotation angle.
        /// </summary>
        /// <param name="angle">The angle.</param>
        /// <param name="centerX">The center X.</param>
        /// <param name="centerY">The center Y.</param>
        public void SetDeltaRotation(double angle, double centerX, double centerY)
        {
            var transform = new TransformGroup();
            transform.Children.Add(new MatrixTransform { Matrix = this.TransformMatrix });
            transform.Children.Add(new RotateTransform { Angle = angle, CenterX = centerX, CenterY = centerY });
            CorrectBounds(this.elementOriginalRect, this.reference.RenderSize, this.scaleOffsets, transform);

            this.element.RenderTransform = transform;
        }

        /// <summary>
        /// Sets the delta translation.
        /// </summary>
        /// <param name="offset">The offset.</param>
        public void SetDeltaTranslation(Point offset)
        {
            var transform = new TransformGroup();
            transform.Children.Add(new MatrixTransform { Matrix = this.TransformMatrix });

            transform.Children.Add(new TranslateTransform { X = offset.X, Y = offset.Y });
            CorrectBounds(this.elementOriginalRect, this.reference.RenderSize, this.scaleOffsets, transform);

            this.element.RenderTransform = transform;
        }

        /// <summary>
        /// Sets the delta zoom that will be applied to the current state of the element. The zoom center will be the center of the transformed element.
        /// </summary>
        /// <param name="zoom">The zoom factor.</param>
        public void SetDeltaZoom(double zoom)
        {
            var center = this.GetTransformedElementCenter();
            this.SetDeltaZoom(zoom, center.X, center.Y);
        }

        /// <summary>
        /// Sets the delta zoom that will be applied to the current state of the element.
        /// </summary>
        /// <param name="zoom">The zoom.</param>
        /// <param name="centerX">The X coordinate of the manipulation center.</param>
        /// <param name="centerY">The Y coordinate of the manipulation center.</param>
        public void SetDeltaZoom(double zoom, double centerX, double centerY)
        {
            var matrix = this.TransformMatrix;
            var transform = new TransformGroup();
            transform.Children.Add(new MatrixTransform { Matrix = matrix });

            var currentScale = Math.Sqrt(matrix.M11 * matrix.M11 + matrix.M12 * matrix.M12);
            var newScale = zoom * currentScale;

            newScale = this.CoerceZoom(newScale) / currentScale;

            transform.Children.Add(new ScaleTransform { ScaleX = newScale, ScaleY = newScale, CenterX = centerX, CenterY = centerY });
            CorrectBounds(this.elementOriginalRect, this.reference.RenderSize, this.scaleOffsets, transform);

            this.element.RenderTransform = transform;
        }

        private static void AddRotation(double angle, Point center, TransformGroup transform)
        {
            var rotationTransform = new RotateTransform
            {
                CenterX = center.X,
                CenterY = center.Y,
                Angle = angle
            };

            transform.Children.Add(rotationTransform);
        }

        private static void AddScale(double scale, double minScale, double maxScale, Point center, TransformGroup transform)
        {
            var matrix = transform.Value;
            var currentScale = Math.Sqrt(matrix.M11 * matrix.M11 + matrix.M12 * matrix.M12);
            if (currentScale * scale > maxScale)
            {
                scale = maxScale / currentScale;
            }

            if (currentScale * scale < minScale)
            {
                scale = minScale / currentScale;
            }

            var scaleTransform = new ScaleTransform
            {
                CenterX = center.X,
                CenterY = center.Y,
                ScaleX = scale,
                ScaleY = scale
            };

            transform.Children.Add(scaleTransform);
        }

        private static void AddTranslation(Point translation, TransformGroup transform)
        {
            var translateTransform = new TranslateTransform
            {
                X = translation.X,
                Y = translation.Y
            };

            transform.Children.Add(translateTransform);
        }

        private static void CorrectBounds(Rect elementRect, Size availableSize, Thickness scaleOffsets, TransformGroup transform)
        {
            var bounds = transform.TransformBounds(elementRect);
            var extendedRect = GetExtendedAvaliableRect(scaleOffsets, new Size(bounds.Width, bounds.Height), availableSize);

            var widthRatio = bounds.Width / extendedRect.Width;
            var heightRatio = bounds.Height / extendedRect.Height;

            var correctionX = 0d;
            var correctionY = 0d;

            if (widthRatio > 1)
            {
                if (bounds.Left > extendedRect.Left)
                {
                    correctionX = extendedRect.Left - bounds.Left;
                }

                if (bounds.Right < extendedRect.Right)
                {
                    correctionX = extendedRect.Right - bounds.Right;
                }
            }
            else
            {
                if (bounds.Left < extendedRect.Left && bounds.Right < extendedRect.Right)
                {
                    correctionX = extendedRect.Left - bounds.Left;
                }

                if (bounds.Left > extendedRect.Left && bounds.Right > extendedRect.Right)
                {
                    correctionX = extendedRect.Right - bounds.Right;
                }
            }

            if (heightRatio > 1)
            {
                if (bounds.Top > extendedRect.Top)
                {
                    correctionY = extendedRect.Top - bounds.Top;
                }

                if (bounds.Bottom < extendedRect.Bottom)
                {
                    correctionY = extendedRect.Bottom - bounds.Bottom;
                }
            }
            else
            {
                if (bounds.Top > extendedRect.Top && bounds.Bottom > extendedRect.Bottom)
                {
                    correctionY = extendedRect.Bottom - bounds.Bottom;
                }

                if (bounds.Top < extendedRect.Top && bounds.Bottom < extendedRect.Bottom)
                {
                    correctionY = extendedRect.Top - bounds.Top;
                }
            }

            if (correctionX != 0 || correctionY != 0)
            {
                AddTranslation(new Point(correctionX, correctionY), transform);
            }
        }

        private static Rect GetExtendedAvaliableRect(Thickness offset, Size elementSize, Size availableSize)
        {
            return new Rect(
                -offset.Left * elementSize.Width,
                -offset.Top * elementSize.Height,
                availableSize.Width + elementSize.Width * (offset.Left + offset.Right),
                availableSize.Height + elementSize.Height * (offset.Top + offset.Bottom));
        }

        private double CoerceZoom(double zoom)
        {
            zoom = Math.Max(this.minZoom, zoom);
            zoom = Math.Min(this.maxZoom, zoom);
            return zoom;
        }

        private void OnDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if (this.ResetOnDoubleTap && !(this.element.RenderTransform as TransformGroup).Value.IsIdentity)
            {
                this.element.RenderTransform = new TransformGroup();
            }
        }

        private void OnManipulationUpdated(object sender, ManipulationUpdatedEventArgs e)
        {
            var transform = new TransformGroup();
            var previousTransform = new MatrixTransform { Matrix = (this.element.RenderTransform as TransformGroup).Value };

            transform.Children.Add(previousTransform);

            if (e.Delta.Rotation != 0)
            {
                AddRotation(e.Delta.Rotation, e.Position, transform);
            }

            if (e.Delta.Scale != 1)
            {
                AddScale(e.Delta.Scale, this.minZoom, this.maxZoom, e.Position, transform);
            }

            if (e.Delta.Translation.X != 0 || e.Delta.Translation.Y != 0)
            {
                AddTranslation(e.Delta.Translation, transform);
            }

            CorrectBounds(this.elementOriginalRect, this.reference.RenderSize, this.scaleOffsets, transform);

            this.element.RenderTransform = transform;
        }

        private void OnPointerCanceled(object sender, PointerRoutedEventArgs args)
        {
            this.gestureRecognizer.CompleteGesture();
            args.Handled = true;
        }

        private void OnPointerMoved(object sender, PointerRoutedEventArgs args)
        {
            try
            {
                // TODO
                this.gestureRecognizer.ProcessMoveEvents(args.GetIntermediatePoints(this.reference));
            }
            catch (Exception e)
            {
                if (e.Message != "Input data cannot be processed in the non-chronological order.\r\n\r\nInput data cannot be processed in the non-chronological order.\r\n")
                {
                    throw;
                }
            }
            finally
            {
                args.Handled = true;
            }
        }

        private void OnPointerPressed(object sender, PointerRoutedEventArgs args)
        {
            // Route the events to the gesture recognizer
            this.gestureRecognizer.ProcessDownEvent(args.GetCurrentPoint(this.reference));

            // Set the pointer capture to the element being interacted with
            this.reference.CapturePointer(args.Pointer);
            args.Handled = true;
        }

        private void OnPointerReleased(object sender, PointerRoutedEventArgs args)
        {
            this.gestureRecognizer.ProcessUpEvent(args.GetCurrentPoint(this.reference));
            args.Handled = true;
        }
    }
}