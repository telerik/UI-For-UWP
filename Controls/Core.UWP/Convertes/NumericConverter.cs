using System;
using System.Collections.Generic;
using System.Globalization;

namespace Telerik.Core
{
    internal static class NumericConverter
    {
        private static readonly HashSet<Type> NumericalTypes;
        private static readonly HashSet<Type> NumericalNullableTypes;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static NumericConverter()
        {
            NumericalTypes = new HashSet<Type>();
            NumericalTypes.Add(typeof(byte));
            NumericalTypes.Add(typeof(sbyte));
            NumericalTypes.Add(typeof(short));
            NumericalTypes.Add(typeof(ushort));
            NumericalTypes.Add(typeof(int));
            NumericalTypes.Add(typeof(uint));
            NumericalTypes.Add(typeof(long));
            NumericalTypes.Add(typeof(ulong));
            NumericalTypes.Add(typeof(float));
            NumericalTypes.Add(typeof(double));
            NumericalTypes.Add(typeof(decimal));

            NumericalNullableTypes = new HashSet<Type>();
            NumericalNullableTypes.Add(typeof(byte?));
            NumericalNullableTypes.Add(typeof(sbyte?));
            NumericalNullableTypes.Add(typeof(short?));
            NumericalNullableTypes.Add(typeof(ushort?));
            NumericalNullableTypes.Add(typeof(int?));
            NumericalNullableTypes.Add(typeof(uint?));
            NumericalNullableTypes.Add(typeof(long?));
            NumericalNullableTypes.Add(typeof(ulong?));
            NumericalNullableTypes.Add(typeof(float?));
            NumericalNullableTypes.Add(typeof(double?));
            NumericalNullableTypes.Add(typeof(decimal?));
        }

        public static bool IsNumericType(Type type)
        {
            return IsNumericType(type, false);
        }

        public static bool IsNumericType(Type type, bool checkForNullable)
        {
            if (NumericalTypes.Contains(type))
            {
                return true;
            }

            if (checkForNullable && NumericalNullableTypes.Contains(type))
            {
                return true;
            }

            return false;
        }

        public static bool TryConvertToDouble(object value, out double result)
        {
            return TryConvertToDouble(value, out result, CultureInfo.CurrentUICulture, false);
        }

        public static bool TryConvertToDouble(object value, out double result, CultureInfo culture, bool checkForNullable)
        {
            result = double.NaN;
            if (value == null)
            {
                return true;
            }

            if (!NumericalTypes.Contains(value.GetType()))
            {
                if (!checkForNullable)
                {
                    return false;
                }

                return TryGetDoubleFromNullableNumeric(value, out result);
            }

            result = Convert.ToDouble(value, culture);
            return true;
        }

        private static bool TryGetDoubleFromNullableNumeric(object value, out double result)
        {
            result = double.NaN;

            if (value is byte?)
            {
                var nullable = (byte?)value;
                if (nullable.HasValue)
                {
                    result = Convert.ToDouble(nullable.Value);
                    return true;
                }

                return false;
            }

            if (value is sbyte?)
            {
                var nullable = (sbyte?)value;
                if (nullable.HasValue)
                {
                    result = Convert.ToDouble(nullable.Value);
                    return true;
                }

                return false;
            }

            if (value is short?)
            {
                var nullable = (short?)value;
                if (nullable.HasValue)
                {
                    result = Convert.ToDouble(nullable.Value);
                    return true;
                }

                return false;
            }

            if (value is ushort?)
            {
                var nullable = (ushort?)value;
                if (nullable.HasValue)
                {
                    result = Convert.ToDouble(nullable.Value);
                    return true;
                }

                return false;
            }

            if (value is int?)
            {
                var nullable = (int?)value;
                if (nullable.HasValue)
                {
                    result = Convert.ToDouble(nullable.Value);
                    return true;
                }

                return false;
            }

            if (value is uint?)
            {
                var nullable = (uint?)value;
                if (nullable.HasValue)
                {
                    result = Convert.ToDouble(nullable.Value);
                    return true;
                }

                return false;
            }

            if (value is long?)
            {
                var nullable = (long?)value;
                if (nullable.HasValue)
                {
                    result = Convert.ToDouble(nullable.Value);
                    return true;
                }

                return false;
            }

            if (value is ulong?)
            {
                var nullable = (ulong?)value;
                if (nullable.HasValue)
                {
                    result = Convert.ToDouble(nullable.Value);
                    return true;
                }

                return false;
            }

            if (value is float?)
            {
                var nullable = (float?)value;
                if (nullable.HasValue)
                {
                    result = Convert.ToDouble(nullable.Value);
                    return true;
                }

                return false;
            }

            if (value is double?)
            {
                var nullable = (double?)value;
                if (nullable.HasValue)
                {
                    result = nullable.Value;
                    return true;
                }

                return false;
            }

            if (value is decimal?)
            {
                var nullable = (decimal?)value;
                if (nullable.HasValue)
                {
                    result = Convert.ToDouble(nullable.Value);
                    return true;
                }
            }

            return false;
        }
    }
}
