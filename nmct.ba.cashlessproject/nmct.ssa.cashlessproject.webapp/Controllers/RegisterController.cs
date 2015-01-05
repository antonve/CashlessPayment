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

        [HttpGet]
        public ActionResult Create()
        {
            return View(new RegisterNewModel());
        }

        [HttpPost]
        public ActionResult Create(RegisterNewModel org)
        {
            Regex rgx = new Regex("[^a-z]");
            string db = rgx.Replace(org.RegisterName.ToLower(), "") + (new Random()).Next(0, 9999).ToString();

            if (ModelState.IsValid)
            {
                Register tempOrg = new Register()
                {
                    Login = org.Login,
                    Password = org.Password,
                    DbLogin = db,
                    DbPassword = (new Regex("[^a-zA-Z0-9]")).Replace(Cryptography.Encrypt((new Random()).Next(999, 999999).ToString()), ""),
                    DbName = db,
                    RegisterName = org.RegisterName,
                    Address = org.Address,
                    Email = org.Email,
                    Phone = org.Phone
                };

                int id = RegisterDA.Save(tempOrg);

                return RedirectToAction("Details", new { id = id });
            }

            return View(org);
        }
         */
    }
}