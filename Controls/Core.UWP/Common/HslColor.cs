using System;
using System.Linq;
using Windows.UI;

namespace Telerik.Core
{
    /// <summary>
    /// Converts colors from RGB to HSL and vice versa.
    /// </summary>
    internal struct HslColor
    {
        public double H;
        public double S;
        public double L;

        /// <summary>
        /// Creates a HSLColor based on an integer containing color information in the rgba format.
        /// </summary>
        /// <param name="rgba">The color in rgba.</param>
        /// <returns>Returns a new instance of HslColor.</returns>
        public static HslColor Parse(int rgba)
        {
            return HslColor.Parse(BitConverter.GetBytes(rgba));
        }

        /// <summary>
        /// Creates a HSLColor based on an integer containing color information in the rgba format.
        /// </summary>
        /// <param name="color">The color in rgba.</param>
        /// <returns>Returns a new instance of HslColor.</returns>
        public static HslColor Parse(Color color)
        {
            return HslColor.Parse(new byte[] { color.R, color.G, color.B, color.A });
        }

        /// <summary>
        /// Creates a HSLColor based on byte array containing color information in the rgba format.
        /// </summary>
        /// <param name="rgba">The color in rgba.</param>
        /// <returns>Returns a new instance of HslColor.</returns>
        public static HslColor Parse(byte[] rgba)
        {
            double redPrime = rgba[0] / 255.0;
            double greenPrime = rgba[1] / 255.0;
            double bluePrime = rgba[2] / 255.0;

            double max = Math.Max(redPrime, Math.Max(greenPrime, bluePrime));
            double min = Math.Min(redPrime, Math.Min(greenPrime, bluePrime));
            double delta = max - min;

            double lightness = (max + min) / 2;
            double saturation = delta == 0 ? 0 : delta / (1 - Math.Abs(2 * lightness - 1));
            double hue = 0;

            if (delta != 0)
            {
                if (max == redPrime)
                {
                    hue = ((greenPrime - bluePrime) / delta) % 6;
                }

                if (max == greenPrime)
                {
                    hue = (bluePrime - redPrime) / delta + 2;
                }

                if (max == bluePrime)
                {
                    hue = (redPrime - greenPrime) / delta + 4;
                }

                hue *= 60;

                if (hue < 0)
                {
                    hue = 360 + hue;
                }
            }

            return new HslColor() { H = hue, S = saturation, L = lightness };
        }

        /// <summary>
        /// Converts this HslColor instance to <see cref="System.Int32"/> in the rgba format.
        /// </summary>
        /// <returns>Returns an integer.</returns>
        public int ToInt()
        {
            byte[] bytes = this.ToRGB();
            return bytes[0] | (bytes[1] << 8) | (bytes[2] << 16) | (bytes[3] << 24);
        }

        /// <summary>
        /// Converts this HslColor to Color.
        /// </summary>
        /// <returns>Returns a color object.</returns>
        public Color ToColor()
        {
            byte[] bytes = this.ToRGB();
            return Color.FromArgb(bytes[3], bytes[0], bytes[1], bytes[2]);
        }

        /// <summary>
        /// Converts this HslColor instance to a byte array in the rgba format.
        /// </summary>
        /// <returns>An byte array.</returns>
        public byte[] ToRGB()
        {
            double chroma = (1 - Math.Abs(2 * this.L - 1)) * this.S;

            double huePrime = this.H / 60;

            double x = chroma * (1 - Math.Abs(huePrime % 2 - 1));

            double r1 = 0;
            double g1 = 0;
            double b1 = 0;

            if (huePrime >= 0 && huePrime < 1)
            {
                r1 = chroma;
                g1 = x;
            }

            if (huePrime >= 1 && huePrime < 2)
            {
                r1 = x;
                g1 = chroma;
            }

            if (huePrime >= 2 && huePrime < 3)
            {
                g1 = chroma;
                b1 = x;
            }

            if (huePrime >= 3 && huePrime < 4)
            {
                g1 = x;
                b1 = chroma;
            }

            if (huePrime >= 4 && huePrime < 5)
            {
                r1 = x;
                b1 = chroma;
            }

            if (huePrime >= 5 && huePrime < 6)
            {
                r1 = chroma;
                b1 = x;
            }

            double m = this.L - chroma / 2;

            byte[] result = new byte[]
            {
                (byte)((r1 + m) * 255),
                (byte)((g1 + m) * 255),
                (byte)((b1 + m) * 255),
                255
            };

            return result;
        }
    }
}