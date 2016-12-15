using System.ComponentModel;
using System.Threading.Tasks;

namespace Telerik.Core
{
    /// <summary>
    /// Defines members that data entity classes can implement to provide custom
    /// synchronous and asynchronous validation support. Extends <see cref="INotifyDataErrorInfo"/> and adds an option for synchronous execution of async validation.
    /// </summary>
    public interface IAsyncDataErrorInfo : INotifyDataErrorInfo
    {
        /// <summary>
        /// Triggers validate logic asynchronously.
        /// </summary>
        /// <param name="propertyName">The name of the property that needs validation.</param>
        Task ValidateAsync(string propertyName);
    }
}
