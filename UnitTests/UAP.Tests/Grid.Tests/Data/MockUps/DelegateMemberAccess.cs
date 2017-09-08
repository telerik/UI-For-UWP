using System;
using Telerik.Data.Core.Fields;

namespace Telerik.UI.Xaml.Controls.Grid.Tests
{
    internal class DelegateMemberAccess<T> : IMemberAccess
    {
        private Func<T, object> access;

        public DelegateMemberAccess(Func<T, object> access)
        {
            this.access = access;
        }

        public object GetValue(object item)
        {
            return this.access((T)item);
        }

        public void SetValue(object item, object fieldValue)
        {
            throw new System.NotImplementedException();
        }
    }
}
