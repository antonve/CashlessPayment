﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nmct.ba.cashlessproject.model
{
    public class OrganisationRegister
    {
        public int ID { get; set; }

        [Required]
        public string RegisterName { get; set; }

        [Required]
        public string Device { get; set; }

        [Required]
        public int OrganisationID { get; set; }

        public int ExternalID { get; set; }

        public Organisation Organisation { get; set; }

        public override string ToString()
        {
            return RegisterName;
        }
    }
}
