using Telerik.Data.Core;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Data.DataForm
{
    /// <summary>
    /// Represents an DataFormLayoutDefinition object.
    /// </summary>
    public abstract class DataFormLayoutDefinition : AttachableObject<RadDataForm>
    {
        /// <inheritdoc/>
        protected internal abstract Panel CreateDataFormPanel();

        /// <inheritdoc/>
        protected internal abstract Panel CreateGroupLayoutPanel(string groupKey);

        /// <inheritdoc/>
        protected internal virtual void SetEditorArrangeMetadata(EntityPropertyControl editorElement, EntityProperty entityProperty, Panel parentPanel)
        {
        }

        /// <inheritdoc/>
        protected internal virtual void SetGroupArrangeMetadata(UIElement groupVisual, string groupName)
        {
        }

        /// <inheritdoc/>
        protected internal abstract void SetEditorElementsArrangeMetadata(EntityPropertyControl editorElement, EntityProperty entityProperty);
    }
}
