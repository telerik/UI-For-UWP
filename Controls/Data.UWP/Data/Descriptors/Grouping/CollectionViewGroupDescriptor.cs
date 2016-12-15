using System.ComponentModel;

namespace Telerik.Data.Core
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class CollectionViewGroupDescriptor : GroupDescriptorBase
    {
        private CollectionViewGroupDescription groupDescription;

        public CollectionViewGroupDescriptor()
        {
        }

        internal override DescriptionBase EngineDescription
        {
            get
            {
                if (this.groupDescription == null)
                {
                    this.groupDescription = new CollectionViewGroupDescription() { SortOrder = this.SortOrder };
                }

                return this.groupDescription;
            }
        }

        internal override bool Equals(DescriptionBase description)
        {
            var propertyDescription = description as CollectionViewGroupDescription;
            if (propertyDescription == null)
            {
                return false;
            }

            return this.groupDescription.Equals(description);
        }

        /// <summary>
        /// Provides an entry point for inheritors to provide additional logic over the PropertyChanged routine.
        /// </summary>
        protected override void PropertyChangedOverride(string changedPropertyName = "")
        {
            if (this.groupDescription != null)
            {
                if (changedPropertyName == "SortOrder")
                {
                    this.groupDescription.SortOrder = this.SortOrder;
                }
            }

            base.PropertyChangedOverride(changedPropertyName);
        }

        internal override bool IsDelegate
        {
            get
            {
                return true;
            }
        }
    }

}
