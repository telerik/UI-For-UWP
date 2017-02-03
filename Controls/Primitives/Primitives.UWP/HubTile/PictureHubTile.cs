using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Telerik.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// An abstract class that provides a PictureNeeded event and a method to raise it.
    /// </summary>
    public abstract class PictureHubTile : HubTileBase, IWeakEventListener
    {
        /// <summary>
        /// Identifies the PicturesSource dependency property.
        /// </summary>
        public static readonly DependencyProperty PicturesSourceProperty =
            DependencyProperty.Register(nameof(PicturesSource), typeof(IEnumerable), typeof(PictureHubTile), new PropertyMetadata(null, OnPicturesSourceChanged));

        /// <summary>
        /// Identifies the <see cref="PictureSourceProvider"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PictureSourceProviderProperty =
            DependencyProperty.Register(nameof(PictureSourceProvider), typeof(IImageSourceProvider), typeof(PictureHubTile), new PropertyMetadata(null));

        private WeakEventHandler<NotifyCollectionChangedEventArgs> collectionChangedEventHandler;
        private IList picturesSourceAsList = new List<object>();
        private Random rand = new Random();

        /// <summary>
        /// Initializes a new instance of the PictureHubTile class.
        /// </summary>
        protected PictureHubTile()
        {
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="PictureHubTile" /> class.
        /// </summary>
        ~PictureHubTile()
        {
            this.UnsubscribeFromColectionChanged();
        }

        /// <summary>
        /// Gets or sets the <see cref="IImageSourceProvider"/> implementation that is used to provide custom image source resolution routine.
        /// </summary>
        public IImageSourceProvider PictureSourceProvider
        {
            get
            {
                return this.GetValue(PictureSourceProviderProperty) as IImageSourceProvider;
            }
            set
            {
                this.SetValue(PictureSourceProviderProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the source that contains all the pictures. Pictures are randomly chosen from this collection when an image needs to be displayed. 
        /// </summary>
        /// <remarks>
        /// To translate the objects from this collection to an actual ImageSource, use the <see cref="PictureSourceProvider"/> property.
        /// If the objects are string uris or ImageSource instances, no translation is necessary.
        /// </remarks>
        public IEnumerable PicturesSource
        {
            get
            {
                return this.GetValue(PicturesSourceProperty) as IEnumerable;
            }
            set
            {
                this.SetValue(PicturesSourceProperty, value);
            }
        }

        /// <summary>
        /// Determines whether the update timer used to update the tile's VisualState needs to be started.
        /// </summary>
        protected override bool IsUpdateTimerNeeded
        {
            get
            {
                if (this.picturesSourceAsList.Count > 0)
                {
                    return true;
                }

                return base.IsUpdateTimerNeeded;
            }
        }

        void IWeakEventListener.ReceiveEvent(object sender, object args)
        {
            this.UpdateTimerState();
        }

        internal virtual void UpdateNextImageOnSourceChange()
        {
        }

        /// <summary>
        /// Creates an ImageSource from a randomly chosen URI from the ImageSources collection and returns it.
        /// </summary>
        /// <returns>Returns an ImageSource with a randomly chosen URI from the ImageSources collection.</returns>
        protected ImageSource GetRandomImageSource()
        {
            if (this.picturesSourceAsList.Count == 0)
            {
                return null;
            }

            int index = this.GetNewIndex(this.picturesSourceAsList.Count);

            object imageSource = this.picturesSourceAsList[index];
            if (imageSource == null)
            {
                return null;
            }

            IImageSourceProvider provider = this.PictureSourceProvider;
            if (provider != null)
            {
                return provider.GetImageSource(imageSource);
            }

            ImageSource directImageSource = imageSource as ImageSource;
            if (directImageSource != null)
            {
                return directImageSource;
            }

            return CreateDefaultImageSource(imageSource.ToString());
        }

        /// <summary>
        /// Should be overridden in descendant classes to generate the new index from the picture collection.
        /// </summary>
        /// <param name="count">The length of the collection.</param>
        /// <returns>Returns new index different from previous.</returns>
        protected virtual int GetNewIndex(int count)
        {
            int index = this.rand.Next(count);

            if (count > 1)
            {
                while (!this.IsNewIndexValid(index))
                {
                    index = this.rand.Next(count);
                }
            }

            return index;
        }

        /// <summary>
        /// Should be overridden in descendant classes to indicate if the same image can be displayed.
        /// many times in a row.
        /// </summary>
        /// <param name="index">The index of the new image.</param>
        /// <returns>Returns true if the image can be repeated and false otherwise.</returns>
        protected virtual bool IsNewIndexValid(int index)
        {
            return true;
        }

        private static ImageSource CreateDefaultImageSource(string uri)
        {
            BitmapImage result = new BitmapImage();
            result.UriSource = new Uri(uri, UriKind.RelativeOrAbsolute);

            return result;
        }

        private static void OnPicturesSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PictureHubTile tile = d as PictureHubTile;

            IList sourceAsList = e.NewValue as IList;
            IEnumerable sourceAsIEnumerable = e.NewValue as IEnumerable;
            INotifyCollectionChanged sourceAsCollectionChanged = e.NewValue as INotifyCollectionChanged;

            if (sourceAsList != null)
            {
                tile.picturesSourceAsList = sourceAsList;
            }
            else
            {
                tile.picturesSourceAsList = new List<object>();

                if (sourceAsIEnumerable != null)
                {
                    foreach (var item in sourceAsIEnumerable)
                    {
                        tile.picturesSourceAsList.Add(item);
                    }
                }
            }

            tile.UnsubscribeFromColectionChanged();
            if (sourceAsCollectionChanged != null)
            {
                tile.collectionChangedEventHandler = new WeakEventHandler<NotifyCollectionChangedEventArgs>(sourceAsCollectionChanged, (IWeakEventListener)tile, KnownEvents.CollectionChanged);
            }

            tile.UpdateNextImageOnSourceChange();
            tile.UpdateTimerState();
        }

        private void UnsubscribeFromColectionChanged()
        {
            if (this.collectionChangedEventHandler != null)
            {
                this.collectionChangedEventHandler.Unsubscribe();
                this.collectionChangedEventHandler = null;
            }
        }
    }
}