namespace Telerik.Core
{
    /// <summary>
    /// Represents the abstract definition of an object which may be suspended. That is to prevent it from preforming certain functionality until resumed.
    /// </summary>
    public class SuspendableObject
    {
        private byte suspendCount;

        /// <summary>
        /// Gets a value indicating whether this instance is currently suspended.
        /// </summary>
        public bool IsSuspended
        {
            get
            {
                return this.suspendCount > 0;
            }
        }

        /// <summary>
        /// Temporarily suspends this instance.
        /// </summary>
        public void Suspend()
        {
            this.suspendCount++;
            if (this.suspendCount == 1)
            {
                this.SuspendOverride();
            }
        }

        /// <summary>
        /// Resumes this instance (allows certain functionality to be performed).
        /// </summary>
        public void Resume()
        {
            this.Resume(true);
        }

        /// <summary>
        /// Resumes this instance (allows certain functionality to be performed).
        /// </summary>
        /// <param name="update">True to perform update after resuming.</param>
        public void Resume(bool update)
        {
            if (this.suspendCount == 0)
            {
                return;
            }

            this.suspendCount--;
            if (this.suspendCount == 0)
            {
                this.OnResumed(update);
            }
        }

        /// <summary>
        /// Allows inheritors to perform additional logic upon suspend.
        /// </summary>
        protected virtual void SuspendOverride()
        {
        }

        /// <summary>
        /// Notifies that this instance is no longer suspended.
        /// Allows inheritors to provide their own update logic.
        /// </summary>
        /// <param name="update">True if an Update is requested, false otherwise.</param>
        protected virtual void OnResumed(bool update)
        {
        }
    }
}