using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Telerik.Core;
using Telerik.UI.Xaml.Controls.Input.ImageEditor;
using Telerik.UI.Xaml.Controls.Input.ImageEditor.Commands;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Linq;

namespace Telerik.UI.Xaml.Controls.Input
{
    [ContentProperty(Name = "ToolGroups")]
    public class RadImageEditor : RadControl, IToolValueProvider
    {

        //TODO: Rename
        public ImageEditorToolsPosition ToolsPosition
        {
            get { return (ImageEditorToolsPosition)GetValue(ToolsPositionProperty); }
            set { SetValue(ToolsPositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ToolsPosition.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ToolsPositionProperty =
            DependencyProperty.Register("ToolsPosition", typeof(ImageEditorToolsPosition), typeof(RadImageEditor), new PropertyMetadata(ImageEditorToolsPosition.Bottom));

        
        public ImageEditorLayerSelector LayerSelector
        {   
            get { return (ImageEditorLayerSelector)GetValue(LayerSelectorProperty); }
            set { SetValue(LayerSelectorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LayerSelector.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LayerSelectorProperty =
            DependencyProperty.Register("LayerSelector", typeof(ImageEditorLayerSelector), typeof(RadImageEditor), new PropertyMetadata(new ImageEditorLayerSelector()));

        
        internal ImageEditorToolFactory DefaultToolFactory
        {
            get { return (ImageEditorToolFactory)GetValue(DefaultToolFactoryProperty); }
            set { SetValue(DefaultToolFactoryProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DefaultToolFactory.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DefaultToolFactoryProperty =
            DependencyProperty.Register("DefaultToolFactory", typeof(ImageEditorToolFactory), typeof(RadImageEditor), new PropertyMetadata(null));

        
        public ImageEditorToolFactory ToolFactory
        {
            get { return (ImageEditorToolFactory)GetValue(ToolFactoryProperty); }
            set { SetValue(ToolFactoryProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ToolFactory.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ToolFactoryProperty =
            DependencyProperty.Register("ToolFactory", typeof(ImageEditorToolFactory), typeof(RadImageEditor), new PropertyMetadata(null));

        
        /// <summary>
        /// Identifies the <see cref="Source"/> dependency property.    
        /// </summary>
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(StorageFile), typeof(RadImageEditor), new PropertyMetadata(null, OnSourceChanged));


        public bool HandleBackButton
        {
            get { return (bool)this.GetValue(HandleBackButtonProperty); }
            set { this.SetValue(HandleBackButtonProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HandleBackButton.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HandleBackButtonProperty =
            DependencyProperty.Register("HandleBackButton", typeof(bool), typeof(RadImageEditor), new PropertyMetadata(true));

        ///// <summary>
        ///// Identifies the <see cref="ModifiedImage"/> dependency property.
        ///// </summary>
        //public static readonly DependencyProperty ModifiedImageProperty =
        //    DependencyProperty.Register("ModifiedImage", typeof(ImageSource), typeof(RadImageEditor), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="FileNamePrefix"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FileNamePrefixProperty =
            DependencyProperty.Register("FileNamePrefix", typeof(string), typeof(RadImageEditor), new PropertyMetadata("Edited_Image"));

        /// <summary>
        /// Identifies the <see cref="Tools"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ToolGroupsProperty =
            DependencyProperty.Register("ToolGroups", typeof(ObservableCollection<ImageEditorToolGroup>), typeof(RadImageEditor), new PropertyMetadata(null));

        // Using a DependencyProperty as the backing store for SelectedGroup.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedGroupProperty =
            DependencyProperty.Register("SelectedGroup", typeof(ImageEditorToolGroup), typeof(RadImageEditor), new PropertyMetadata(null, OnSelectedGroupChanged));

        /// <summary>
        /// Identifies the <see cref="CurrentTool"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CurrentToolProperty =
            DependencyProperty.Register("CurrentTool", typeof(ImageEditorTool), typeof(RadImageEditor), new PropertyMetadata(null, OnCurrentToolChanged));

        // Using a DependencyProperty as the backing store for PreviewImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ModifiedImageProperty =
            DependencyProperty.Register("ModifiedImage", typeof(ImageSource), typeof(RadImageEditor), new PropertyMetadata(null, OnModifiedImageChagned));

        public bool DisplayOriginalImage
        {
            get { return (bool)GetValue(DisplayOriginalImageProperty); }
            set { SetValue(DisplayOriginalImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DisplayOriginalImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayOriginalImageProperty =
            DependencyProperty.Register("DisplayOriginalImage", typeof(bool), typeof(RadImageEditor), new PropertyMetadata(false, OnDisplayOriginalImage));



        public Style IndicatorStyle
        {
            get { return (Style)GetValue(IndicatorStyleProperty); }
            set { SetValue(IndicatorStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IndicatorStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IndicatorStyleProperty =
            DependencyProperty.Register("IndicatorStyle", typeof(Style), typeof(RadImageEditor), new PropertyMetadata(null));

        
        public string StatusMessage
        {
            get { return (string)GetValue(StatusMessageProperty); }
            set { SetValue(StatusMessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StatusMessage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StatusMessageProperty =
            DependencyProperty.Register("StatusMessage", typeof(string), typeof(RadImageEditor), new PropertyMetadata(null));

        private CommandService commandService;

        /// <summary>
        /// Gets the collection with all the custom commands registered with the <see cref="CommandService"/>. Custom commands have higher priority than the built-in (default) ones.
        /// </summary>
        public CommandCollection<RadImageEditor> Commands
        {
            get
            {
                return this.commandService.UserCommands;
            }
        }
      
        /// <summary>
        /// Gets the <see cref="CommandService"/> instance that manages the commanding behavior of this instance.
        /// </summary>
        public CommandService CommandService
        {
            get
            {
                return this.commandService;
            }
        }

        internal IRandomAccessStream previewBitmapStream;
        private IRandomAccessStream modifiedBitmapStream;

        private WriteableBitmap previewBitmap;

        private WriteableBitmap originalBitmap;
        private RadPanAndZoomImage imageView;
        
        private ImageEditorToolbar toolbar;

        private ImageEditorLayer toolLayer;

        private Grid toolsContainer;

        public RadImageEditor()
        {
            this.DefaultStyleKey = typeof(RadImageEditor);

            if (this.ToolGroups == null)
            {
                this.ToolGroups = new ObservableCollection<ImageEditorToolGroup>();
                this.ToolGroups.CollectionChanged += ToolGroups_CollectionChanged;
            }

            this.commandService = new CommandService(this);
        }

        /// <summary>
        /// Fired before the image editor saves the modified image to the media library or local storage.
        /// </summary>
        public event EventHandler<ImageSavingEventArgs> ImageSaving;

        /// <summary>
        /// Fired when the user presses the cancel button.
        /// </summary>
        public event EventHandler<ImageEditRevertedEventArgs> ImageReverted;
 

        public object SelectedGroup
        {
            get { return (object)this.GetValue(SelectedGroupProperty); }
            set { this.SetValue(SelectedGroupProperty, value); }
        }

        /// <summary>
        /// Gets or sets a prefix that will be prepended to the file name of the saved image.
        /// </summary>
        public string FileNamePrefix
        {
            get
            {
                return (string)this.GetValue(RadImageEditor.FileNamePrefixProperty);
            }

            set
            {
                this.SetValue(RadImageEditor.FileNamePrefixProperty, value);
            }
        }

        public ImageSource ModifiedImage
        {
            get { return (ImageSource)GetValue(ModifiedImageProperty); }
            set { SetValue(ModifiedImageProperty, value); }
        }

        /// <summary>
        /// Gets or sets the image that will be edited inside RadImageEditor.
        /// </summary>
        public StorageFile Source
        {
            get
            {
                return (StorageFile)this.GetValue(RadImageEditor.SourceProperty);
            }

            set
            {
                this.SetValue(RadImageEditor.SourceProperty, value);
            }
        }

        /// <summary>
        /// Gets the currently used tool in the image editor.
        /// </summary>
        public ImageEditorTool CurrentTool
        {
            get
            {
                return (ImageEditorTool)this.GetValue(RadImageEditor.CurrentToolProperty);
            }

            set
            {
                this.SetValue(RadImageEditor.CurrentToolProperty, value);
            }
        }

        /// <summary>
        /// Gets the collection of tools of RadImageEditor.
        /// </summary>
        public ObservableCollection<ImageEditorToolGroup> ToolGroups
        {
            get
            {
                return (ObservableCollection<ImageEditorToolGroup>)this.GetValue(RadImageEditor.ToolGroupsProperty);
            }

            private set
            {
                this.SetValue(RadImageEditor.ToolGroupsProperty, value);
            }
        }

        internal static int GetIntFromComponents(int alpha, int red, int green, int blue)
        {
            return BitConverter.ToInt32(new byte[] { (byte)red, (byte)green, (byte)blue, (byte)alpha }, 0);
        }

        internal static T Clamp<T>(T value, T min, T max)
        {
            Comparer<T> comparer = Comparer<T>.Default;

            int c = comparer.Compare(value, min);
            if (c < 0)
            {
                return min;
            }

            c = comparer.Compare(value, max);
            if (c > 0)
            {
                return max;
            }

            return value;
        }

        //TODO add save methods

        /// <summary>
        /// A virtual callback that fires the ImageEditCancelled event.
        /// </summary>
        internal virtual void OnImageEditCancelled()
        {
            var handler = this.ImageReverted;
            if (handler != null)
            {
                this.ImageReverted(this, new ImageEditRevertedEventArgs());
            }
        }

        //protected virtual void OnImageSaved(string fileName)
        //{
        //    EventHandler<ImageSavedEventArgs> eh = this.ImageSaved;

        //    if (eh != null)
        //    {
        //        eh(this, new ImageSavedEventArgs(fileName));
        //    }
        //}

        private ButtonBase previewOriginalButton;
        private ButtonBase cancelGroupsButton;
        private Canvas imageHost;
        private Grid imageContainer;
        private ButtonBase cancelToolsButton;
        private ButtonBase saveImageButton;
        private ButtonBase saveToolsButton;
        private RadPanAndZoomImage originalImage;

        protected override bool ApplyTemplateCore()
        {
            var applied = base.ApplyTemplateCore();

            this.toolsContainer = this.GetTemplatePartField<Grid>("PART_ToolsContainer");
            applied &= this.toolsContainer != null;

            this.imageView = this.GetTemplatePartField<RadPanAndZoomImage>("PART_Image");
            applied &= this.imageView != null;

            this.saveToolsButton = this.GetTemplatePartField<ButtonBase>("PART_SaveToolsButton");
            applied &= this.saveToolsButton != null;

            #if WINDOWS_PHONE_APP
            this.toolbar = this.GetTemplatePartField<ImageEditorToolbar>("PART_Toolbar");
            applied &= this.toolbar != null;
            #endif  

            this.cancelGroupsButton = this.GetTemplatePartField<ButtonBase>("PART_CancelGroupsButton");
            applied &= this.cancelGroupsButton != null;

            this.imageHost = this.GetTemplatePartField<Canvas>("PART_ImageHost");
            applied &= this.imageHost != null;

            this.imageContainer = this.GetTemplatePartField<Grid>("PART_ImageContainer");
            applied &= this.imageContainer != null;

            // On WP cancel tools button is the Hardware Back Button.
#if WINDOWS_APP
            this.cancelToolsButton = this.GetTemplatePartField<ButtonBase>("PART_CancelToolsButton");
            applied &= this.cancelToolsButton != null;
#endif

            this.saveImageButton = this.GetTemplatePartField<ButtonBase>("PART_SaveImageButton");
            applied &= this.saveImageButton != null;

            return applied;
        }

        protected override void OnTemplateApplied()
        {
            base.OnTemplateApplied();

            if (this.IsLoaded)
            {
                this.InitPreviewImage();
            }

            this.OnToolGroupsAdded(this.ToolGroups);

            this.saveToolsButton.Click += this.OnSaveToolsButtonClick;
            this.cancelGroupsButton.Click += this.OnCancelGroupsButtonClicked;
           
            this.saveImageButton.Click += this.OnsSaveImageButtonClick;

#if WINDOWS_APP
            this.cancelToolsButton.Click += this.OnCancelToolsButtonClick;
#else
            Windows.Phone.UI.Input.HardwareButtons.BackPressed += this.HardwareButtons_BackPressed;
#endif


            this.imageView.GestureSettings = Windows.UI.Input.GestureSettings.None;


            VisualStateManager.GoToState(this, "Group", true);
        }

        protected override void UnapplyTemplateCore()
        {
            base.UnapplyTemplateCore();

            this.saveToolsButton.Click -= this.OnSaveToolsButtonClick;
            this.cancelGroupsButton.Click -= this.OnCancelGroupsButtonClicked;
            this.saveImageButton.Click -= this.OnsSaveImageButtonClick;

#if WINDOWS_APP
            this.cancelToolsButton.Click -= this.OnCancelToolsButtonClick;
#else
            Windows.Phone.UI.Input.HardwareButtons.BackPressed -= this.HardwareButtons_BackPressed;
#endif
        }

#if WINDOWS_PHONE_APP
        private void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            if (this.SelectedGroup != null)
            {
                this.OnBackKeyPress();
            }
             
            e.Handled = true;
        }
#endif


        private void OnsSaveImageButtonClick(object sender, RoutedEventArgs e)
        {
            this.OnSaveImage();
        }

        private async void OnCancelToolsButtonClick(object sender, RoutedEventArgs e)
        {
            this.OnBackKeyPress();
        }

        // CHECK!
        private async Task RevertOriginalImage()
        {
            this.modifiedBitmapStream.Seek(0);  
            await this.previewBitmap.SetSourceAsync(this.modifiedBitmapStream);
            this.previewBitmap.Invalidate();
        }

        void OnCancelGroupsButtonClicked(object sender, RoutedEventArgs e)
        {
            this.commandService.ExecuteCommand(CommandId.RevertImage, null);
        }

        internal async void OnSaveImage()
        {
            if (this.Source == null)
            {
                return;
            }

            var fileName = this.Source.Name;

            this.modifiedBitmapStream.Seek(0);
            var streamCopy = new InMemoryRandomAccessStream();

            await RandomAccessStream.CopyAsync(this.modifiedBitmapStream, streamCopy);

            this.modifiedBitmapStream.Seek(0);
            streamCopy.Seek(0);

            this.OnImageSaving(streamCopy, this.FileNamePrefix + fileName);
        }

        public static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[32768];
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, read);
            }
        }

        /// <inheritdoc/>
        protected override void OnLoaded()
        {
            base.OnLoaded();

            if (this.IsTemplateApplied)
            {
                this.InitPreviewImage();
            }
        }

        /// <summary>
        /// A virtual callback that is called when the <see cref="Source"/> property changes.
        /// </summary>
        /// <param name="newValue"></param>
        /// <param name="oldValue"></param>
        protected virtual void OnSourceChanged(StorageFile newValue, StorageFile oldValue)
        {
            if (!this.IsLoaded || !this.IsTemplateApplied)
            {
                return;
            }

            this.InitPreviewImage();

            this.ResetToolValues();
        }

        private void ResetToolValues()
        {
            foreach (var group in this.ToolGroups)
            {
                foreach (var tool in group.Tools)
                {
                    tool.ResetValues();
                }
            }
        }

        ///// <summary>
        ///// A virtual callback that fires the ImageSaving event.
        ///// </summary>
        ///// <returns>Returns the event arguments of the ImageSaving event modified by the user.</returns>
        protected virtual void OnImageSaving(IRandomAccessStream stream, string filename)
        {
            ImageSavingEventArgs args = new ImageSavingEventArgs(stream, filename);
            if (this.ImageSaving != null)
            {
                this.ImageSaving(this, args);
            }

            this.commandService.ExecuteCommand(CommandId.Saving, args);
        }

        /// <summary>
        /// A virtual callback that is called when the <see cref="CurrentTool"/> property changes.
        /// </summary>
        /// <param name="newValue"></param>
        /// <param name="oldValue"></param>
        protected async virtual void OnCurrentToolChanged(ImageEditorTool newValue, ImageEditorTool oldValue)
        {

            if (oldValue != null)
            {
                oldValue.CleanUp();
                bool shouldDeselect = !(oldValue is ImageEditorEffectTool && newValue != null && !(newValue is ImageEditorEffectTool));
                if(shouldDeselect)
                {
                    oldValue.IsSelected = false;
                }
            }

            if (newValue != null)
            {
                if (newValue is ImageEditorEffectTool)
                {
                    var group = this.SelectedGroup as ImageEditorToolGroup;
                    var selectedTools = group.Tools.Where((ImageEditorTool tool) =>
                        {
                            return tool != newValue && tool.IsSelected;
                        });
                    foreach (var tool in selectedTools)
                    {
                        tool.IsSelected = false;
                    }
                }
                await this.SetupNewTool(newValue);
            }

            this.ResetImageManipulations();
            this.SelectLayer(newValue);
        }

        private void SelectLayer(ImageEditorTool tool)
        {
            this.RemoveLayer(this.toolLayer, this.imageHost);

            this.toolLayer = this.LayerSelector != null ? this.LayerSelector.SelectLayer(tool) : null;

            this.AddLayer(this.toolLayer, this.imageHost);
           
            this.InvalidateArrange();
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var size = base.ArrangeOverride(finalSize);
            this.ArrangeLayer();

            return size;
        }
        private void ArrangeLayer()
        {
            if (this.toolLayer != null)
            {
                this.toolLayer.ArrangeLayer(new RadSize(this.imageHost.ActualWidth, this.imageHost.ActualHeight));
            }   
        }

        private void AddLayer(ImageEditorLayer layer, Panel parent)
        {
            if (layer != null && parent != null)
            {
               layer.Owner = this;
               layer.AttachUI(parent);
            }
        }

        private void RemoveLayer(ImageEditorLayer layer, Panel parent)
        {
            if (layer != null && parent != null)
            {
                layer.DetachUI(parent);
                layer.Owner = null;
            }
        }


        private async Task UpdateCurrentImageSource()
        {
            if (this.IsTemplateApplied)
            {
                if (this.CurrentTool != null && !this.DisplayOriginalImage)
                {
                    await this.CurrentTool.RefreshPreviewImage();
                }
                else
                {
                    this.modifiedBitmapStream.Seek(0);
                    await this.previewBitmap.SetSourceAsync(this.modifiedBitmapStream);

                    this.modifiedBitmapStream.Seek(0);
                    await this.originalBitmap.SetSourceAsync(this.modifiedBitmapStream);
                }

                this.previewBitmap.Invalidate();

                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        this.imageView.Source = this.previewBitmap;
                    });
            }
        }

        private async Task SetupNewTool(ImageEditorTool newValue)
        {
            //  var stream = await this.Source.OpenReadAsync();
            //var properties = await this.Source.Properties.GetImagePropertiesAsync();

            if (this.Source != null && newValue != null)
            {
                if (newValue is ImageEditorEffectTool)
                {
                    this.RevertPreviewBitmap();
                }

                await newValue.Init(this.previewBitmap, this as IToolValueProvider);              
            }

            if (newValue is IRotateTool)
            {
                if (newValue.workingBitmap != null)
                {
                    this.previewBitmap = newValue.workingBitmap;
                    this.previewBitmap.Invalidate();
                    this.ModifiedImage = this.previewBitmap;
                }

                newValue.IsSelected = false;
            }
        }

        private static void OnDisplayOriginalImage(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = d as RadImageEditor;

            if ((bool)e.NewValue)
            {
                editor.imageView.Source = editor.originalBitmap;
            }
            else
            {
               editor.imageView.Source = editor.previewBitmap;
            }
        }

        private static void OnModifiedImageChagned(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = d as RadImageEditor;

            editor.UpdateCurrentImageSource();
        }

        private static void OnSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadImageEditor editor = (RadImageEditor)sender;
            editor.OnSourceChanged((StorageFile)args.NewValue, (StorageFile)args.OldValue);
        }

        private static void OnSelectedGroupChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {         
            var editor = d as RadImageEditor;

            editor.ManipulationProcessor.SetAbsoluteRotation(0);
            editor.RotatedImageSize = new Size(0, 0);

            if (e.OldValue != null)
            {
                if (editor.RotateTool != null)
                {
                    editor.RotateTool.ResetValues();
                    editor.RotateTool = null;
                }

                var oldGroup = e.OldValue as ImageEditorToolGroup;

                oldGroup.IsSelected = false;
            }  

            if (e.NewValue != null)
            {
                editor.OriginalImageSize = new Size(editor.previewBitmap.PixelWidth, editor.previewBitmap.PixelHeight);
                editor.StatusMessage = (e.NewValue as ImageEditorToolGroup).Name;

                #if WINDOWS_PHONE_APP
                editor.toolbar.ForceClose();
                editor.toolbar.IsExpanded = false;
                editor.toolbar.IsExpandable = false;
                #endif

                var newGroup = e.NewValue as ImageEditorToolGroup;
                if (newGroup.Tools.Count > 0)
                {
                    newGroup.Tools[0].IsSelected = true;                     
                }
            }
            else
            {
                if (editor.CurrentTool != null)
                {
                    editor.RemoveTool();
                }
                VisualStateManager.GoToState(editor, "Group", true);
              #if WINDOWS_PHONE_APP
                editor.toolbar.IsExpandable = true;
              #endif
            }

        }

        private async Task ApplyTool(ImageEditorTool tool, IRandomAccessStream stream)
        {
            if (!tool.HandleApply())
            {
                this.previewBitmap = await tool.Apply(this.previewBitmapStream, this.previewBitmap);
                this.previewBitmap.Invalidate();
                var newSize = new Size(this.previewBitmap.PixelWidth, this.previewBitmap.PixelHeight);
                if(this.RotatedImageSize.Width != 0 && this.RotatedImageSize.Height != 0)
                {
                    if (newSize.Width != this.RotatedImageSize.Width || newSize.Height != this.RotatedImageSize.Height)
                    {
                        this.OriginalImageSize = newSize;
                    }
                }
                else
                {
                        this.OriginalImageSize = newSize;
                }
                 
            }

            this.ModifiedImage = this.previewBitmap;

            var destStream = new InMemoryRandomAccessStream();

            var pixelStream = this.previewBitmap.PixelBuffer.AsStream();
            byte[] pixels = new byte[pixelStream.Length];

            BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, destStream);

            pixelStream.ReadAsync(pixels, 0, pixels.Length).Wait();

            encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore,
                              (uint)this.previewBitmap.PixelWidth,
                              (uint)this.previewBitmap.PixelHeight,
                              96.0,
                              96.0,
                              pixels);

            await encoder.FlushAsync();
            this.previewBitmapStream = destStream;
        }
       

        private bool shouldApplyTool = true;
        private static async void OnCurrentToolChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //TODO change PreviewImage
            var editor = d as RadImageEditor;

            if (e.OldValue != null && e.OldValue != e.NewValue)
            {
                var oldTool = e.OldValue as ImageEditorTool;

                if (editor.shouldApplyTool && editor.SelectedGroup != null)
                {
                  await  editor.ApplyTool(oldTool, editor.previewBitmapStream);
                }

                if (editor.toolsContainer.Children.Count > 0)
                {
                    FrameworkElement oldToolElement = editor.toolsContainer.Children[0] as FrameworkElement;
                    if (oldToolElement != null)
                    {
                        var factory = editor.ToolFactory != null ? editor.ToolFactory : editor.DefaultToolFactory;
                        if (factory != null)
                        {
                            factory.Detach(oldToolElement, editor);
                        }
                        editor.toolsContainer.Children.Clear();
                    }
                }

            }

            if (e.NewValue != null)
            {
                var newTool = e.NewValue as ImageEditorTool;

                if (newTool is IRotateTool)
                {
                    editor.RotateTool = newTool;
                }

                editor.StatusMessage = (newTool).Name;
                VisualStateManager.GoToState(editor, "SelectedTool", true);

                DataTemplate toolTemplate = null;
                FrameworkElement toolElement = null;

                if (editor.ToolFactory != null)
                {
                    toolTemplate = editor.ToolFactory.GetTemplate(newTool);
                    if (toolTemplate != null)
                    {
                        toolElement = toolTemplate.LoadContent() as FrameworkElement;
                        if (toolElement != null)
                        {
                            toolElement.DataContext = newTool;
                            editor.ToolFactory.Attach(toolElement, editor);

                            editor.toolsContainer.Children.Clear();
                            editor.toolsContainer.Children.Add(toolElement);
                        }
                    }
                }

                if(toolTemplate == null)
                {
                    toolTemplate = editor.DefaultToolFactory.GetTemplate(newTool);
                    if(toolTemplate != null)
                    {
                        toolElement = toolTemplate.LoadContent() as FrameworkElement;
                        if (toolElement != null)
                        {
                            toolElement.DataContext = newTool;

                            editor.DefaultToolFactory.Attach(toolElement, editor);

                            editor.toolsContainer.Children.Clear();
                            editor.toolsContainer.Children.Add(toolElement);
                        }
                    }
                }
            }
            else
            {
                editor.ManipulationProcessor.GestureSettings = Windows.UI.Input.GestureSettings.None;
                if (editor.SelectedGroup != null)
                {
                    VisualStateManager.GoToState(editor, "Tool", true);
                }             
            }

            editor.OnCurrentToolChanged(e.NewValue as ImageEditorTool, e.OldValue as ImageEditorTool);
        }

        internal void UpdateToolState(bool isToolUpdating)
        {
            if (isToolUpdating)
            {
                VisualStateManager.GoToState(this, "RangeUpdating", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "RangeNormal", true);
            }   
        }


        internal void OnRevertImage()
        {
            if (this.IsLoaded && this.IsTemplateApplied)
            {
                this.InitPreviewImage();
                this.OnImageEditCancelled();
            }
        }

        // Get the ratio between device pixels and bitmap's pixels. Aspect ratio is preserved.
        private double GetPixelRatio()
        {
            var widthRatio = this.imageView.ActualWidth / this.previewBitmap.PixelWidth;
            var heightRatio = this.imageView.ActualHeight / this.previewBitmap.PixelHeight;

            return widthRatio < heightRatio ? widthRatio : heightRatio;
        }

        internal RadSize GetActualImageSize()
        {
            var uniformRatio = this.GetPixelRatio();
            var width = this.previewBitmap.PixelWidth * uniformRatio;
            var height = this.previewBitmap.PixelHeight * uniformRatio;

            return new RadSize(width, height);
        }

        internal FrameworkElement GetSelectedToolUI()
        {
            FrameworkElement uiElement  = null;
            if(this.ToolFactory != null)
            {
              uiElement = this.ToolFactory.GetSelectedElement(this);
            }

            if(uiElement == null)
            {
                uiElement = this.DefaultToolFactory.GetSelectedElement(this);
            }

            return uiElement;
        }

        // TODO: must navigate to groups. Tools should apply effect immediately.
        private async void OnSaveToolsButtonClick(object sender, RoutedEventArgs e)
        {
            if (this.Source == null)
            {
                return;
            }

            if (this.CurrentTool != null)
            {
                if (!(this.CurrentTool is IRotateTool))
                {
                    this.shouldApplyTool = false;
                    await this.ApplyTool(this.CurrentTool, this.previewBitmapStream);
                    this.CurrentTool = null;
                    this.shouldApplyTool = true;
                }                
             }

            var destStream = new InMemoryRandomAccessStream();

            var pixelStream = this.previewBitmap.PixelBuffer.AsStream();
            byte[] pixels = new byte[pixelStream.Length];

            BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, destStream);

            pixelStream.ReadAsync(pixels, 0, pixels.Length).Wait();

            encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore,
                                (uint)this.previewBitmap.PixelWidth,
                                (uint)this.previewBitmap.PixelHeight,
                                96.0,
                                96.0,
                                pixels);

            await encoder.FlushAsync();

            this.previewBitmapStream = destStream;
            this.modifiedBitmapStream = this.previewBitmapStream.CloneStream();
            this.originalBitmap.SetSource(this.modifiedBitmapStream);

            this.SelectedGroup = null;
        }

        

        public Rect GetLayerCoordinates()
        {
            var area = this.toolLayer.LayerCoordinates;

            // Get translation due to panning the image.
            var translation = this.imageView.ManipulationProcessor.GetTranslation();

            // Get the zoom factor applied.
            var zoom = this.imageView.ManipulationProcessor.GetZoom();

            var uniformRatio = this.GetPixelRatio();

            // calculate the offset caused by the preserved aspect ratio
            var actualImageSize = this.GetActualImageSize();

            var imageViewTranslationX = (this.imageHost.ActualWidth - this.imageView.ActualWidth) / 2;
            var imageViewTranslationY = (this.imageHost.ActualHeight - this.imageView.ActualHeight) / 2;

            // default offset 
            var defaultTranslationX = (zoom * actualImageSize.Width - this.imageView.ActualWidth) / 2 - imageViewTranslationX;
            var defaultTranslationY = (zoom * actualImageSize.Height - this.imageView.ActualHeight) / 2 - imageViewTranslationY;

            var leftAlignedTranslationX = translation.X - defaultTranslationX;
            var topAlignedTranslationY = translation.Y - defaultTranslationY;

            //translate x and y
            var x = area.X - leftAlignedTranslationX;
            var y = area.Y - topAlignedTranslationY;

            var width = area.Width;
            var height = area.Height;

            // take in mind the aspect ratio and zoom
            var zoomedRatio = uniformRatio * zoom;

            x /= zoomedRatio;
            y /= zoomedRatio;
            width /= zoomedRatio;
            height /= zoomedRatio;

            return new Rect(x, y, width, height);
        }

        public async Task<WriteableBitmap> GetLayerBitmap()
        {        
            var uniformRatio = this.GetPixelRatio();

            var actualImageSize = this.GetActualImageSize();

            var offsetX = (this.imageView.ActualWidth - actualImageSize.Width) / 2;
            var offsetY = (this.imageView.ActualHeight - actualImageSize.Height) / 2;


            var layerVisual = this.toolLayer.VisualElement as FrameworkElement;
       

            var renderBitmap = new RenderTargetBitmap();
            await renderBitmap.RenderAsync(layerVisual,(int)(layerVisual.ActualWidth / uniformRatio), (int)(layerVisual.ActualHeight / uniformRatio));

            var pixelBuffer = await renderBitmap.GetPixelsAsync();
            byte[] pixelsArr = pixelBuffer.ToArray();

            var writeableBitmap = new WriteableBitmap((int)renderBitmap.PixelWidth, (int)renderBitmap.PixelHeight);

            using (Stream stream = writeableBitmap.PixelBuffer.AsStream())
            {
                await stream.WriteAsync(pixelsArr, 0, pixelsArr.Length);
                stream.Seek(0, 0);
            }


            return writeableBitmap; 
        }

        private async Task<Windows.Storage.Streams.IRandomAccessStream> ToStream(Windows.Storage.Streams.IBuffer ibuffer)
        {
            var stream = new Windows.Storage.Streams.InMemoryRandomAccessStream();
            var outputStream = stream.GetOutputStreamAt(0);
            var datawriter = new Windows.Storage.Streams.DataWriter(outputStream);
            datawriter.WriteBuffer(ibuffer);
            await datawriter.StoreAsync();
            await outputStream.FlushAsync();
            return stream;
        }

        internal async void InitPreviewImage()
        {
            if (this.Source == null)
            {
                return;
            }

            using (var fileStream = await this.Source.OpenReadAsync())
            {
                InMemoryRandomAccessStream memoryStream = new InMemoryRandomAccessStream();

                using (var readStream = fileStream.GetInputStreamAt(0))
                {

                    var reader = new Windows.Storage.Streams.DataReader(readStream);
                    await reader.LoadAsync((uint)fileStream.Size);

                    byte[] bytes = new byte[fileStream.Size];
                    reader.ReadBytes(bytes);

                    var writeStream = memoryStream.GetOutputStreamAt(0).AsStreamForWrite();
                    writeStream.WriteAsync(bytes, 0, (int)fileStream.Size);
                    await writeStream.FlushAsync();
                }

                this.modifiedBitmapStream = memoryStream.CloneStream();
                this.previewBitmapStream = this.modifiedBitmapStream;


            }

            var decoder = await BitmapDecoder.CreateAsync(this.previewBitmapStream);
            //   this.originalBitmapStream = await this.Source.OpenReadAsync();
            //  this.previewBitmapStream = await this.Source.OpenReadAsync();
            var properties = await this.Source.Properties.GetImagePropertiesAsync();

            System.Threading.SynchronizationContext.Current.Post((args) =>
            {
                System.Threading.SynchronizationContext.Current.Post((args2) =>
                {
                    this.previewBitmap = new WriteableBitmap((int)properties.Width, (int)properties.Height);
                    this.originalBitmap = new WriteableBitmap((int)properties.Width, (int)properties.Height);
                    this.previewBitmap.SetSource(this.modifiedBitmapStream);
                    this.originalBitmap.SetSource(this.modifiedBitmapStream);

                    this.previewBitmap.Invalidate();
                    this.originalBitmap.Invalidate();

                    this.ModifiedImage = this.previewBitmap;

                }, null);
            }, null);
        }

        private void RevertPreviewBitmap()
        {
            this.previewBitmapStream = this.modifiedBitmapStream.CloneStream();
            this.previewBitmap.SetSource(this.previewBitmapStream);
            this.previewBitmap.Invalidate();
        }

        private void OnBackKeyPress()
        {
            this.RevertPreviewBitmap();

            if (this.CurrentTool != null)
            {
                this.shouldApplyTool = false;
                this.RemoveTool();
                this.shouldApplyTool = true;
            }

            this.SelectedGroup = null;
        }

        private void ResetImageManipulations()
        {
            if (this.ManipulationProcessor != null)
            {
                this.ManipulationProcessor.SetAbsoluteTranslation(new Point(0,0));
                this.ManipulationProcessor.SetAbsoluteZoom(1);
            }
        }

        private void RemoveTool()
        {
            if (this.CurrentTool == null)
            {
                return;
            }
            this.CurrentTool.CleanUp();
            this.CurrentTool = null;
        }

        private void ToolGroups_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!this.IsTemplateApplied)
            {
                return;
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    this.OnToolGroupsAdded(e.NewItems);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    this.OnToolGroupsRemoved(e.NewItems);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    this.OnToolGroupsReplaced(e.NewItems, e.OldItems);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    this.OnToolGroupsReplaced(e.NewItems, e.OldItems);
                    break;
            }
        }

        private void OnToolGroupPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsSelected")
            {
                ImageEditorToolGroup group = (ImageEditorToolGroup)sender;

                if (group.IsSelected)
                {
                    this.SelectedGroup = group;
                }
                else if (this.SelectedGroup == sender)
                {
                    this.SelectedGroup = null;
                }
            }
        }

        private void OnToolGroupsAdded(IList addedToolGroups)
        {
            if (addedToolGroups == null)
            {
                return;
            }

            foreach (ImageEditorToolGroup group in addedToolGroups)
            {
                group.PropertyChanged += this.OnToolGroupPropertyChanged;
                // group.CollectionIndex = this.ToolGroups.IndexOf(tool);

                this.OnToolsAdded(group.Tools);
            }
        }

        private void OnToolGroupsRemoved(IList removedToolGroups)
        {
            if (removedToolGroups == null)
            {
                return;
            }

            foreach (ImageEditorToolGroup group in removedToolGroups)
            {
                group.PropertyChanged -= this.OnToolGroupPropertyChanged;
                // tool.CollectionIndex = -1;

                this.OnToolsRemoved(group.Tools);
            }
        }

        private void OnToolGroupsReplaced(IList newToolGroups, IList replacedToolGroups)
        {
            this.OnToolGroupsAdded(newToolGroups);
            this.OnToolGroupsRemoved(replacedToolGroups);
        }

        private void OnToolsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!this.IsTemplateApplied)
            {
                return;
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    this.OnToolsAdded(e.NewItems);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    this.OnToolsRemoved(e.NewItems);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    this.OnToolsReplaced(e.NewItems, e.OldItems);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    this.OnToolsReplaced(e.NewItems, e.OldItems);
                    break;
            }
        }

        private void OnToolPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsSelected")
            {
                ImageEditorTool tool = (ImageEditorTool)sender;

                if (tool.IsSelected)
                {
                    this.CurrentTool = tool;
                }
                else if (this.CurrentTool == sender)
                {
                    this.CurrentTool = null;
                }
            }
            else if (e.PropertyName == "Value")
            {
                RangeTool tool = sender as RangeTool;
                if(tool != null)
                this.StatusMessage = tool.Value.ToString();
            }

            //if (e.PropertyName == "ModifiedImage" && this.CurrentTool != null)
            //{
            //    this.ModifiedImage = this.CurrentTool.ModifiedImage;
            //}
        }

        private void OnToolsAdded(IList addedTools)
        {
            if (addedTools == null)
            {
                return;
            }

            foreach (ImageEditorTool tool in addedTools)
            {
                tool.PropertyChanged += this.OnToolPropertyChanged;
                // tool.CollectionIndex = this.Tools.IndexOf(tool);
            }
        }

        private void OnToolsRemoved(IList removedTools)
        {
            if (removedTools == null)
            {
                return;
            }

            foreach (ImageEditorTool tool in removedTools)
            {
                tool.PropertyChanged -= this.OnToolPropertyChanged;
                tool.CollectionIndex = -1;
            }
        }

        private void OnToolsReplaced(IList newTools, IList replacedTools)
        {
            this.OnToolsAdded(newTools);
            this.OnToolsRemoved(replacedTools);
        }

        public ManipulationInputProcessor ManipulationProcessor
        {
            get
            {
                return this.imageView.ManipulationProcessor;
            }
        }

        public ImageEditorTool RotateTool { get; set; }


        public Size OriginalImageSize
        {
            get;
            set;
        }

        public Size RotatedImageSize
        {
            get;
            set;
        }
    }
}
