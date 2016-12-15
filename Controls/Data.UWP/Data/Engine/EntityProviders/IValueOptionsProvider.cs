using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.Data.Core
{
    public interface IValueOptionsProvider
    {
        IList GetOptions(EntityProperty property);
    }
}
