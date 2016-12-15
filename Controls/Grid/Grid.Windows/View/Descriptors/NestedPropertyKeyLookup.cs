using System;

namespace Telerik.UI.Xaml.Controls.Grid
{
    internal class NestedPropertyKeyLookup : IKeyLookup
    {
        public Func<object, object> InstanceValueGetter { get; set; }

        public Func<object, object> DisplayValueGetter { get; set; }

        public object GetKey(object instance)
        {
            if (this.InstanceValueGetter != null)
            {
                var item = this.InstanceValueGetter(instance);
                if (this.DisplayValueGetter != null)
                {
                    return this.DisplayValueGetter(item);
                }
                else
                {
                    return item;
                }
            }
            else
            {
                return instance;
            }
        }
    }
}