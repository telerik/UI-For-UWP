using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Telerik.Core;
using Windows.UI.Xaml.Controls;
using WinRT.Testing;

namespace Telerik.UI.Xaml.Controls.Input.Tests.SegmentedControl
{
    [TestClass]
    [Tag("Input")]
    [Tag("SegmentedControl")]
    public class SegmentedControlTestsBase : RadControlUITest
    {
        protected RadSegmentedControl segmentedControl;
        protected List<object> items;

        public override void TestInitialize()
        {
            base.TestInitialize();

            this.items = new List<object> { "str", new Button { Content = "button" }, new TextBlock { Text = "textblock" }, 10 };
            this.segmentedControl = new RadSegmentedControl();
        }

        protected ObservableCollection<Data> GetDataSource(int count)
        {
            var data = new ObservableCollection<Data>();
            for (int i = 0; i < count; i++)
            {
                data.Add(new Data { DisplayValue = "display: " + i, Value = i });
            }

            return data;
        }

        protected void AddItems(IEnumerable items)
        {
            foreach (var item in items)
            {
                this.segmentedControl.Items.Add(item);
            }
        }

        public class Data : ViewModelBase
        {
            private int value;

            public int Value
            {
                get { return value; }
                set
                {
                    if (this.value != value)
                    {
                        this.value = value;
                        OnPropertyChanged();
                    }
                }
            }

            private string displayValue;

            public string DisplayValue
            {
                get { return displayValue; }
                set
                {
                    if (this.displayValue != value)
                    {
                        this.displayValue = value;
                        OnPropertyChanged();
                    }
                }
            }

            public override string ToString()
            {
                return string.Format("Value: {0}", this.Value);
            }
        }
    }
}
