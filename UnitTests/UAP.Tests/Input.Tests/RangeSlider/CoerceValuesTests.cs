using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinRT.Testing;

namespace Telerik.UI.Xaml.Controls.Input.Tests.RangeSlider
{
    [TestClass]
    [Tag("Input")]
    [Tag("RangeSlider")]
    public class CoerceValuesTests : RadControlUITest
    {
        private RadRangeSlider slider;

        public override void TestInitialize()
        {
            base.TestInitialize();

            this.slider = new RadRangeSlider();
        }

        [TestMethod]
        public void CoerceValues_SetBothMaximumAndMinimum_AssertMaximumCoercedWithNewMinimum()
        {
            this.slider.Maximum = 50;
            this.slider.Minimum = 45;

            this.CreateAsyncTest(this.slider, () =>
            {
                this.slider.Maximum = 20;
                this.slider.Minimum = 10;

                Assert.AreEqual<double>(this.slider.Maximum, 20, "Maximum coerced with Minimum before new value of Minimum is set");
                Assert.AreEqual<double>(this.slider.Minimum, 10);

                this.slider.Maximum = 30;
                this.slider.Minimum = 40;
            }, () =>
                {
                    Assert.AreEqual<double>(this.slider.Maximum, 40, "Maximum not coerced with the new value of Minimum");
                    Assert.AreEqual<double>(this.slider.Minimum, 40);
                });
        }

        [TestMethod]
        public void CoerceValues_SetBothMaximumAndMinimum_AssertMinimumCoercedWithNewMaximum()
        {
            this.slider.Maximum = 50;
            this.slider.Minimum = 45;

            this.CreateAsyncTest(this.slider, () =>
            {
                this.slider.Minimum = 60;
                this.slider.Maximum = 70;

                Assert.AreEqual<double>(this.slider.Minimum, 60, "Minimum coerced with Maximum before new value of Maximum is set");
                Assert.AreEqual<double>(this.slider.Maximum, 70);

                this.slider.Minimum = 100;
                this.slider.Maximum = 80;

            }, () =>
            {
                Assert.AreEqual<double>(this.slider.Maximum, 80);
                Assert.AreEqual<double>(this.slider.Minimum, 80, "Minimum is not coerced with the new value of Maximum");
            });
        }

        [TestMethod]
        public void CoerceValues_MaximumSmallerThanMinimum()
        {
            this.slider.Minimum = 100;
            this.slider.Maximum = 0;

            this.CreateAsyncTest(this.slider, () =>
              {
                  Assert.AreEqual(this.slider.Maximum, 100);
                  Assert.AreEqual(this.slider.Minimum, 100);

                  this.slider.Minimum = 200;
              }, () =>
              {
                  Assert.AreEqual(this.slider.Maximum, 100);
                  Assert.AreEqual(this.slider.Minimum, 100);

                  this.slider.Minimum = 20;
              }, () =>
                  {
                      Assert.AreEqual(this.slider.Maximum, 100);
                      Assert.AreEqual(this.slider.Minimum, 20);
                  });
        }

        [TestMethod]
        public void CoerceValues_MinimumGreaterThanMaximum()
        {
            this.slider.Maximum = 0;
            this.slider.Minimum = 100;

            this.CreateAsyncTest(this.slider, () =>
            {
                Assert.AreEqual(this.slider.Maximum, 0);
                Assert.AreEqual(this.slider.Minimum, 0);

                this.slider.Minimum = 20;
            }, () =>
            {
                Assert.AreEqual(this.slider.Maximum, 0);
                Assert.AreEqual(this.slider.Minimum, 0);

                this.slider.Maximum = 20;
            }, () =>
            {
                Assert.AreEqual(this.slider.Maximum, 20);
                Assert.AreEqual(this.slider.Minimum, 0);
            });
        }

        [TestMethod]
        public void CoerceValues_MaximumAndMinum_EqualToZero_SelectionRangeCoerced()
        {
            this.slider.Minimum = 0;
            this.slider.Maximum = 0;

            this.CreateAsyncTest(this.slider, () =>
            {
                Assert.AreEqual(this.slider.SelectionStart, 0);
                Assert.AreEqual(this.slider.SelectionEnd, 0);

                this.slider.SelectionEnd = 10;
                Assert.AreEqual(this.slider.SelectionStart, 0);
                Assert.AreEqual(this.slider.SelectionEnd, 0);
            });
        }

        [TestMethod]
        public void CoerceValues_MinimumChange_CoerceSelectionRangeProperly()
        {
            this.CreateAsyncTest(this.slider, () =>
               {
                   this.slider.Minimum = 5;
                   this.slider.Maximum = 10;
               }, () =>
               {
                   Assert.AreEqual(this.slider.Minimum, 5);
                   Assert.AreEqual(this.slider.SelectionStart, 5);
                   Assert.AreEqual(this.slider.SelectionEnd, 6);

                   this.slider.Minimum = 0;
               }, () =>
               {
                   Assert.AreEqual(this.slider.Minimum, 0);
                   Assert.AreEqual<double>(this.slider.SelectionStart, 4, "SelectionStart value is not coerced to its previous value");
                   Assert.AreEqual(this.slider.SelectionEnd, 6);

                   this.slider.Minimum = 20;
               }, () =>
                   {
                       Assert.AreEqual(this.slider.Maximum, 10);
                       Assert.AreEqual(this.slider.Minimum, this.slider.Maximum);
                       Assert.AreEqual(this.slider.SelectionStart, this.slider.Maximum);
                       Assert.AreEqual(this.slider.SelectionEnd, this.slider.Maximum);
                   });
        }

        [TestMethod]
        public void CoerceValues_MaximumChange_CoerceSelectionRangeProperly()
        {
            this.slider.SelectionEnd = 10;
            this.slider.Maximum = 8;
            this.CreateAsyncTest(this.slider, () =>
            {

                Assert.AreEqual(this.slider.Maximum, 8);
                Assert.AreEqual(this.slider.SelectionStart, 4);
                Assert.AreEqual(this.slider.SelectionEnd, 8);

                this.slider.Maximum = 10;
            }, () =>
            {
                Assert.AreEqual(this.slider.Maximum, 10);
                Assert.AreEqual(this.slider.SelectionStart, 4);
                Assert.AreEqual<double>(this.slider.SelectionEnd, 8, "SelectionEnd value is not coerced to its previous value");

                this.slider.Maximum = -10;
            }, () =>
            {
                Assert.AreEqual(this.slider.Maximum, 0);
                Assert.AreEqual(this.slider.Minimum, this.slider.Maximum);
                Assert.AreEqual(this.slider.SelectionStart, this.slider.Maximum);
                Assert.AreEqual(this.slider.SelectionEnd, this.slider.Maximum);
            });
        }

        [TestMethod]
        public void CoerceValues_SelectionStart_GreaterThanSelectionEnd()
        {
            this.slider.SelectionEnd = 10;
            this.slider.SelectionStart = 15;
            this.slider.Maximum = 10;

            this.CreateAsyncTest(this.slider, () =>
            {
                Assert.AreEqual(this.slider.SelectionStart, 10);
                Assert.AreEqual(this.slider.SelectionEnd, 10);

                this.slider.SelectionStart = 20;

                Assert.AreEqual(this.slider.SelectionStart, 10);
                Assert.AreEqual(this.slider.SelectionEnd, 10);

                this.slider.SelectionStart = 5;
                Assert.AreEqual(this.slider.SelectionStart, 5);
                Assert.AreEqual(this.slider.SelectionEnd, 10);

            });
        }

        [TestMethod]
        public void CoerceValues_SelectionStart_GreaterThanSelectionEnd_AssertSelectionEndUpdated()
        {
            this.slider.SelectionEnd = 10;
            this.slider.SelectionStart = 15;
            this.slider.Maximum = 20;

            this.CreateAsyncTest(this.slider, () =>
            {
                Assert.AreEqual(this.slider.SelectionStart, 15);
                Assert.AreEqual<double>(this.slider.SelectionEnd, 15, "SelectionEnd is not updated when SelectionStart larger");

                this.slider.SelectionStart = 10;
            }, () =>
                {
                    Assert.AreEqual(this.slider.SelectionStart, 10);
                    Assert.AreEqual(this.slider.SelectionEnd, 15);
                });
        }

        [TestMethod]
        public void CoerceValues_SelectionStart_GreaterThanSelectionEnd_AssertSelectionEnd_NotCoercedToPreviousValue()
        {
            this.slider.SelectionStart = 80;
            this.slider.SelectionEnd = 70;

            this.CreateAsyncTest(this.slider, () =>
            {
                Assert.AreEqual(this.slider.SelectionStart, 80);
                Assert.AreEqual<double>(this.slider.SelectionEnd, 80);

                this.slider.SelectionStart = 50;
            }, () =>
            {
                Assert.AreEqual(this.slider.SelectionStart, 50);
                Assert.AreEqual(this.slider.SelectionEnd, 80);
            });
        }

        [TestMethod]
        public void CoerceValues_SelectionEnd_SmallerThanSelectionStart()
        {
            this.slider.Maximum = 10;
            this.slider.SelectionStart = 5;
            this.slider.SelectionEnd = 4;

            this.CreateAsyncTest(this.slider, () =>
            {
                Assert.AreEqual(this.slider.SelectionStart, 5);
                Assert.AreEqual(this.slider.SelectionEnd, 5);

                this.slider.SelectionStart = 20;
                Assert.AreEqual(this.slider.Maximum, 10);
                Assert.AreEqual(this.slider.SelectionStart, this.slider.Maximum);
                Assert.AreEqual(this.slider.SelectionEnd, this.slider.Maximum);

                this.slider.SelectionStart = 5;
                Assert.AreEqual(this.slider.SelectionStart, 5);
                Assert.AreEqual(this.slider.SelectionEnd, 10);

            });
        }

        [TestMethod]
        public void CoerceValues_SelectionStart_LargerThanMaximum()
        {
            this.slider.SelectionStart = 50;
            this.slider.SelectionEnd = 10;
            this.slider.Maximum = 20;

            this.CreateAsyncTest(this.slider, () =>
            {
                Assert.AreEqual(this.slider.SelectionStart, 20);
                Assert.AreEqual(this.slider.SelectionEnd, 20);

                this.slider.SelectionStart = 15;
                Assert.AreEqual(this.slider.Maximum, 20);
                Assert.AreEqual(this.slider.SelectionStart, 15);
                Assert.AreEqual(this.slider.SelectionEnd, this.slider.Maximum);

                this.slider.SelectionStart = 5;
                Assert.AreEqual(this.slider.SelectionStart, 5);
                Assert.AreEqual(this.slider.SelectionEnd, 20, "SelectionEnd coerced to its initial value");

            });
        }

        [TestMethod]
        public void CoerceValues_SelectionEnd_LargerThanMaximum_AssertDesiredValueRestored()
        {
            this.slider.SelectionStart = 5;
            this.slider.SelectionEnd = 15;
            this.slider.Maximum = 10;

            this.CreateAsyncTest(this.slider, () =>
            {
                Assert.AreEqual(this.slider.Maximum, 10);
                Assert.AreEqual(this.slider.SelectionStart, 5);
                Assert.AreEqual(this.slider.SelectionEnd, 10);

                this.slider.Maximum = 20;
            }, () =>
                {
                    Assert.AreEqual(this.slider.Maximum, 20);
                    Assert.AreEqual<double>(this.slider.SelectionEnd, 10, "SelectionEnd is  restored to its desired value after Maximum increased");
                });
        }

        [TestMethod]
        public void CoerceValues_SelectionStart_SmallerThanMinimum_AssertDesiredValueRestored()
        {
            this.slider.SelectionStart = -10;
            this.slider.SelectionEnd = 10;
            this.slider.Minimum = 0;
            this.slider.Maximum = 10;

            this.CreateAsyncTest(this.slider, () =>
            {
                Assert.AreEqual(this.slider.Minimum, 0);
                Assert.AreEqual(this.slider.SelectionStart, 0);
                Assert.AreEqual(this.slider.SelectionEnd, 10);

                this.slider.Minimum = -20;
            }, () =>
            {
                Assert.AreEqual(this.slider.Minimum, -20);
                Assert.AreEqual<double>(this.slider.SelectionStart, -10, "SelectionStart is not restored to its desired value after Minimum increased");
            });
        }

        [TestMethod]
        public void CoerceValues_SelectionEnd_SmallerThanMinimum()
        {
            this.slider.SelectionStart = 5;
            this.slider.SelectionEnd = -10;
            this.slider.Minimum = 0;

            this.CreateAsyncTest(this.slider, () =>
            {
                Assert.AreEqual(this.slider.SelectionStart, 5);
                Assert.AreEqual(this.slider.SelectionEnd, 5);

                this.slider.SelectionEnd = 10;
                Assert.AreEqual(this.slider.SelectionStart, 5);
                Assert.AreEqual(this.slider.SelectionEnd, 10);
            });
        }
    }
}
