using System;
using System.Collections;
using System.ComponentModel;
using Telerik.Data.Core;

namespace Telerik.Data.Core
{
    internal sealed class DataSettings<TFilter, TGroup, TAggregate, TSort> : SettingsNode, IDataSettings, ISupportInitialize, INotifyPropertyChanged
        where TFilter : SettingsNode
        where TGroup : SettingsNode
        where TAggregate : SettingsNode
        where TSort : SettingsNode
    {
        private int aggregatesLevel;
        private DataAxis aggregatesPosition;

        internal DataSettings()
        {
            this.FilterDescriptions = new DescriptionsSettingsList<TFilter>(this);
            this.RowGroupDescriptions = new DescriptionsSettingsList<TGroup>(this);
            this.ColumnGroupDescriptions = new DescriptionsSettingsList<TGroup>(this);
            this.AggregateDescriptions = new DescriptionsSettingsList<TAggregate>(this);
            this.SortDescriptions = new DescriptionsSettingsList<TSort>(this);

            this.aggregatesLevel = -1;
            this.aggregatesPosition = DataAxis.Columns;
        }

        public event EventHandler<EventArgs> DescriptionsChanged;

        public SettingsNodeCollection<TFilter> FilterDescriptions { get; private set; }
        public SettingsNodeCollection<TGroup> RowGroupDescriptions { get; private set; }
        public SettingsNodeCollection<TGroup> ColumnGroupDescriptions { get; private set; }
        public SettingsNodeCollection<TAggregate> AggregateDescriptions { get; private set; }
        public SettingsNodeCollection<TSort> SortDescriptions { get; private set; }

        public IGroupFactory GroupFactory
        {
            get;
            internal set;
        }

        public int AggregatesLevel
        {
            get
            {
                return this.aggregatesLevel;
            }

            set
            {
                if (this.aggregatesLevel != value)
                {
                    this.aggregatesLevel = value;
                    this.OnPropertyChanged(nameof(this.AggregatesLevel));
                    this.NotifyLayoutChanged();
                    this.NotifyChange(new SettingsChangedEventArgs());
                }
            }
        }

        public DataAxis AggregatesPosition
        {
            get
            {
                return this.aggregatesPosition;
            }

            set
            {
                if (this.aggregatesPosition != value)
                {
                    this.aggregatesPosition = value;
                    this.OnPropertyChanged(nameof(this.AggregatesPosition));
                    this.NotifyLayoutChanged();
                    this.NotifyChange(new SettingsChangedEventArgs());
                }
            }
        }

        IList IDataSettings.FilterDescriptions
        {
            get
            {
                return this.FilterDescriptions;
            }
        }

        IList IDataSettings.RowGroupDescriptions
        {
            get
            {
                return this.RowGroupDescriptions;
            }
        }

        IList IDataSettings.ColumnGroupDescriptions
        {
            get
            {
                return this.ColumnGroupDescriptions;
            }
        }

        IList IDataSettings.AggregateDescriptions
        {
            get
            {
                return this.AggregateDescriptions;
            }
        }

        IList IDataSettings.SortDescriptions
        {
            get
            {
                return this.SortDescriptions;
            }
        }

        internal void NotifyLayoutChanged()
        {
            if (this.DescriptionsChanged != null)
            {
                this.DescriptionsChanged(this, EventArgs.Empty);
            }
        }

        protected override Cloneable CreateInstanceCore()
        {
            return new DataSettings<TFilter, TGroup, TAggregate, TSort>();
        }

        protected override void CloneCore(Cloneable source)
        {
            var original = source as DataSettings<TFilter, TGroup, TAggregate, TSort>;
            if (original != null)
            {
                this.FilterDescriptions.CloneItemsFrom(original.FilterDescriptions);
                this.RowGroupDescriptions.CloneItemsFrom(original.RowGroupDescriptions);
                this.ColumnGroupDescriptions.CloneItemsFrom(original.ColumnGroupDescriptions);
                this.AggregateDescriptions.CloneItemsFrom(original.AggregateDescriptions);
            }
        }

        /// <summary>
        /// SettingsNodeCollection with notification rerouting.
        /// </summary>
        private sealed class DescriptionsSettingsList<T> : SettingsNodeCollection<T>
            where T : SettingsNode
        {
            private DataSettings<TFilter, TGroup, TAggregate, TSort> parent;

            public DescriptionsSettingsList(DataSettings<TFilter, TGroup, TAggregate, TSort> parent)
                : base(parent)
            {
                this.parent = parent;
            }

            protected override void InsertItem(int index, T item)
            {
                base.InsertItem(index, item);
                this.NotifyChange(new SettingsChangedEventArgs());
                this.parent.NotifyLayoutChanged();
            }

            protected override void SetItem(int index, T item)
            {
                base.SetItem(index, item);
                this.NotifyChange(new SettingsChangedEventArgs());
                this.parent.NotifyLayoutChanged();
            }

            protected override void RemoveItem(int index)
            {
                base.RemoveItem(index);
                this.NotifyChange(new SettingsChangedEventArgs());
                this.parent.NotifyLayoutChanged();
            }

            protected override void ClearItems()
            {
                base.ClearItems();
                this.NotifyChange(new SettingsChangedEventArgs());
                this.parent.NotifyLayoutChanged();
            }
        }
    }
}