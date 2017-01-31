using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Core;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Serves as a sync service for all the internal updates that originate within the grid.
    /// </summary>
    internal abstract class UpdateServiceBase<T> : ServiceBase<RadControl>
    {
        protected int pendingUpdateFlags;
        private const int MaxScheduleCount = 10;

        private byte suspendCount;
        private bool isUpdateScheduled;
        private bool isDispatching;
        private List<Update<T>> updatesQueue;
        private bool shouldExecuteSynchronously;
        private bool layoutUpdatedHooked;
        private CoreDispatcherPriority? updatePriority;

        private bool isOperational;

        internal UpdateServiceBase(RadControl owner, bool shouldExecuteSynchronously)
            : base(owner)
        {
            this.updatesQueue = new List<Update<T>>();
            this.shouldExecuteSynchronously = shouldExecuteSynchronously;
        }

        public T PendingUpdateFlags
        {
            get
            {
                return (T)Enum.ToObject(typeof(T), this.pendingUpdateFlags);
            }
        }

        public abstract bool HasPendingUpdates { get; }

        public bool IsSuspended
        {
            get
            {
                return this.suspendCount > 0;
            }
        }

        public bool IsDispatching
        {
            get
            {
                return this.isDispatching;
            }
        }

        internal Update<T> CurrentExecutingUpdate { get; set; }

        protected override bool IsOperational
        {
            get
            {
                return this.isOperational;
            }
        }

        public void DispatchOnUIThread(bool isHighPriority, DispatchedHandler action)
        {
            if (this.Owner != null)
            {
                var priority = isHighPriority ? CoreDispatcherPriority.High : CoreDispatcherPriority.Normal;
                if (this.shouldExecuteSynchronously)
                {
                    //// TODO: How to synchronize on the UI thread at Design-time?
                    action();
                }
                else
                {
                    var warningSuppression = this.Dispatcher.RunAsync(priority, action);
                }
            }
        }

        public void SuspendUpdates()
        {
            this.suspendCount++;
        }

        public virtual void ResumeUpdates()
        {
            this.ResumeUpdates(true);
        }

        public void ResumeUpdates(bool dispatch)
        {
            if (this.suspendCount == 0)
            {
                return;
            }

            this.suspendCount--;
            if (this.suspendCount > 0)
            {
                return;
            }

            if (dispatch && this.HasPendingUpdates)
            {
                this.RegisterUpdateCallback();
            }
        }

        public void RegisterUpdate(Update<T> update)
        {
            update.ScheduleCount++;
            this.updatesQueue.Add(update);

            if (!this.updatePriority.HasValue)
            {
                this.updatePriority = update.Priority;
            }
            else if (update.Priority > this.updatePriority.Value)
            {
                this.updatePriority = update.Priority;
            }

            this.RegisterUpdate(update.UpdateFlagsIndex);
        }

        public void RegisterUpdate(int flags)
        {
            this.pendingUpdateFlags |= flags;
            this.RegisterUpdateCallback();
        }

        public void RemoveUpdateFlag(int flag)
        {
            this.pendingUpdateFlags &= ~flag;
        }

        public void Start()
        {
            this.isOperational = true;

            var flags = this.pendingUpdateFlags;
            this.pendingUpdateFlags = 0;

            if (!this.TryRefreshData(flags))
            {
                // At the beginning (before the first Measure pass) we are interested in Callback updates ONLY
                if (this.updatesQueue.Count > 0)
                {
                    this.RegisterUpdateCallback();
                }
            }
        }

        public void Stop()
        {
            this.isOperational = false;
        }

        protected virtual void ResetPendingFlags()
        {
            // this.pendingUpdateFlags = UpdateFlags.None;
        }

        protected virtual bool ShouldProcessUpdate(Update<T> update)
        {
            return update.RequiresValidMeasure &&
                   update.ScheduleCount <= MaxScheduleCount;
        }

        protected abstract bool TryRefreshData(int flags);

        ////{
        ////    if ((flags & UpdateFlags.AffectsData) == UpdateFlags.AffectsData && this.Owner.Model.ItemsSource != null)
        ////    {
        ////        this.Owner.Model.RefreshData();
        ////        // The RefreshData call will schedule a new update of the UI
        ////        return true;
        ////    }
        ////    return false;
        ////}
        protected abstract void ProcessUpdateFlags(int flags);

        ////        this.Owner.Model.GridView.RebuildUI();
        ////    }
        ////}
        ////    if (!this.TryRefreshData(flags))
        ////    {
        ////        if ((flags & UpdateFlags.AffectsScrollPosition) == UpdateFlags.AffectsScrollPosition)
        ////        {
        ////            this.Owner.Model.GridView.SetScrollPosition(RadPoint.Empty, false, true);
        ////        }
        ////this.Owner.Model.ColumnPool.Update(flags);
        ////this.Owner.Model.RowPool.Update(flags);
        ////this.Owner.Model.CellsController.Update(flags);
        ////this.Owner.Model.DecorationsController.Update(flags);
        ////{
        ////if (flags == UpdateFlags.None)
        ////{
        ////    return;
        ////}

        private void ProcessUpdate(Update<T> update)
        {
            this.CurrentExecutingUpdate = update;

            update.Process();
        }

        private void RegisterUpdateCallback()
        {
            if (!this.IsOperational || this.isUpdateScheduled || this.suspendCount > 0)
            {
                return;
            }

            this.isUpdateScheduled = true;

            if (this.shouldExecuteSynchronously)
            {
                this.OnUpdateCallback();
                this.ScheduleClearCurrentUpdate();
            }
            else
            {
                var priority = this.updatePriority.HasValue ? this.updatePriority.Value : CoreDispatcherPriority.Low;
                this.updatePriority = null;

                var suppressionVariable = this.Dispatcher.RunAsync(priority, this.OnUpdateCallback);
                suppressionVariable = this.Dispatcher.RunAsync(priority, () => this.CurrentExecutingUpdate = null);
            }
        }

        private void ScheduleClearCurrentUpdate()
        {
            if (!this.layoutUpdatedHooked && !this.Owner.IsUnloaded)
            {
                this.Owner.LayoutUpdated += this.OnLayoutUpdated;
                this.layoutUpdatedHooked = true;
            }
        }

        private void OnLayoutUpdated(object sender, object e)
        {
            this.Owner.LayoutUpdated -= this.OnLayoutUpdated;
            this.CurrentExecutingUpdate = null;
            this.layoutUpdatedHooked = false;
        }

        private void OnUpdateCallback()
        {
            this.isUpdateScheduled = false;
            if (this.suspendCount > 0)
            {
                return;
            }

            this.DispatchUpdates();
        }

        private void DispatchUpdates()
        {
            if (!this.IsOperational)
            {
                return;
            }

            this.isDispatching = true;

            // process the so far accumulated update flags
            var currentFlags = this.pendingUpdateFlags;

            this.ResetPendingFlags();

            this.ProcessUpdateFlags(currentFlags);
            this.ProcessUpdatesQueue();

            this.isDispatching = false;
        }

        private void ProcessUpdatesQueue()
        {
            if (this.Owner.IsUnloaded)
            {
                this.updatesQueue.Clear();
                return;
            }

            // process individual updates
            // create a copy of the queue since an update may trigger another RegisterUpdate call - e.g. when Scrolling into view
            var updatesCopy = new List<Update<T>>(this.updatesQueue);
            this.updatesQueue.Clear();

            for (int i = 0; i < updatesCopy.Count; i++)
            {
                if (this.ShouldProcessUpdate(updatesCopy[i]))
                {
                    if (this.shouldExecuteSynchronously)
                    {
                        this.Owner.UpdateLayout();
                        this.ProcessUpdate(updatesCopy[i]);
                    }
                    else
                    {
                        this.RegisterUpdate(updatesCopy[i]);
                    }
                }
                else
                {
                    this.ProcessUpdate(updatesCopy[i]);
                }
            }
        }
    }
}