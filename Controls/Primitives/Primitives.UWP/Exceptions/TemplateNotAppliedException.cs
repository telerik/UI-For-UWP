using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.UI.Xaml.Controls
{
    /// <summary>
    /// Represents an exception raised when a control was unable to load its template correctly.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors")]
    public class TemplateNotAppliedException : Exception
    {
        private Type controlType;
        private object controlStyleKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateNotAppliedException"/> class.
        /// </summary>
        /// <param name="type">The type of the control failed to load template.</param>
        /// <param name="defaultStyleKey">The DefaultStyleKey of control failed to load template.</param>
        public TemplateNotAppliedException(Type type, object defaultStyleKey)
            : base()
        {
            this.controlType = type;
            this.controlStyleKey = defaultStyleKey;
        }

        /// <summary>
        /// Gets the type of control failed to load template.
        /// </summary>
        public Type ControlType
        {
            get
            {
                return this.controlType;
            }
        }

        /// <summary>
        /// Gets the DefaultStyleKey of control failed to load template.
        /// </summary>
        public object ControlStyleKey
        {
            get
            {
                return this.controlStyleKey;
            }
        }
    }
}
