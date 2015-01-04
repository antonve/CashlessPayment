using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nmct.ba.cashlessproject.model
{
    public class Organisation
    {
        public int ID { get; set; }

        [DisplayName("Username")]
        public string Login { get; set; }

        public string Password { get; set; }

        [DisplayName("Database name")]
        public string DbName { get; set; }

        [DisplayName("Database username")]
        public string DbLogin { get; set; }

        public string DbPassword { get; set; }

        [DisplayName("Organisation name")]
        public string OrganisationName { get; set; }

        public string Address { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }
    }
}
