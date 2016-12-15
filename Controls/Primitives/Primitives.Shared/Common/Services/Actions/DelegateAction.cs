using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    internal class DelegateAction : ActionBase
    {
        private Action action;

        public DelegateAction(Action action)
        {
            this.action = action;
        }

        public override void Execute()
        {
            if (this.action != null)
            {
                this.action();
            }

            this.OnCompleted();
        }

        public override void ForceCompletion()
        {
            this.OnCompleted();
        }
    }
}
