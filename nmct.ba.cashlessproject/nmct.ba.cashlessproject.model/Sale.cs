using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nmct.ba.cashlessproject.model
{
    public class Sale
    {
        public int ID { get; set; }

        public int RegisterID { get; set; }

        public string RegisterName { get; set; }

        public int CustomerID { get; set; }

        public string CustomerName { get; set; }

        public int ProductID { get; set; }

        public string ProductName { get; set; }

        public int Amount { get; set; }

        public double SinglePrice { get; set; }

        public double TotalPrice { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
