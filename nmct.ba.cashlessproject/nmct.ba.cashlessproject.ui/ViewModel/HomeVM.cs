using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nmct.ba.cashlessproject.ui.ViewModel
{
    class HomeVM : ObservableObject, IPage
    {
        public HomeVM()
        {
        }

        public string Name
        {
            get { return "Home"; }
        }
    }
}
