using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nmct.ba.cashlessproject.ui.ViewModel
{
    class ManagementProductsVM : ObservableObject, IPage
    {
        public ManagementProductsVM()
        {

        }

        public string Name
        {
            get { return "Products"; }
        }
    }
}
