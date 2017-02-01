using System;
using System.Collections.Generic;
using System.Text;
using Telerik.Data.Core;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Data.DataForm
{
    public abstract class DataFormLayoutDefinition : AttachableObject<RadDataForm>
    {
        protected internal abstract Panel CreateDataFormPanel();

        protected internal abstract Panel CreateGroupLayoutPanel(string groupKey);

        protected internal virtual void SetEditorArrangeMetadata(EntityPropertyControl editorElement, EntityProperty entityProperty, Panel parentPanel)
        {
        }

        protected internal virtual void SetGroupArrangeMetadata(UIElement groupVisual, string groupName)
        {
        }

        protected internal abstract void SetEditorElementsArrangeMetadata(EntityPropertyControl editorElement, EntityProperty entityProperty);
    }
}
