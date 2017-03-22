using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDKExamples.UWP
{
    public class Example
    {
        public Example(string typeName, string displayName)
        {
            this.TypeName = typeName;
            this.DisplayName = displayName;
        }
        public string TypeName { get; set; }
        public string DisplayName { get; set; }

        public override string ToString()
        {
            return this.DisplayName;
        }
    }
}
