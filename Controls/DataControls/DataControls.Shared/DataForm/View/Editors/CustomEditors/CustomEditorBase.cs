using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Data.DataForm;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Data
{
    public abstract class CustomEditorBase<T> : RadControl, ITypeEditor where T : FrameworkElement
    {
        /// <summary>
        /// Gets the inner control part name. Its default value is PART_EditorControl.
        /// </summary>
        protected virtual string ControlPartName
        {
            get
            {
                return "PART_EditorControl";
            }
        }

        protected virtual T EditorControl
        {
            get;
            private set;
        }

        public virtual void BindEditor()
        {
            Binding b3 = new Binding();
            b3.Converter = new IsEnabledEditorConvetrer();
            b3.Path = new PropertyPath(string.Empty);
            this.SetBinding(Control.IsEnabledProperty, b3);
        }

        protected override bool ApplyTemplateCore()
        {
            this.EditorControl = this.GetTemplatePartField<T>(this.ControlPartName);
            return base.ApplyTemplateCore();
        }

    }
}
