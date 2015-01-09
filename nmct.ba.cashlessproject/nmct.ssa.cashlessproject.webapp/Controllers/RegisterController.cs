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
using nmct.ssa.cashlessproject.webapp.PresentationModels;

namespace nmct.ssa.cashlessproject.webapp.fonts
{
    public class RegisterController : Controller
    {
        public ActionResult Index()
        {
            List<OrganisationRegister> registers = RegisterDA.GetRegisters();

            return View(registers);
        }
        /*
        public ActionResult Details(int id)
        {
            Register org = RegisterDA.GetRegister(id);

            return View(org);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            Register org = RegisterDA.GetRegister(id);

            if (org == null)
            {
                return HttpNotFound();
            }

            return View(new RegisterEditModel()
            {
                ID = org.ID,
                Login = org.Login,
                Password = org.Password,
                RegisterName = org.RegisterName,
                Address = org.Address,
                Email = org.Email,
                Phone = org.Phone
            });
        }

        [HttpPost]
        public ActionResult Edit(RegisterEditModel org)
        {
            if (ModelState.IsValid)
            {
                RegisterDA.Save(new Register()
                {
                    ID = org.ID,
                    Login = org.Login,
                    Password = org.Password,
                    RegisterName = org.RegisterName,
                    Address = org.Address,
                    Email = org.Email,
                    Phone = org.Phone
                });

                return RedirectToAction("Index");
            }

            return View(org);
        }
         */

        [HttpGet]
        public ActionResult Create()
        {
            List<SelectListItem> items = GetOrganisations();

            return View(new PMOrganisationRegister() { Organisations = items });
        }

        [HttpPost]
        public ActionResult Create(PMOrganisationRegister reg)
        {
            reg.Organisations = GetOrganisations();

            if (ModelState.IsValid)
            {
                reg.DataOrganisationRegister.Organisation = OrganisationDA.GetOrganisation(reg.DataOrganisationRegister.OrganisationID);
                int id = RegisterDA.Save(reg.DataOrganisationRegister);

                return RedirectToAction("Index");
            }

            return View(reg);
        }

        private List<SelectListItem> GetOrganisations()
        {
            List<SelectListItem> items = new List<SelectListItem>();

            foreach (Organisation org in OrganisationDA.GetOrganisations())
            {
                items.Add(new SelectListItem() { Value = org.ID.ToString(), Text = org.OrganisationName });
            }

            return items;
        }
    }
}