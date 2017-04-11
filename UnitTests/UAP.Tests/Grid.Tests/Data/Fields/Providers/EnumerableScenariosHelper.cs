using System.Collections;
using System.Collections.Generic;

namespace Telerik.UI.Xaml.Controls.Grid.Tests
{
    public static class EnumerableScenariosHelper
    {
        public class BaseType
        {
            public string NormalPropertyOne { get; set; }
            public int NormalPropertyTwo { get; set; }
        }

        public class ExtendedType : BaseType
        {
            public string NormalExtendedProperty { get; set; }
        }

        public class NonGenericEnumerable : IEnumerable
        {
            private IEnumerable items;

            public NonGenericEnumerable(IEnumerable items)
            {
                this.items = items;
            }

            public IEnumerator GetEnumerator()
            {
                return this.items.GetEnumerator();
            }
        }

        public static IEnumerable GetNonGenericEmptyEnumerable()
        {
            return new NonGenericEnumerable(new List<BaseType>() { });
        }

        public static IEnumerable GetGenericEmptyEnumerableOfBaseType()
        {
            return new List<BaseType>() { };
        }

        public static  IEnumerable GetGenericEnumerableOfBaseTypeWithItemsThatInheritFromBaseType()
        {
            return new List<BaseType>() 
            {
                new ExtendedType(),
                new ExtendedType()
            };
        }

        public static IEnumerable GetGenericEnumerableOfObjectWithItemsOfBaseType()
        {
            return new List<object>() 
            {
                new BaseType(),
                new BaseType()
            };
        }

        public static IEnumerable GetNonGenericEnumerableWithItemsWithDifferentTypes()
        {
            return new NonGenericEnumerable(new List<BaseType>() 
            {
                new BaseType(),
                new ExtendedType()
             });
        }
    }
}
