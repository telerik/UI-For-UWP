using Telerik.Geospatial;
using Windows.ApplicationModel;

namespace Telerik.UI.Xaml.Controls.Map
{
    /// <summary>
    /// Base class for all layers that may reside within a <see cref="RadMap"/> instance.
    /// </summary>
    public abstract class MapLayer : RadControl
    {
        private RadMap map;
        private bool updateScheduled;

        /// <summary>
        /// Gets the bounding rectangle of the layer in geographical coordinates.
        /// </summary>
        public LocationRect Bounds
        {
            get;
            internal set;
        }

        internal RadMap Owner
        {
            get
            {
                return this.map;
            }
        }

        /// <summary>
        /// Gets an ID property for logical separation of all the shapes that reside within one D2DCanvas instance.
        /// </summary>
        internal virtual int Id
        {
            get
            {
                return -1;
            }
        }

        internal void Attach(RadMap owner)
        {
            this.map = owner;
            this.OnAttached();
        }

        internal void Detach()
        {
            RadMap oldMap = this.map;
            this.map = null;
            this.OnDetached(oldMap);
        }

        internal virtual void OnAttached()
        {
        }

        internal virtual void OnDetached(RadMap oldMap)
        {
        }

        internal virtual void OnViewChanged(ViewChangedContext context)
        {
        }

        internal virtual void OnZoomChanged()
        {
        }

        internal virtual void OnScrollOffsetChanged()
        {
        }

        internal virtual void UpdateUI()
        {
        }

        internal void ScheduleUpdate()
        {
            if (this.updateScheduled)
            {
                return;
            }

            if (DesignMode.DesignModeEnabled)
            {
                if (this.IsLoaded || this.IsLoading)
                {
                    this.UpdateUI();
                    this.updateScheduled = false;
                }
            }
            else
            {
                this.updateScheduled = this.InvokeAsync(() =>
                    {
                        this.updateScheduled = false;
                        if (this.IsLoaded)
                        {
                            this.UpdateUI();
                        }
                    });
            }
        }

        /// <summary>
        /// Called within the handler of the <see cref="E:Unloaded" /> event. Allows inheritors to provide their specific logic.
        /// </summary>
        protected override void UnloadCore()
        {
            base.UnloadCore();

            this.updateScheduled = false;
        }
    }
}
