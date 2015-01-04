using nmct.ba.cashlessproject.model;
using nmct.ssa.cashlessproject.webapp.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace nmct.ssa.cashlessproject.webapp.fonts
{
    [RoutePrefix("Organisation")]
    public class OrganisationController : Controller
    {
        // GET: Organisation
        public ActionResult Index()
        {
            List<Organisation> organisations = OrganisationDA.GetOrganisations();

            return View(organisations);
        }

        [Route("Details")]
        public ActionResult Details(int id)
        {
            Organisation org = OrganisationDA.GetOrganisation(id);

            return View(org);
        }
    }
}