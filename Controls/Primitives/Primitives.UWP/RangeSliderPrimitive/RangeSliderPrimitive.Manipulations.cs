using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.UI.Automation.Peers;
using Telerik.UI.Xaml.Controls.Primitives.RangeSlider;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    public partial class RangeSliderPrimitive : SliderBase
    {
        internal double endThumbAccumulatedDelta = 0.0d;
        internal double startThumbAccumulatedDelta = 0.0d;

        private DispatcherTimer thumbsTimer;
        private Point trackBarPointerPressedPosition;
        private bool trackBarPressed = false;

        private double startThumbInitialDelta = 0.0d;

        private double endThumbInitialDelta = 0.0d;

        private double middleThumbInitialDelta = 0.0d;
        private double middleThumbAccumulatedDelta = 0.0d;

        private bool isMiddleThumbEnteringFromLeft = true;
        private bool isMiddleThumbEnteringFromRight = true;

        internal void StopTimer()
        {
            this.thumbsTimer.Stop();
        }

        internal void HandleSelectionStartThumbDragDelta(double offsetchange)
        {
            var delta = this.GetRelativeOffset(offsetchange);
            this.startThumbAccumulatedDelta += delta;

            if (this.startThumbInitialDelta + this.startThumbAccumulatedDelta <= this.Minimum)
            {
                this.UpdateSelectionStateValue(RangeSliderPrimitive.SelectionStartProperty, this.Minimum, (s) => this.VisualSelection = new SelectionRange(s, this.visualSelection.End));
            }
            else if (this.startThumbInitialDelta + this.startThumbAccumulatedDelta >= this.SelectionEnd)
            {
                this.UpdateSelectionStateValue(RangeSliderPrimitive.SelectionStartProperty, this.SelectionEnd, (s) => this.VisualSelection = new SelectionRange(s, this.visualSelection.End));
            }
            else
            {
                this.UpdateSelectionStart(delta);
            }

            this.InvalidateThumbsPanelArrange();
        }

        internal void HandleSelectionEndThumbDragDelta(double offsetchange)
        {
            var delta = this.GetRelativeOffset(offsetchange);
            this.endThumbAccumulatedDelta += delta;

            if (this.endThumbInitialDelta + this.endThumbAccumulatedDelta >= this.Maximum)
            {
                this.UpdateSelectionStateValue(RangeSliderPrimitive.SelectionEndProperty, this.Maximum, (s) => this.VisualSelection = new SelectionRange(this.VisualSelection.Start, s));
            }
            else if (this.endThumbInitialDelta + this.endThumbAccumulatedDelta <= this.SelectionStart)
            {
                this.UpdateSelectionStateValue(RangeSliderPrimitive.SelectionEndProperty, this.SelectionStart, (s) => this.VisualSelection = new SelectionRange(this.VisualSelection.Start, s));
            }
            else
            {
                this.UpdateSelectionEnd(delta);
            }

            this.InvalidateThumbsPanelArrange();
        }

        internal void HandleSelectionMiddleThumbDragDelta(double offsetchange)
        {
            var delta = this.GetRelativeOffset(offsetchange);
            var newDelta = delta;

            SelectionRange selectionRange = this.VisualSelection;

            if (!this.TryUpdateMiddleThumb(delta))
            {
                return;
            }

            if (selectionRange.End + delta > this.Maximum || selectionRange.Start + delta < this.Minimum)
            {
                newDelta = 0d;
                if (selectionRange.End + delta > this.Maximum)
                {
                    newDelta = this.Maximum - selectionRange.End;
                }

                if (selectionRange.Start + delta < this.Minimum)
                {
                    newDelta = this.Minimum - selectionRange.Start;
                }
            }

            this.UpdateSelectionRange(newDelta);
        }

        internal void HandleTrackBarPointerPressed(Point pointerPosition)
        {
            this.trackBarPointerPressedPosition = pointerPosition;
            this.trackBarPressed = true;

            double pointerPositionValue = this.Orientation == Windows.UI.Xaml.Controls.Orientation.Horizontal ? pointerPosition.X : pointerPosition.Y;

            if (this.Orientation == Windows.UI.Xaml.Controls.Orientation.Horizontal)
            {
                this.HandleHorizontalTrackBarPressed(pointerPositionValue);
            }
            else
            {
                this.HandleVerticalTrackBarPressed(pointerPositionValue);
            }

            this.InvalidateThumbsPanelArrange();
            this.StartTimer();
        }

        /// <summary>
        /// Called before the KeyDown event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            e.Handled = this.HandleKeyDown(e.Key);
            base.OnKeyDown(e);
        }

        private void HandleHorizontalTrackBarPressed(double pointerPositionValue)
        {
            double delta = 0d;

            SelectionRange selectionRange = this.VisualSelection;
            double selectionStartPosition = ((selectionRange.Start - this.Minimum) * this.coeficient) - this.selectionStartOffset;
            double selectionEndPosition = ((selectionRange.End - this.Minimum) * this.coeficient) + this.selectionEndOffset;

            if (pointerPositionValue < selectionEndPosition && pointerPositionValue > selectionStartPosition)
            {
                return;
            }

            if (pointerPositionValue < selectionStartPosition)
            {
                switch (this.TrackTapMode)
                {
                    case RangeSliderTrackTapMode.None:
                        return;
                    case RangeSliderTrackTapMode.MoveToTapPosition:

                        var desiredValue = pointerPositionValue / this.coeficient;
                        delta = (this.Minimum + desiredValue) - this.SelectionStart;
                        break;
                    case RangeSliderTrackTapMode.IncrementByLargeChange:
                        delta = -this.LargeChange;
                        break;
                    case RangeSliderTrackTapMode.IncrementBySmallChange:
                        delta = -this.SmallChange;
                        break;
                    default:
                        delta = -this.LargeChange;
                        break;
                }

                this.UpdateSelectionStart(delta);
                this.isLargeIncreaseButtonPressed = true;
            }
            else
            {
                switch (this.TrackTapMode)
                {
                    case RangeSliderTrackTapMode.None:
                        return;
                    case RangeSliderTrackTapMode.MoveToTapPosition:

                        var desiredValue = pointerPositionValue / this.coeficient;
                        delta = (this.Minimum + desiredValue) - this.SelectionEnd;
                        break;
                    case RangeSliderTrackTapMode.IncrementByLargeChange:
                        delta = this.LargeChange;
                        break;
                    case RangeSliderTrackTapMode.IncrementBySmallChange:
                        delta = this.SmallChange;
                        break;
                    default:
                        delta = this.LargeChange;
                        break;
                }

                this.UpdateSelectionEnd(delta);
                this.isLargeIncreaseButtonPressed = false;
            }
        }

        private void HandleVerticalTrackBarPressed(double pointerPositionValue)
        {
            double delta = 0d;

            var desiredValue = this.Maximum - pointerPositionValue / this.coeficient;

            if (desiredValue < this.SelectionEnd && desiredValue > this.SelectionStart)
            {
                return;
            }

            if (desiredValue > this.SelectionStart)
            {
                switch (this.TrackTapMode)
                {
                    case RangeSliderTrackTapMode.None:
                        return;
                    case RangeSliderTrackTapMode.MoveToTapPosition:
                        delta = desiredValue - this.SelectionEnd;
                        break;
                    case RangeSliderTrackTapMode.IncrementByLargeChange:
                        delta = this.LargeChange;
                        break;
                    case RangeSliderTrackTapMode.IncrementBySmallChange:
                        delta = this.SmallChange;
                        break;
                    default:
                        delta = this.LargeChange;
                        break;
                }

                this.UpdateSelectionEnd(delta);
                this.isLargeIncreaseButtonPressed = true;
            }
            else
            {
                switch (this.TrackTapMode)
                {
                    case RangeSliderTrackTapMode.None:
                        return;
                    case RangeSliderTrackTapMode.MoveToTapPosition:
                        desiredValue = this.Maximum - pointerPositionValue / this.coeficient;
                        delta = -(this.SelectionStart - desiredValue);
                        break;
                    case RangeSliderTrackTapMode.IncrementByLargeChange:
                        delta = -this.LargeChange;
                        break;
                    case RangeSliderTrackTapMode.IncrementBySmallChange:
                        delta = -this.SmallChange;
                        break;
                    default:
                        delta = -this.LargeChange;
                        break;
                }

                this.UpdateSelectionStart(delta);
                this.isLargeIncreaseButtonPressed = false;
            }
        }

        private bool HandleKeyDown(VirtualKey key)
        {
            var delta = this.GetDeltaByKey(key);
            var handled = true;

            if (this.selectionMiddleThumb != null && this.selectionMiddleThumb.FocusState != FocusState.Unfocused)
            {
                this.UpdateSelectionRange(delta);
            }
            else if (this.selectionStartThumb != null && this.selectionStartThumb.FocusState != FocusState.Unfocused)
            {
                this.UpdateSelectionStart(delta);
            }
            else if (this.selectionEndThumb != null && this.selectionEndThumb.FocusState != FocusState.Unfocused)
            {
                this.UpdateSelectionEnd(delta);
            }
            else
            {
                handled = false;
            }

            return handled && delta != 0;
        }

        private double GetDeltaByKey(VirtualKey key)
        {
            double delta = .0;
            if (key == VirtualKey.Left || key == VirtualKey.Down)
            {
                delta = -this.SmallChange;
            }
            else if (key == VirtualKey.Right || key == VirtualKey.Up)
            {
                delta = this.SmallChange;
            }
            else if (key == VirtualKey.Home)
            {
                delta = this.Minimum - this.Maximum;
            }
            else if (key == VirtualKey.End)
            {
                delta = this.Maximum;
            }
            else if (key == VirtualKey.PageUp)
            {
                delta = this.LargeChange;
            }
            else if (key == VirtualKey.PageDown)
            {
                delta = -this.LargeChange;
            }

            return this.GetValueBasedOnSnapping(delta);
        }

        private double GetValueBasedOnSnapping(double value)
        {
            if (this.SnapsTo == SnapsTo.Ticks)
            {
                if (value > 0)
                {
                    value = Math.Max(this.TickFrequency, value);
                }
                else if (value < 0)
                {
                    value = Math.Min(-this.TickFrequency, value);
                }
            }
            return value;
        }

        private void AttachThumbsEvents()
        {
            this.selectionStartThumb.SizeChanged += this.OnThumbSizeChanged;
            this.selectionStartThumb.PointerEntered += this.OnThumbPointerEntered;
            this.selectionStartThumb.DragDelta += this.OnSelectionStartThumbDragDelta;
            this.selectionStartThumb.DragCompleted += this.OnSelectionStartThumbDragCompleted;
            this.selectionStartThumb.DragStarted += this.OnSelectionStartThumbDragStarted;
            this.selectionStartThumb.AddHandler(Thumb.PointerPressedEvent, new PointerEventHandler(this.OnThumbPointerPressed), true);

            this.selectionEndThumb.SizeChanged += this.OnThumbSizeChanged;
            this.selectionEndThumb.PointerEntered += this.OnThumbPointerEntered;
            this.selectionEndThumb.DragDelta += this.OnSelectionEndThumbDragDelta;
            this.selectionEndThumb.DragCompleted += this.OnSelectionEndThumbDragCompleted;
            this.selectionEndThumb.DragStarted += this.OnSelectionEndThumbDragStarted;
            this.selectionEndThumb.AddHandler(Thumb.PointerPressedEvent, new PointerEventHandler(this.OnThumbPointerPressed), true);

            this.selectionMiddleThumb.PointerEntered += this.OnThumbPointerEntered;
            this.selectionMiddleThumb.DragDelta += this.OnSelectionMiddleThumbDragDelta;
            this.selectionMiddleThumb.DragCompleted += this.OnSelectionMiddleThumbDragCompleted;
            this.selectionMiddleThumb.DragStarted += this.OnSelectionMiddleThumbDragStarted;
            this.selectionMiddleThumb.AddHandler(Thumb.PointerPressedEvent, new PointerEventHandler(this.OnThumbPointerPressed), true);

            this.trackBar.PointerPressed += this.OnTrackBarPointerPressed;
            this.trackBar.PointerReleased += this.OnTrackBarPointerReleased;
            this.trackBar.PointerExited += this.OnTrackBarPointerExited;
            this.trackBar.PointerMoved += this.OnTrackBarPointerMoved;

            this.PointerEntered += this.OnRangeSliderPrimitivePointerEntered;
        }

        private void OnRangeSliderPrimitivePointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (this.valueToolTip.IsEnabled)
            {
                Point pointerPosition = e.GetCurrentPoint(this).Position;
                double selectionOffests = this.SelectionStartOffset + this.selectionEndOffset;

                if (this.Minimum == this.Maximum)
                {
                    this.valueToolTip.Content = this.ReturnFormatedValue(this.Minimum);
                    return;
                }

                if (this.Orientation == Windows.UI.Xaml.Controls.Orientation.Horizontal)
                {
                    double desiredValue = this.Minimum + Math.Abs(pointerPosition.X - selectionOffests / 2) / this.coeficient;
                    this.valueToolTip.Content = this.ReturnFormatedValue(desiredValue);
                }
                else
                {
                    double desiredValue = this.Maximum - (pointerPosition.Y - selectionOffests / 2) / this.coeficient;
                    this.valueToolTip.Content = this.ReturnFormatedValue(desiredValue);
                }
            }
        }

        private void DetachThumbsEvents()
        {
            this.selectionStartThumb.SizeChanged -= this.OnThumbSizeChanged;
            this.selectionStartThumb.PointerEntered -= this.OnThumbPointerEntered;
            this.selectionStartThumb.DragDelta -= this.OnSelectionStartThumbDragDelta;
            this.selectionStartThumb.DragCompleted -= this.OnSelectionStartThumbDragCompleted;
            this.selectionStartThumb.DragStarted -= this.OnSelectionStartThumbDragStarted;
            this.selectionStartThumb.RemoveHandler(Thumb.PointerPressedEvent, new PointerEventHandler(this.OnThumbPointerPressed));

            this.selectionEndThumb.SizeChanged -= this.OnThumbSizeChanged;
            this.selectionEndThumb.PointerEntered -= this.OnThumbPointerEntered;
            this.selectionEndThumb.DragDelta -= this.OnSelectionEndThumbDragDelta;
            this.selectionEndThumb.DragCompleted -= this.OnSelectionEndThumbDragCompleted;
            this.selectionEndThumb.DragStarted -= this.OnSelectionEndThumbDragStarted;
            this.selectionEndThumb.RemoveHandler(Thumb.PointerPressedEvent, new PointerEventHandler(this.OnThumbPointerPressed));

            this.selectionMiddleThumb.PointerEntered -= this.OnThumbPointerEntered;
            this.selectionMiddleThumb.DragDelta -= this.OnSelectionMiddleThumbDragDelta;
            this.selectionMiddleThumb.DragCompleted -= this.OnSelectionMiddleThumbDragCompleted;
            this.selectionMiddleThumb.DragStarted -= this.OnSelectionMiddleThumbDragStarted;
            this.selectionMiddleThumb.RemoveHandler(Thumb.PointerPressedEvent, new PointerEventHandler(this.OnThumbPointerPressed));

            this.trackBar.PointerPressed -= this.OnTrackBarPointerPressed;
            this.trackBar.PointerReleased -= this.OnTrackBarPointerReleased;
            this.trackBar.PointerExited -= this.OnTrackBarPointerExited;
            this.trackBar.PointerMoved -= this.OnTrackBarPointerMoved;

            this.PointerEntered -= this.OnRangeSliderPrimitivePointerEntered;
        }

        private void StartTimer()
        {
            this.thumbsTimer.Start();
        }

        private void OnTimerTick(object sender, object e)
        {
            if (this.isLargeIncreaseButtonPressed)
            {
                if (!this.rangeToolTip.IsOpen)
                {
                    this.rangeToolTipContent.ShowToolTip();
                }

                if (this.Orientation == Windows.UI.Xaml.Controls.Orientation.Horizontal)
                {
                    var selectionStartTransform = this.selectionStartThumb.TransformToVisual(this);
                    Point selectionStartLocation = selectionStartTransform.TransformPoint(new Point(0, 0));

                    if (this.trackBarPointerPressedPosition.X < selectionStartLocation.X)
                    {
                        switch (this.TrackTapMode)
                        {
                            case RangeSliderTrackTapMode.IncrementByLargeChange:
                                this.UpdateSelectionStart(-this.LargeChange, true);
                                this.rangeToolTipContent.UpdateToolTipContext();
                                break;
                            case RangeSliderTrackTapMode.IncrementBySmallChange:
                                this.UpdateSelectionStart(-this.SmallChange, true);
                                this.rangeToolTipContent.UpdateToolTipContext();
                                break;
                            default:
                                break;
                        }
                    }
                }
                else
                {
                    var selectionEndTransform = this.selectionEndThumb.TransformToVisual(this);
                    Point selectionEndLocation = selectionEndTransform.TransformPoint(new Point(0, 0));

                    if (this.trackBarPointerPressedPosition.Y < selectionEndLocation.Y)
                    {
                        switch (this.TrackTapMode)
                        {
                            case RangeSliderTrackTapMode.IncrementByLargeChange:
                                this.UpdateSelectionEnd(this.LargeChange, true);
                                this.rangeToolTipContent.UpdateToolTipContext();
                                break;
                            case RangeSliderTrackTapMode.IncrementBySmallChange:
                                this.UpdateSelectionEnd(this.SmallChange, true);
                                this.rangeToolTipContent.UpdateToolTipContext();
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            else
            {
                if (!this.rangeToolTip.IsOpen)
                {
                    this.rangeToolTipContent.ShowToolTip();
                }

                if (this.Orientation == Windows.UI.Xaml.Controls.Orientation.Horizontal)
                {
                    var selectionEndTransform = this.selectionEndThumb.TransformToVisual(this);
                    Point selectionEndLocation = selectionEndTransform.TransformPoint(new Point(0, 0));

                    if (this.trackBarPointerPressedPosition.X > selectionEndLocation.X)
                    {
                        switch (this.TrackTapMode)
                        {
                            case RangeSliderTrackTapMode.IncrementByLargeChange:
                                this.UpdateSelectionEnd(this.LargeChange, true);
                                this.rangeToolTipContent.UpdateToolTipContext();
                                break;
                            case RangeSliderTrackTapMode.IncrementBySmallChange:
                                this.UpdateSelectionEnd(this.SmallChange, true);
                                this.rangeToolTipContent.UpdateToolTipContext();
                                break;
                            default:
                                break;
                        }
                    }
                }
                else
                {
                    var selectionStartTransform = this.selectionStartThumb.TransformToVisual(this);
                    Point selectionStartLocation = selectionStartTransform.TransformPoint(new Point(0, 0));

                    if (this.trackBarPointerPressedPosition.Y > selectionStartLocation.Y)
                    {
                        switch (this.TrackTapMode)
                        {
                            case RangeSliderTrackTapMode.IncrementByLargeChange:
                                this.UpdateSelectionStart(-this.LargeChange, true);
                                this.rangeToolTipContent.UpdateToolTipContext();
                                break;
                            case RangeSliderTrackTapMode.IncrementBySmallChange:
                                this.UpdateSelectionStart(-this.SmallChange, true);
                                this.rangeToolTipContent.UpdateToolTipContext();
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            this.InvalidateThumbsPanelArrange();
        }
        
        private void OnSliderPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            this.UpdateSelectionStart(0, true);
            this.UpdateSelectionEnd(0, true);

            this.InvalidateThumbsPanelArrange();
            this.StopTimer();
        }

        private void OnThumbSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.UpdateThumbsOffset();
            this.InvalidateThumbsPanelMeasure();
        }

        private void OnThumbPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            this.StopTimer();
        }

        private void OnThumbPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            Thumb thumb = sender as Thumb;
            if (thumb != null && thumb.IsTabStop)
            {
                if (thumb.FocusState == FocusState.Unfocused)
                {
                    thumb.Focus(FocusState.Programmatic);
                }
            }
        }

        private void OnTrackBarPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (this.trackBarPressed)
            {
                this.trackBarPointerPressedPosition = e.GetCurrentPoint(this.trackBar).Position;
            }
        }

        private void OnTrackBarPointerPressed(object sender, PointerRoutedEventArgs args)
        {
            Point pointerPosition = args.GetCurrentPoint(this.trackBar).Position;
            this.HandleTrackBarPointerPressed(pointerPosition);

            this.rangeToolTipContent.UpdateToolTipContext();
            this.rangeToolTipContent.ShowToolTip();
        }

        private void OnTrackBarPointerExited(object sender, PointerRoutedEventArgs e)
        {
            this.StopTimer();
            this.rangeToolTipContent.HideToolTip(true);
        }

        private void OnTrackBarPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            this.StopTimer();
            this.trackBarPressed = false;
            this.rangeToolTipContent.HideToolTip(true);
        }

        private void OnSelectionMiddleThumbDragStarted(object sender, DragStartedEventArgs e)
        {
            this.middleThumbInitialDelta = this.SelectionStart;
            this.isMiddleThumbEnteringFromLeft = true;
            this.isMiddleThumbEnteringFromRight = true;
        }

        private void OnSelectionMiddleThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            double offsetchange = this.Orientation == Windows.UI.Xaml.Controls.Orientation.Horizontal ? e.HorizontalChange : e.VerticalChange * -1;

            this.HandleSelectionMiddleThumbDragDelta(offsetchange);
            this.rangeToolTipContent.UpdateToolTipContext();
            this.rangeToolTipContent.ShowToolTip();
        }

        private void OnSelectionMiddleThumbDragCompleted(object sender, DragCompletedEventArgs e)
        {
            this.middleThumbInitialDelta = 0;
            this.middleThumbAccumulatedDelta = 0;

            this.UpdateSelectionEnd(0, true);
            this.UpdateSelectionStart(0, true);

            this.rangeToolTipContent.HideToolTip();
            this.InvalidateThumbsPanelArrange();
        }

        private void OnSelectionStartThumbDragStarted(object sender, DragStartedEventArgs e)
        {
            this.startThumbInitialDelta = this.SelectionStart;
        }

        private void OnSelectionStartThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            var offsetchange = this.Orientation == Windows.UI.Xaml.Controls.Orientation.Horizontal ? e.HorizontalChange : e.VerticalChange * -1;
            this.HandleSelectionStartThumbDragDelta(offsetchange);
            this.rangeToolTipContent.UpdateToolTipContext();
            this.rangeToolTipContent.ShowToolTip();
        }

        private void OnSelectionStartThumbDragCompleted(object sender, DragCompletedEventArgs e)
        {
            this.UpdateSelectionStart(0, true);

            this.rangeToolTipContent.HideToolTip();
            this.InvalidateThumbsPanelArrange();
            this.startThumbAccumulatedDelta = 0;
        }

        private void OnSelectionEndThumbDragStarted(object sender, DragStartedEventArgs e)
        {
            this.endThumbAccumulatedDelta = this.SelectionEnd;
        }

        private void OnSelectionEndThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            var offsetchange = this.Orientation == Windows.UI.Xaml.Controls.Orientation.Horizontal ? e.HorizontalChange : e.VerticalChange * -1;
            this.HandleSelectionEndThumbDragDelta(offsetchange);
            this.rangeToolTipContent.UpdateToolTipContext();
            this.rangeToolTipContent.ShowToolTip();
        }

        private void OnSelectionEndThumbDragCompleted(object sender, DragCompletedEventArgs e)
        {
            this.UpdateSelectionEnd(0, true);

            this.rangeToolTipContent.HideToolTip();
            this.InvalidateThumbsPanelArrange();
            this.endThumbAccumulatedDelta = 0;
        }

        private bool TryUpdateMiddleThumb(double delta)
        {
            SelectionRange selectionRange = this.VisualSelection;

            if (!this.isMiddleThumbEnteringFromLeft)
            {
                this.middleThumbAccumulatedDelta += delta;

                if (selectionRange.Start + this.middleThumbAccumulatedDelta >= this.Minimum)
                {
                    this.UpdateThumbToTrimmedDelta(selectionRange.Start, this.Minimum);
                    this.isMiddleThumbEnteringFromLeft = true;
                }

                return false;
            }

            if (!this.isMiddleThumbEnteringFromRight)
            {
                this.middleThumbAccumulatedDelta += delta;

                if (selectionRange.End + this.middleThumbAccumulatedDelta <= this.Maximum)
                {
                    this.UpdateThumbToTrimmedDelta(selectionRange.End, this.Maximum);
                    this.isMiddleThumbEnteringFromRight = true;
                }

                return false;
            }

            if (this.isMiddleThumbEnteringFromLeft && selectionRange.Start + delta <= this.Minimum)
            {
                this.UpdateMiddleThumbPosition(selectionRange.Start, this.Minimum, delta);
                this.isMiddleThumbEnteringFromLeft = false;

                return false;
            }

            if (this.isMiddleThumbEnteringFromRight && selectionRange.End + delta >= this.Maximum)
            {
                this.UpdateMiddleThumbPosition(selectionRange.End, this.Maximum, delta);
                this.isMiddleThumbEnteringFromRight = false;

                return false;
            }

            this.middleThumbAccumulatedDelta = 0;

            return true;
        }

        private void UpdateThumbToTrimmedDelta(double rangePosition, double rangeBoundary)
        {
            double delta = rangePosition + this.middleThumbAccumulatedDelta - rangeBoundary;
            this.UpdateSelectionRange(delta);
            this.middleThumbAccumulatedDelta = 0;
        }

        private void UpdateMiddleThumbPosition(double rangePosition, double rangeBoundary, double delta)
        {
            delta = rangePosition + delta - rangeBoundary;

            this.middleThumbAccumulatedDelta += delta;
            var newDelta = rangeBoundary - rangePosition;
            this.UpdateSelectionRange(newDelta);
        }

        private void UpdateSelectionRange(double delta)
        {
            var multiplier = delta > 0 ? 1 : -1;
            var selection = this.VisualSelection;
            var maxDelta = delta > 0 ? this.Maximum - selection.End : selection.Start - this.Minimum;

            if (maxDelta <= delta * multiplier)
            {
                delta = maxDelta * multiplier;
            }

            if (delta > 0)
            {
                this.UpdateSelectionEnd(delta);
                this.UpdateSelectionStart(delta);
            }
            else
            {
                this.UpdateSelectionStart(delta);
                this.UpdateSelectionEnd(delta);
            }

            this.InvalidateThumbsPanelArrange();
        }
    }
}
