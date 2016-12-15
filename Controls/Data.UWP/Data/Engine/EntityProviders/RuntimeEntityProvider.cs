using System;
using System.Collections;
using System.Reflection;
using Telerik.Core;
using Windows.UI.Xaml.Data;

namespace Telerik.Data.Core
{
    public class RuntimeEntityProvider : EntityProvider
    {
        protected override IEnumerable GetProperties()
        {
            if (this.Context == null)
            {
                return System.Linq.Enumerable.Empty<PropertyInfo>();
            }

            if (this.IteratorMode == PropertyIteratorMode.All)
            {
                return this.Context.GetType().GetRuntimeProperties();
            }

            return this.Context.GetType().GetTypeInfo().DeclaredProperties;
        }

        protected override bool ShouldGenerateEntityProperty(object property)
        {
            var propertyInfo = property as PropertyInfo;

            if (propertyInfo != null)
            {
                if (!this.IsPublic(propertyInfo))
                {
                    return false;
                }

                var ignoreAttribute = propertyInfo.GetCustomAttribute(typeof(IgnoreAttribute));
                if (ignoreAttribute != null)
                {
                    return false;
                }
                return true;
            }

            return false;
        }

        protected override ISupportEntityValidation GetItemValidator(Entity entity)
        {
            ISupportEntityValidation validator = null;

            if (this.Context != null)
            {
                validator = this.Context as ISupportEntityValidation;
            }

            return validator;
        }

        protected override Type GetEntityPropertyType(object property)
        {
            var propertyInfo = property as PropertyInfo;
            if (propertyInfo != null)
            {
                return typeof(RuntimeEntityProperty);
            }

            return null;
        }

        private bool IsPublic(PropertyInfo propertyInfo)
        {
            if (propertyInfo != null && (propertyInfo.SetMethod != null && propertyInfo.SetMethod.IsPublic) &&
                (propertyInfo.GetMethod != null && propertyInfo.GetMethod.IsPublic))
            {
                return true;
            }

            return false;
        }
    }
}
