using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Telerik.Data.Core.Fields;
using Windows.UI.Xaml.Markup;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Represents a special <see cref="FilterDescriptorBase"/> that stores an arbitrary number of other <see cref="Descriptors"/> instances.
    /// The logical AND or OR operator is applied upon all composed filters to determine the result of the PassesFilter routine.
    /// </summary>
    [ContentProperty(Name = "Descriptors")]
    public class CompositeFilterDescriptor : FilterDescriptorBase
    {
        private DelegateFilterDescription engineDescription;
        private ObservableCollection<FilterDescriptorBase> descriptors;
        private LogicalOperator logicalOperator;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeFilterDescriptor" /> class.
        /// </summary>
        public CompositeFilterDescriptor()
        {
            this.descriptors = new ObservableCollection<FilterDescriptorBase>();
            this.descriptors.CollectionChanged += this.OnDescriptorsCollectionChanged;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeFilterDescriptor" /> class.
        /// </summary>
        /// <param name="filterDescriptors">The filter descriptors to add to the <see cref="Descriptors"/> collection.</param>
        public CompositeFilterDescriptor(IEnumerable<FilterDescriptorBase> filterDescriptors) : this()
        {
            if (filterDescriptors == null)
            {
                throw new ArgumentNullException(nameof(filterDescriptors));
            }

            foreach (var descriptor in filterDescriptors)
            {
                this.descriptors.Add(descriptor);
            }
        }

        /// <summary>
        /// Gets the collection with all the <see cref="FilterDescriptorBase"/> objects composed by this instance.
        /// </summary>
        public ObservableCollection<FilterDescriptorBase> Descriptors
        {
            get
            {
                return this.descriptors;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="LogicalOperator"/> value that defines whether logical conjunction or disjunction will be used for the PassesFilter routine.
        /// </summary>
        public LogicalOperator Operator
        {
            get
            {
                return this.logicalOperator;
            }
            set
            {
                if (this.logicalOperator == value)
                {
                    return;
                }

                this.logicalOperator = value;
                this.OnPropertyChanged();
            }
        }

        internal override DescriptionBase EngineDescription
        {
            get
            {
                if (this.engineDescription == null)
                {
                    this.engineDescription = new DelegateFilterDescription(this);
                    this.engineDescription.Filter = this.PassesFilter;
                }

                return this.engineDescription;
            }
        }

        internal override void AttachOverride()
        {
            base.AttachOverride();

            if (this.Host == null)
            {
                return;
            }

            if (this.Host.CurrentDataProvider == null ||
                this.Host.CurrentDataProvider.FieldDescriptions == null)
            {
                return;
            }

            UpdateFilterMemberAccess(this.Host.CurrentDataProvider.FieldDescriptions, this.descriptors);
        }

        /// <summary>
        /// Composes Where conditions for composite filters.
        /// </summary>
        internal override string SerializeToSQLiteWhereCondition()
        {
            if (this.descriptors.Count == 0)
            {
                return string.Empty;
            }

            string condition = string.Empty;
            string logicalOperatorString = string.Empty;

            if (this.logicalOperator == LogicalOperator.And)
            {
                logicalOperatorString = " AND ";
            }
            else
            {
                logicalOperatorString = " OR ";
            }

            int flag = 0;
            foreach (var filter in this.descriptors)
            {
                if (flag != 0)
                {
                    condition += logicalOperatorString;
                }
                condition += filter.SerializeToSQLiteWhereCondition();
                flag++;
            }

            return condition;
        }

        internal override bool PassesFilter(object item)
        {
            if (this.descriptors.Count == 0)
            {
                return true;
            }

            bool? passes = null;

            foreach (var filter in this.descriptors)
            {
                bool filterValue = filter.PassesFilter(item);
                if (this.logicalOperator == LogicalOperator.And)
                {
                    if (!passes.HasValue)
                    {
                        passes = filterValue;
                    }
                    else
                    {
                        passes = passes.Value && filterValue;
                    }
                }
                else if (filterValue)
                {
                    passes = true;
                }
            }

            return passes.GetValueOrDefault();
        }

        private static void UpdateFilterMemberAccess(IFieldInfoData data, IEnumerable<FilterDescriptorBase> descriptors)
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

        private void OnDescriptorsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.engineDescription != null)
            {
                this.engineDescription.RaiseFilterChanged();
            }
        }
    }
}