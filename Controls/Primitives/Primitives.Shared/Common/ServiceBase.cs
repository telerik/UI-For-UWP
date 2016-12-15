using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Base class for all services that support the <see cref="RadControl"/> infrastructure.
    /// </summary>
    public class ServiceBase<T> : AttachableObject<T> where T : RadControl
    {
        internal ServiceBase(T owner)
            : base(owner)
        {
            if (this.Owner == null)
            {
                throw new ArgumentException("Service cannot work without owner provided!", "owner");
            }
        }

        /// <summary>
        /// Gets a value indicating whether the service is operational (may provide its functionality).
        /// </summary>
        protected virtual bool IsOperational
        {
            get
            {
                return this.Owner.IsTemplateApplied;
            }
        }
    }
}
