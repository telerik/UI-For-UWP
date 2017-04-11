using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using WinRT.Testing;

namespace Telerik.UI.Xaml.Controls.Input.Tests.RangeSlider
{
    [TestClass]
    [Tag("Input")]
    [Tag("RangeSlider")]
    public class CommonTests : RadControlUITest
    {
        private RadRangeSlider slider;

        public override void TestInitialize()
        {
            base.TestInitialize();

            this.slider = new RadRangeSlider();
        }

        [TestMethod]
        public void RangeSliderTest_Header_AssertHeaderText()
        {
            this.slider.Header = "New header";
            this.CreateAsyncTest(this.slider, () =>
             {
                 var headerVisual = ElementTreeHelper.EnumVisualDescendants<ContentControl>(this.slider).Where(x => (x is ContentControl)).FirstOrDefault();

                 Assert.IsNotNull(headerVisual);
                 Assert.AreEqual("New header", headerVisual.Content);
             });
        }

        [TestMethod]
        public void RangeSliderTest_HeaderStyle_AssertHeaderStyle()
        {
            Style headerStyle = new Style(typeof(ContentControl));

            headerStyle.Setters.Add(new Setter(ContentControl.FontSizeProperty , 20));
            headerStyle.Setters.Add(new Setter(ContentControl.ForegroundProperty, new SolidColorBrush(Colors.Red)));
         
            this.slider.Header = "New header";
            this.slider.HeaderStyle = headerStyle;

            this.CreateAsyncTest(this.slider, () =>
            {
                var headerVisual = ElementTreeHelper.EnumVisualDescendants<ContentControl>(this.slider).Where(x => (x is ContentControl)).FirstOrDefault();

                Assert.IsNotNull(headerVisual);
                Assert.AreEqual("New header", headerVisual.Content);
                Assert.AreEqual(headerVisual.FontSize, 20);
                Assert.AreEqual((headerVisual.Foreground as SolidColorBrush).Color, new SolidColorBrush(Colors.Red).Color);
            });
        }

        [TestMethod]
        public void RangeSliderTest_HeaderTemplate_AssertHeaderTemplate()
        {
            var template = XamlReader.Load(@" 
            <DataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""> 
                <Rectangle Fill=""Yellow"" Height=""20"" Width=""50"" Name=""headerTemplate""/> 
            </DataTemplate> 
            ") as DataTemplate;

            this.slider.HeaderTemplate = template;

            this.CreateAsyncTest(this.slider, () =>
            {
                var headerVisual = ElementTreeHelper.EnumVisualDescendants<Rectangle>(this.slider).Where(x => (x is Rectangle) && x.Name == "headerTemplate").FirstOrDefault();

                Assert.IsNotNull(headerVisual);
                Assert.AreEqual(headerVisual.Width, 50);
                Assert.AreEqual(headerVisual.Height, 20);
                Assert.AreEqual((headerVisual.Fill as SolidColorBrush).Color, new SolidColorBrush(Colors.Yellow).Color);
            });
        }

        [TestMethod]
        public void RangeSliderTest_SnapToTickFrequency_AssertSelectionStartSnapped()
        {
            this.slider.TickFrequency = 1;
            this.slider.SnapsTo = Primitives.SnapsTo.Ticks;
            this.slider.SelectionStart = 5;
            double middleThumbSize = 0;

            this.CreateAsyncTest(this.slider, () =>
             {
                 middleThumbSize = this.slider.RangeSlider.SelectionMiddleThumb.ActualWidth;
                 this.slider.SelectionStart += 0.3;

                 Assert.AreEqual(5, slider.SelectionStart);

                 this.slider.SelectionStart += 0.5;
                 Assert.AreEqual(6, slider.SelectionStart);

                 this.slider.SelectionStart += 3.5;
                 Assert.AreEqual(10, slider.SelectionStart);
             }, () =>
                 {
                     this.AssertMiddleThumbSizeChanged(middleThumbSize, false);
                     middleThumbSize = this.slider.RangeSlider.SelectionMiddleThumb.ActualWidth;

                     this.slider.SelectionStart -= 0.5;
                     Assert.AreEqual(10, slider.SelectionStart);

                     this.slider.SelectionStart -= 0.6;
                     Assert.AreEqual(9, slider.SelectionStart);
                 }, () =>
                     {
                         this.AssertMiddleThumbSizeChanged(middleThumbSize, true);
                     });
        }

        [TestMethod]
        public void RangeSliderTest_SnapToTickFrequency_AssertSelectionStart_SnappedToBoundaries()
        {
            this.slider.TickFrequency = 1;
            this.slider.SnapsTo = Primitives.SnapsTo.Ticks;
            this.slider.SelectionStart = 9;
            this.slider.Maximum = 10;

            this.CreateAsyncTest(this.slider, () =>
            {
                this.slider.SelectionStart += 1.6;

                Assert.AreEqual<double>(this.slider.Maximum, slider.SelectionStart, "SelectionStart is not snapped to Maximum");

                this.slider.Maximum = 15;
            }, () =>
                {
                    Assert.AreEqual<double>(10, slider.SelectionStart, "SelectionStart returned its desired value");
                    this.slider.SelectionStart -= 11.6;

                    Assert.AreEqual<double>(this.slider.Minimum, slider.SelectionStart, "SelectionStart not snapped to Minimum");
                });
        }

        [TestMethod]
        public void RangeSliderTest_SnapToTickFrequency_AssertSelectionEndSnapped()
        {
            this.slider.TickFrequency = 1;
            this.slider.SnapsTo = Primitives.SnapsTo.Ticks;
            this.slider.SelectionEnd = 6;
            this.slider.SelectionStart = 5;
            double middleThumbSize = 0;
            this.slider.Maximum = 10;

            this.CreateAsyncTest(this.slider, () =>
            {
                middleThumbSize = this.slider.RangeSlider.SelectionMiddleThumb.ActualWidth;
                this.slider.SelectionEnd += 0.3;

                Assert.AreEqual(6, slider.SelectionEnd);

                this.slider.SelectionEnd += 0.5;
                Assert.AreEqual(6, slider.SelectionEnd);

                this.slider.SelectionEnd += 3.5;
                Assert.AreEqual(10, slider.SelectionEnd);
            }, () =>
            {
                this.AssertMiddleThumbSizeChanged(middleThumbSize, true);
                middleThumbSize = this.slider.RangeSlider.SelectionMiddleThumb.ActualWidth;
                this.slider.SelectionEnd -= -0.5;
                Assert.AreEqual(10, slider.SelectionEnd);

                this.slider.SelectionEnd -= 0.6;
                Assert.AreEqual(9, slider.SelectionEnd);
            }, () =>
                {
                    this.AssertMiddleThumbSizeChanged(middleThumbSize, false);
                });
        }

        [TestMethod]
        public void RangeSliderTest_SnapToTickFrequency_AssertSelectionEnd_SnappedToBoundaries()
        {
            this.slider.TickFrequency = 1;
            this.slider.SnapsTo = Primitives.SnapsTo.Ticks;
            this.slider.SelectionEnd = 6;
            this.slider.SelectionStart = 5;
            this.slider.Maximum = 10;

            this.CreateAsyncTest(this.slider, () =>
            {
                this.slider.SelectionEnd += 10;

                Assert.AreEqual(this.slider.Maximum, slider.SelectionEnd);

                this.slider.Maximum = 15;
            }, () =>
            {
                Assert.AreEqual<double>(10, slider.SelectionEnd, "SelectionEnd returned its desired value");
                this.slider.SelectionEnd -= 20;

                Assert.AreEqual<double>(this.slider.SelectionStart, slider.SelectionEnd, "SelectionEnd not snapped to SelectionStart");
            });
        }

        [TestMethod]
        public void RangeSliderTest_SnapToTickFrequency_AssertSelectionEnd_SnappedAfterCoerce()
        {
            this.slider.TickFrequency = 2;
            this.slider.SnapsTo = Primitives.SnapsTo.Ticks;
            this.slider.SelectionEnd = 15.1;
            this.slider.SelectionStart = 10;
            this.slider.Maximum = 10;

            this.CreateAsyncTest(this.slider, () =>
            {
                Assert.AreEqual(this.slider.Maximum, slider.SelectionEnd);

                this.slider.Maximum = 20;
            }, () =>
            {
                Assert.AreEqual<double>(10, slider.SelectionEnd, "SelectionEnd not snapped to nearest tick");
                this.slider.SelectionEnd = 11.1;
            }, () =>
                {
                    this.slider.SelectionStart = 0;
                    Assert.AreEqual<double>(12, slider.SelectionEnd, "SelectionEnd not snapped to nearest tick");
                });
        }

        [TestMethod]
        public void RangeSliderTest_SnapToTickFrequency_AssertSelectionStart_SnappedAfterCoerce()
        {
            this.slider.TickFrequency = 2;
            this.slider.SnapsTo = Primitives.SnapsTo.Ticks;
            this.slider.SelectionStart = -1.1;
            this.slider.SelectionEnd = 5;
            this.slider.Maximum = 10;

            this.CreateAsyncTest(this.slider, () =>
            {
                Assert.AreEqual(this.slider.Minimum, slider.SelectionStart);
                this.slider.Minimum = -10;
            }, () =>
            {
                Assert.AreEqual<double>(-2, slider.SelectionStart, "SelectionStart not snapped to nearest tick");
                this.slider.SelectionEnd = 11.1;
            }, () =>
            {
                this.slider.SelectionStart = 7.1;
                Assert.AreEqual<double>(8, slider.SelectionStart, "SelectionStart not snapped to nearest tick");
            });
        }

        [TestMethod]
        public void RangeSliderTest_SnapToLargeChange_AssertSelectionStartSnapped()
        {
            this.slider.LargeChange = 1;
            this.slider.SnapsTo = Primitives.SnapsTo.LargeChange;
            this.slider.SelectionStart = 5;
            this.slider.Maximum = 10;

            this.CreateAsyncTest(this.slider, () =>
            {
                this.slider.SelectionStart += 0.3;

                Assert.AreEqual(5, slider.SelectionStart);

                this.slider.SelectionStart += 0.5;
                Assert.AreEqual(6, slider.SelectionStart);

                this.slider.SelectionStart += 3.5;
                Assert.AreEqual(10, slider.SelectionStart);
            }, () =>
            {
                this.slider.SelectionStart -= 0.5;
                Assert.AreEqual(10, slider.SelectionStart);

                this.slider.SelectionStart -= 0.6;
                Assert.AreEqual(9, slider.SelectionStart);
            });
        }

        [TestMethod]
        public void RangeSliderTest_SnapToLargeChange_AssertSelectionStart_SnappedToBoundaries()
        {
            this.slider.LargeChange = 1;
            this.slider.SnapsTo = Primitives.SnapsTo.LargeChange;
            this.slider.SelectionStart = 9;
            this.slider.Maximum = 10;

            this.CreateAsyncTest(this.slider, () =>
            {
                this.slider.SelectionStart += 1.6;

                Assert.AreEqual<double>(this.slider.Maximum, slider.SelectionStart, "SelectionStart is not snapped to Maximum");

                this.slider.Maximum = 15;
            }, () =>
            {
                Assert.AreEqual<double>(10, slider.SelectionStart, "SelectionStart returned its desired value");
                this.slider.SelectionStart -= 11.6;

                Assert.AreEqual<double>(this.slider.Minimum, slider.SelectionStart, "SelectionStart not snapped to Minimum");
            });
        }

        [TestMethod]
        public void RangeSliderTest_SnapToLargeChange_AssertSelectionEndSnapped()
        {
            this.slider.LargeChange = 1;
            this.slider.SnapsTo = Primitives.SnapsTo.LargeChange;
            this.slider.SelectionEnd = 6;
            this.slider.SelectionStart = 5;
            this.slider.Maximum = 10;

            this.CreateAsyncTest(this.slider, () =>
            {
                this.slider.SelectionEnd += 0.3;

                Assert.AreEqual(6, slider.SelectionEnd);

                this.slider.SelectionEnd += 0.5;
                Assert.AreEqual(6, slider.SelectionEnd);

                this.slider.SelectionEnd += 3.5;
                Assert.AreEqual(10, slider.SelectionEnd);
            }, () =>
            {
                this.slider.SelectionEnd -= 0.5;
                Assert.AreEqual(10, slider.SelectionEnd);

                this.slider.SelectionEnd -= 0.6;
                Assert.AreEqual(9, slider.SelectionEnd);
            });
        }

        [TestMethod]
        public void RangeSliderTest_SnapToLargeChange_AssertSelectionEnd_SnappedToBoundaries()
        {
            this.slider.LargeChange = 1;
            this.slider.SnapsTo = Primitives.SnapsTo.LargeChange;
            this.slider.SelectionEnd = 6;
            this.slider.SelectionStart = 5;
            this.slider.Maximum = 10;

            this.CreateAsyncTest(this.slider, () =>
            {
                this.slider.SelectionEnd += 10;

                Assert.AreEqual(this.slider.Maximum, slider.SelectionEnd);

                this.slider.Maximum = 15;
            }, () =>
            {
                Assert.AreEqual<double>(10, slider.SelectionEnd, "SelectionEnd returned its desired value");
                this.slider.SelectionEnd -= 20;

                Assert.AreEqual<double>(this.slider.SelectionStart, slider.SelectionEnd, "SelectionEnd not snapped to SelectionStart");
            });
        }

        [TestMethod]
        public void RangeSliderTest_SnapToLargeChange_AssertSelectionEnd_SnappedAfterCoerce()
        {
            this.slider.LargeChange = 2;
            this.slider.SnapsTo = Primitives.SnapsTo.LargeChange;
            this.slider.SelectionEnd = 15.1;
            this.slider.SelectionStart = 10;
            this.slider.Maximum = 10;

            this.CreateAsyncTest(this.slider, () =>
            {
                Assert.AreEqual(this.slider.Maximum, slider.SelectionEnd);

                this.slider.Maximum = 20;
            }, () =>
            {
                Assert.AreEqual<double>(10, slider.SelectionEnd, "SelectionEnd not snapped to nearest tick");
                this.slider.SelectionEnd = 11.1;
            }, () =>
            {
                this.slider.SelectionStart = 0;
                Assert.AreEqual<double>(12, slider.SelectionEnd, "SelectionEnd not snapped to nearest tick");
            });
        }

        [TestMethod]
        public void RangeSliderTest_SnapToLargeChange_AssertSelectionStart_SnappedAfterCoerce()
        {
            this.slider.LargeChange = 2;
            this.slider.SnapsTo = Primitives.SnapsTo.LargeChange;
            this.slider.SelectionStart = -1.1;
            this.slider.SelectionEnd = 5;
            this.slider.Maximum = 10;

            this.CreateAsyncTest(this.slider, () =>
            {
                Assert.AreEqual(this.slider.Minimum, slider.SelectionStart);
                this.slider.Minimum = -10;
            }, () =>
            {
                Assert.AreEqual<double>(-2, slider.SelectionStart, "SelectionStart not snapped to nearest tick");
                this.slider.SelectionEnd = 11.1;
            }, () =>
            {
                this.slider.SelectionStart = 7.1;
                Assert.AreEqual<double>(8, slider.SelectionStart, "SelectionStart not snapped to nearest tick");
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
