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
            ViewBag.PossibleOrganisations = OrganisationDA.GetOrganisations();
            return View(new OrganisationRegister());
        }

        [HttpPost]
        public ActionResult Create(OrganisationRegister reg)
        {
            if (ModelState.IsValid)
            {
                int id = RegisterDA.Save(reg);

                return RedirectToAction("Index");
            }

            return View(reg);
        }
    }
}