using System;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Represents a converter that changes the letter case of the provided System.String value.
    /// </summary>
    public class StringCaseConverter : IValueConverter
    {
        /// <summary>
        /// Gets or sets the <see cref="StringCaseMode"/> value that specifies which letter case to be used.
        /// </summary>
        public StringCaseMode CaseMode { get; set; }

        /// <summary>
        /// Converts a string to upper/lower case depending on the value of the <see cref="CaseMode"/> property.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string stringValue = value as string;
            if (stringValue == null)
            {
                return value;
            }

            switch (this.CaseMode)
            {
                case StringCaseMode.ToLower:
                    stringValue = stringValue.ToLower();
                    break;
                case StringCaseMode.ToUpper:
                    stringValue = stringValue.ToUpper();
                    break;
            }

            return stringValue;
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException("ConvertBack is not supported for StringCaseConverter.");
        }
    }
}
