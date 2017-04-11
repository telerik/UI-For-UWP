using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Input.Tests.RangeSlider
{
    public static class VisualTestsHelper
    {
        public static void AssertSelectionThumbs_DefaultStyle(Thumb thumb)
        {
            Assert.IsNotNull(thumb);

            if (thumb.Name == "PART_SelectionMiddleThumb")
            {
                Assert.AreEqual((thumb.Background as SolidColorBrush).Color, new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0x6A, 0xC1)).Color);
            }
            else
            {
                Assert.AreEqual((thumb.Background as SolidColorBrush).Color, new SolidColorBrush(Colors.White).Color);
                Assert.AreEqual(thumb.Width, 11);
                Assert.AreEqual(thumb.Height, 11);
            }

            Assert.AreEqual(thumb.BorderThickness, new Thickness(0));
            Assert.AreEqual(thumb.Margin, new Thickness(0));
        }



        public static void AssertSelectionThumbs_CustomStyle(Thumb thumb, Style style)
        {
            Assert.IsNotNull(thumb);

            SolidColorBrush desiredColor = (GetStyleSetterPropertyValue(style, Thumb.BackgroundProperty)) as SolidColorBrush;
            double desiredWidth = Convert.ToDouble((GetStyleSetterPropertyValue(style, Thumb.WidthProperty)));
            double desiredHeight = Convert.ToDouble((GetStyleSetterPropertyValue(style, Thumb.HeightProperty)));

            Assert.AreEqual((thumb.Background as SolidColorBrush).Color, desiredColor.Color);
            Assert.AreEqual(thumb.Height, desiredHeight);

            if (thumb.Name != "PART_SelectionMiddleThumb")
            {
                Assert.AreEqual(thumb.Width, desiredWidth);  
            }
        }

        public static object GetStyleSetterPropertyValue(Style style, DependencyProperty property)
        {
            if (style == null)
            {
                return null;
            }

            foreach (Setter setter in style.Setters)
            {
                if (setter.Property == property)
                {
                    return setter.Value;
                }
            }

            return null;
        }
    }
}
