using Telerik.UI.Xaml.Controls.Data.DataForm;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Data
{
    /// <summary>
    /// Represents a CustomEditorBase control.
    /// </summary>
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

        /// <summary>
        /// Gets the editor control.
        /// </summary>
        protected virtual T EditorControl
        {
            get;
            private set;
        }

        /// <summary>
        /// Method used for generating bindings for the <see cref="ITypeEditor"/> properties.
        /// </summary>
        public virtual void BindEditor()
        {
            Binding b3 = new Binding();
            b3.Converter = new IsEnabledEditorConvetrer();
            b3.Path = new PropertyPath(string.Empty);
            this.SetBinding(Control.IsEnabledProperty, b3);
        }

        /// <inheritdoc/>
        protected override bool ApplyTemplateCore()
        {
            this.EditorControl = this.GetTemplatePartField<T>(this.ControlPartName);
            return base.ApplyTemplateCore();
        }
    }
}
