using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace TestApp1.ViewModels
{
    public class MainViewModel : Screen
    {
        public class RecordData
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Price { get; set; }
        }
        public struct ComboKbnValue
        {
            public int Kbn { get; set; }
            public string Caption { get; set; }
        }

        /// <summary>
        /// List of ComboBox
        /// </summary>
        public List<ComboKbnValue> ColumnKbns { get; set; } = new List<ComboKbnValue>()
        {
            new ComboKbnValue() {Kbn=1,Caption="Id" },
            new ComboKbnValue() {Kbn=2,Caption="Name" },
            new ComboKbnValue() {Kbn=3,Caption="Price" },
        };
        public static class ZaikoKbn
        {
            public const int C_ID = 1;
            public const int C_NAME = 2;
            public const int C_PRICE = 3;
        }
        public int StatKbn { get; set; } = 1;
        public ComboKbnValue SelectedColumnKbn { get; set; }
        public bool ColumnIdEnable { get { return SelectedColumnKbn.Kbn != ZaikoKbn.C_ID; } }
        public bool ColumnNameEnable { get { return SelectedColumnKbn.Kbn != ZaikoKbn.C_NAME; } }
        public bool ColumnPriceEnable { get { return SelectedColumnKbn.Kbn != ZaikoKbn.C_PRICE; } }

        public List<RecordData> Records { get; set; }


        private readonly INavigationService navigationService;
        public MainViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;
            Records = new List<RecordData>()
            {
                new RecordData() { Id=1, Name="aaa", Price=100 },
                new RecordData() { Id=2, Name="bbb", Price=200 },
                new RecordData() { Id=3, Name="ccc", Price=300 },
            };
            UpdateComboKbns();
        }

        void UpdateComboKbns()
        {
            if (ColumnKbns != null)
            {
                var matched = ColumnKbns.Where(t => t.Kbn == StatKbn);
                if (matched.Count() > 0)
                {
                    var selected = matched.First();
                    SelectedColumnKbn = selected;
                    NotifyOfPropertyChange(() => SelectedColumnKbn);
                }
            }

            NotifyOfPropertyChange(() => SelectedColumnKbn);
            NotifyOfPropertyChange(() => ColumnKbns);
            NotifyOfPropertyChange(() => ColumnIdEnable);
            NotifyOfPropertyChange(() => ColumnNameEnable);
            NotifyOfPropertyChange(() => ColumnPriceEnable);
            NotifyOfPropertyChange(() => Records);
        }
        public void SelectComboKbn()
        {
            StatKbn = SelectedColumnKbn.Kbn;
            NotifyOfPropertyChange(() => SelectedColumnKbn);

            Window.Current.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                NotifyOfPropertyChange(() => ColumnKbns);
                NotifyOfPropertyChange(() => ColumnIdEnable);
                NotifyOfPropertyChange(() => ColumnNameEnable);
                NotifyOfPropertyChange(() => ColumnPriceEnable);
            });

            NotifyOfPropertyChange(() => Records);
        }

    }
}
