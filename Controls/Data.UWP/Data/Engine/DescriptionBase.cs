using System.Diagnostics.CodeAnalysis;
using Telerik.Data.Core.Fields;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Contains mechanisms to access and describe properties of objects used as source in data grouping.
    /// </summary>
    internal abstract class DescriptionBase : SettingsNode, IDescriptionBase
    {
        private string customName;
        private string propertyName;
        private IMemberAccess memberAccess;

        internal DescriptionBase()
        {
        }

        /// <summary>
        /// Gets the display-friendly name.
        /// </summary>
        public string DisplayName
        {
            get
            {
                return this.GetDisplayName();
            }
        }

        /// <summary>
        /// Gets or sets the custom name that will be used as display name.
        /// </summary>
        public string CustomName
        {
            get
            {
                return this.customName;
            }

            set
            {
                if (this.customName != value)
                {
                    this.customName = value;
                    this.OnPropertyChanged(nameof(this.CustomName));
                    this.OnPropertyChanged(nameof(this.DisplayName));
                    this.NotifyChange(new SettingsChangedEventArgs());
                }
            }
        }

        /// <summary>
        /// Gets or sets a value identifying a property on the grouped items.
        /// </summary>
        public string PropertyName
        {
            get
            {
                return this.propertyName;
            }

            set
            {
                if (this.propertyName != value)
                {
                    this.propertyName = value;
                    this.OnPropertyChanged(nameof(this.PropertyName));
                    this.OnPropertyChanged(nameof(this.DisplayName));
                    this.NotifyChange(new SettingsChangedEventArgs());
                }
            }
        }

        internal IMemberAccess MemberAccess
        {
            get
            {
                return this.memberAccess;
            }
            set
            {
                if (this.memberAccess != value)
                {
                    this.memberAccess = value;
                    this.OnPropertyChanged(nameof(this.MemberAccess));
                    this.NotifyChange(new SettingsChangedEventArgs());
                }
            }
        }

        /// <inheritdoc />
        string IDescriptionBase.GetMemberName()
        {
            return this.GetMemberName();
        }

        /// <inheritdoc />
        IDescriptionBase IDescriptionBase.Clone()
        {
            return this.Clone() as IDescriptionBase;
        }

        /// <inheritdoc />
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Design choice.")]
        protected internal virtual string GetMemberName()
        {
            return this.PropertyName;
        }

        /// <inheritdoc />
        protected override void CloneCore(Cloneable source)
        {
            DescriptionBase original = source as DescriptionBase;
            if (original != null)
            {
                this.CustomName = original.CustomName;
                this.MemberAccess = original.MemberAccess;
                this.PropertyName = original.PropertyName;
            }
        }

        /// <summary>
        /// Gets the display-friendly name.
        /// </summary>
        /// <returns>A <see cref="string"/> name.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Design choice.")]
        protected virtual string GetDisplayName()
        {
            if (string.IsNullOrEmpty(this.CustomName))
            {
                return this.PropertyName;
            }

            return this.CustomName;
        }
    }
}