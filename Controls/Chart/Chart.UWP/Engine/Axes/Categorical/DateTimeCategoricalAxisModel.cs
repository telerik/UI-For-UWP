using System;
using System.Collections.Generic;
using System.Globalization;
using Telerik.Core;

namespace Telerik.Charting
{
    internal class DateTimeCategoricalAxisModel : CategoricalAxisModel
    {
        internal static readonly int DateTimeComponentPropertyKey = PropertyKeys.Register(typeof(DateTimeCategoricalAxisModel), "DateTimeComponent", ChartAreaInvalidateFlags.All);

        private const string CompositeCategoryKeyFormat = "{0}_{1}";
        private bool validCategories;

        internal DateTimeComponent DateTimeComponent
        {
            get
            {
                return this.GetTypedValue<DateTimeComponent>(DateTimeComponentPropertyKey, DateTimeComponent.Ticks);
            }
            set
            {
                this.SetValue(DateTimeComponentPropertyKey, value);
            }
        }

        internal override object GetCategoryValue(DataPoint point)
        {
            object value = base.GetCategoryValue(point);

            DateTime date;
            if (DateTimeHelper.TryGetDateTime(value, out date))
            {
                return date;
            }

            return value;
        }

        internal override object GetCategoryKey(DataPoint point, object value)
        {
            DateTime date;
            if (!DateTimeHelper.TryGetDateTime(value, out date))
            {
                this.validCategories = false;
                return base.GetCategoryKey(point, value);
            }

            switch (this.DateTimeComponent)
            {
                case DateTimeComponent.Ticks:
                    return date.Ticks;
                case DateTimeComponent.Year:
                    return date.Year;
                case DateTimeComponent.Quarter:
                    return string.Format(CultureInfo.CurrentUICulture, CompositeCategoryKeyFormat, date.Year, DateTimeExtensions.GetQuarterOfYear(date));
                case DateTimeComponent.Month:
                    return date.Month;
                case DateTimeComponent.Week:
                    return DateTimeExtensions.GetWeekOfYear(date);
                case DateTimeComponent.Hour:
                    return DateTimeExtensions.GetHourOfYear(date);
                case DateTimeComponent.Minute:
                    return DateTimeExtensions.GetMinuteOfYear(date);
                case DateTimeComponent.Second:
                    return DateTimeExtensions.GetSecondOfYear(date);
                case DateTimeComponent.Millisecond:
                    return DateTimeExtensions.GetMillisecondOfYear(date);
                case DateTimeComponent.Date:
                    return date.Date;
                case DateTimeComponent.TimeOfDay:
                    return date.TimeOfDay;
                case DateTimeComponent.Day:
                    return date.Day;
                case DateTimeComponent.DayOfWeek:
                    return date.DayOfWeek;
                case DateTimeComponent.DayOfYear:
                    return date.DayOfYear;
                default:
                    throw new ArgumentException("Unrecognized DateTime component has been specified for grouping the DateTimeCategoricalAxis.");
            }
        }

        internal override void UpdateCore(AxisUpdateContext context)
        {
            // reset the validation flag
            this.validCategories = true;

            base.UpdateCore(context);

            if (this.validCategories)
            {
                // sort the categories so that they are arranged in a chronological order
                this.categories.Sort(new DateTimeComparer());
            }
        }

        internal override object GetLabelContent(AxisTickModel tick)
        {
            if (tick.virtualIndex >= this.categories.Count)
            {
                return null;
            }

            return this.categories[tick.virtualIndex].KeySource;
        }

        private class DateTimeComparer : IComparer<AxisCategory>
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Already validated"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Already validated")]
            public int Compare(AxisCategory x, AxisCategory y)
            {
                if (x == null && y == null)
                {
                    return 0;
                }

                if (x == null)
                {
                    return -1;
                }

                if (y == null)
                {
                    return 1;
                }

                if (x.KeySource == null && y.KeySource == null)
                {
                    return 0;
                }

                if (x.KeySource == null)
                {
                    return -1;
                }

                if (y.KeySource == null)
                {
                    return 1;
                }

                return ((DateTime)x.KeySource).CompareTo((DateTime)y.KeySource);
            }
        }
    }
}
