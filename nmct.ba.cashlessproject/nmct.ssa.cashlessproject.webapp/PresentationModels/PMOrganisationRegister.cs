using nmct.ba.cashlessproject.model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace nmct.ssa.cashlessproject.webapp.PresentationModels
{
    public class PMOrganisationRegister
    {
        public PMOrganisationRegister()
        {
            DataOrganisationRegister = new OrganisationRegister();
        }

        [Required]
        public OrganisationRegister DataOrganisationRegister { get; set; }

        public List<SelectListItem> Organisations;
    }
}