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

        internal override object GetEditorType(object item)
        {
            return ImageType;
        }

        internal override object GetContainerType(object rowItem)
        {
            return ImageType;
        }

        internal override object CreateContainer(object rowItem)
        {
            return new Image();
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

        internal override void ClearCell(GridCellModel cell)
        {
            base.ClearCell(cell);

            Image image = cell.Container as Image;
            if (image != null)
            {
                image.ImageOpened -= this.OnImageOpened;
                SetIsImageOpened(image, false);
            }
        }

        internal override async void PrepareCell(GridCellModel cell)
        {
            base.PrepareCell(cell);

            var image = cell.Container as Image;
            if (image == null)
            {
                return;
            }

            if (cell.Value == null)
            {
                image.Source = null;
            }
            else if (cell.Value is ImageSource)
            {
                image.Source = cell.Value as ImageSource;
            }
            else if (cell.Value is string)
            {
                try
                {
                    var source = image.Source as BitmapImage;
                    var uri = new Uri((string)cell.Value, UriKind.RelativeOrAbsolute);
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
            else if (cell.Value is byte[])
            {
                image.Source = await this.LoadImageFromBytes(cell.Value as byte[]);
            }

            if (!GetIsImageOpened(image))
            {
                image.ImageOpened += this.OnImageOpened;
            }
        }

        internal override FrameworkElement CreateEditorContentVisual()
        {
            return new Image();
        }

        internal override void PrepareEditorContentVisual(FrameworkElement editorContent, Windows.UI.Xaml.Data.Binding binding)
        {
        }

        internal override void ClearEditorContentVisual(FrameworkElement editorContent)
        {
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
