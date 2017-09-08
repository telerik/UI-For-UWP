using System;
using System.Dynamic;
using System.Runtime.CompilerServices;
using Microsoft.CSharp.RuntimeBinder;

namespace Telerik.Data.Core.Fields
{
    internal class DynamicObjectFieldInfo : IDataFieldInfo, IMemberAccess
    {
        private string name;
        private Type dataType;
        private CallSite<Func<CallSite, object, object>> callSite;

        public DynamicObjectFieldInfo(string name)
        {
            this.name = name;
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public Type DataType
        {
            get
            {
                return this.dataType;
            }
        }

        public FieldRole Role
        {
            get;
            set;
        }

        public string DisplayName
        {
            get
            {
                return this.name;
            }
        }

        public object GetValue(object item)
        {
            DynamicObject dynamicObject = item as DynamicObject;
            if (dynamicObject != null)
            {
                if (this.callSite == null)
                {
                    var binder = Binder.GetMember(CSharpBinderFlags.None, this.name, item.GetType(), new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
                    this.callSite = CallSite<Func<CallSite, object, object>>.Create(binder);
                }

                // Consider ICustomPropertyProvider + ICustomProperty
                var value = this.callSite.Target(this.callSite, item);
                if (this.dataType == null && value != null)
                {
                    // This late evaluation of DataType is potentially error-prone
                    this.dataType = value.GetType();
                }

                return value;
            }

            return null;
        }

        public void SetValue(object item, object fieldValue)
        {
            throw new NotImplementedException();
        }

        public bool Equals(IDataFieldInfo info)
        {
            return false;
        }
    }
}
