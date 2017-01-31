using System;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Provides data for the <see cref="SettingsNode.SettingsChanged"/> event.
    /// </summary>
    internal class SettingsChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsChangedEventArgs"/> class.
        /// </summary>
        public SettingsChangedEventArgs()
        {
        }

        /// <summary>
        /// Gets the <see cref="SettingsNode"/> from which the change originated.
        /// </summary>
        public SettingsNode OriginalSource { get; internal set; }
    }
}
