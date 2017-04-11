using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using WinRT.Testing;

namespace Telerik.UI.Xaml.Controls.Input.Tests.RangeSlider
{
    [TestClass]
    [Tag("Input")]
    [Tag("RangeSlider")]
    [Tag("Scale")]
    public class RangeSliderVisualTests : RadControlUITest
    {
        private RangeSliderPrimitive rangeSlider;

        public override void TestInitialize()
        {
            base.TestInitialize();

            this.rangeSlider = new RangeSliderPrimitive();
        }

        [TestMethod]
        public void RangeSliderVisualTests_SelectionThumbs_AssertDefaultStyle()
        {
            this.CreateAsyncTest(this.rangeSlider, () =>
            {
                var selectionStartThumb = ElementTreeHelper.EnumVisualDescendants<Thumb>(this.rangeSlider).Where(x => (x is Thumb) && x.Name == "PART_SelectionStartThumb").FirstOrDefault();

                var selectionEndThumb = ElementTreeHelper.EnumVisualDescendants<Thumb>(this.rangeSlider).Where(x => (x is Thumb) && x.Name == "PART_SelectionEndThumb").FirstOrDefault();

                var selectionMiddleThumb = ElementTreeHelper.EnumVisualDescendants<Thumb>(this.rangeSlider).Where(x => (x is Thumb) && x.Name == "PART_SelectionMiddleThumb").FirstOrDefault();

                VisualTestsHelper.AssertSelectionThumbs_DefaultStyle(selectionStartThumb);
                VisualTestsHelper.AssertSelectionThumbs_DefaultStyle(selectionEndThumb);
                VisualTestsHelper.AssertSelectionThumbs_DefaultStyle(selectionMiddleThumb);
            });
        }

        [TestMethod]
        public void RangeSliderVisualTests_SelectionStartThumb_CustomStyle_AssertCustomStyleApplied()
        {
            var thumbStyle = new Style(typeof(Thumb));
            thumbStyle.Setters.Add(new Setter(Thumb.BackgroundProperty, new SolidColorBrush(Colors.Red)));
            thumbStyle.Setters.Add(new Setter(Thumb.WidthProperty, 20));
            thumbStyle.Setters.Add(new Setter(Thumb.HeightProperty, 20));

            this.rangeSlider.SelectionStartThumbStyle = thumbStyle;

            this.CreateAsyncTest(this.rangeSlider, () =>
            {
                var selectionStartThumb = ElementTreeHelper.EnumVisualDescendants<Thumb>(this.rangeSlider).Where(x => (x is Thumb) && x.Name == "PART_SelectionStartThumb").FirstOrDefault();

                Assert.IsNotNull(selectionStartThumb);
                VisualTestsHelper.AssertSelectionThumbs_CustomStyle(selectionStartThumb, thumbStyle);

                Assert.AreEqual(this.rangeSlider.SelectionStartOffset, selectionStartThumb.Width);
            });
        }

        [TestMethod]
        public void RangeSliderVisualTests_SelectionEndThumb_CustomStyle_AssertCustomStyleApplied()
        {
            var thumbStyle = new Style(typeof(Thumb));
            thumbStyle.Setters.Add(new Setter(Thumb.BackgroundProperty, new SolidColorBrush(Colors.Red)));
            thumbStyle.Setters.Add(new Setter(Thumb.WidthProperty, 20));
            thumbStyle.Setters.Add(new Setter(Thumb.HeightProperty, 20));

            this.rangeSlider.SelectionEndThumbStyle = thumbStyle;

            this.CreateAsyncTest(this.rangeSlider, () =>
            {
                var selectionEndThumb = ElementTreeHelper.EnumVisualDescendants<Thumb>(this.rangeSlider).Where(x => (x is Thumb) && x.Name == "PART_SelectionEndThumb").FirstOrDefault();

                Assert.IsNotNull(selectionEndThumb);
                VisualTestsHelper.AssertSelectionThumbs_CustomStyle(selectionEndThumb, thumbStyle);

                Assert.AreEqual(this.rangeSlider.SelectionEndOffset, selectionEndThumb.Width);
            });
        }

        [TestMethod]
        public void RangeSliderVisualTests_SelectionMiddleThumb_CustomStyle_AssertCustomStyleApplied()
        {
            var thumbStyle = new Style(typeof(Thumb));
            thumbStyle.Setters.Add(new Setter(Thumb.BackgroundProperty, new SolidColorBrush(Colors.Red)));
            thumbStyle.Setters.Add(new Setter(Thumb.HeightProperty, 50));

            this.rangeSlider.SelectionMiddleThumbStyle = thumbStyle;

            this.CreateAsyncTest(this.rangeSlider, () =>
            {
                var selectionMiddleThumb = ElementTreeHelper.EnumVisualDescendants<Thumb>(this.rangeSlider).Where(x => (x is Thumb) && x.Name == "PART_SelectionMiddleThumb").FirstOrDefault();

                Assert.IsNotNull(selectionMiddleThumb);
                VisualTestsHelper.AssertSelectionThumbs_CustomStyle(selectionMiddleThumb, thumbStyle);

            });
        }
    }

}
