using System;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Provides an attribute that lets you specify value options for the attributed members of entity class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ValueOptionsPropertyNameAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValueOptionsPropertyNameAttribute"/> class.
        /// </summary>
        /// <param name="propertyName">The name of the member that provides value options.</param>
        public ValueOptionsPropertyNameAttribute(string propertyName)
        {
            this.PropertyName = propertyName;
        }

        /// <summary>
        /// Gets or sets the name of the member that provides value options.
        /// </summary>
        public string PropertyName { get; set; }
    }
}