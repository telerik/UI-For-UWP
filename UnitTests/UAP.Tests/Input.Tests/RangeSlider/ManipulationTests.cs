using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.Foundation;
using WinRT.Testing;

namespace Telerik.UI.Xaml.Controls.Input.Tests.RangeSlider
{
    [TestClass]
    [Tag("Input")]
    [Tag("RangeSlider")]
    public class ManipulationTests : RadControlUITest
    {
        private RadRangeSlider slider;

        public override void TestInitialize()
        {
            base.TestInitialize();

            this.slider = new RadRangeSlider();
        }

        [TestMethod]
        public void RangeSliderTest_AssertSelectionEnd_SnappedAfterTrackPressed()
        {
            this.slider.SelectionStart = 5;
            this.slider.SelectionEnd = 5;
            this.slider.LargeChange = 2;
            Point clickPoint = new Point();
            double middleThumbSize = 0;

            this.CreateAsyncTest(this.slider, () =>
            {
                middleThumbSize = this.slider.RangeSlider.SelectionMiddleThumb.ActualWidth;
                var trackWidth = this.slider.RangeSlider.ActualWidth;
                clickPoint = new Point(trackWidth, 0);

                this.slider.RangeSlider.HandleTrackBarPointerPressed(clickPoint);
            }, () =>
            {
                this.AssertMiddleThumbSizeChanged(middleThumbSize, true);
                middleThumbSize = this.slider.RangeSlider.SelectionMiddleThumb.ActualWidth;
                Assert.AreEqual(7, this.slider.SelectionEnd);
                this.slider.LargeChange = 1;
                this.slider.RangeSlider.StopTimer();
            }, () =>
            {
                this.slider.RangeSlider.HandleTrackBarPointerPressed(clickPoint);
            }, () =>
            {
                this.AssertMiddleThumbSizeChanged(middleThumbSize, true);
                Assert.AreEqual(8, this.slider.SelectionEnd);
            });
        }

        [TestMethod]
        public void RangeSliderTest_AssertSelectionStart_SnappedAfterTrackPressed()
        {
            this.slider.SelectionStart = 5;
            this.slider.SelectionEnd = 5;
            this.slider.LargeChange = 2;
            Point clickPoint = new Point();
            double middleThumbSize = 0;

            this.CreateAsyncTest(this.slider, () =>
            {
                middleThumbSize = this.slider.RangeSlider.SelectionMiddleThumb.ActualWidth;
                clickPoint = new Point(0, 0);
                this.slider.RangeSlider.HandleTrackBarPointerPressed(clickPoint);
            }, () =>
            {
                this.AssertMiddleThumbSizeChanged(middleThumbSize, true);
                middleThumbSize = this.slider.RangeSlider.SelectionMiddleThumb.ActualWidth;
                Assert.AreEqual(3, this.slider.SelectionStart);
                this.slider.LargeChange = 1;
                this.slider.RangeSlider.StopTimer();
            }, () =>
            {
                this.slider.RangeSlider.HandleTrackBarPointerPressed(clickPoint);
            }, () =>
            {
                this.AssertMiddleThumbSizeChanged(middleThumbSize, true);
                Assert.AreEqual(2, this.slider.SelectionStart);
            });
        }

        [TestMethod]
        public void RangeSliderTest_TrackTapMode_None_AssertSelectionEnd_SnappedAfterTrackPressed()
        {
            this.slider.TrackTapMode = RangeSliderTrackTapMode.None;
            this.slider.SelectionStart = 5;
            this.slider.SelectionEnd = 5;
            this.slider.LargeChange = 2;
            Point clickPoint = new Point();
            this.slider.Maximum = 10;

            this.CreateAsyncTest(this.slider, () =>
            {
                var trackWidth = this.slider.RangeSlider.ActualWidth;
                clickPoint = new Point(trackWidth, 0);

                this.slider.RangeSlider.HandleTrackBarPointerPressed(clickPoint);
            }, () =>
            {
                Assert.AreEqual(5, this.slider.SelectionEnd);
            });
        }

        [TestMethod]
        public void RangeSliderTest_TrackTapMode_LargeChange_AssertSelectionEnd_SnappedAfterTrackPressed()
        {
            this.slider.TrackTapMode = RangeSliderTrackTapMode.IncrementByLargeChange;
            this.slider.SelectionStart = 5;
            this.slider.SelectionEnd = 5;
            this.slider.LargeChange = 2;
            Point clickPoint = new Point();
            this.slider.Maximum = 10;

            this.CreateAsyncTest(this.slider, () =>
            {
                var trackWidth = this.slider.RangeSlider.ActualWidth;
                clickPoint = new Point(trackWidth, 0);

                this.slider.RangeSlider.HandleTrackBarPointerPressed(clickPoint);
            }, () =>
            {
                Assert.AreEqual(7, this.slider.SelectionEnd);
            });
        }

        [TestMethod]
        public void RangeSliderTest_TrackTapMode_SmallChange_AssertSelectionEnd_SnappedAfterTrackPressed()
        {
            this.slider.TrackTapMode = RangeSliderTrackTapMode.IncrementBySmallChange;
            this.slider.SelectionStart = 5;
            this.slider.SelectionEnd = 5;
            this.slider.LargeChange = 2;
            this.slider.SmallChange = 4;
            Point clickPoint = new Point();
            this.slider.Maximum = 10;

            this.CreateAsyncTest(this.slider, () =>
            {
                var trackWidth = this.slider.RangeSlider.ActualWidth;
                clickPoint = new Point(trackWidth, 0);

                this.slider.RangeSlider.HandleTrackBarPointerPressed(clickPoint);
            }, () =>
            {
                Assert.AreEqual(9, this.slider.SelectionEnd);
            });
        }

        [TestMethod]
        public void RangeSliderTest_TrackTapMode_TapPosition_AssertSelectionEnd_SnappedAfterTrackPressed()
        {
            this.slider.TrackTapMode= RangeSliderTrackTapMode.MoveToTapPosition;
            this.slider.SelectionStart = 5;
            this.slider.SelectionEnd = 5;
            this.slider.LargeChange = 2;
            Point clickPoint = new Point();
            this.slider.Maximum = 10;

            this.CreateAsyncTest(this.slider, () =>
            {
                var trackWidth = this.slider.RangeSlider.ActualWidth;
                clickPoint = new Point(trackWidth, 0);

                this.slider.RangeSlider.HandleTrackBarPointerPressed(clickPoint);
            }, () =>
            {
                Assert.AreEqual(this.slider.Maximum, this.slider.SelectionEnd);
                Assert.AreEqual(10, this.slider.Maximum);
            });
        }

        [TestMethod]
        public void RangeSliderTest_TrackTapMode_TapPosition_AssertSelectionStart_SnappedAfterTrackPressed()
        {
            this.slider.TrackTapMode = RangeSliderTrackTapMode.MoveToTapPosition;
            this.slider.SelectionStart = 5;
            this.slider.SelectionEnd = 5;
            Point clickPoint = new Point();

            this.CreateAsyncTest(this.slider, () =>
            {
                clickPoint = new Point(-100, 0);
                this.slider.RangeSlider.HandleTrackBarPointerPressed(clickPoint);
            }, () =>
            {
                Assert.AreEqual(this.slider.Minimum, this.slider.SelectionStart);
                Assert.AreEqual(0, this.slider.Minimum);
            });
        }

        [TestMethod]
        public void RangeSliderTest_TrackTapMode_None_AssertSelectionStart_SnappedAfterTrackPressed()
        {
            this.slider.TrackTapMode = RangeSliderTrackTapMode.None;
            this.slider.SelectionStart = 5;
            this.slider.SelectionEnd = 5;
            Point clickPoint = new Point();

            this.CreateAsyncTest(this.slider, () =>
            {
                clickPoint = new Point(-100, 0);
                this.slider.RangeSlider.HandleTrackBarPointerPressed(clickPoint);
            }, () =>
            {
                Assert.AreEqual(5, this.slider.SelectionStart);
            });
        }

        [TestMethod]
        public void RangeSliderTest_TrackTapMode_SmallChange_AssertSelectionStart_SnappedAfterTrackPressed()
        {
            this.slider.TrackTapMode = RangeSliderTrackTapMode.IncrementBySmallChange;
            this.slider.SelectionStart = 5;
            this.slider.SelectionEnd = 5;
            this.slider.SmallChange = 3;
            Point clickPoint = new Point();

            this.CreateAsyncTest(this.slider, () =>
            {
                clickPoint = new Point(-100, 0);
                this.slider.RangeSlider.HandleTrackBarPointerPressed(clickPoint);
            }, () =>
            {
                Assert.AreEqual(2, this.slider.SelectionStart);
            });
        }

        [TestMethod]
        public void RangeSliderTest_TrackTapMode_LargeChange_AssertSelectionStart_SnappedAfterTrackPressed()
        {
            this.slider.TrackTapMode = RangeSliderTrackTapMode.IncrementByLargeChange;
            this.slider.SelectionStart = 5;
            this.slider.SelectionEnd = 5;
            this.slider.LargeChange = 4;
            Point clickPoint = new Point();

            this.CreateAsyncTest(this.slider, () =>
            {
                clickPoint = new Point(-100, 0);
                this.slider.RangeSlider.HandleTrackBarPointerPressed(clickPoint);
            }, () =>
            {
                Assert.AreEqual(1, this.slider.SelectionStart);
            });
        }

        [TestMethod]
        public void RangeSliderTest_DragEndThumb_AssertSelectionEnd()
        {
            this.slider.SelectionStart = 0;
            this.slider.SelectionEnd = 0;
            double trackWidth = 0;
            double middleThumbSize = 0;
            this.slider.Maximum = 10;

            this.CreateAsyncTest(this.slider, () =>
            {
                middleThumbSize = this.slider.RangeSlider.SelectionMiddleThumb.ActualWidth;
                trackWidth = this.slider.RangeSlider.ActualWidth - this.slider.RangeSlider.SelectionEndOffset - this.slider.RangeSlider.SelectionStartOffset;

                // Drag the SelectionEndThumb to the half of the range
                this.slider.RangeSlider.endThumbAccumulatedDelta = this.slider.RangeSlider.SelectionEnd;
                this.slider.RangeSlider.HandleSelectionEndThumbDragDelta(trackWidth / 2);
            }, () =>
            {
                this.AssertMiddleThumbSizeChanged(middleThumbSize, true);
                middleThumbSize = this.slider.RangeSlider.SelectionMiddleThumb.ActualWidth;
                Assert.AreEqual(this.slider.Maximum / 2, this.slider.SelectionEnd);
                Assert.AreEqual(0, this.slider.Minimum);

                this.slider.SelectionEnd = 0;
            }, () =>
            {
                this.slider.RangeSlider.endThumbAccumulatedDelta = this.slider.RangeSlider.SelectionEnd;
                this.slider.RangeSlider.HandleSelectionEndThumbDragDelta(trackWidth / 5);
            }, () =>
            {
                this.AssertMiddleThumbSizeChanged(middleThumbSize, false);
                Assert.AreEqual(2, this.slider.SelectionEnd);
            });
        }

        [TestMethod]
        public void RangeSliderTest_DragEndThumb_AssertSelectionEnd_CoercedToSelectionStart()
        {
            this.slider.SelectionStart = 6;
            this.slider.SelectionEnd = 10;
            double trackWidth = 0;

            this.CreateAsyncTest(this.slider, () =>
            {
                trackWidth = this.slider.RangeSlider.ActualWidth - this.slider.RangeSlider.SelectionEndOffset - this.slider.RangeSlider.SelectionStartOffset;

                // Drag the SelectionEndThumb to the half of the range
                this.slider.RangeSlider.endThumbAccumulatedDelta = this.slider.RangeSlider.SelectionEnd;
                this.slider.RangeSlider.HandleSelectionEndThumbDragDelta(-(trackWidth / 2));
            }, () =>
            {
                Assert.AreEqual(this.slider.SelectionStart, this.slider.SelectionEnd);
                Assert.AreEqual(0, this.slider.Minimum);
            });
        }

        [TestMethod]
        public void RangeSliderTest_DragEndThumb_AssertSelectionEnd_SnappedToTicksAfterDrag()
        {
            this.slider.TickFrequency = 2;
            this.slider.SnapsTo = Primitives.SnapsTo.Ticks;
            this.slider.SelectionStart = 0;
            this.slider.SelectionEnd = 0;
            double trackWidth = 0;
            this.slider.Maximum = 10;

            this.CreateAsyncTest(this.slider, () =>
            {
                trackWidth = this.slider.RangeSlider.ActualWidth - this.slider.RangeSlider.SelectionEndOffset - this.slider.RangeSlider.SelectionStartOffset;

                // Drag the SelectionEndThumb to the half of the range
                this.slider.RangeSlider.endThumbAccumulatedDelta = this.slider.RangeSlider.SelectionEnd;
                this.slider.RangeSlider.HandleSelectionEndThumbDragDelta(trackWidth / 2);
            }, () =>
            {
                Assert.AreEqual(4, this.slider.SelectionEnd);
                this.slider.SelectionEnd = 0;
            }, () =>
            {
                this.slider.RangeSlider.endThumbAccumulatedDelta = this.slider.RangeSlider.SelectionEnd;
                this.slider.RangeSlider.HandleSelectionEndThumbDragDelta(trackWidth);
            }, () =>
            {
                Assert.AreEqual(this.slider.Maximum, this.slider.SelectionEnd);
            });
        }

        [TestMethod]
        public void RangeSliderTest_DragEndThumb_AssertSelectionEnd_SnappedToLargeChangeAfterDrag()
        {
            this.slider.LargeChange = 3;
            this.slider.SnapsTo = SnapsTo.LargeChange;
            this.slider.SelectionStart = 0;
            this.slider.SelectionEnd = 0;
            double trackWidth = 0;
            this.slider.Maximum = 10;

            this.CreateAsyncTest(this.slider, () =>
            {
                trackWidth = this.slider.RangeSlider.ActualWidth - this.slider.RangeSlider.SelectionEndOffset - this.slider.RangeSlider.SelectionStartOffset;

                // Drag the SelectionEndThumb to the half of the range
                this.slider.RangeSlider.endThumbAccumulatedDelta = this.slider.RangeSlider.SelectionEnd;
                this.slider.RangeSlider.HandleSelectionEndThumbDragDelta(trackWidth / 2);
            }, () =>
            {
                Assert.AreEqual(6, this.slider.SelectionEnd);
                this.slider.SelectionEnd = 0;
            }, () =>
            {
                this.slider.RangeSlider.endThumbAccumulatedDelta = this.slider.RangeSlider.SelectionEnd;
                this.slider.RangeSlider.HandleSelectionEndThumbDragDelta(trackWidth / 5);
            }, () =>
            {
                Assert.AreEqual(3, this.slider.SelectionEnd);
            });
        }

        [TestMethod]
        public void RangeSliderTest_DragEndThumb_AssertSelectionEndChangedAfterDrag_DeferredDraggingEnabled()
        {
            this.slider.IsDeferredDraggingEnabled = true;
            this.slider.LargeChange = 3;
            this.slider.SnapsTo = Primitives.SnapsTo.LargeChange;
            this.slider.SelectionStart = 0;
            this.slider.SelectionEnd = 0;
            double trackWidth = 0;
            this.slider.Maximum = 10;

            this.CreateAsyncTest(this.slider, () =>
            {
                trackWidth = this.slider.RangeSlider.ActualWidth - this.slider.RangeSlider.SelectionEndOffset - this.slider.RangeSlider.SelectionStartOffset;

                // Drag the SelectionStartThumb to the half of the range.
                this.slider.RangeSlider.endThumbAccumulatedDelta = this.slider.RangeSlider.SelectionEnd;
                this.slider.RangeSlider.HandleSelectionEndThumbDragDelta(trackWidth / 2);
            }, () =>
            {
                Assert.AreEqual<double>(0, this.slider.SelectionEnd, "SelectionEnd updated before drag completed");
                this.slider.RangeSlider.UpdateSelectionEnd(0, true);
            }, () =>
            {
                Assert.AreEqual<double>(6, this.slider.SelectionEnd, "SelectionEnd not updated after drag completed");

                // Drag the SelectionEndThumb to the end of the control.
                this.slider.RangeSlider.startThumbAccumulatedDelta = this.slider.RangeSlider.SelectionEnd;
                this.slider.RangeSlider.HandleSelectionEndThumbDragDelta(trackWidth);
            }, () =>
            {
                Assert.AreEqual<double>(6, this.slider.SelectionEnd, "SelectionEnd updated before drag completed");
                this.slider.RangeSlider.UpdateSelectionEnd(0, true);
            }, () =>
            {
                Assert.AreEqual<double>(10, this.slider.SelectionEnd, "SelectionEnd not updated after drag completed");
            });
        }

        [TestMethod]
        public void RangeSliderTest_DragStartThumb_AssertSelectionStart()
        {
            this.slider.SelectionStart = 10;
            this.slider.SelectionEnd = 10;
            double trackWidth = 0;
            double middleThumbSize = 0;
            this.slider.Maximum = 10;

            this.CreateAsyncTest(this.slider, () =>
            {
                middleThumbSize = this.slider.RangeSlider.SelectionMiddleThumb.ActualWidth;
                trackWidth = this.slider.RangeSlider.ActualWidth - this.slider.RangeSlider.SelectionEndOffset - this.slider.RangeSlider.SelectionStartOffset;

                // Drag the SelectionEndThumb to the half of the range
                this.slider.RangeSlider.startThumbAccumulatedDelta = this.slider.RangeSlider.SelectionStart;
                this.slider.RangeSlider.HandleSelectionStartThumbDragDelta(-(trackWidth / 2));
            }, () =>
            {
                this.AssertMiddleThumbSizeChanged(middleThumbSize, true);
                middleThumbSize = this.slider.RangeSlider.SelectionMiddleThumb.ActualWidth;
                Assert.AreEqual(this.slider.Maximum / 2, this.slider.SelectionStart);
                Assert.AreEqual(0, this.slider.Minimum);

                this.slider.SelectionStart = 0;
            }, () =>
            {
                this.slider.RangeSlider.startThumbAccumulatedDelta = this.slider.RangeSlider.SelectionStart;
                this.slider.RangeSlider.HandleSelectionStartThumbDragDelta(trackWidth / 5);
            }, () =>
            {
                this.AssertMiddleThumbSizeChanged(middleThumbSize, true);
                Assert.AreEqual(2, this.slider.SelectionStart);
            });
        }

        [TestMethod]
        public void RangeSliderTest_DragStartThumb_AssertSelectionStart_CoercedToSelectionEnd()
        {
            this.slider.SelectionStart = 0;
            this.slider.SelectionEnd = 10;
            double trackWidth = 0;
            this.slider.Maximum = 10;

            this.CreateAsyncTest(this.slider, () =>
            {
                trackWidth = this.slider.RangeSlider.ActualWidth - this.slider.RangeSlider.SelectionEndOffset - this.slider.RangeSlider.SelectionStartOffset;

                // Drag the SelectionStartThumb to the half of the range
                this.slider.RangeSlider.startThumbAccumulatedDelta = this.slider.RangeSlider.SelectionStart;
                this.slider.RangeSlider.HandleSelectionStartThumbDragDelta(trackWidth / 2);
            }, () =>
            {
                Assert.AreEqual(5, this.slider.SelectionStart);
                this.slider.SelectionEnd = 5;
            }, () =>
            {
                this.slider.RangeSlider.startThumbAccumulatedDelta = this.slider.RangeSlider.SelectionStart;
                this.slider.RangeSlider.HandleSelectionStartThumbDragDelta(trackWidth);
            }, () =>
            {
                Assert.AreEqual(5, this.slider.SelectionEnd);
                Assert.AreEqual(5, this.slider.SelectionStart);
            });
        }

        [TestMethod]
        public void RangeSliderTest_DragStartThumb_AssertSelectionStart_SnappedToTicksAfterDrag()
        {
            this.slider.TickFrequency = 2;
            this.slider.SnapsTo = Primitives.SnapsTo.Ticks;
            this.slider.SelectionStart = 0;
            this.slider.SelectionEnd = 10;
            double trackWidth = 0;
            this.slider.Maximum = 10;

            this.CreateAsyncTest(this.slider, () =>
            {
                trackWidth = this.slider.RangeSlider.ActualWidth - this.slider.RangeSlider.SelectionEndOffset - this.slider.RangeSlider.SelectionStartOffset;

                // Drag the SelectionStartThumb to the half of the range
                this.slider.RangeSlider.startThumbAccumulatedDelta = this.slider.RangeSlider.SelectionStart;
                this.slider.RangeSlider.HandleSelectionStartThumbDragDelta(trackWidth / 2);
            }, () =>
            {
                Assert.AreEqual(4, this.slider.SelectionStart);
                this.slider.SelectionStart = 0;
            }, () =>
            {
                this.slider.RangeSlider.startThumbAccumulatedDelta = this.slider.RangeSlider.SelectionStart;
                this.slider.RangeSlider.HandleSelectionStartThumbDragDelta(trackWidth);
            }, () =>
            {
                Assert.AreEqual(this.slider.Maximum, this.slider.SelectionStart);
            });
        }

        [TestMethod]
        public void RangeSliderTest_DragStartThumb_AssertSelectionStart_SnappedToLargeChangeAfterDrag()
        {
            this.slider.LargeChange = 3;
            this.slider.SnapsTo = Primitives.SnapsTo.LargeChange;
            this.slider.SelectionStart = 0;
            this.slider.SelectionEnd = 10;
            double trackWidth = 0;
            this.slider.Maximum = 10;

            this.CreateAsyncTest(this.slider, () =>
            {
                trackWidth = this.slider.RangeSlider.ActualWidth - this.slider.RangeSlider.SelectionEndOffset - this.slider.RangeSlider.SelectionStartOffset;

                // Drag the SelectionStartThumb to the half of the range
                this.slider.RangeSlider.startThumbAccumulatedDelta = this.slider.RangeSlider.SelectionStart;
                this.slider.RangeSlider.HandleSelectionStartThumbDragDelta(trackWidth / 2);
            }, () =>
            {
                Assert.AreEqual(6, this.slider.SelectionStart);
                this.slider.SelectionStart = 0;
            }, () =>
            {
                this.slider.RangeSlider.startThumbAccumulatedDelta = this.slider.RangeSlider.SelectionStart;
                this.slider.RangeSlider.HandleSelectionStartThumbDragDelta(trackWidth / 5);
            }, () =>
            {
                Assert.AreEqual(3, this.slider.SelectionStart);
            });
        }

        [TestMethod]
        public void RangeSliderTest_DragStartThumb_AssertSelectionStartChangedAfterDrag_DeferredDraggingEnabled()
        {
            this.slider.IsDeferredDraggingEnabled = true;
            this.slider.LargeChange = 3;
            this.slider.SnapsTo = Primitives.SnapsTo.LargeChange;
            this.slider.SelectionStart = 0;
            this.slider.SelectionEnd = 10;
            double trackWidth = 0;
            this.slider.Maximum = 10;

            this.CreateAsyncTest(this.slider, () =>
            {
                trackWidth = this.slider.RangeSlider.ActualWidth - this.slider.RangeSlider.SelectionEndOffset - this.slider.RangeSlider.SelectionStartOffset;

                // Drag the SelectionStartThumb to the half of the range.
                this.slider.RangeSlider.startThumbAccumulatedDelta = this.slider.RangeSlider.SelectionStart;
                this.slider.RangeSlider.HandleSelectionStartThumbDragDelta(trackWidth / 2);
            }, () =>
            {
                Assert.AreEqual<double>(0, this.slider.SelectionStart, "SelectionStart updated before drag completed");
                this.slider.RangeSlider.UpdateSelectionStart(0, true);
            }, () =>
            {
                Assert.AreEqual<double>(6, this.slider.SelectionStart, "SelectionStart not updated after drag completed");

                // Drag the SelectionStartThumb to the end of the control.
                this.slider.RangeSlider.startThumbAccumulatedDelta = this.slider.RangeSlider.SelectionStart;
                this.slider.RangeSlider.HandleSelectionStartThumbDragDelta(trackWidth);
            }, () =>
            {
                Assert.AreEqual<double>(6, this.slider.SelectionStart, "SelectionStart updated before drag completed");
                this.slider.RangeSlider.UpdateSelectionStart(0, true);
            }, () =>
            {
                Assert.AreEqual<double>(10, this.slider.SelectionStart, "SelectionStart not updated after drag completed");
            });
        }

        [TestMethod]
        public void RangeSliderTest_DragMiddleThumb_AssertSelectionRangeChanged()
        {
            this.slider.SelectionStart = 4;
            this.slider.SelectionEnd = 5;
            double trackWidth = 0;
            double middleThumbSize = 0;
            this.slider.Maximum = 10;

            this.CreateAsyncTest(this.slider, () =>
            {
                middleThumbSize = this.slider.RangeSlider.SelectionMiddleThumb.ActualWidth;
                trackWidth = this.slider.RangeSlider.ActualWidth - this.slider.RangeSlider.SelectionEndOffset - this.slider.RangeSlider.SelectionStartOffset;

                // Drag the SelectionEndThumb to the half of the range
                this.slider.RangeSlider.HandleSelectionMiddleThumbDragDelta(-(trackWidth));
            }, () =>
            {
                Assert.AreEqual(middleThumbSize, this.slider.RangeSlider.SelectionMiddleThumb.ActualWidth);

                middleThumbSize = this.slider.RangeSlider.SelectionMiddleThumb.ActualWidth;
                Assert.AreEqual(0, this.slider.SelectionStart);
                Assert.AreEqual(1, this.slider.SelectionEnd);
            }, () =>
            {
                this.slider.RangeSlider.HandleSelectionMiddleThumbDragDelta(trackWidth * 2);
            }, () =>
            {
                middleThumbSize = this.slider.RangeSlider.SelectionMiddleThumb.ActualWidth;

                Assert.AreEqual(middleThumbSize, this.slider.RangeSlider.SelectionMiddleThumb.ActualWidth);
                Assert.AreEqual(9, this.slider.SelectionStart);
                Assert.AreEqual(this.slider.Maximum, this.slider.SelectionEnd);
            });
        }

        private void AssertMiddleThumbSizeChanged(double oldMiddleThumbSize, bool shouldExceedOldValue)
        {
            if (shouldExceedOldValue)
            {
                Assert.IsTrue(this.slider.RangeSlider.SelectionMiddleThumb.ActualWidth > oldMiddleThumbSize);
            }
            else
            {
                Assert.IsTrue(this.slider.RangeSlider.SelectionMiddleThumb.ActualWidth < oldMiddleThumbSize);
            }
        }
    }
}
