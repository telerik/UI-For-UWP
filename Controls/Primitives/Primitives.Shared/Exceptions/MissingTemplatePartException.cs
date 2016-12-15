using System;

namespace Telerik.UI.Xaml.Controls
{
    /// <summary>
    /// Represents an exception raised when a template part, as defined by the template contract, is missing during template initialization.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors", Justification = "No need of standard constructors.")]
    public class MissingTemplatePartException : Exception
    {
        private string partName;
        private Type partType;

        /// <summary>
        /// Initializes a new instance of the <see cref="MissingTemplatePartException"/> class.
        /// </summary>
        /// <param name="name">The name of the missing part.</param>
        /// <param name="type">The type of the missing part.</param>
        public MissingTemplatePartException(string name, Type type)
        {
            this.partName = name;
            this.partType = type;
        }

        /// <summary>
        /// Gets the name of the missing part.
        /// </summary>
        public string PartName
        {
            get
            {
                return this.partName;
            }
        }

        /// <summary>
        /// Gets the type of the missing part.
        /// </summary>
        public Type PartType
        {
            get
            {
                return this.partType;
            }
        }
    }
}
