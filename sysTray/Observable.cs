using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sysTray
{
    interface Observable
    {
        void subscribeObserver(Observer o);
        void unsubscribeObserver(Observer o);
        void notifyObservers();
    }
}
