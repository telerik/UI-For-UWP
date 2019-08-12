using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    internal class StyleManager
    {
        internal static Style MergeStyles(Style defaultStyle, Style customStyle)
        {
            if (customStyle == null)
            {
                return defaultStyle;
            }

            if (defaultStyle == null)
            {
                return customStyle;
            }

            Style actualStyle;
            if (StyleContainsAllProperties(customStyle, defaultStyle.Setters))
            {
                actualStyle = customStyle;
            }
            else
            {
                actualStyle = GetOrCreateMergedStyle(defaultStyle, customStyle);
            }

            return actualStyle;
        }

        internal static bool StyleContainsAllProperties(Style style, SetterBaseCollection setters)
        {
            if (style != null)
            {
                HashSet<DependencyProperty> propertySet = new HashSet<DependencyProperty>(style.Setters.Where(s => s is Setter).Select(s => ((Setter)s).Property));
                bool styleContainsAllProperties = setters.All(s => s is Setter && propertySet.Contains(((Setter)s).Property));
                return styleContainsAllProperties;
            }

            return false;
        }

        private static Style GetOrCreateMergedStyle(Style defaultStyle, Style customStyle)
        {
            Style mergedStyle = new Style(customStyle.TargetType)
            {
                BasedOn = customStyle,
            };

            HashSet<DependencyProperty> customStyleProperties = GetProperties(customStyle);
            foreach (Setter setter in defaultStyle.Setters)
            {
                if (customStyleProperties.Contains(setter.Property))
                {
                    continue;
                }

                mergedStyle.Setters.Add(setter);
            }

            return mergedStyle;
        }

        private static HashSet<DependencyProperty> GetProperties(Style style)
        {
            HashSet<DependencyProperty> properties = new HashSet<DependencyProperty>();
            AddProperties(style, properties);
            return properties;
        }

        private static void AddProperties(Style style, HashSet<DependencyProperty> properties)
        {
            if (style == null)
            {
                return;
            }

            foreach (Setter setter in style.Setters)
            {
                if (!properties.Contains(setter.Property))
                {
                    properties.Add(setter.Property);
                }
            }

            AddProperties(style.BasedOn, properties);
        }
    }
}
