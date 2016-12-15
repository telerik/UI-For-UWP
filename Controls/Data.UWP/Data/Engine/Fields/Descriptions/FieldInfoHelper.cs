using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Telerik.Data.Core.Fields
{
    internal static class FieldInfoHelper
    {
        /// <summary>
        /// Gets the <see cref="FieldRole"/> for the specified type.
        /// </summary>
        /// <param name="fieldType">Field type.</param>
        public static FieldRole GetRoleForType(Type fieldType)
        {
            // All types that would support smaller range of variation should be here. If we have all unique values for a proeprty and they are about a doesn you could put them here
            IEnumerable<Type> columnBaseTypes = new List<Type>() { typeof(Enum), typeof(char) };

            var role = FieldRole.Row;

            if (IsNumericType(fieldType))
            {
                role = FieldRole.Value;
            }
            else
            {
                var fieldTypeInfo = fieldType.GetTypeInfo();
                if (Enumerable.Any(columnBaseTypes, t => fieldType == t || fieldTypeInfo.IsSubclassOf(t)))
                {
                    role = FieldRole.Column;
                }
            }

            return role;
        }

        public static bool IsNumericType(Type type)
        {
            return type == typeof(double)
               || type == typeof(Nullable<double>)
               || type == typeof(int)
               || type == typeof(Nullable<int>)
               || type == typeof(byte)
               || type == typeof(Nullable<byte>)
               || type == typeof(short)
               || type == typeof(Nullable<short>)
               || type == typeof(decimal)
               || type == typeof(Nullable<decimal>)
               || type == typeof(float)
               || type == typeof(Nullable<float>)
               || type == typeof(long)
               || type == typeof(Nullable<long>)
               || type == typeof(uint)
               || type == typeof(Nullable<uint>)
               || type == typeof(sbyte)
               || type == typeof(Nullable<sbyte>)
               || type == typeof(ushort)
               || type == typeof(Nullable<ushort>)
               || type == typeof(ulong)
               || type == typeof(Nullable<ulong>);                
        }
    }
}