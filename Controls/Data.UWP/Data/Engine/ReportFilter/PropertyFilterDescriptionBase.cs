using System;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Report <see cref="FilterDescription"/> implementation.
    /// </summary>
    internal abstract class PropertyFilterDescriptionBase : FilterDescription
    {
        private Condition condition;

        /// <summary>
        /// Gets or sets the <see cref="Condition"/> used to filter the groups.
        /// </summary>
        public Condition Condition
        {
            get
            {
                return this.condition;
            }

            set
            {
                if (this.condition != value)
                {
                    this.ChangeSettingsProperty(ref this.condition, value);
                    this.OnPropertyChanged(nameof(Condition));
                    this.NotifyChange(new SettingsChangedEventArgs());
                }
            }
        }

        /// <summary>
        /// Gets the item that is used in filtering for the provided <paramref name="fact"/>.
        /// </summary>
        /// <param name="fact">The data to be filtered.</param>
        /// <returns>The value used for filtering.</returns>
        /// <seealso cref="PassesFilter"/>
        protected internal virtual object GetFilterItem(object fact)
        {
            if (this.MemberAccess != null)
            {
                return this.MemberAccess.GetValue(fact);
            }

            return null;
        }

        /// <summary>
        /// Checks if a value generated from <see cref="GetFilterItem"/> passes the filter.
        /// </summary>
        /// <param name="value">The value to filter.</param>
        /// <returns>True if the <paramref name="value"/> passes the filter; otherwise - false.</returns>
        /// <seealso cref="GetFilterItem"/>
        protected internal virtual bool PassesFilter(object value)
        {
            if (this.condition != null)
            {
                return this.condition.PassesFilter(value);
            }

            return true;
        }

        /// <summary>
        /// Return a value the <paramref name="item"/>.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>A name for the group that would contain the <paramref name="item"/>.</returns>
        protected object ExtractValue(object item)
        {
            if (this.MemberAccess == null)
            {
                throw new InvalidOperationException("Member access  has not been initialized. Most probably item does not have property with name: " + this.PropertyName);
            }

            return this.MemberAccess.GetValue(item);
        }

        /// <inheritdoc />
        protected override sealed void CloneCore(Cloneable source)
        {
            this.CloneOverride(source);
            PropertyFilterDescriptionBase original = source as PropertyFilterDescriptionBase;
            if (original != null)
            {
                this.Condition = Cloneable.CloneOrDefault(original.Condition);
            }

            base.CloneCore(source);
        }

        /// <summary>
        /// Makes the instance a clone (deep copy) of the specified <see cref="Telerik.Data.Core.Cloneable"/>.
        /// </summary>
        /// <param name="source">The object to clone.</param>
        /// <remarks>Notes to Inheritors
        /// If you derive from <see cref="Telerik.Data.Core.Cloneable"/>, you need to override this method to copy all properties.
        /// It is essential that all implementations call the base implementation of this method (if you don't call base you should manually copy all needed properties including base properties).
        /// </remarks>
        protected abstract void CloneOverride(Cloneable source);
    }
}