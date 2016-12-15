using System.ComponentModel;
using System.Threading.Tasks;

namespace Telerik.Data.Core
{
    public interface ISupportEntityValidation : INotifyDataErrorInfo
    {
        Task ValidatePropertyAsync(Entity entity, string propertyName);
    }
}
