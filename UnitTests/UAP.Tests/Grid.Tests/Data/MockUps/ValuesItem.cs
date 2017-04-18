using System;

namespace Telerik.UI.Xaml.Controls.Grid.Tests
{
    internal class ValuesItem
    {
        public string StringProperty { get; set; }
        public double DoubleProperty { get; set; }
        public int IntProperty { get; set; }
        public bool BooleanProperty { get; set; }
        public object ObjectProperty { get; set; }
        public DateTime DateTimeProperty { get; set; }

        /// <summary>
        /// Throws ErrorPropertyException on get.
        /// </summary>
        public object ErrorProperty
        {
            get
            {
                throw new ErrorPropertyException();
            }
        }
    }
}
