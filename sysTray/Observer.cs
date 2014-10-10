using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sysTray
{
    interface Observer
    {
        void update(Observable obs,  Object o);
    }
}
