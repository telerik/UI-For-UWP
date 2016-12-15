using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace Telerik.Data.Core.Fields
{
    /// <summary>
    /// An <see cref="IFieldDescriptionProvider"/> for a generic ItemsSource.
    /// </summary>
    internal class LocalDataSourceFieldDescriptionsProvider : LocalFieldDescriptionsProviderBase
    {
        private IFieldInfoExtractor currentExtractor;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDataSourceFieldDescriptionsProvider"/> class.
        /// </summary>
        public LocalDataSourceFieldDescriptionsProvider()
        {
        }

        public override bool IsReady
        {
            get
            {
                return this.currentExtractor != null && this.currentExtractor.IsInitialized;
            }
        }

        internal override bool IsDynamic
        {
            get
            {
                return this.currentExtractor != null && this.currentExtractor.IsDynamic;
            }
        }

        /// <inheritdoc />
        protected override IFieldInfoData GenerateDescriptionsData()
        {
            this.currentExtractor = GetConcreteExtractor(this.CurrentState);
            var fieldDescriptions = this.GetDescriptions(this.currentExtractor);
            var root = this.GetFieldDescriptionHierarchy(fieldDescriptions);
            var data = new FieldInfoData(root);

            return data;
        }

        /// <summary>
        /// Gets the <see cref="IDataFieldInfo"/> for the itemsSource.
        /// </summary>
        /// <returns>The <see cref="IEnumerable{IDataFieldInfo}"/> with all <see cref="IDataFieldInfo"/>s for this provider.</returns>
        protected virtual IEnumerable<IDataFieldInfo> GetDescriptions(IFieldInfoExtractor getter)
        {
            return getter.GetDescriptions();
        }

        private static IFieldInfoExtractor GetConcreteExtractor(object itemsSource)
        {
            var genericList = itemsSource as IEnumerable;
            if (genericList != null)
            {
                IFieldInfoExtractor provider = GetDynamicObjectFieldExtractor(genericList);
                if (provider == null)
                {
                    provider = new EnumerableFieldDescriptionsExtractor(genericList);
                }

                return provider;
            }

            return new EnumerableFieldDescriptionsExtractor(new List<object>());
        }

        private static IFieldInfoExtractor GetDynamicObjectFieldExtractor(IEnumerable items)
        {
            var enumerator = items.GetEnumerator();
            object firstItem = null;
            if (enumerator.MoveNext())
            {
                firstItem = enumerator.Current;
            }

            ExpandoObject expandoObject = firstItem as ExpandoObject;
            if (expandoObject != null)
            {
                return new ExpandoObjectFieldDescriptionsInfoProvider(expandoObject);
            }

            DynamicObject dynamicObject = firstItem as DynamicObject;
            if (dynamicObject != null)
            {
                return new DynamicObjectFieldInfoDescriptionsProvider(dynamicObject);
            }

            var type = items.GetType().GetTypeInfo();
            var argumentType = type.GenericTypeArguments.LastOrDefault();

            if (argumentType != null)
            {
                var argumentTypeInfo = argumentType.GetTypeInfo();
                if (typeof(DynamicObject).GetTypeInfo().IsAssignableFrom(argumentTypeInfo))
                {
                    return new DynamicObjectFieldInfoDescriptionsProvider(null);
                }
                else if (typeof(ExpandoObject).GetTypeInfo().IsAssignableFrom(argumentTypeInfo))
                {
                    return new ExpandoObjectFieldDescriptionsInfoProvider(null);
                }
            }

            return null;
        }
    }
}