using System;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Provides an attribute that lets you specify value options for the attributed members of entity class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class PropertyValueOptionsAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyValueOptionsAttribute"/> class.
        /// </summary>
        public PropertyValueOptionsAttribute(Type optionsProviderType)
        {
            this.OptionsProvider = optionsProviderType;
        }

        /// <summary>
        /// Gets or sets a provider for the available member values that can be selected from the UI.
        /// </summary>
        public Type OptionsProvider { get; set; }
    }
}