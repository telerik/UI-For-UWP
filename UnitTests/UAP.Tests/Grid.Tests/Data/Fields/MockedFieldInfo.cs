using System;
using Telerik.Data.Core.Fields;

namespace Telerik.UI.Xaml.Controls.Grid.Tests
{
    internal class MockedFieldInfo : IDataFieldInfo
    {
        public string Name { get; set; }

        public Type DataType { get; set; }

        public FieldRole Role { get; set; }

        public string DisplayName { get; set; }

        public Type RootClassType { get; }

        public bool Equals(IDataFieldInfo info)
        {
            return false;
        }
    }
}