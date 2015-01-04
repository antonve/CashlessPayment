using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nmct.ba.cashlessproject.model
{
    public class SalesAuth
    {
        public int OrganisationID { get; set; }
        public string DbName { get; set; }
        public string DbLogin { get; set; }
        public string DbPassword { get; set; }
        public string OrganisationName { get; set; }
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public bool Authorized { get; set; }
    }
}
