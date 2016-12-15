using System;
using System.Linq;
using Telerik.Core;
using Windows.Foundation;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Represents an extended control that aggregates a <see cref="Image"/> instance and allows the user to pinch/pan the displayed picture.
    /// </summary>
    public class RadPanAndZoomImage : RadControl
    {
        /// <summary>
        /// Identifies the <see cref="Source"/> property.
        /// </summary>
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(ImageSource), typeof(RadPanAndZoomImage), new PropertyMetadata(null, OnSourceChanged));

        public static readonly DependencyProperty MinZoomProperty =
            DependencyProperty.Register("MinZoom", typeof(double), typeof(RadPanAndZoomImage), new PropertyMetadata(0.5, OnMinZoomChanged));

        public static readonly DependencyProperty MaxZoomProperty =
            DependencyProperty.Register("MaxZoom", typeof(double), typeof(RadPanAndZoomImage), new PropertyMetadata(2d, OnMaxZoomChanged));

        public static readonly DependencyProperty OffsetMarginScaleProperty =
            DependencyProperty.Register("OffsetMarginScale", typeof(Thickness), typeof(RadPanAndZoomImage), new PropertyMetadata(new Thickness(1), OnOffsetMarginScaleChanged));

        public static readonly DependencyProperty ImageMarginProperty =
            DependencyProperty.Register("ImageMargin", typeof(Thickness), typeof(RadPanAndZoomImage), new PropertyMetadata(new Thickness(0)));

        public static readonly DependencyProperty ImageStretchProperty =
            DependencyProperty.Register("ImageStretch", typeof(Stretch), typeof(RadPanAndZoomImage), new PropertyMetadata(Stretch.Uniform));

        public static readonly DependencyProperty ZoomModeProperty =
            DependencyProperty.Register("ZoomMode", typeof(ImageZoomMode), typeof(RadPanAndZoomImage), new PropertyMetadata(ImageZoomMode.Free, OnZoomModeChanged));

        public static readonly DependencyProperty ResetOnDoubleTapProperty =
            DependencyProperty.Register("ResetOnDoubleTap", typeof(bool), typeof(RadPanAndZoomImage), new PropertyMetadata(false, OnResetOnDoubleTapChanged));

        public static readonly DependencyProperty GestureSettingsProperty =
            DependencyProperty.Register("GestureSettings", typeof(GestureSettings), typeof(RadPanAndZoomImage),
                new PropertyMetadata(GestureSettings.ManipulationTranslateX |
                                     GestureSettings.ManipulationTranslateY |
                                     GestureSettings.ManipulationRotate |
                                     GestureSettings.ManipulationScale |
                                     GestureSettings.ManipulationMultipleFingerPanning, OnGestureSettingsChanged));//reduces zoom jitter when panning with multiple fingers

        private BitmapImage source;
        private ManipulationInputProcessor manipulationInputProcessor;
        private Image image;
        private Panel manupulationPanel;
        private bool imageDownloaded;

        public RadPanAndZoomImage()
        {
            this.DefaultStyleKey = typeof(RadPanAndZoomImage);
        }

        public event EventHandler<ExceptionRoutedEventArgs> ImageFailed;

        public event EventHandler<RoutedEventArgs> ImageOpened;

        public bool ResetOnDoubleTap
        {
            get
            {
                return (bool)this.GetValue(ResetOnDoubleTapProperty);
            }
            set
            {
                this.SetValue(ResetOnDoubleTapProperty, value);
            }
        }

        public ManipulationInputProcessor ManipulationProcessor
        {
            get
            {
                return this.manipulationInputProcessor;
            }
        }

        public Thickness ImageMargin
        {
            get
            {
                return (Thickness)this.GetValue(ImageMarginProperty);
            }
            set
            {
                this.SetValue(ImageMarginProperty, value);
            }
        }

        public Stretch ImageStretch
        {
            get
            {
                return (Stretch)this.GetValue(ImageStretchProperty);
            }
            set
            {
                this.SetValue(ImageStretchProperty, value);
            }
        }

        public double MinZoom
        {
            get
            {
                return (double)this.GetValue(MinZoomProperty);
            }
            set
            {
                this.SetValue(MinZoomProperty, value);
            }
        }

        public double MaxZoom
        {
            get
            {
                return (double)this.GetValue(MaxZoomProperty);
            }
            set
            {
                this.SetValue(MaxZoomProperty, value);
            }
        }

        public Thickness OffsetMarginScale
        {
            get
            {
                return (Thickness)this.GetValue(OffsetMarginScaleProperty);
            }
            set
            {
                this.SetValue(OffsetMarginScaleProperty, value);
            }
        }

        public ImageZoomMode ZoomMode
        {
            get
            {
                return (ImageZoomMode)this.GetValue(ZoomModeProperty);
            }
            set
            {
                this.SetValue(ZoomModeProperty, value);
            }
        }

        public ImageSource Source
        {
            get
            {
                return this.GetValue(SourceProperty) as ImageSource;
            }
            set
            {
                this.SetValue(SourceProperty, value);
            }
        }

        public GestureSettings GestureSettings
        {
            get
            {
                return (GestureSettings)this.GetValue(GestureSettingsProperty);
            }
            set
            {
                this.SetValue(GestureSettingsProperty, value);
            }
        }

        public Rect GetImageOriginalRect()
        {
            return this.manipulationInputProcessor.ElementOriginalRect;
        }

        protected override bool ApplyTemplateCore()
        {
            var isApplied = base.ApplyTemplateCore();

            this.image = this.GetTemplateChild("PART_Image") as Image;
            this.manupulationPanel = this.GetTemplateChild("PART_ManipulationPanel") as Panel;

            isApplied &= this.image != null && this.manupulationPanel != null;

            if (isApplied)
            {
                this.image.ImageFailed += this.OnImageImageFailed;
                this.image.ImageOpened += this.OnImageOpened;
                this.image.SizeChanged += this.OnImageSizeChanged;

                this.manipulationInputProcessor = new ManipulationInputProcessor(this.image, this.manupulationPanel, this.GestureSettings);

                return true;
            }

            return false;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var size = base.ArrangeOverride(finalSize);
            this.manupulationPanel.Clip = new RectangleGeometry { Rect = new Rect(0, 0, size.Width, size.Height) };
            return size;
        }

        private static void OnMinZoomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as RadPanAndZoomImage;
            if (control.manipulationInputProcessor != null)
            {
                control.manipulationInputProcessor.MinZoom = (double)e.NewValue;
            }
        }

        private static void OnMaxZoomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as RadPanAndZoomImage;
            if (control.manipulationInputProcessor != null)
            {
                control.manipulationInputProcessor.MaxZoom = (double)e.NewValue;
            }
        }

        private static void OnOffsetMarginScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as RadPanAndZoomImage;
            if (control.manipulationInputProcessor != null)
            {
                control.manipulationInputProcessor.ScaleOffsets = (Thickness)e.NewValue;
            }
        }

        private static void OnResetOnDoubleTapChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as RadPanAndZoomImage;
            if (control.manipulationInputProcessor != null)
            {
                control.manipulationInputProcessor.ResetOnDoubleTap = (bool)e.NewValue;
            }
        }

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as RadPanAndZoomImage).OnSourceChanged((ImageSource)e.NewValue, (ImageSource)e.OldValue);
        }

        private static void OnZoomModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as RadPanAndZoomImage;
            if (control.manipulationInputProcessor != null)
            {
                control.manipulationInputProcessor.MinZoom = control.GetMinZoom();
                control.manipulationInputProcessor.MaxZoom = control.GetMaxZoom();
            }
        }

        private static void OnGestureSettingsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as RadPanAndZoomImage;
            if (control.manipulationInputProcessor != null)
            {
                control.manipulationInputProcessor.GestureSettings = (GestureSettings)e.NewValue;
            }
        }

        private void OnSourceChanged(ImageSource newValue, ImageSource oldValue)
        {
            this.imageDownloaded = false;

            if (oldValue is BitmapImage)
            {
                (oldValue as BitmapImage).DownloadProgress -= this.OnDownloadProgress;
            }

            this.source = newValue as BitmapImage;
            if (this.source != null)
            {
                if (this.source.PixelHeight > 0 || this.source.PixelWidth > 0)
                {
                    this.imageDownloaded = true;
                    this.OnImageOpened(this.image, new RoutedEventArgs());
                }
                else
                {
                    this.source.DownloadProgress += this.OnDownloadProgress;
                }
            }
            else if (newValue is WriteableBitmap)
            {
                this.imageDownloaded = true;
            }
        }

        private void OnDownloadProgress(object sender, DownloadProgressEventArgs e)
        {
            if (e.Progress == 100)
            {
                return;
            }
        }

        private void OnImageOpened(object sender, RoutedEventArgs e)
        {
            this.imageDownloaded = true;
            this.manipulationInputProcessor.InitializeTransforms(this.GetMinZoom(), this.GetMaxZoom(), this.OffsetMarginScale, this.ResetOnDoubleTap);
            var handler = this.ImageOpened;
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        private double GetMaxZoom()
        {
            if (this.ZoomMode == ImageZoomMode.None)
            {
                return 1;
            }

            return this.MaxZoom;
        }

        private double GetMinZoom()
        {
            if (this.ZoomMode == ImageZoomMode.None)
            {
                return 1;
            }

            return this.MinZoom;
        }

        private void OnImageImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            this.imageDownloaded = false;

            var handler = this.ImageFailed;
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        private void OnImageSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.Source == null)
            {
                return;
            }

            this.imageDownloaded = true;
            this.manipulationInputProcessor.InitializeTransforms(this.GetMinZoom(), this.GetMaxZoom(), this.OffsetMarginScale, this.ResetOnDoubleTap);
        }
    }
}