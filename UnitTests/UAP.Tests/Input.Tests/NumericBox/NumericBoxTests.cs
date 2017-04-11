using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using Telerik.UI.Xaml.Controls.Primitives;
using Telerik.UI.Xaml.Controls.Primitives.Common;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using WinRT.Testing;

namespace Telerik.UI.Xaml.Controls.Input.Tests
{
    [TestClass]
    [Tag("Input")]
    [Tag("NumericBox")]
    public class NumericBoxTests : RadControlUITest
    {
        private RadNumericBox box;

        public override void TestInitialize()
        {
            base.TestInitialize();

            this.box = new RadNumericBox();
        }

        [TestMethod]
        public void Test_DefaultValues()
        {
            Assert.IsTrue(this.box.ValueFormat == "{0,0:N2}");
            Assert.IsTrue(this.box.ValueString == null);
            Assert.IsTrue(this.box.IncreaseButtonStyle == null);
            Assert.IsTrue(this.box.DecreaseButtonStyle == null);
            Assert.IsTrue(this.box.Watermark == null);
            Assert.IsTrue(this.box.IsEditable);
            Assert.IsTrue(this.box.ButtonsVisibility == Visibility.Visible);
           
            this.CreateAsyncTest(this.box, () =>
              {
                  Assert.IsTrue(this.box.Value == null);
                  Assert.IsTrue(this.box.Minimum == 0);
                  Assert.IsTrue(this.box.Maximum == 100);
                  Assert.IsTrue(this.box.SmallChange == 1);
                  Assert.IsTrue(this.box.LargeChange == 10);

              });
        }

        [TestMethod]
        public void Test_ValueClamp_InRange()
        {
            this.box.Value = -5;
            this.CreateAsyncTest(this.box, () =>
                {
                    Assert.IsTrue(this.box.Value == 0);

                    this.box.Minimum = 10;
                }, () =>
                    {
                        Assert.IsTrue(this.box.Value == 10);

                        this.box.Minimum = -10;
                        this.box.Maximum = 5;
                    }, () =>
                 {
                     Assert.IsTrue(this.box.Value == 5);

                     this.box.Minimum = 10;
                 }, () =>
                {
                    Assert.IsTrue(this.box.Minimum == 5);
                });
        }

        [TestMethod]
        public void Test_ValueChanged()
        {
            int changeCount = 0;
            this.box.ValueChanged += (s, e) =>
                {
                    changeCount++;
                };

            this.CreateAsyncTest(this.box, () =>
            {
                this.box.Value = 10;
                Assert.IsTrue(changeCount == 1);

                this.box.Value = 120;
                Assert.IsTrue(this.box.Value == 100);
                Assert.IsTrue(changeCount == 2);

                // change the value to a bigger than the Maximum number, ValueChanged should not be raised
                this.box.Value = 150;
                Assert.IsTrue(this.box.Value == 100);
                Assert.IsTrue(changeCount == 2);

                this.box.Value = -10;
                Assert.IsTrue(this.box.Value == 0);
                Assert.IsTrue(changeCount == 3);

                // change the value to a smaller than the Minimum number, ValueChanged should not be raised
                this.box.Value = -100;
                Assert.IsTrue(this.box.Value == 0);
                Assert.IsTrue(changeCount == 3);
            });
        }

        [TestMethod]
        public void Test_CultureSupport()
        {
            CultureService.SetCultureName(this.box, "de-DE");
            this.box.Value = 10;

            this.CreateAsyncTest(this.box, () =>
            {
                ICultureAware cultureAware = this.box as ICultureAware;
                Assert.IsTrue(cultureAware.CurrentCulture.Name == "de-DE");
                Assert.IsTrue(this.box.ValueString == "10,00");
            });
        }

        [TestMethod]
        public void Test_VisualStates_Watermark()
        {
            this.box.Watermark = "No value";
            this.CreateAsyncTest(this.box, () =>
            {
                Assert.IsTrue(this.box.CurrentVisualState == "WatermarkVisible");
            });
        }

        [TestMethod]
        public void Test_VisualStates_NoWatermark()
        {
            this.box.Value = 10;

            this.CreateAsyncTest(this.box,
                () => { },
                () =>
                {

                    Assert.IsTrue(this.box.CurrentVisualState == "WatermarkHidden");
                });
        }

        [TestMethod]
        public void Test_DecreaseButtonStyle()
        {
            this.box.Value = 10;
            Style style = new Style(typeof(InlineButton));
            style.Setters.Add(new Setter(InlineButton.BackgroundProperty, new SolidColorBrush(Colors.Red)));

            this.box.DecreaseButtonStyle = style;

            this.CreateAsyncTest(this.box, () =>
            {
                Assert.IsTrue(this.box.DecreaseButtonStyle == style);
                var brush = this.box.DecreaseButton.Background as SolidColorBrush;
                Assert.IsNotNull(brush);
                Assert.IsTrue(brush.Color == Colors.Red);
            });
        }

        [TestMethod]
        public void Test_IncreaseButtonStyle()
        {
            this.box.Value = 10;
            Style style = new Style(typeof(InlineButton));
            style.Setters.Add(new Setter(InlineButton.BackgroundProperty, new SolidColorBrush(Colors.Green)));

            this.box.IncreaseButtonStyle = style;

            this.CreateAsyncTest(this.box, () =>
            {
                Assert.IsTrue(this.box.IncreaseButtonStyle == style);
                var brush = this.box.IncreaseButton.Background as SolidColorBrush;
                Assert.IsNotNull(brush);
                Assert.IsTrue(brush.Color == Colors.Green);
            });
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Test_ValueString_IsReadonly()
        {
            this.box.SetValue(RadNumericBox.ValueStringProperty, "Test");
        }

        [TestMethod]
        public void Test_ValueFormat()
        {
            this.box.Value = 10;
            this.box.ValueFormat = "{0,0:C2}";

            this.CreateAsyncTest(this.box, () =>
            {
                Assert.IsTrue(this.box.ValueString == "$10.00");

                this.box.ValueFormat = "{0,0:C0}";
                Assert.IsTrue(this.box.ValueString == "$10");
            });
        }

        [TestMethod]
        public void Test_ButtonsVisibility()
        {
            this.box.ButtonsVisibility = Visibility.Collapsed;

            this.CreateAsyncTest(this.box, () =>
            {
                Assert.IsTrue(this.box.DecreaseButton.Visibility == Visibility.Collapsed);
                Assert.IsTrue(this.box.IncreaseButton.Visibility == Visibility.Collapsed);
            });
        }

        [TestMethod]
        public void Test_BeginEdit_CommitEdit()
        {
            this.box.Value = 10;

            this.CreateAsyncTest(this.box, () =>
            {
                Assert.IsTrue(this.box.ValueString == "10.00");

                this.box.BeginEdit();
                Assert.IsTrue(this.box.ValueString == "10");

                this.box.CommitEdit();
                Assert.IsTrue(this.box.ValueString == "10.00");
            });
        }

        [TestMethod]
        public void Test_PreviewKeyDown()
        {
            this.CreateAsyncTest(this.box, () =>
                {
                    Assert.IsFalse(this.box.PreviewKeyDown(VirtualKey.A));
                    Assert.IsFalse(this.box.PreviewKeyDown(VirtualKey.F1));
                    Assert.IsFalse(this.box.PreviewKeyDown(VirtualKey.Decimal));
                    Assert.IsFalse(this.box.PreviewKeyDown(VirtualKey.Down));
                    Assert.IsFalse(this.box.PreviewKeyDown(VirtualKey.Enter));
                    Assert.IsTrue(this.box.PreviewKeyDown(VirtualKey.Number0));
                });
        }

        [TestMethod]
        public void Test_IsEditable()
        {
            this.box.IsEditable = false;

            this.CreateAsyncTest(this.box, () =>
            {
                Assert.IsTrue(this.box.TextBox.IsReadOnly);

                this.box.IsEditable = true;
                Assert.IsFalse(this.box.TextBox.IsReadOnly);
            });
        }
    }
}
