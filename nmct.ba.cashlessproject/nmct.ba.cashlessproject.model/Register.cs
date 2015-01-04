using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nmct.ba.cashlessproject.model
{
    public class Register
    {
        public int ID { get; set; }

        public string RegisterName { get; set; }

        public string Device { get; set; }

        public List<RegisterLog> Logs { get; set; }

        public override string ToString()
        {
            return RegisterName;
        }
    }
}
