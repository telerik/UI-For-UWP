using System;
using System.IO;
using System.Threading.Tasks;
using Telerik.Core;
using Telerik.UI.Xaml.Controls.Grid.Primitives;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Telerik.UI.Xaml.Controls.Grid
{
    /// <summary>
    /// Represents a concrete <see cref="DataGridTypedColumn"/> that may present the associated data through <see cref="Image"/> controls.
    /// </summary>
    public class DataGridImageColumn : DataGridTypedColumn
    {
        private static readonly DependencyProperty IsImageOpenedProperty =
            DependencyProperty.RegisterAttached("IsImageOpened", typeof(bool), typeof(DataGridImageColumn), new PropertyMetadata(false));

        private static readonly Type ImageType = typeof(Image);

        internal override bool CanSort
        {
            get
            {
                return false;
            }
        }

        internal override bool CanFilter
        {
            get
            {
                // TODO: Update when FilterDescriptor on a per Column basis added
                return false;
            }
        }

        internal override bool CanGroupBy
        {
            get
            {
                return false;
            }
        }

        internal override bool CanEdit
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the type of the editor for the DataGridImageColumn that is visualized when entering in edit mode.
        /// </summary>
        /// <returns>The type of the editor.</returns>
        public override object GetEditorType(object item)
        {
            return ImageType;
        }

        /// <summary>
        /// Gets the type of the control visualized when the image column is not currently edited.
        /// </summary>
        /// <returns>The type of the control.</returns>
        public override object GetContainerType(object rowItem)
        {
            return ImageType;
        }

        /// <summary>
        /// Creates an instance of an Image visualized when the column is not edited.
        /// </summary>
        /// <returns>An instance of the control.</returns>
        public override object CreateContainer(object rowItem)
        {
            return new Image();
        }

        /// <summary>
        /// Creates an instance of an Image used by the column when entering edit mode.
        /// </summary>
        /// <returns>An instance of the editor.</returns>
        public override FrameworkElement CreateEditorContentVisual()
        {
            return new Image();
        }

        /// <summary>
        /// Prepares all bindings and content set to the Image visualized when entering edit mode.
        /// </summary>
        /// <param name="editorContent">The editor itself.</param>
        /// <param name="binding">The binding set to the editor of the cell.</param>
        public override void PrepareEditorContentVisual(FrameworkElement editorContent, Windows.UI.Xaml.Data.Binding binding)
        {
        }

        /// <summary>
        /// Clears all bindings and content set to the Image visualized when entering edit mode.
        /// </summary>
        /// <param name="editorContent">The editor itself.</param>
        public override void ClearEditorContentVisual(FrameworkElement editorContent)
        {
        }

        /// <inheritdoc/>
        public override void ClearCell(object container)
        {
            base.ClearCell(container);

            Image image = container as Image;
            if (image != null)
            {
                image.ImageOpened -= this.OnImageOpened;
                SetIsImageOpened(image, false);
            }
        }

        /// <inheritdoc/>
        public override async void PrepareCell(object container, object value, object item)
        {
            base.PrepareCell(container, value, item);

            var image = container as Image;
            if (image == null)
            {
                return;
            }

            if (value == null)
            {
                image.Source = null;
            }
            else if (value is ImageSource)
            {
                image.Source = value as ImageSource;
            }
            else if (value is string)
            {
                try
                {
                    var source = image.Source as BitmapImage;
                    var uri = new Uri((string)value, UriKind.RelativeOrAbsolute);
                    if (source == null)
                    {
                        source = new BitmapImage(uri);
                        image.Source = source;
                    }
                    else
                    {
                        source.UriSource = uri;
                    }
                }
                catch
                {
                    // TODO: What exceptions can be caught here?
                }
            }
            else if (value is byte[])
            {
                image.Source = await this.LoadImageFromBytes(value as byte[]);
            }

            if (!GetIsImageOpened(image))
            {
                image.ImageOpened += this.OnImageOpened;
            }
        }

        internal override Size MeasureCellContainer(double availableWidth, UIElement container)
        {
            var size = base.MeasureCellContainer(availableWidth, container);

            var image = container as Image;
            if (image != null)
            {
                var bitmapImage = image.Source as BitmapImage;
                if (bitmapImage != null && bitmapImage.PixelWidth > 0)
                {
                    size.Width = bitmapImage.PixelWidth;
                    size.Height = bitmapImage.PixelHeight;
                }
            }

            return size;
        }

        /// <summary>
        /// Returns null as the Image column may not be filtered by default.
        /// </summary>
        protected internal override DataGridFilterControlBase CreateFilterControl()
        {
            return null;
        }

        private static bool GetIsImageOpened(DependencyObject d)
        {
            return (bool)d.GetValue(IsImageOpenedProperty);
        }

        private static void SetIsImageOpened(DependencyObject d, bool value)
        {
            d.SetValue(IsImageOpenedProperty, value);
        }

        private async Task<BitmapImage> LoadImageFromBytes(byte[] bytes)
        {
            BitmapImage source = new BitmapImage();
            MemoryStream stream = new MemoryStream(bytes);
            source.SetSource(await stream.AsRandomAccessStreamAsync());

            return source;
        }

        private void OnImageOpened(object sender, RoutedEventArgs e)
        {
            var image = sender as Image;
            image.ImageOpened -= this.OnImageOpened;
            SetIsImageOpened(image, true);

            this.OnPropertyChange(UpdateFlags.AffectsContent);
        }
    }
}
