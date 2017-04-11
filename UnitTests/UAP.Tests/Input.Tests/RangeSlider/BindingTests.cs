using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WinRT.Testing;

namespace Telerik.UI.Xaml.Controls.Input.Tests
{
    [TestClass]
    [Tag("Input")]
    [Tag("RangeSlider")]
    public class BindingTests : RadControlUITest
    {
        private RadRangeSlider slider;

        public override void TestInitialize()
        {
            base.TestInitialize();

            this.slider = new RadRangeSlider();
        }

        [TestMethod]
        public void BindingTests_DefaultValues()
        {
            Assert.AreEqual(this.slider.Minimum, 0);
            Assert.AreEqual(this.slider.Maximum, 100);
            Assert.AreEqual(this.slider.SmallChange, 1);
            Assert.AreEqual(this.slider.LargeChange, 10);
            Assert.AreEqual(this.slider.SelectionStart, 4);
            Assert.AreEqual(this.slider.SelectionEnd, 6);
            Assert.AreEqual(this.slider.TickFrequency, 1);
            Assert.AreEqual(this.slider.SnapsTo, SnapsTo.None);
            Assert.IsTrue(this.slider.TrackTapMode == RangeSliderTrackTapMode.IncrementByLargeChange);
            Assert.IsFalse(this.slider.IsDeferredDraggingEnabled);
            Assert.IsNull(this.slider.LabelFormat);
            Assert.IsNull(this.slider.ScaleBottomRight);
            Assert.IsNull(this.slider.ScaleTopLeft);
            Assert.IsNull(this.slider.BottomRightScaleStyle);
            Assert.IsNull(this.slider.TopLeftScaleStyle);
        }

        [TestMethod]
        public void BindingTests_MinimumAndMaximum_AssertChildrenControlsBinding()
        {
            this.slider.Maximum = 100;
            this.slider.Minimum = 10;

            this.CreateAsyncTest(this.slider, () =>
           {
               Assert.AreEqual(this.slider.RangeSlider.Maximum, 100);
               Assert.AreEqual(this.slider.RangeSlider.Minimum, 10);
               Assert.AreEqual(this.slider.ScaleTopLeft.Maximum, 100);
               Assert.AreEqual(this.slider.ScaleTopLeft.Minimum, 10);
               Assert.AreEqual(this.slider.ScaleBottomRight.Maximum, 100);
               Assert.AreEqual(this.slider.ScaleBottomRight.Minimum, 10);

           });
        }

        [TestMethod]
        public void BindingTests_TickFrequencyAndOrientation_AssertChildrenControlsBinding()
        {
            this.slider.TickFrequency = 5;
            this.slider.Orientation = Windows.UI.Xaml.Controls.Orientation.Vertical;

            this.CreateAsyncTest(this.slider, () =>
            {
                Assert.AreEqual(this.slider.RangeSlider.TickFrequency, 5);
                Assert.AreEqual(this.slider.ScaleTopLeft.TickFrequency, 5);
                Assert.AreEqual(this.slider.ScaleBottomRight.TickFrequency, 5);
     
                Assert.AreEqual<Orientation>(this.slider.RangeSlider.Orientation, Orientation.Vertical);
                Assert.AreEqual<Orientation>(this.slider.ScaleTopLeft.Orientation, Orientation.Vertical);
                Assert.AreEqual<Orientation>(this.slider.ScaleBottomRight.Orientation, Orientation.Vertical);

                this.slider.TickFrequency = 1;
                this.slider.Orientation = Windows.UI.Xaml.Controls.Orientation.Horizontal;

                Assert.AreEqual(this.slider.RangeSlider.TickFrequency, 1);
                Assert.AreEqual(this.slider.ScaleTopLeft.TickFrequency, 1);
                Assert.AreEqual(this.slider.ScaleBottomRight.TickFrequency, 1);

                Assert.AreEqual<Orientation>(this.slider.RangeSlider.Orientation, Orientation.Horizontal);
                Assert.AreEqual<Orientation>(this.slider.ScaleTopLeft.Orientation, Orientation.Horizontal);
                Assert.AreEqual<Orientation>(this.slider.ScaleBottomRight.Orientation, Orientation.Horizontal);
            });
        }

        [TestMethod]
        public void BindingTests_RangeSliderToRangePrimitive_AssertChildrenControlsBinding()
        {
            this.slider.TrackTapMode = RangeSliderTrackTapMode.MoveToTapPosition;
            this.slider.LargeChange = 5;
            this.slider.SmallChange = 3;
            this.slider.IsDeferredDraggingEnabled = true;

            this.CreateAsyncTest(this.slider, () =>
            {
                Assert.IsTrue(this.slider.RangeSlider.TrackTapMode == RangeSliderTrackTapMode.MoveToTapPosition );
                Assert.AreEqual(this.slider.RangeSlider.LargeChange, 5);
                Assert.AreEqual(this.slider.RangeSlider.SmallChange, 3);
                Assert.AreEqual(this.slider.RangeSlider.IsDeferredDraggingEnabled, true);

                this.slider.TrackTapMode = RangeSliderTrackTapMode.None;
                this.slider.LargeChange = 0;
                this.slider.SmallChange = 0;
                this.slider.IsDeferredDraggingEnabled = false;

                Assert.IsTrue(this.slider.RangeSlider.TrackTapMode == RangeSliderTrackTapMode.None);
                Assert.AreEqual(this.slider.RangeSlider.LargeChange, 0);
                Assert.AreEqual(this.slider.RangeSlider.SmallChange, 0);
                Assert.AreEqual(this.slider.RangeSlider.IsDeferredDraggingEnabled, false);
            });
        }

        [TestMethod]
        public void BindingTests_ScalePrimitive_AssertStyleBinding()
        {
            Style scaleStyle = new Style();
            scaleStyle.TargetType = typeof(ScalePrimitive);
            scaleStyle.Setters.Add(new Setter(ScalePrimitive.TickPlacementProperty, ScaleElementPlacement.None));
            scaleStyle.Setters.Add(new Setter(ScalePrimitive.LabelPlacementProperty, ScaleElementPlacement.None));
            this.slider.TopLeftScaleStyle = scaleStyle;
            this.slider.BottomRightScaleStyle = scaleStyle;

            this.CreateAsyncTest(this.slider, () =>
            {
                Assert.IsNotNull(this.slider.TopLeftScaleStyle);
                Assert.IsNotNull(this.slider.BottomRightScaleStyle);
                Assert.IsNotNull(this.slider.ScaleTopLeft);
                Assert.IsNotNull(this.slider.ScaleBottomRight);

                Assert.AreEqual<Style>(this.slider.TopLeftScaleStyle, scaleStyle);
                Assert.AreEqual<Style>(this.slider.ScaleBottomRight.Style, scaleStyle);
                Assert.AreEqual<Style>(this.slider.ScaleTopLeft.Style, scaleStyle);
            });
        }

        [TestMethod]
        public void BindingTests_RangeSliderPrimitive_AssertSelectionRangeBinding()
        {
            this.slider.SelectionStart = 0;
            this.slider.SelectionEnd = 9;

            this.CreateAsyncTest(this.slider, () =>
            {
                Assert.AreEqual(this.slider.SelectionEnd, 9);
                Assert.AreEqual(this.slider.SelectionStart, 0);
                Assert.AreEqual(this.slider.RangeSlider.SelectionEnd, 9);
                Assert.AreEqual(this.slider.RangeSlider.SelectionStart, 0);

                this.slider.RangeSlider.SelectionStart = 5;
                this.slider.RangeSlider.SelectionEnd = 10;

                Assert.AreEqual(this.slider.SelectionEnd, 10);
                Assert.AreEqual(this.slider.SelectionStart, 5);
                Assert.AreEqual(this.slider.RangeSlider.SelectionEnd, 10);
                Assert.AreEqual(this.slider.RangeSlider.SelectionStart, 5);
            });
        }
    }
}
