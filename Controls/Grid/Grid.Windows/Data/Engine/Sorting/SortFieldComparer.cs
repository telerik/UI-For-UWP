using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Telerik.UI.Xaml.Controls.Grid;

namespace Telerik.Data.Core
{
    internal class SortFieldComparer : IComparer<object>
    {
        private Comparer comparer;
        private IReadOnlyList<SortDescription> sortFields;

        internal SortFieldComparer(IReadOnlyList<SortDescription> sortFieldsCopy, CultureInfo culture)
        {
            this.sortFields = sortFieldsCopy;
            if (culture == null || culture == CultureInfo.InvariantCulture)
            {
                this.comparer = new Comparer(CultureInfo.InvariantCulture);
            }
            else if (culture == CultureInfo.CurrentCulture)
            {
                this.comparer = new Comparer(CultureInfo.CurrentCulture);
            }
            else
            {
                this.comparer = new Comparer(culture);
            }
        }

        public int Compare(object o1, object o2)
        {
            int num = 0;
            for (int i = 0; i < this.sortFields.Count; i++)
            {
                object a = this.sortFields[i].GetValueForItem(o1);
                object b = this.sortFields[i].GetValueForItem(o2);
                num = this.comparer.Compare(a, b);
                if (this.sortFields[i].SortOrder == SortOrder.Descending)
                {
                    num = -num;
                }

                if (num != 0)
                {
                    return num;
                }
            }

            return num;
        }

        internal class InternalTestHook
        {
            private SortFieldComparer comparer;
            public InternalTestHook(SortFieldComparer comparer)
            {
                this.comparer = comparer;
            }

            internal IReadOnlyList<SortDescription> SortDescriptions
            {
                get
                {
                    return this.comparer.sortFields;
                }
            }
        }

        private sealed class Comparer : IComparer
        {
            private const string CompareInfoName = "CompareInfo";
            private System.Globalization.CompareInfo compareInfo;

            public Comparer(System.Globalization.CultureInfo culture)
            {
                if (culture == null)
                {
                    throw new ArgumentNullException("culture");
                }
                this.compareInfo = culture.CompareInfo;
            }

            private Comparer()
            {
                this.compareInfo = null;
            }

            public int Compare(object a, object b)
            {
                if (a == b)
                {
                    return 0;
                }

                if (a == null)
                {
                    return -1;
                }

                if (b == null)
                {
                    return 1;
                }

                if (this.compareInfo != null)
                {
                    string str = a as string;
                    string str2 = b as string;
                    if ((str != null) && (str2 != null))
                    {
                        return this.compareInfo.Compare(str, str2);
                    }
                }

                IComparable comparable = a as IComparable;
                if (comparable != null)
                {
                    return comparable.CompareTo(b);
                }

                IComparable comparable2 = b as IComparable;
                if (comparable2 == null)
                {
                    throw new ArgumentException("Argument_ImplementIComparable");
                }

                return -comparable2.CompareTo(a);
            }
        }
    }
}