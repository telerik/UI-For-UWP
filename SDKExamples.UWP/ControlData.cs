using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDKExamples.UWP
{
    public class ControlData
    {
        public ControlData(string name, IEnumerable<Example> examples)
        {
            this.Name = name;
            this.Examples = examples;
        }

        public string Name { get; set; }
        public IEnumerable<Example> Examples { get; set; }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
