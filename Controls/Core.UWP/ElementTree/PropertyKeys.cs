using System;
using System.Collections.Generic;
using System.Reflection;

namespace Telerik.Core
{
    internal static class PropertyKeys
    {
        private static int counter;
        private static Dictionary<Type, PropertyLookup> properties = new Dictionary<Type, PropertyLookup>(32);
        private static Dictionary<int, object> propertyFlags = new Dictionary<int, object>(32);

        /// <summary>
        /// Registers an integer value that uniquely identifies a property.
        /// </summary>
        /// <param name="type">The type that declares the property.</param>
        /// <param name="propertyName">The name of the property.</param>
        public static int Register(Type type, string propertyName)
        {
            PropertyLookup lookup = FindProperties(type, false);
            if (lookup == null)
            {
                lookup = new PropertyLookup();
                properties.Add(type, lookup);
            }

            int key = counter++;
            lookup.NamesByKey.Add(key, propertyName);
            lookup.KeysByName.Add(propertyName, key);

            return key;
        }

        /// <summary>
        /// Registers an integer value that uniquely identifies a property.
        /// </summary>
        /// <param name="type">The type that declares the property.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="flags">Optional metadata, associated with the property.</param>
        public static int Register(Type type, string propertyName, object flags)
        {
            int key = Register(type, propertyName);
            propertyFlags.Add(key, flags);

            return key;
        }

        public static T GetPropertyFlags<T>(int key)
        {
            object flags;
            if (propertyFlags.TryGetValue(key, out flags))
            {
                return (T)flags;
            }

            return default(T);
        }

        public static string GetNameByKey(Type type, int key)
        {
            Type searchType = type;
            PropertyLookup props = FindProperties(searchType, true);
            string name;

            // traverse all the properties down in the type hierarchy
            while (props != null)
            {
                if (props.NamesByKey.TryGetValue(key, out name))
                {
                    return name;
                }

                searchType = searchType.GetTypeInfo().BaseType;
                props = FindProperties(searchType, true);
            }

            return string.Empty;
        }

        private static PropertyLookup FindProperties(Type type, bool lookUpBase)
        {
            PropertyLookup props;
            if (!lookUpBase)
            {
                properties.TryGetValue(type, out props);
                return props;
            }

            Type currentType = type;
            Type baseType = typeof(PropertyBagObject);

            while (currentType != null && currentType != baseType)
            {
                if (properties.TryGetValue(currentType, out props))
                {
                    return props;
                }

                currentType = currentType.GetTypeInfo().BaseType;
            }

            return null;
        }

        private class PropertyLookup
        {
            public Dictionary<int, string> NamesByKey = new Dictionary<int, string>(8);
            public Dictionary<string, int> KeysByName = new Dictionary<string, int>(8);
        }
    }
}
