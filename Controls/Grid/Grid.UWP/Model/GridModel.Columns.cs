using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Telerik.Core;
using Telerik.Data.Core;
using Telerik.Data.Core.Fields;
using Telerik.UI.Xaml.Controls.Grid.Commands;

namespace Telerik.UI.Xaml.Controls.Grid.Model
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Will refactor at a later stage by using Aggregation")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    internal partial class GridModel
    {
        internal static readonly int AutoGenerateColumnsPropertyKey = PropertyKeys.Register(typeof(GridModel), "AutoGenerateColumns");

        internal static readonly int FrozenColumnCountPropertyKey = PropertyKeys.Register(typeof(GridModel), "FrozenColumnCount");

        internal DataGridColumnCollection columns;
        private bool areColumnsGenerated;

        public int FrozenColumnCount
        {
            get
            {
                return this.GetTypedValue<int>(FrozenColumnCountPropertyKey, 0);
            }
            set
            {
                this.SetValue(FrozenColumnCountPropertyKey, value);
            }
        }

        public bool AutoGenerateColumns
        {
            get
            {
                return this.GetTypedValue<bool>(AutoGenerateColumnsPropertyKey, true);
            }
            set
            {
                this.SetValue(AutoGenerateColumnsPropertyKey, value);
            }
        }

        public double VerticalBufferScale { get; internal set; }

        internal IFieldInfoData FieldInfoData
        {
            get
            {
                if (this.CurrentDataProvider != null)
                {
                    return this.CurrentDataProvider.FieldDescriptions;
                }

                return null;
            }
        }

        internal IEnumerable<DataGridColumn> VisibleColumns
        {
            get
            {
                return this.columns.Where(col => col.IsVisible);
            }
        }

        internal static void UpdateFilterMemberAccess(IFieldInfoData data, IEnumerable<FilterDescriptorBase> descriptors)
        {
            foreach (var filterDescriptor in descriptors)
            {
                var propertyFilterDescriptor = filterDescriptor as PropertyFilterDescriptor;
                if (propertyFilterDescriptor != null)
                {
                    IDataFieldInfo field = data.GetFieldDescriptionByMember(propertyFilterDescriptor.PropertyName);
                    if (field != null)
                    {
                        propertyFilterDescriptor.MemberAccess = field as IMemberAccess;
                    }
                }
                else
                {
                    var compositeFilter = filterDescriptor as CompositeFilterDescriptor;
                    if (compositeFilter != null)
                    {
                        UpdateFilterMemberAccess(data, compositeFilter.Descriptors);
                    }
                }
            }
        }

        internal void OnColumnsCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (e == null || this.updatingData)
            {
                return;
            }

            if (this.columns.Count == 0)
            {
                this.CleanUp();
            }
            else
            {
                this.columnLayout.SetSource(this.VisibleColumns);
                if (this.itemsSource != null && !this.isDataProviderUpdating &&
                    this.CurrentDataProvider.Status == DataProviderStatus.Ready &&
                    (this.rowLayout.ItemsSource == null || this.rowLayout.ItemsSource.Count == 0))
                {
                    this.SetRowLayoutSource();
                }

                var flags = (e.Action == NotifyCollectionChangedAction.Move) ?
                                                                              UpdateFlags.AffectsContent | UpdateFlags.AffectsColumnsWidth | UpdateFlags.AffectsDecorations :
                            UpdateFlags.AllButData;

                this.GridView.UpdateService.RegisterUpdate((int)flags);
            }
        }

        internal override void OnPropertyChanged(RadPropertyEventArgs e)
        {
            if (e != null && e.Key == AutoGenerateColumnsPropertyKey)
            {
                this.areColumnsGenerated = false;
                this.RefreshLayout();
            }
            else if (e != null && e.Key == RowHeightPropertyKey)
            {
                this.rowHeightIsNaN = double.IsNaN((double)e.NewValue);
            }
            else if (e != null && e.Key == FrozenColumnCountPropertyKey)
            {
                this.GridView.UpdateService.RegisterUpdate((int)UpdateFlags.AffectsContent);
            }

            base.OnPropertyChanged(e);
        }

        private void GenerateColumns()
        {
            // TODO: Reset this flag upon ItemSource change
            if (this.areColumnsGenerated)
            {
                return;
            }

            this.ClearAutoGeneratedColumns();
            this.BuildAutoGeneratedColumns();

            // loop through all the data descriptors and check whether they are compatible with the new items source
            this.CheckDataDescriptorsCompatibility();
        }

        private void ClearAutoGeneratedColumns()
        {
            for (int i = 0; i < this.columns.Count; i++)
            {
                if (this.columns[i].IsAutoGenerated)
                {
                    this.columns.RemoveAt(i--);
                }
            }
        }

        private void BuildAutoGeneratedColumns()
        {
            bool autoGenerate = this.AutoGenerateColumns;
            bool hasNonAutoGenerated = this.columns.Count > 0;

            this.columns.SuspendNotifications();

            foreach (FieldInfoNode fieldInfoNode in this.FieldInfoData.RootFieldInfo.Children)
            {
                if (autoGenerate)
                {
                    var column = this.CreateColumn(fieldInfoNode.FieldInfo);
                    if (column == null)
                    {
                        continue;
                    }

                    this.columns.Add(column);
                    if (hasNonAutoGenerated)
                    {
                        this.UpdateColumnFieldInfo(fieldInfoNode);
                    }
                }
                else
                {
                    this.UpdateColumnFieldInfo(fieldInfoNode);
                }
            }

            this.columnLayout.SetSource(this.VisibleColumns);
            this.areColumnsGenerated = true;

            UpdateFilterMemberAccess(this.FieldInfoData, this.filterDescriptors);

            this.columns.ResumeNotifications();
        }

        private DataGridColumn CreateColumn(IDataFieldInfo fieldInfo)
        {
            var context = new GenerateColumnContext();
            if (fieldInfo != null)
            {
                context.FieldInfo = fieldInfo;
            }

            this.GridView.CommandService.ExecuteCommand(CommandId.GenerateColumn, context);
            if (context.Result == null)
            {
                // user have not specified a result, we assume that this means to skip the current column
                return null;
            }

            DataGridTypedColumn typedColumn = context.Result as DataGridTypedColumn;
            if (typedColumn != null)
            {
                typedColumn.PropertyInfo = fieldInfo;
            }

            context.Result.IsAutoGenerated = true;

            return context.Result;
        }

        private void UpdateColumnFieldInfo(FieldInfoNode fieldInfoNode)
        {
            foreach (var column in this.columns)
            {
                var typedColumn = column as DataGridTypedColumn;
                if (typedColumn != null && !typedColumn.IsAutoGenerated && typedColumn.PropertyName == fieldInfoNode.FieldInfo.Name)
                {
                    typedColumn.PropertyInfo = fieldInfoNode.FieldInfo;
                }
            }
        }
    }
}