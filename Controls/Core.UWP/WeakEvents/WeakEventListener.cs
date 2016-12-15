using System;

namespace Telerik.Core
{
    internal interface IWeakEventListener
    {
        void ReceiveEvent(object sender, object args);
    }
}
