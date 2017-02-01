using System;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Defines the possible casing modes to be applied over a System.String object.
    /// </summary>
    public enum StringCaseMode
    {
        /// <summary>
        /// No casing is applied - the string is left in its original form.
        /// </summary>
        Default,

        /// <summary>
        /// All characters are converted to upper case.
        /// </summary>
        ToUpper,

        /// <summary>
        /// All characters are converted to lower case.
        /// </summary>
        ToLower,
    }
}
