using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using Telerik.Data.Core;

namespace Telerik.UI.Xaml.Controls.Grid.Tests
{
    [TestClass]
    public class PropertyAggregateDescriptionBaseTests
    {
        private class PropertyAggregateDescriptionBaseStub : PropertyAggregateDescriptionBase
        {
            protected override void CloneOverride(Cloneable source)
            {
                throw new NotImplementedException();
            }

            protected override Cloneable CreateInstanceCore()
            {
                throw new NotImplementedException();
            }
        }
    }
}