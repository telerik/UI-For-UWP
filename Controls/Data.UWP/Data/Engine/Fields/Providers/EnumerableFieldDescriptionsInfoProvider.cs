using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Telerik.Core;

namespace Telerik.Data.Core.Fields
{
    /// <summary>
    /// An <see cref="IFieldInfoExtractor"/> for <see cref="IEnumerable"/> source.
    /// </summary>
    internal class EnumerableFieldDescriptionsExtractor : IFieldInfoExtractor
    {
        private IEnumerable source;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumerableFieldDescriptionsExtractor"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        public EnumerableFieldDescriptionsExtractor(IEnumerable source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            this.source = source;
        }

        public bool IsInitialized
        {
            get
            {
                return true;
            }
        }

        public bool IsDynamic
        {
            get
            {
                return false;
            }
        }

        /// <inheritdoc />
        public IEnumerable<IDataFieldInfo> GetDescriptions()
        {
            return this.GetDescriptionsFromEnumerable();
        }

        private static Type TryGetConcreteTypeFromGenericArguments(Type sourceType)
        {
            var itemType = Enumerable.LastOrDefault(sourceType.GetTypeInfo().GenericTypeArguments);
            var baseType = sourceType.GetTypeInfo().BaseType;

            while (baseType != null && itemType == null)
            {
                itemType = Enumerable.LastOrDefault(baseType.GetTypeInfo().GenericTypeArguments);
                baseType = baseType.GetTypeInfo().BaseType;
            }

            if (itemType == typeof(object))
            {
                return null;
            }
            else
            {
                return itemType;
            }
        }

        private static IList<IDataFieldInfo> GetDescriptionsForItemType(Type dataType)
        {
            var infos = new List<IDataFieldInfo>();

            Queue<Type> typesToReflect = new Queue<Type>();
            typesToReflect.Enqueue(dataType);

            while (typesToReflect.Count > 0)
            {
                var currentType = typesToReflect.Dequeue();
                TypeInfo typeInfo = currentType.GetTypeInfo();
                if (typeInfo.IsInterface)
                {
                    foreach (Type interfaceType in typeInfo.ImplementedInterfaces)
                    {
                        typesToReflect.Enqueue(interfaceType);
                    }
                }

                var props = currentType.GetRuntimeProperties().Where(pi => pi.GetMethod != null && !pi.GetIndexParameters().Any() && pi.GetMethod.IsPublic && pi.GetCustomAttribute<SkipAutoGenerateAttribute>() == null);
                foreach (var propertyInfo in props)
                {
                    var propertyAccess = BindingExpressionHelper.CreateGetValueFunc(currentType, propertyInfo.Name);
                    var propertySetter = BindingExpressionHelper.CreateSetValueAction(currentType, propertyInfo.Name);
                    var newInfo = new PropertyInfoFieldInfo(propertyInfo, propertyAccess, propertySetter, currentType);
                    newInfo.Role = FieldInfoHelper.GetRoleForType(propertyInfo.PropertyType);
                    infos.Add(newInfo);
                }
            }

            return infos;
        }

        private object TryGetFirstItemFromSource()
        {
            object firstElement = null;

            foreach (var item in this.source)
            {
                firstElement = item;
                break;
            }

            return firstElement;
        }

        private Type ExtractItemTypeFromFirstElement()
        {
            var firstElement = this.TryGetFirstItemFromSource();

            if (firstElement != null)
            {
                return firstElement.GetType();
            }
            else
            {
                return null;
            }
        }

        private IList<IDataFieldInfo> GetDescriptionsFromEnumerable()
        {
            var dataType = this.ExtractItemType();
            if (dataType == null)
            {
                return new List<IDataFieldInfo>();
            }

            return GetDescriptionsForItemType(dataType);
        }

        private Type ExtractItemType()
        {
            // get the type from the first item
            Type itemType = this.ExtractItemTypeFromFirstElement();

            // get the generic type (if any)
            Type genericItemType = TryGetConcreteTypeFromGenericArguments(this.source.GetType());

            if (itemType != null && genericItemType != null)
            {
                if (itemType.GetTypeInfo().IsAssignableFrom(genericItemType.GetTypeInfo()))
                {
                    return itemType;
                }

                if (genericItemType.GetTypeInfo().IsAssignableFrom(itemType.GetTypeInfo()))
                {
                    return genericItemType;
                }
            }

            if (itemType != null)
            {
                return itemType;
            }

            return genericItemType;
        }
    }
}