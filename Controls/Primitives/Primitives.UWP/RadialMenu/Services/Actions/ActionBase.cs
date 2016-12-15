using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.UI.Xaml.Controls.Primitives.Menu
{
    internal abstract class ActionBase
    {
        public ActionBase()
        {
            this.IsDependant = true;
        }

        public event EventHandler Completed;

        public bool IsCompleted { get; private set; }

        public bool IsDependant { get; set; }

        public abstract void Execute();

        public abstract void ForceCompletion();

        protected virtual void OnCompleted()
        {
            this.IsCompleted = true;

            if (this.Completed != null)
            {
                this.Completed(this, EventArgs.Empty);
            }
        }
    }
}
