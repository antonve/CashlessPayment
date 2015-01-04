using nmct.ba.cashlessproject.model;
using nmct.ssa.cashlessproject.webapp.DataAccess;
using nmct.ssa.cashlessproject.webapp.Models;
using nmct.ssa.cashlessproject.webapp.helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace nmct.ssa.cashlessproject.webapp.fonts
{
    public class OrganisationController : Controller
    {
        // GET: Organisation
        public ActionResult Index()
        {
            List<Organisation> organisations = OrganisationDA.GetOrganisations();

            return View(organisations);
        }
        
        public ActionResult Details(int id)
        {
            Organisation org = OrganisationDA.GetOrganisation(id);

            return View(org);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            Organisation org = OrganisationDA.GetOrganisation(id);

            if (org == null)
            {
                return HttpNotFound();
            }

            return View(new OrganisationEditModel()
            {
                ID = org.ID,
                Login = org.Login,
                Password = org.Password,
                OrganisationName = org.OrganisationName,
                Address = org.Address,
                Email = org.Email,
                Phone = org.Phone
            });
        }

        [HttpPost]
        public ActionResult Edit(OrganisationEditModel org)
        {
            if (ModelState.IsValid)
            {
                OrganisationDA.Save(new Organisation()
                {
                    ID = org.ID,
                    Login = org.Login,
                    Password = org.Password,
                    OrganisationName = org.OrganisationName,
                    Address = org.Address,
                    Email = org.Email,
                    Phone = org.Phone
                });

                return RedirectToAction("Index");
            }

            return View(org);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View(new OrganisationNewModel());
        }

        [HttpPost]
        public ActionResult Create(OrganisationNewModel org)
        {
            Regex rgx = new Regex("[^a-z]");
            string db = rgx.Replace(org.OrganisationName.ToLower(), "") + (new Random()).Next(0, 9999).ToString();

            if (ModelState.IsValid)
            {
                Organisation tempOrg = new Organisation()
                {
                    Login = org.Login,
                    Password = org.Password,
                    DbLogin = db,
                    DbPassword = (new Regex("[^a-zA-Z0-9]")).Replace(Cryptography.Encrypt((new Random()).Next(999, 999999).ToString()), ""),
                    DbName = db,
                    OrganisationName = org.OrganisationName,
                    Address = org.Address,
                    Email = org.Email,
                    Phone = org.Phone
                };

                int id = OrganisationDA.Save(tempOrg);

                return RedirectToAction("Details", new { id = id });
            }

            return View(org);
        }
    }
}