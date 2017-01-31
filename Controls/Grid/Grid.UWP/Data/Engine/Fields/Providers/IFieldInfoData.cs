namespace Telerik.Data.Core.Fields
{
    /// <summary>
    /// Interface used to provide <see cref="IDataFieldInfo"/> for specific data source.
    /// </summary>
    internal interface IFieldInfoData
    {
        /// <summary>
        /// Gets the root node of the hierarchy of <see cref="IDataFieldInfo"/> instances.
        /// </summary>
        /// <returns></returns>
        ContainerNode RootFieldInfo { get; }
        
        /// <summary>
        /// Gets a <see cref="IDataFieldInfo"/> instance by name.
        /// </summary>
        /// <param name="name">Name of a description.</param>
        /// <returns></returns>
        IDataFieldInfo GetFieldDescriptionByMember(string name);
    }
}