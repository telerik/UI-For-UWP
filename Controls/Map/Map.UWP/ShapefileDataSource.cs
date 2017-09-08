using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Geospatial;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Map
{
    /// <summary>
    /// Represents a <see cref="IShapeDataSource"/> implementation that loads and parses shape files.
    /// The default transport resolver is set to a <see cref="LocalTransportResolver"/> instance, allowing loading of application-defined files, reachable through the "ms-appx" and "ms-appdata" Uri schema.
    /// </summary>
    ////TODO: Review trigger condition for shapefile parsing -- currently only Uri change triggers new parsing operation.
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed.")]
    public class ShapefileDataSource : RadDependencyObject, IShapeDataSource
    {
        /// <summary>
        /// Identifies the <see cref="SourceUri"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SourceUriProperty =
            DependencyProperty.Register(nameof(SourceUri), typeof(Uri), typeof(ShapefileDataSource), new PropertyMetadata(null, OnUriPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="DataSourceUri"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DataSourceUriProperty =
            DependencyProperty.Register(nameof(DataSourceUri), typeof(Uri), typeof(ShapefileDataSource), new PropertyMetadata(null, OnUriPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="SourceUriString"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SourceUriStringProperty =
            DependencyProperty.Register(nameof(SourceUriString), typeof(string), typeof(ShapefileDataSource), new PropertyMetadata(null, OnSourceUriStringPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="DataSourceUriString"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DataSourceUriStringProperty =
            DependencyProperty.Register(nameof(DataSourceUriString), typeof(string), typeof(ShapefileDataSource), new PropertyMetadata(null, OnDataSourceUriStringPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="Encoding"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EncodingProperty =
            DependencyProperty.Register(nameof(Encoding), typeof(Encoding), typeof(ShapefileDataSource), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="TransportResolver"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TransportResolverProperty =
            DependencyProperty.Register(nameof(TransportResolver), typeof(ITransportResolver), typeof(ShapefileDataSource), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="AttributesToLoad"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AttributesToLoadProperty =
            DependencyProperty.Register(nameof(AttributesToLoad), typeof(string), typeof(ShapefileDataSource), new PropertyMetadata(null, OnAttributesToLoadPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="AttributeValueConverter"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AttributeValueConverterProperty =
            DependencyProperty.Register(nameof(AttributeValueConverter), typeof(IAttributeValueConverter), typeof(ShapefileDataSource), new PropertyMetadata(null, OnAttributeValueConverterPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="CoordinateValueConverter"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CoordinateValueConverterProperty =
            DependencyProperty.Register(nameof(CoordinateValueConverter), typeof(ICoordinateValueConverter), typeof(ShapefileDataSource), new PropertyMetadata(null, OnCoordinateValueConverterPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="Shapes"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShapesProperty =
            DependencyProperty.Register(ShapesPropertyName, typeof(MapShapeModelCollection), typeof(ShapefileDataSource), new PropertyMetadata(null, OnShapesPropertyChanged));

        internal const string ShapesPropertyName = "Shapes";

        private readonly string[] separators = { ",", ".", ";", ":", " ", "|" };

        private ShapefileReader reader;
        private bool shapeProcessingScheduled = false;
        private PropertyChangedEventHandler propertyChangedHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShapefileDataSource"/> class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        public ShapefileDataSource()
        {
            this.TransportResolver = new LocalTransportResolver();
            this.reader = new ShapefileReader();
        }

        /// <summary>
        /// Occurs when the asynchronous processing of the file has been completed.
        /// </summary>
        public event EventHandler ShapeProcessingCompleted;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add
            {
                this.propertyChangedHandler += value;
            }
            remove
            {
                this.propertyChangedHandler -= value;
            }
        }

        /// <summary>
        /// Gets or sets the Uri that points to the file to read data from.
        /// </summary>
        public Uri SourceUri
        {
            get
            {
                return (Uri)this.GetValue(SourceUriProperty);
            }
            set
            {
                this.SetValue(SourceUriProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Uri that points to the file, containing the data (or attributes) for each shape within the shape file.
        /// </summary>
        public Uri DataSourceUri
        {
            get
            {
                return (Uri)this.GetValue(DataSourceUriProperty);
            }
            set
            {
                this.SetValue(DataSourceUriProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the string representation of the <see cref="P:SourceUri"/> value. Primarily used to enable declarative Uri definition.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Used to enable declarative setup.")]
        public string SourceUriString
        {
            get
            {
                return (string)this.GetValue(SourceUriStringProperty);
            }
            set
            {
                this.SetValue(SourceUriStringProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the string representation of the <see cref="P:DataSourceUri"/> value. Primarily used to enable declarative Uri definition.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Used to enable declarative setup.")]
        public string DataSourceUriString
        {
            get
            {
                return (string)this.GetValue(DataSourceUriStringProperty);
            }
            set
            {
                this.SetValue(DataSourceUriStringProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the comma-delimited attributes to load from the specified <see cref="DataSourceUri"/> file. Useful when the data file contains large amount of attributes and only a small subset of them is required.
        /// </summary>
        public string AttributesToLoad
        {
            get
            {
                return (string)this.GetValue(AttributesToLoadProperty);
            }
            set
            {
                this.SetValue(AttributesToLoadProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="IAttributeValueConverter"/> implementation that allows specific attribute values to be handled explicitly.
        /// </summary>
        public IAttributeValueConverter AttributeValueConverter
        {
            get
            {
                return (IAttributeValueConverter)this.GetValue(AttributeValueConverterProperty);
            }
            set
            {
                this.SetValue(AttributeValueConverterProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="ICoordinateValueConverter"/> implementation that allows the <see cref="Location"/> value of each processed shape to be modified externally.
        /// </summary>
        public ICoordinateValueConverter CoordinateValueConverter
        {
            get
            {
                return (ICoordinateValueConverter)this.GetValue(CoordinateValueConverterProperty);
            }
            set
            {
                this.SetValue(CoordinateValueConverterProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Encoding"/> of the file to read.
        /// </summary>
        public Encoding Encoding
        {
            get
            {
                return (Encoding)this.GetValue(EncodingProperty);
            }
            set
            {
                this.SetValue(EncodingProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the current <see cref="ITransportResolver"/> implementation. Defaults to a <see cref="LocalTransportResolver"/> instance.
        /// </summary>
        public ITransportResolver TransportResolver
        {
            get
            {
                return (ITransportResolver)this.GetValue(TransportResolverProperty);
            }
            set
            {
                this.SetValue(TransportResolverProperty, value);
            }
        }

        /// <summary>
        /// Gets the collection of <see cref="IMapShape"/> instances that represents the final result of the file processing.
        /// </summary>
        public MapShapeModelCollection Shapes
        {
            get
            {
                return (MapShapeModelCollection)this.GetValue(ShapesProperty);
            }
        }

        internal bool IsInTestMode
        {
            get;
            set;
        }

        /// <summary>
        /// This method is exposed for testing purposes only, do not use it outside the testing project.
        /// </summary>
        internal void LoadShapesSynchronously()
        {
            var context = new ShapefileContext()
            {
                DataSourceUri = this.DataSourceUri,
                Encoding = this.Encoding,
                SourceUri = this.SourceUri,
                Transport = this.TransportResolver
            };

            MapShapeModelCollection collection = null;
            Task.Run(async () =>
                {
                    collection = await this.LoadShapes(context);
                }).Wait();

            this.ChangePropertyInternally(ShapesProperty, collection);
        }

        private static void OnUriPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var datasource = (ShapefileDataSource)sender;

            if (!datasource.shapeProcessingScheduled && !Windows.ApplicationModel.DesignMode.DesignModeEnabled && !datasource.IsInTestMode)
            {
                var warningSuppresion = datasource.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => datasource.ProcessShapes());
                datasource.shapeProcessingScheduled = true;
            }
        }

        private static void OnSourceUriStringPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var datasource = (ShapefileDataSource)sender;
            datasource.SourceUri = new Uri(datasource.SourceUriString);
        }

        private static void OnDataSourceUriStringPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var datasource = (ShapefileDataSource)sender;
            datasource.DataSourceUri = new Uri(datasource.DataSourceUriString);
        }

        private static void OnAttributeValueConverterPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var dataSource = (ShapefileDataSource)sender;

            dataSource.reader.AttributeValueConverter = dataSource.AttributeValueConverter;
        }

        private static void OnCoordinateValueConverterPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var dataSource = (ShapefileDataSource)sender;

            dataSource.reader.CoordinateValueConverter = dataSource.CoordinateValueConverter;
        }

        private static void OnAttributesToLoadPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var datasource = (ShapefileDataSource)sender;

            List<string> splitAttributes = null;
            if (!string.IsNullOrEmpty(datasource.AttributesToLoad))
            {
                splitAttributes = SplitAttributesToLoad(datasource.AttributesToLoad, datasource.separators);
            }

            datasource.reader.AttributesToLoad = splitAttributes;
        }

        private static List<string> SplitAttributesToLoad(string attributesToSplit, string[] separators)
        {
            var attributes = attributesToSplit.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            if (attributes.Length == 0)
            {
                return null;
            }

            for (int i = 0; i < attributes.Length; i++)
            {
                attributes[i] = attributes[i].Trim();
            }

            return attributes.ToList();
        }

        private static void OnShapesPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var datasource = (ShapefileDataSource)sender;
            if (!datasource.IsInternalPropertyChange)
            {
                throw new InvalidOperationException("The Property Shapes is readonly.");
            }

            var shapeProcessingHandler = datasource.ShapeProcessingCompleted;
            if (shapeProcessingHandler != null)
            {
                shapeProcessingHandler(datasource, EventArgs.Empty);
            }

            if (datasource.propertyChangedHandler != null)
            {
                datasource.propertyChangedHandler(datasource, new PropertyChangedEventArgs("Shapes"));
            }
        }

        private async void ProcessShapes()
        {
            this.shapeProcessingScheduled = false;
            this.reader.CancelOperation();

            var context = new ShapefileContext()
            {
                DataSourceUri = this.DataSourceUri,
                Encoding = this.Encoding,
                SourceUri = this.SourceUri,
                Transport = this.TransportResolver
            };

            var collection = await this.LoadShapes(context);
            this.ChangePropertyInternally(ShapesProperty, collection);
        }

        private async Task<MapShapeModelCollection> LoadShapes(object context)
        {
            var shapeContext = context as ShapefileContext;
            if (shapeContext.SourceUri == null || shapeContext.Transport == null)
            {
                return null;
            }

            StorageFile shapeFile = await shapeContext.Transport.GetStorageFile(shapeContext.SourceUri);
            if (shapeFile == null)
            {
                return null;
            }

            StorageFile dataFile = null;
            if (shapeContext.DataSourceUri != null)
            {
                dataFile = await shapeContext.Transport.GetStorageFile(shapeContext.DataSourceUri);
            }

            using (var shapeStream = await shapeFile.OpenAsync(FileAccessMode.Read))
            {
                try
                {
                    if (dataFile != null)
                    {
                        using (var dataStream = await dataFile.OpenAsync(FileAccessMode.Read))
                        {
                            return await this.reader.Read(shapeStream, dataStream, shapeContext.Encoding);
                        }
                    }
                    else
                    {
                        return await this.reader.Read(shapeStream, null, shapeContext.Encoding);
                    }
                }
                catch (TaskCanceledException)
                {
                    return null;
                }
            }
        }

        private class ShapefileContext
        {
            public Uri SourceUri;
            public Uri DataSourceUri;
            public ITransportResolver Transport;
            public Encoding Encoding;
        }
    }
}
