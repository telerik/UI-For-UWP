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

namespace Telerik.UI.Xaml.Controls.Input.Tests.RangeSlider.Scale
{
    [TestClass]
    [Tag("Input")]
    [Tag("RangeSlider")]
    [Tag("Scale")]
    public class ScaleVisualTests : RadControlUITest
    {
        private ScalePrimitive scale;

        public override void TestInitialize()
        {
            base.TestInitialize();

            this.scale = new ScalePrimitive();
        }

        [TestMethod]
        public void ScaleVisualTests_AssertDefaultStyle()
        {
            this.scale.Minimum = 0;
            this.scale.Maximum = 10;

            this.CreateAsyncTest(this.scale, () =>
            {
                var labels = ElementTreeHelper.EnumVisualDescendants<TextBlock>(this.scale).Where(x => (x is TextBlock));
                var ticks = ElementTreeHelper.EnumVisualDescendants<Rectangle>(this.scale).Where(x => (x is Rectangle) && x.Name != "PART_Line");
                Rectangle line = ElementTreeHelper.EnumVisualDescendants<Rectangle>(this.scale).Where(x => (x is Rectangle) && x.Name == "PART_Line").FirstOrDefault();

                Assert.IsNotNull(labels);
                Assert.IsNotNull(line);
                Assert.IsNotNull(ticks);

                Assert.AreEqual(labels.Count(), 2);
                Assert.AreEqual(ticks.Count(), 11);
                Assert.AreEqual((line.Fill as SolidColorBrush).Color, new SolidColorBrush(Colors.Transparent).Color);

                foreach (Rectangle tick in ticks)
                {
                    this.AssertDefaultTickStyle(tick);
                }

                foreach (TextBlock label in labels)
                {
                    Assert.AreEqual(label.FontSize, 14);
                    Assert.AreEqual((label.Foreground as SolidColorBrush).Color, new SolidColorBrush(Colors.White).Color);
                }
            });
        }

        [TestMethod]
        public void ScaleVisualTests_AssertLabelPlacement()
        {
            this.scale.Minimum = 0;
            this.scale.Maximum = 10;
            double ticksYPosition = 0;

            this.CreateAsyncTest(this.scale, () =>
            {
                List<TextBlock> labels = ElementTreeHelper.EnumVisualDescendants<TextBlock>(this.scale)
                    .Where(x => (x is TextBlock)).ToList();
                List<Rectangle> ticks = ElementTreeHelper.EnumVisualDescendants<Rectangle>(this.scale)
                    .Where(x => (x is Rectangle) && x.Name != "PART_Line").ToList();

                ticksYPosition = ticks[0].TransformToVisual(this.scale).TransformPoint(new Point(0, 0)).Y;

                foreach (TextBlock label in labels)
                {
                    double labelPosition = label.TransformToVisual(this.scale).TransformPoint(new Point(0, 0)).Y;
                    Assert.IsTrue(ticksYPosition < labelPosition);
                }

                this.scale.LabelPlacement = ScaleElementPlacement.TopLeft;
            }, () =>
                {
                    List<TextBlock> labels = ElementTreeHelper.EnumVisualDescendants<TextBlock>(this.scale)
                        .Where(x => (x is TextBlock)).ToList();
                    List<Rectangle> ticks = ElementTreeHelper.EnumVisualDescendants<Rectangle>(this.scale)
                        .Where(x => (x is Rectangle) && x.Name != "PART_Line").ToList();

                    ticksYPosition = ticks[0].TransformToVisual(this.scale).TransformPoint(new Point(0, 0)).Y;

                    foreach (TextBlock label in labels)
                    {
                        double labelPosition = label.TransformToVisual(this.scale).TransformPoint(new Point(0, 0)).Y;
                        Assert.IsTrue(ticksYPosition > labelPosition);
                    }
                });
        }

        [TestMethod]
        public void ScaleVisualTests_AssertTickPlacement()
        {
            this.scale.Minimum = 0;
            this.scale.Maximum = 10;
            double ticksYPosition = 0;
            double linePosition = 0;
            double tickHeight = 0;

            Style lineStyle = new Style(typeof(Rectangle));
            lineStyle.Setters.Add(new Setter(Rectangle.FillProperty, new SolidColorBrush(Colors.White)));
            this.scale.LineStyle = lineStyle;

            this.CreateAsyncTest(this.scale, () =>
            {
                Rectangle line = ElementTreeHelper.EnumVisualDescendants<Rectangle>(this.scale)
                    .Where(x => (x is Rectangle) && x.Name == "PART_Line")
                    .FirstOrDefault();

                List<Rectangle> ticks = ElementTreeHelper.EnumVisualDescendants<Rectangle>(this.scale).
                    Where(x => (x is Rectangle) && x.Name != "PART_Line").ToList();

                tickHeight = ticks[0].ActualHeight;
                ticksYPosition = ticks[0].TransformToVisual(this.scale).TransformPoint(new Point(0, 0)).Y;
                linePosition = line.TransformToVisual(this.scale).TransformPoint(new Point(0, 0)).Y;

                Assert.IsTrue(ticksYPosition > linePosition);
                this.scale.TickPlacement = ScaleElementPlacement.TopLeft;
            }, () =>
                {
                    Rectangle line = ElementTreeHelper.EnumVisualDescendants<Rectangle>(this.scale)
                        .Where(x => (x is Rectangle) && x.Name == "PART_Line")
                        .FirstOrDefault();

                    List<Rectangle> ticks = ElementTreeHelper.EnumVisualDescendants<Rectangle>(this.scale)
                        .Where(x => (x is Rectangle) && x.Name != "PART_Line")
                        .ToList();

                    ticksYPosition = ticks[0].TransformToVisual(this.scale).TransformPoint(new Point(0, 0)).Y;
                    linePosition = line.TransformToVisual(this.scale).TransformPoint(new Point(0, 0)).Y;

                    Assert.IsTrue(ticksYPosition < linePosition);
                    this.scale.TickPlacement = ScaleElementPlacement.Center;
                }, () =>
                    {
                        Rectangle line = ElementTreeHelper.EnumVisualDescendants<Rectangle>(this.scale)
                                               .Where(x => (x is Rectangle) && x.Name == "PART_Line")
                                               .FirstOrDefault();

                        linePosition = line.TransformToVisual(this.scale).TransformPoint(new Point(0, 0)).Y;

                        Assert.AreEqual(linePosition, Math.Round(tickHeight / 2));
                    });
        }

        [Ignore]
        [Tag("IGNORED_FAILED")]
        [TestMethod]
        public void ScaleVisualTests_AssertTicksCount_WhenRangeAndTickFrequencyChanged()
        {
            this.scale.Maximum = 50;
            this.scale.Minimum = 0;

            this.CreateAsyncTest(this.scale, () =>
            {
                var ticks = ElementTreeHelper.EnumVisualDescendants<Rectangle>(this.scale).
                    Where(x => (x is Rectangle) && x.Visibility == Visibility.Visible && x.Name != "PART_Line");

                double ticksCount = ((this.scale.Maximum - this.scale.Minimum) / this.scale.TickFrequency) + 1;
                Assert.AreEqual(ticks.Count(), ticksCount);

                this.scale.Minimum = 30;
            }, () =>
                {
                    var ticks = ElementTreeHelper.EnumVisualDescendants<Rectangle>(this.scale)
                        .Where(x => (x is Rectangle) && x.Visibility == Visibility.Visible && x.Name != "PART_Line");

                    double ticksCount = ((this.scale.Maximum - this.scale.Minimum) / this.scale.TickFrequency) + 1;
                    Assert.AreEqual(ticks.Count(), ticksCount);

                    this.scale.Maximum = 50;
                    this.scale.Minimum = 0;
                    this.scale.TickFrequency = 5;

                },
                () =>
                {
                    this.WaitForUpdate();
                }, () =>
                {
                    var ticks = ElementTreeHelper.EnumVisualDescendants<Rectangle>(this.scale)
                       .Where(x => (x is Rectangle) && x.Visibility == Visibility.Visible && x.Name != "PART_Line");

                    double ticksCount = ((this.scale.Maximum - this.scale.Minimum) / this.scale.TickFrequency) + 1;
                    Assert.AreEqual(ticks.Count(), ticksCount);
                });
        }

        [TestMethod]
        public void ScaleVisualTests_TickLength_AssertTickLengthApplied_InBothOrienations()
        {
            this.scale.TickLength = 10;

            this.CreateAsyncTest(this.scale, () =>
            {
                var ticks = ElementTreeHelper.EnumVisualDescendants<Rectangle>(this.scale).
                    Where(x => (x is Rectangle) && x.Visibility == Visibility.Visible && x.Name != "PART_Line");

                Assert.AreEqual(10, this.scale.TickLength);

                foreach (Rectangle tick in ticks)
                {
                    Assert.AreEqual(10, tick.ActualHeight);
                }

                this.scale.TickLength = 50;
            }, () =>
            {
                var ticks = ElementTreeHelper.EnumVisualDescendants<Rectangle>(this.scale).
                   Where(x => (x is Rectangle) && x.Visibility == Visibility.Visible && x.Name != "PART_Line");

                Assert.AreEqual(50, this.scale.TickLength);

                foreach (Rectangle tick in ticks)
                {
                    Assert.AreEqual(50, tick.ActualHeight);
                }

                this.scale.Orientation = Orientation.Vertical;
            }, () =>
                {
                    var ticks = ElementTreeHelper.EnumVisualDescendants<Rectangle>(this.scale).
                 Where(x => (x is Rectangle) && x.Visibility == Visibility.Visible && x.Name != "PART_Line");

                    Assert.AreEqual(50, this.scale.TickLength);

                    foreach (Rectangle tick in ticks)
                    {
                        Assert.AreEqual(50, tick.ActualWidth);
                    }
                });
        }

        [TestMethod]
        public void ScaleVisualTests_TickThickness_AssertTickThicknessApplied_InBothOrienations()
        {
            this.scale.TickThickness = 10;

            this.CreateAsyncTest(this.scale, () =>
            {
                var ticks = ElementTreeHelper.EnumVisualDescendants<Rectangle>(this.scale).
                    Where(x => (x is Rectangle) && x.Visibility == Visibility.Visible && x.Name != "PART_Line");

                Assert.AreEqual(10, this.scale.TickThickness);

                foreach (Rectangle tick in ticks)
                {
                    Assert.AreEqual(10, tick.ActualWidth);
                }

                this.scale.TickThickness = 50;
            }, () =>
            {
                var ticks = ElementTreeHelper.EnumVisualDescendants<Rectangle>(this.scale).
                   Where(x => (x is Rectangle) && x.Visibility == Visibility.Visible && x.Name != "PART_Line");

                Assert.AreEqual(50, this.scale.TickThickness);

                foreach (Rectangle tick in ticks)
                {
                    Assert.AreEqual(50, tick.ActualWidth);
                }

                this.scale.Orientation = Orientation.Vertical;
            }, () =>
            {
                var ticks = ElementTreeHelper.EnumVisualDescendants<Rectangle>(this.scale).
             Where(x => (x is Rectangle) && x.Visibility == Visibility.Visible && x.Name != "PART_Line");

                Assert.AreEqual(50, this.scale.TickThickness);

                foreach (Rectangle tick in ticks)
                {
                    Assert.AreEqual(50, tick.ActualHeight);
                }
            });
        }

        [TestMethod]
        public void ScaleVisualTests_CustomTickStyle_AssertTicksStyleApplied()
        {
            Style tickStyle = new Style(typeof(Rectangle));
            tickStyle.Setters.Add(new Setter(Rectangle.FillProperty, new SolidColorBrush(Colors.Red)));
            tickStyle.Setters.Add(new Setter(Rectangle.StrokeThicknessProperty, 4));
            tickStyle.Setters.Add(new Setter(Rectangle.StrokeProperty, new SolidColorBrush(Colors.Yellow)));

            this.scale.TickStyle = tickStyle;

            this.CreateAsyncTest(this.scale, () =>
            {
                var ticks = ElementTreeHelper.EnumVisualDescendants<Rectangle>(this.scale).
                    Where(x => (x is Rectangle) && x.Visibility == Visibility.Visible && x.Name != "PART_Line");

                double ticksCount = ((this.scale.Maximum - this.scale.Minimum) / this.scale.TickFrequency) + 1;

                foreach (Rectangle tick in ticks)
                {
                    SolidColorBrush desiredFillColor = (VisualTestsHelper.GetStyleSetterPropertyValue(tickStyle, Rectangle.FillProperty)) as SolidColorBrush;
                    SolidColorBrush strokeColor = (VisualTestsHelper.GetStyleSetterPropertyValue(tickStyle, Rectangle.StrokeProperty)) as SolidColorBrush;
                    double strokeThickness = Convert.ToDouble((VisualTestsHelper.GetStyleSetterPropertyValue(tickStyle, Rectangle.StrokeThicknessProperty)));

                    Assert.AreEqual((tick.Fill as SolidColorBrush).Color, desiredFillColor.Color);
                    Assert.AreEqual((tick.Stroke as SolidColorBrush).Color, strokeColor.Color);
                    Assert.AreEqual(tick.StrokeThickness, strokeThickness);
                }
            });
        }

        [TestMethod]
        public void ScaleVisualTests_CustomLabelStyle_AssertLabelStyleApplied()
        {
            Style labelStyle = new Style(typeof(TextBlock));
            labelStyle.Setters.Add(new Setter(TextBlock.ForegroundProperty, new SolidColorBrush(Colors.Red)));
            labelStyle.Setters.Add(new Setter(TextBlock.FontSizeProperty, 20));

            this.scale.LabelStyle = labelStyle;

            this.CreateAsyncTest(this.scale, () =>
            {
                var labels = ElementTreeHelper.EnumVisualDescendants<TextBlock>(this.scale).
                    Where(x => (x is TextBlock));

                foreach (TextBlock label in labels)
                {
                    SolidColorBrush desiredForegroundColor = (VisualTestsHelper.GetStyleSetterPropertyValue(labelStyle, TextBlock.ForegroundProperty)) as SolidColorBrush;

                    double fontSize = Convert.ToDouble((VisualTestsHelper.GetStyleSetterPropertyValue(labelStyle, TextBlock.FontSizeProperty)));

                    Assert.AreEqual(label.FontSize, fontSize);
                }
            });
        }

        [TestMethod]
        public void ScaleVisualTests_CustomLineStyle_AssertLineStyleApplied()
        {
            Style lineStyle = new Style(typeof(Rectangle));
            lineStyle.Setters.Add(new Setter(Rectangle.FillProperty, new SolidColorBrush(Colors.Red)));

            this.scale.LineStyle = lineStyle;

            this.CreateAsyncTest(this.scale, () =>
            {
                var line = ElementTreeHelper.EnumVisualDescendants<Rectangle>(this.scale).
                       Where(x => (x is Rectangle) && x.Visibility == Visibility.Visible && x.Name == "PART_Line").FirstOrDefault();

                SolidColorBrush desiredFillColor = (VisualTestsHelper.GetStyleSetterPropertyValue(lineStyle, Rectangle.FillProperty)) as SolidColorBrush;

                Assert.AreEqual((line.Fill as SolidColorBrush).Color, desiredFillColor.Color);

            });
        }

        [TestMethod]
        public void ScaleVisualTests_CustomTickTemplate_AssertTickTemplateApplied()
        {
            var template = XamlReader.Load(@" 
            <DataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""> 
                <Rectangle Fill=""Yellow"" Height=""1"" Width=""50"" Tag=""1""/> 
            </DataTemplate> 
            ") as DataTemplate;

            this.scale.TickTemplate = template;
            this.scale.Orientation = Orientation.Vertical;

            this.CreateAsyncTest(this.scale, () =>
            {
                var ticks = ElementTreeHelper.EnumVisualDescendants<Rectangle>(this.scale).
                  Where(x => (x is Rectangle) && x.Visibility == Visibility.Visible && x.Name != "PART_Line");

                Assert.IsNotNull(this.scale.TickTemplate);

                foreach (Rectangle tick in ticks)
                {
                    SolidColorBrush desiredFillColor = new SolidColorBrush(Colors.Yellow);

                    Assert.AreEqual((tick.Fill as SolidColorBrush).Color, desiredFillColor.Color);
                    Assert.AreEqual(tick.Width, 50);
                    Assert.AreEqual(tick.Height, 1);
                }
            });
        }

        [TestMethod]
        public void ScaleVisualTests_CustomLabelTemplate_AssertLabelTemplateApplied()
        {
            var template = XamlReader.Load(@" 
            <DataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""> 
                <Ellipse Fill=""Yellow"" Height=""10"" Width=""10"" Tag=""1""/> 
            </DataTemplate> 
            ") as DataTemplate;

            this.scale.LabelTemplate = template;
            this.scale.Orientation = Orientation.Vertical;

            this.CreateAsyncTest(this.scale, () =>
            {
                var labels = ElementTreeHelper.EnumVisualDescendants<Ellipse>(this.scale).
                    Where(x => (x is Ellipse));

                Assert.IsNotNull(this.scale.LabelTemplate);

                foreach (Ellipse label in labels)
                {
                    SolidColorBrush desiredFillColor = new SolidColorBrush(Colors.Yellow);

                    Assert.AreEqual((label.Fill as SolidColorBrush).Color, desiredFillColor.Color);
                    Assert.AreEqual(label.Width, 10);
                    Assert.AreEqual(label.Height, 10);
                }
            });
        }

        public void AssertDefaultTickStyle(Rectangle tick)
        {
            Assert.AreEqual(tick.ActualHeight, 5);
            Assert.AreEqual(tick.ActualWidth, 1);
            Assert.AreEqual((tick.Fill as SolidColorBrush).Color, new SolidColorBrush(Color.FromArgb(0x59, 0xFF, 0xFF, 0xFF)).Color);
        }
    }
}
