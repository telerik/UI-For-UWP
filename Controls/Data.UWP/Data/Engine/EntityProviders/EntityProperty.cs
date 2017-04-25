using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Windows.UI.Xaml.Data;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Represents a base model of property of Entity with ability to extract its metadata, commit and revert data.
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public abstract class EntityProperty : INotifyPropertyChanged
    {
        private string label;
        private object propertyValue;
        private ObservableCollection<object> errors;
        private object postitiveMessage;
        private bool displayPositiveMessage;
        private string watermark;
        private bool isValid = true;

        private INumericalRange range;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityProperty"/> class.
        /// </summary>
        /// <param name="propertyContext">The property info that object is associated with.</param>
        /// <param name="item">The data item.</param>
        /// <param name="converter">The converter. Optionally you can convert your source property to a different format depending on the UI and scenario.</param>
        public EntityProperty(object propertyContext, object item, IPropertyConverter converter)
        {
            this.PropertyContext = propertyContext;
            this.Errors = new ObservableCollection<object>();
            this.DataItem = item;
            this.PropertyConverter = converter;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityProperty"/> class.
        /// </summary>
        /// <param name="property">The property info that object is associated with.</param>
        /// <param name="item">The data item.</param>
        public EntityProperty(object property, object item) : this(property, item, null)
        {
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the property converter used to convert source data to property value.
        /// </summary>
        /// <value>
        /// The property converter.
        /// </value>
        public IPropertyConverter PropertyConverter
        {
            get; private set;
        }

        /// <summary>
        /// Gets the parent entity the property is associated with.
        /// </summary>
        /// <value>
        /// The entity.
        /// </value>
        public Entity Entity { get; internal set; }

        /// <summary>
        /// Gets or sets the possible value options. Used to provide the UI like selectors available options.
        /// </summary>
        /// <value>
        /// The value options.
        /// </value>
        public IList ValueOptions { get; set; }

        /// <summary>
        /// Gets or sets the range of a property.
        /// </summary>
        /// <value>
        /// The range.
        /// </value>
        public INumericalRange Range
        {
            get
            {
                return this.range;
            }
            set
            {
                this.range = value;
                this.OnPropertyChanged(nameof(this.Range));
            }
        }

        /// <summary>
        /// Gets or sets the property value after converter is applied.
        /// </summary>
        /// <value>
        /// The property value.
        /// </value>
        public virtual object PropertyValue
        {
            get
            {
                return this.propertyValue;
            }
            set
            {
                this.propertyValue = value;

                this.DisplayPositiveMessage = false;

                this.OnPropertyChanged(nameof(this.PropertyValue));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether positive message should be displayed.
        /// </summary>
        /// <value>
        /// <c>true</c> if message should be displayed; otherwise, <c>false</c>.
        /// </value>
        public bool DisplayPositiveMessage
        {
            get
            {
                return this.displayPositiveMessage;
            }
            set
            {
                this.displayPositiveMessage = value;
                this.OnPropertyChanged(nameof(this.DisplayPositiveMessage));
            }
        }

        /// <summary>
        /// Gets or sets the property label.
        /// </summary>
        /// <value>
        /// The label.
        /// </value>
        public string Label
        {
            get
            {
                return this.label;
            }
            set
            {
                this.label = value;
                this.OnPropertyChanged(nameof(this.Label));
            }
        }

        /// <summary>
        /// Gets or sets the positive message.
        /// </summary>
        /// <value>
        /// The positive message.
        /// </value>
        public object PositiveMessage
        {
            get
            {
                return this.postitiveMessage;
            }
            set
            {
                this.postitiveMessage = value;
                this.OnPropertyChanged(nameof(this.PositiveMessage));
            }
        }

        /// <summary>
        /// Gets or sets the watermark.
        /// </summary>
        /// <value>
        /// The watermark.
        /// </value>
        public string Watermark
        {
            get
            {
                return this.watermark;
            }
            set
            {
                this.watermark = value;
                this.OnPropertyChanged(nameof(this.Watermark));
            }
        }

        /// <summary>
        /// Gets or sets the name of the property.
        /// </summary>
        /// <value>
        /// The name of the property.
        /// </value>
        public string PropertyName { get; set; }

        /// <summary>
        /// Gets or sets the type of the property.
        /// </summary>
        /// <value>
        /// The type of the property.
        /// </value>
        public Type PropertyType { get; set; }

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        /// <value>
        /// The index.
        /// </value>
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the property is read only.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is read only; otherwise, <c>false</c>.
        /// </value>
        public bool IsReadOnly { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is requires value set.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is required; otherwise, <c>false</c>.
        /// </value>
        public bool IsRequired { get; set; }

        /// <summary>
        /// Gets the errors after validation is triggered.
        /// </summary>
        /// <value>
        /// The errors.
        /// </value>
        public ObservableCollection<object> Errors
        {
            get
            {
                return this.errors;
            }
            internal set
            {
                if (this.errors != null)
                {
                    this.errors.CollectionChanged -= this.Errors_CollectionChanged;
                }

                this.errors = value;

                if (this.errors != null)
                {
                    this.errors.CollectionChanged += this.Errors_CollectionChanged;
                }

                this.OnPropertyChanged(nameof(this.Errors));

                this.IsValid = this.errors.Count == 0;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the value set valid - returns true if the value set is valid.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </value>
        public bool IsValid
        {
            get
            {
                return this.isValid;
            }
            set
            {
                if (this.isValid != value)
                {
                    this.isValid = value;
                    this.OnPropertyChanged(nameof(this.IsValid));
                }
            }
        }

        /// <summary>
        /// Gets or sets the group key.
        /// </summary>
        /// <value>
        /// The group key.
        /// </value>
        public string GroupKey
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the source data item.
        /// </summary>
        /// <value>
        /// The data item.
        /// </value>
        public object DataItem
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Gets the property info.
        /// </summary>
        protected object PropertyContext { get; private set; }

        /// <summary>
        /// Commits the value to the source item.
        /// </summary>
        public abstract void Commit();
        
        /// <summary>
        /// Populates the property metadata from the property context.
        /// </summary>
        public virtual void PopulatePropertyMetadata()
        {
            this.PropertyType = this.GetPropertyType(this.PropertyContext);
            this.PropertyName = this.GetPropertyName(this.PropertyContext);

            this.Label = this.GetLabel(this.PropertyContext);
            this.Watermark = this.GetWatermark(this.PropertyContext);
            this.IsReadOnly = this.GetIsReadOnly(this.PropertyContext);
            this.GroupKey = this.GetPropertyGroupKey(this.PropertyContext);
            this.Index = this.GetPropertyIndex(this.PropertyContext);

            var originalValue = this.PropertyConverter != null ? this.PropertyConverter.Convert(this.GetOriginalValue()) : this.GetOriginalValue();
            this.PropertyValue = originalValue;
            this.IsRequired = this.GetIsRequired(this.PropertyContext);
            this.ValueOptions = this.GetValueOptions(this.PropertyContext);
            this.Range = this.GetValueRange(this.PropertyContext);
        }

        /// <summary>
        /// Gets the original value of source object.
        /// </summary>
        public abstract object GetOriginalValue();

        /// <summary>
        /// Gets the property value is required based on the property context.
        /// </summary>
        /// <param name="property">The property.</param>
        protected abstract bool GetIsRequired(object property);

        /// <summary>
        /// Gets the index of the property based on the property context.
        /// </summary>
        /// <param name="property">The property.</param>
        protected abstract int GetPropertyIndex(object property);

        /// <summary>
        /// Gets the property group key based on the property context.
        /// </summary>
        /// <param name="property">The property.</param>
        protected abstract string GetPropertyGroupKey(object property);

        /// <summary>
        /// Gets the name of the property based on the property context.
        /// </summary>
        /// <param name="property">The property.</param>
        protected abstract string GetPropertyName(object property);

        /// <summary>
        /// Gets the type of the property based on the property context.
        /// </summary>
        /// <param name="property">The property.</param>
        protected abstract Type GetPropertyType(object property);

        /// <summary>
        /// Gets the label based on the property context.
        /// </summary>
        /// <param name="property">The property.</param>
        protected abstract string GetLabel(object property);

        /// <summary>
        /// Gets the watermark based on the property context.
        /// </summary>
        /// <param name="property">The property.</param>
        protected abstract string GetWatermark(object property);

        /// <summary>
        /// Gets is the property is read-only based on the property context.
        /// </summary>
        /// <param name="property">The property.</param>
        protected abstract bool GetIsReadOnly(object property);

        /// <summary>
        /// Gets the available value options based on the property context.
        /// </summary>
        /// <param name="property">The property.</param>
        protected abstract IList GetValueOptions(object property);

        /// <summary>
        /// Gets the value range based on the property context.
        /// </summary>
        /// <param name="property">The property.</param>
        protected abstract INumericalRange GetValueRange(object property);

        /// <summary>
        /// Called when property changed.
        /// </summary>
        /// <param name="name">The name of property changed.</param>
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        private void Errors_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.IsValid = this.errors.Count == 0;
        }
    }
}