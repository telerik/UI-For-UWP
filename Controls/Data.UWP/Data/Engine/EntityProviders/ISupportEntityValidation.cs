using System.ComponentModel;
using System.Threading.Tasks;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Represents an <see cref="INotifyDataErrorInfo"/> implementation.
    /// </summary>
    public interface ISupportEntityValidation : INotifyDataErrorInfo
    {
        /// <summary>
        /// Validates a specific property asynchronous.
        /// </summary>
        Task ValidatePropertyAsync(Entity entity, string propertyName);
    }
}
