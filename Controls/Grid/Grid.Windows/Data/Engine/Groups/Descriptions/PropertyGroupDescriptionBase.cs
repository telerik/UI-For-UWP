using System;
using System.Collections.Generic;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Base class used to group items, provide well known groups, sort and filter the groups for a LocalDataSourceProvider based on the item's PropertyName.
    /// </summary>
    internal abstract class PropertyGroupDescriptionBase : GroupDescription
    {
        /// <summary>
        /// Return a name for group that would contain the <paramref name="item"/>.
        /// </summary>
        /// <param name="item">The item to group.</param>
        /// <param name="level">The level of grouping for this <see cref="GroupDescription"/>.</param>
        /// <returns>A name for the group that would contain the <paramref name="item"/>.</returns>
        protected internal virtual object GroupNameFromItem(object item, int level)
        {
            if (this.MemberAccess == null)
            {
                throw new InvalidOperationException("Member access has not been initialized. Most probably item does not have property with name: " + this.PropertyName);
            }

            var value = this.MemberAccess.GetValue(item);
            var processedValue = ReturnInvalidValuesAsNull(value);

            return processedValue;
        }

        /// <inheritdoc />
        protected internal override IEnumerable<object> GetAllNames(IEnumerable<object> uniqueNames, IEnumerable<object> parentGroupNames)
        {
            return uniqueNames;
        }

        /// <inheritdoc />
        protected sealed override void CloneCore(Cloneable source)
        {
            base.CloneCore(source);
            this.CloneOverride(source);
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

        private static object ReturnInvalidValuesAsNull(object value)
        {
            // TODO: Preparation for DBNULL (remove the private DBNull class when WinRT supports DBNull.
            if (value == DBNull.Value)
            {
                return null;
            }

            return value;
        }

        private sealed class DBNull
        {
            public static readonly DBNull Value = new DBNull();

            private DBNull()
            {
            }
        }
    }
}