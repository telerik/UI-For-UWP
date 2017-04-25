using System.ComponentModel;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Represents the class for all descriptors that define a grouping operation within a data instance.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class CollectionViewGroupDescriptor : GroupDescriptorBase
    {
        private CollectionViewGroupDescription groupDescription;

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionViewGroupDescriptor"/> class.
        /// </summary>
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

        internal override bool IsDelegate
        {
            get
            {
                return true;
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
    }
}
