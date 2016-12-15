using Windows.UI.Text;

namespace Telerik.UI.Xaml.Controls.Input
{
    internal static class FontWeightNameHelper
    {
        internal static FontWeight GetFontWeight(FontWeightName fontWeightName)
        {
            switch (fontWeightName)
            {
                case FontWeightName.Black:
                    return FontWeights.Black;
                case FontWeightName.Bold:
                    return FontWeights.Bold;
                case FontWeightName.ExtraBlack:
                    return FontWeights.ExtraBlack;
                case FontWeightName.ExtraBold:
                    return FontWeights.ExtraBold;
                case FontWeightName.ExtraLight:
                    return FontWeights.ExtraLight;
                case FontWeightName.Light:
                    return FontWeights.Light;
                case FontWeightName.Medium:
                    return FontWeights.Medium;
                case FontWeightName.SemiBold:
                    return FontWeights.SemiBold;
                case FontWeightName.SemiLight:
                    return FontWeights.SemiLight;
                case FontWeightName.Thin:
                    return FontWeights.Thin;
                default:
                    return FontWeights.Normal;
            }
        }
    }
}
