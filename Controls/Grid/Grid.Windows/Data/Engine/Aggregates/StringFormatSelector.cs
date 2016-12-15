using System;

namespace Telerik.Data.Core.Aggregates
{
    /// <summary>
    /// Provides a way to choose a string format for a <see cref="PropertyAggregateDescriptionBase"/>.
    /// </summary>
    internal abstract class StringFormatSelector : SettingsNode
    {
        /// <summary>
        /// Select a StringFormat suitable to format the <see cref="AggregateValue"/>s provided for the <paramref name="aggregateDescription"/> and <paramref name="dataType"/>.
        /// </summary>
        /// <param name="dataType">The type of the data items.</param>
        /// <param name="aggregateDescription">The <see cref="PropertyAggregateDescriptionBase"/> for which <see cref="AggregateValue"/>s a StringFormat is selected.</param>
        /// <returns>A string format.</returns>
        public virtual string SelectStringFormat(Type dataType, PropertyAggregateDescriptionBase aggregateDescription)
        {
            return DefaultSelectionMethod(dataType, aggregateDescription);
        }

        internal static string SelectStringFormat(StringFormatSelector selector, Type dataType, PropertyAggregateDescriptionBase aggregateDescription)
        {
            if (selector != null)
            {
                return selector.SelectStringFormat(dataType, aggregateDescription);
            }
            else
            {
                return DefaultSelectionMethod(dataType, aggregateDescription);
            }
        }

        private static string DefaultSelectionMethod(Type dataType, PropertyAggregateDescriptionBase aggregateDescription)
        {
            string stringFormat = aggregateDescription.StringFormat;

            if (aggregateDescription.AggregateFunction != null)
            {
                stringFormat = aggregateDescription.AggregateFunction.GetStringFormat(dataType, stringFormat);
            }

            IAggregateDescription aggregateDescriptionInternal = (IAggregateDescription)aggregateDescription;
            if (aggregateDescriptionInternal.TotalFormat != null)
            {
                stringFormat = aggregateDescriptionInternal.TotalFormat.GetStringFormat(dataType, stringFormat);
            }

            return stringFormat;
        }
    }
}
