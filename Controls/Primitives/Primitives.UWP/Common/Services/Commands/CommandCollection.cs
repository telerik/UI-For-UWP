using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Telerik.Core;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Represents a strongly-typed collection of <see cref="ControlCommandBase{T}"/> instances.
    /// </summary>
    public class CommandCollection<T> : AttachableObjectCollection<T, ControlCommandBase<T>> where T : RadControl
    {
        internal CommandCollection(T owner)
            : base(owner)
        {
        }
    }
}
