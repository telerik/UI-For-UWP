using System.Text;

namespace Telerik.Geospatial
{
    internal static class DbfEncoding
    {
        /// <summary>
        /// OEM United States.
        /// </summary>
        private const string OemUnitedStates = "IBM437";

        /// <summary>
        /// Western European (DOS).
        /// </summary>
        private const string WesternEuropeanDos = "ibm850";

        /// <summary>
        /// Western European (Windows).
        /// </summary>
        private const string WesternEuropeanWindows = "Windows-1252";

        /// <summary>
        /// Western European (Mac).
        /// </summary>
        private const string WesternEuropeanMac = "macintosh";

        /// <summary>
        /// Central European (DOS).
        /// </summary>
        private const string CentralEuropeanDos = "ibm852";

        /// <summary>
        /// Nordic (DOS).
        /// </summary>
        private const string NordicDos = "IBM865";

        /// <summary>
        /// Cyrillic (DOS).
        /// </summary>
        private const string CyrillicDos = "cp866";

        /// <summary>
        /// Icelandic (DOS).
        /// </summary>
        private const string IcelandicDos = "ibm861";

        /// <summary>
        /// Greek (DOS).
        /// </summary>
        private const string GreekDos = "ibm737";

        /// <summary>
        /// Turkish (DOS).
        /// </summary>
        private const string TurkishDos = "ibm857";

        /// <summary>
        /// Cyrillic (Mac).
        /// </summary>
        private const string CyrillicMac = "x-mac-cyrillic";

        /// <summary>
        /// Central European (Mac).
        /// </summary>
        private const string CentralEuropeanMac = "x-mac-ce";

        /// <summary>
        /// Greek (Mac).
        /// </summary>
        private const string GreekMac = "x-mac-greek";

        /// <summary>
        /// Central European (Windows).
        /// </summary>
        private const string CentralEuropeanWindows = "windows-1250";

        /// <summary>
        /// Cyrillic (Windows).
        /// </summary>
        private const string CyrillicWindows = "windows-1251";

        /// <summary>
        /// Turkish (Windows).
        /// </summary>
        private const string TurkishWindows = "windows-1254";

        /// <summary>
        /// Greek (Windows).
        /// </summary>
        private const string GreekWindows = "windows-1253";

        public static Encoding GetEncoding(byte languageDriver)
        {
            Encoding encoding = Encoding.UTF8;

            switch (languageDriver)
            {
                case 0x01:
                case 0x69: /* Mazovia encoding (Polish MS-DOS) uses IBM's code page 437 with some positions filled with Polish characters */
                    encoding = Encoding.GetEncoding(DbfEncoding.OemUnitedStates);
                    break;
                case 0x02:
                    encoding = Encoding.GetEncoding(DbfEncoding.WesternEuropeanDos);
                    break;
                case 0x03:
                    encoding = Encoding.GetEncoding(DbfEncoding.WesternEuropeanWindows);
                    break;
                case 0x04:
                    encoding = Encoding.GetEncoding(DbfEncoding.WesternEuropeanMac);
                    break;
                case 0x64:
                case 0x68: /* Kamenický encoding (Czech and Slovak MS-DOS) uses IBM's code page 852 */
                    encoding = Encoding.GetEncoding(DbfEncoding.CentralEuropeanDos);
                    break;
                case 0x65:
                    encoding = Encoding.GetEncoding(DbfEncoding.NordicDos);
                    break;
                case 0x66:
                    encoding = Encoding.GetEncoding(DbfEncoding.CyrillicDos);
                    break;
                case 0x67:
                    encoding = Encoding.GetEncoding(DbfEncoding.IcelandicDos);
                    break;
                case 0x6A:
                    encoding = Encoding.GetEncoding(DbfEncoding.GreekDos);
                    break;
                case 0x6B:
                    encoding = Encoding.GetEncoding(DbfEncoding.TurkishDos);
                    break;
                case 0x96:
                    encoding = Encoding.GetEncoding(DbfEncoding.CyrillicMac);
                    break;
                case 0x97:
                    encoding = Encoding.GetEncoding(DbfEncoding.CentralEuropeanMac);
                    break;
                case 0x98:
                    encoding = Encoding.GetEncoding(DbfEncoding.GreekMac);
                    break;
                case 0xC8:
                    encoding = Encoding.GetEncoding(DbfEncoding.CentralEuropeanWindows);
                    break;
                case 0xC9:
                    encoding = Encoding.GetEncoding(DbfEncoding.CyrillicWindows);
                    break;
                case 0xCA:
                    encoding = Encoding.GetEncoding(DbfEncoding.TurkishWindows);
                    break;
                case 0xCB:
                    encoding = Encoding.GetEncoding(DbfEncoding.GreekWindows);
                    break;
            }

            return encoding;
        }
    }
}
