using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.UI.Xaml.Controls.Primitives.Menu
{
    internal class MenuViewState
    {
        private List<double> startAngleLevels;
        private Stack<RadialMenuItem> menuLevels;
        private double currentStartAngle;

        public MenuViewState()
        {
            this.menuLevels = new Stack<RadialMenuItem>();
            this.startAngleLevels = new List<double>();
        }

        public double CurrentStartAngle
        {
            get
            {
                return this.currentStartAngle;
            }

            set
            {
                this.currentStartAngle = value;
            }
        }

        public Stack<RadialMenuItem> MenuLevels
        {
            get
            {
                return this.menuLevels;
            }

            set
            {
                this.menuLevels = value;
            }
        }

        public List<double> StartAngleLevels
        {
            get
            {
                return this.startAngleLevels;
            }

            set
            {
                this.startAngleLevels = value;
            }
        }
    }
}
