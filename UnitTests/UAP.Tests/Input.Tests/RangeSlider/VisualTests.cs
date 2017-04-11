using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using WinRT.Testing;

namespace Telerik.UI.Xaml.Controls.Input.Tests.RangeSlider
{
    [TestClass]
    [Tag("Input")]
    [Tag("RangeSlider")]
    public class VisualTests : RadControlUITest
    {
        private RadRangeSlider slider;

        public override void TestInitialize()
        {
            base.TestInitialize();

            this.slider = new RadRangeSlider();
        }

        [TestMethod]
        public void VisualTests_RangeSliderStyle_SelectionStartEndThumbStyle_AssertCustomStyleApplied()
        {
            var thumbStyle = new Style(typeof(Thumb));
            thumbStyle.Setters.Add(new Setter(Thumb.BackgroundProperty, new SolidColorBrush(Colors.Red)));
            thumbStyle.Setters.Add(new Setter(Thumb.WidthProperty, 20));
            thumbStyle.Setters.Add(new Setter(Thumb.HeightProperty, 20));

            var rangeSliderStyle = new Style(typeof(RangeSliderPrimitive));
            rangeSliderStyle.Setters.Add(new Setter(RangeSliderPrimitive.SelectionStartThumbStyleProperty, thumbStyle));
            rangeSliderStyle.Setters.Add(new Setter(RangeSliderPrimitive.SelectionEndThumbStyleProperty, thumbStyle));

            this.slider.SliderPrimitiveStyle = rangeSliderStyle;
          
            this.CreateAsyncTest(this.slider, () =>
            {
                var selectionStartThumb = ElementTreeHelper.EnumVisualDescendants<Thumb>(this.slider.RangeSlider).Where(x => (x is Thumb) && x.Name == "PART_SelectionStartThumb").FirstOrDefault();
               
                var selectionEndThumb = ElementTreeHelper.EnumVisualDescendants<Thumb>(this.slider.RangeSlider).Where(x => (x is Thumb) && x.Name == "PART_SelectionEndThumb").FirstOrDefault();

                VisualTestsHelper.AssertSelectionThumbs_CustomStyle(selectionEndThumb, thumbStyle);
                VisualTestsHelper.AssertSelectionThumbs_CustomStyle(selectionStartThumb, thumbStyle);
            });
        }

        [TestMethod]
        [Ignore]
        public void VisualTests_RangeSliderStyle_ChangeStartThumbStyleDynamically_AssertSelectionStartOffsetChanged()
        {
            var thumbStyle = new Style(typeof(Thumb));
            thumbStyle.Setters.Add(new Setter(Thumb.BackgroundProperty, new SolidColorBrush(Colors.Red)));

            var rangeSliderStyle = new Style(typeof(RangeSliderPrimitive));
            rangeSliderStyle.Setters.Add(new Setter(RangeSliderPrimitive.SelectionStartThumbStyleProperty, thumbStyle));
            thumbStyle.Setters.Add(new Setter(Thumb.WidthProperty, 120));
            thumbStyle.Setters.Add(new Setter(Thumb.HeightProperty, 40));

            this.slider.SliderPrimitiveStyle = rangeSliderStyle;

            this.CreateAsyncTest(this.slider, () =>
            {
                var selectionStartThumb = ElementTreeHelper.EnumVisualDescendants<Thumb>(this.slider.RangeSlider).Where(x => (x is Thumb) && x.Name == "PART_SelectionStartThumb").FirstOrDefault();

                thumbStyle = new Style(typeof(Thumb));
                thumbStyle.Setters.Add(new Setter(Thumb.BackgroundProperty, new SolidColorBrush(Colors.Red)));
                thumbStyle.Setters.Add(new Setter(Thumb.WidthProperty, 120));
                thumbStyle.Setters.Add(new Setter(Thumb.HeightProperty, 40));

                rangeSliderStyle = new Style(typeof(RangeSliderPrimitive));
                rangeSliderStyle.Setters.Add(new Setter(RangeSliderPrimitive.SelectionStartThumbStyleProperty, thumbStyle));

                this.slider.SliderPrimitiveStyle = rangeSliderStyle;

                VisualTestsHelper.AssertSelectionThumbs_CustomStyle(selectionStartThumb, thumbStyle);
            },()=>
                {
                    var s = this.slider;
            });
        }

        [TestMethod]
        public void VisualTests_RangeSliderStyle_SelectionMiddleThumbStyle_AssertCustomStyleApplied()
        {
            var thumbStyle = new Style(typeof(Thumb));
            thumbStyle.Setters.Add(new Setter(Thumb.BackgroundProperty, new SolidColorBrush(Colors.Red)));
            thumbStyle.Setters.Add(new Setter(Thumb.HeightProperty, 40));

            var rangeSliderStyle = new Style(typeof(RangeSliderPrimitive));
            rangeSliderStyle.Setters.Add(new Setter(RangeSliderPrimitive.SelectionMiddleThumbStyleProperty, thumbStyle));

            this.slider.SliderPrimitiveStyle = rangeSliderStyle;

            this.CreateAsyncTest(this.slider, () =>
            {

                var selectionMiddleThumb = ElementTreeHelper.EnumVisualDescendants<Thumb>(this.slider.RangeSlider).Where(x => (x is Thumb) && x.Name == "PART_SelectionMiddleThumb").FirstOrDefault();

                VisualTestsHelper.AssertSelectionThumbs_CustomStyle(selectionMiddleThumb, thumbStyle);
            });
        }

        [TestMethod]
        public void VisualTests_TopLeftScaleStyle_ShowHideScale_AssertScaleVisibility()
        {
            var topLeftScaleStyle = new Style(typeof(ScalePrimitive));
            topLeftScaleStyle.Setters.Add(new Setter(ScalePrimitive.VisibilityProperty, Visibility.Collapsed));

            this.slider.TopLeftScaleStyle = topLeftScaleStyle;

            this.CreateAsyncTest(this.slider, () =>
            {
                var topLeftScale = ElementTreeHelper.EnumVisualDescendants<ScalePrimitive>(this.slider).Where(x => (x is ScalePrimitive) && x.Name == "PART_ScaleTopLeft").FirstOrDefault();

                var bottomRightScale = ElementTreeHelper.EnumVisualDescendants<ScalePrimitive>(this.slider).Where(x => (x is ScalePrimitive) && x.Name == "PART_ScaleBottomRight").FirstOrDefault();

                Assert.IsNotNull(topLeftScale);
                Assert.IsNotNull(bottomRightScale);
                Assert.IsTrue(topLeftScale.Visibility == Visibility.Collapsed);
                Assert.IsTrue(bottomRightScale.Visibility == Visibility.Visible);

                topLeftScaleStyle = new Style(typeof(ScalePrimitive));
                topLeftScaleStyle.Setters.Add(new Setter(ScalePrimitive.VisibilityProperty, Visibility.Visible));

                this.slider.TopLeftScaleStyle = topLeftScaleStyle;
            },()=>
            {
                var topLeftScale = ElementTreeHelper.EnumVisualDescendants<ScalePrimitive>(this.slider).Where(x => (x is ScalePrimitive) && x.Name == "PART_ScaleTopLeft").FirstOrDefault();

                Assert.IsNotNull(topLeftScale);
                Assert.IsTrue(topLeftScale.Visibility == Visibility.Visible);
            });
        }
     
        [TestMethod]
        public void VisualTests_BottomRightScaleStyle_ShowHideScale_AssertScaleVisibility()
        {
            var bottomRightScaleStyle = new Style(typeof(ScalePrimitive));
            bottomRightScaleStyle.Setters.Add(new Setter(ScalePrimitive.VisibilityProperty, Visibility.Collapsed));

            this.slider.BottomRightScaleStyle = bottomRightScaleStyle;

            this.CreateAsyncTest(this.slider, () =>
            {
                var topLeftScale = ElementTreeHelper.EnumVisualDescendants<ScalePrimitive>(this.slider).Where(x => (x is ScalePrimitive) && x.Name == "PART_ScaleTopLeft").FirstOrDefault();

                var bottomRightScale = ElementTreeHelper.EnumVisualDescendants<ScalePrimitive>(this.slider).Where(x => (x is ScalePrimitive) && x.Name == "PART_ScaleBottomRight").FirstOrDefault();

                Assert.IsNotNull(topLeftScale);
                Assert.IsNotNull(bottomRightScale);
                Assert.IsTrue(topLeftScale.Visibility == Visibility.Visible);
                Assert.IsTrue(bottomRightScale.Visibility == Visibility.Collapsed);

                bottomRightScaleStyle = new Style(typeof(ScalePrimitive));
                bottomRightScaleStyle.Setters.Add(new Setter(ScalePrimitive.VisibilityProperty, Visibility.Visible));

                this.slider.BottomRightScaleStyle = bottomRightScaleStyle;
            }, () =>
            {
                var bottomRightScale = ElementTreeHelper.EnumVisualDescendants<ScalePrimitive>(this.slider).Where(x => (x is ScalePrimitive) && x.Name == "PART_ScaleBottomRight").FirstOrDefault();

                Assert.IsNotNull(bottomRightScale);
                Assert.IsTrue(bottomRightScale.Visibility == Visibility.Visible);
            });
        }

        [TestMethod]
        public void VisualTests_CustomScalesStyle_AssertCustomStylesApplied()
        {
            var tickStyle = new Style(typeof(Rectangle));
            tickStyle.Setters.Add(new Setter(Rectangle.FillProperty, new SolidColorBrush(Colors.Red)));

            var scalesStyle = new Style(typeof(ScalePrimitive));
            scalesStyle.Setters.Add(new Setter(ScalePrimitive.LabelPlacementProperty, ScaleElementPlacement.None));
            scalesStyle.Setters.Add(new Setter(ScalePrimitive.TickStyleProperty, tickStyle));
            scalesStyle.Setters.Add(new Setter(ScalePrimitive.TickLengthProperty, 50));
            scalesStyle.Setters.Add(new Setter(ScalePrimitive.TickThicknessProperty, 2));

            this.slider.TopLeftScaleStyle = scalesStyle;
            this.slider.BottomRightScaleStyle = scalesStyle;

            this.CreateAsyncTest(this.slider, () =>
            {
                var scales = ElementTreeHelper.EnumVisualDescendants<ScalePrimitive>(this.slider).Where(x => (x is ScalePrimitive));

                Assert.IsNotNull(scales);

                foreach (ScalePrimitive scale in scales)
                {
                    var labelPlacement = (VisualTestsHelper.GetStyleSetterPropertyValue(scalesStyle, ScalePrimitive.LabelPlacementProperty));
                    double tickLength = Convert.ToDouble(VisualTestsHelper.GetStyleSetterPropertyValue(scalesStyle, ScalePrimitive.TickLengthProperty));
                    double tickThickness = Convert.ToDouble(VisualTestsHelper.GetStyleSetterPropertyValue(scalesStyle, ScalePrimitive.TickThicknessProperty));
                    double tickFrequency = Convert.ToDouble(VisualTestsHelper.GetStyleSetterPropertyValue(scalesStyle, ScalePrimitive.TickFrequencyProperty));

                    Assert.AreEqual(scale.LabelPlacement, labelPlacement);
                    Assert.AreEqual(scale.TickLength, tickLength);
                    Assert.AreEqual(scale.TickThickness, tickThickness);
                }
            });
        }

    }
}
