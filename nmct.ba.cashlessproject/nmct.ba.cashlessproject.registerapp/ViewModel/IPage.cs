using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nmct.ba.cashlessproject.registerapp.ViewModel
{
    interface IPage
    {
        string Name { get; }
        bool IsActive { get; set; }
    }
}
