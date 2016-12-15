using System;
using System.Linq;

namespace Telerik.Data.Core
{
    internal class PlaceholderInfo
    {
        public PlaceholderInfo(PlaceholderInfoType type)
        {
            this.Type = type;
        }

        public PlaceholderInfoType Type { get; private set; }
    }
}