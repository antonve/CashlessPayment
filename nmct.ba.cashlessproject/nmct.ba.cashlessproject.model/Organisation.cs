using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nmct.ba.cashlessproject.model
{
    public class Organisation
    {
        public int ID { get; set; }

        [Required]
        [DisplayName("Username")]
        public string Login { get; set; }

        public string Password { get; set; }

        [Required]
        [DisplayName("Database name")]
        public string DbName { get; set; }

        [Required]
        [DisplayName("Database username")]
        public string DbLogin { get; set; }

        [DisplayName("Database password")]
        public string DbPassword { get; set; }

        [Required]
        [DisplayName("Organisation name")]
        public string OrganisationName { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Phone { get; set; }

        public override string ToString()
        {
            return OrganisationName;
        }
    }
}
