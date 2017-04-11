using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Telerik.UI.Xaml.Controls.Input.AutoCompleteBox;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Input.Tests.AutoCompleteBox
{
    public class ItemHighlightHelper
    {

        public static void AssertHighlightedRun_InDefaultStyle(Run HighlightRun, bool isItemSelected)
        {
            if (isItemSelected)
            {
                Assert.AreEqual((HighlightRun.Foreground as SolidColorBrush).Color, Colors.White);
            }
            else
            {
                Assert.AreEqual((HighlightRun.Foreground as SolidColorBrush).Color, (Color.FromArgb(0xFF, 0x26, 0xA0, 0xDA)));
            }

            Assert.AreEqual(HighlightRun.FontWeight.Weight, 400);
        }

        public static void AssertDefaultRunStyle(Run HighlightRun, bool isItemSelected)
        {
            if (isItemSelected)
            {
                Assert.AreEqual((HighlightRun.Foreground as SolidColorBrush).Color, Colors.White);
            }
            else
            {
                Assert.AreEqual((HighlightRun.Foreground as SolidColorBrush).Color, Colors.Black);
            }

            Assert.AreEqual(HighlightRun.FontWeight.Weight, 400);
            Assert.AreEqual(HighlightRun.FontSize, 14.6, 0.1);
            Assert.AreEqual(HighlightRun.FontStyle.ToString(), "Normal");
            Assert.AreEqual(HighlightRun.FontFamily.Source, "Segoe UI");
        }

        public static void AssertHighlightedRun_InCustomStyle(Run HighlightRun, HighlightStyle customStyle, bool isItemSelected)
        {
            if (customStyle.Foreground != null)
            {
                if (isItemSelected)
                {
                    Assert.AreNotEqual((HighlightRun.Foreground as SolidColorBrush).Color, (customStyle.Foreground as SolidColorBrush).Color);
                }
                else
                {
                    Assert.AreEqual((HighlightRun.Foreground as SolidColorBrush).Color, (customStyle.Foreground as SolidColorBrush).Color);
                }
            }
            else
            {
                Assert.AreNotEqual((HighlightRun.Foreground as SolidColorBrush).Color, (customStyle.Foreground as SolidColorBrush).Color);
            }

            if (customStyle.ReadLocalValue(HighlightStyle.FontSizeProperty) != DependencyProperty.UnsetValue)
            {
                Assert.AreEqual(HighlightRun.FontSize, customStyle.FontSize);
            }

            if (customStyle.ReadLocalValue(HighlightStyle.FontWeightProperty) != DependencyProperty.UnsetValue)
            {
                Assert.AreEqual(HighlightRun.FontWeight, FontWeightNameHelper.GetFontWeight(customStyle.FontWeight));
            }

            if (customStyle.ReadLocalValue(HighlightStyle.FontStyleProperty) != DependencyProperty.UnsetValue)
            {
                Assert.AreEqual(HighlightRun.FontStyle.ToString(), customStyle.FontStyle.ToString());
            }

            if (customStyle.FontFamily != null)
            {
                Assert.AreEqual(HighlightRun.FontFamily.Source, customStyle.FontFamily.Source);
            }
        }
    }
}
