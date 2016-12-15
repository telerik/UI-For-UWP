using System;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Provides an attribute that lets you specify display options for members of entity classes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DisplayAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayAttribute"/> class.
        /// </summary>
        public DisplayAttribute(string header = null, string group = null, int position = 0, string placeholderText = null)
        {
            this.Header = header;
            this.Group = group;
            this.PlaceholderText = placeholderText;
            this.Position = position;
        }

        /// <summary>
        /// Gets or sets a value that is used for display in the UI.
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// Gets or sets a value that is used to group fields in the UI.
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// Gets or sets a value that is used to position the element in the UI.
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        ///  Gets or sets a value that is used to set the watermark for prompts in the UI.
        /// </summary>
        public string PlaceholderText { get; set; }
    }
}