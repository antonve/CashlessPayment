using nmct.ba.cashlessproject.api.Models;
using nmct.ba.cashlessproject.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace nmct.ba.cashlessproject.api.Controllers
{
    public class OrganisationController : ApiController
    {
        // GET: api/Organisation
        public List<Organisation> Get()
        {
            return OrganisationDA.GetOrganisations();
        }

        [HttpPost]
        public SalesAuth AuthSales([FromBody]SalesAuth data)
        {
            try
            {
                Organisation org = OrganisationDA.GetOrganisation(data.OrganisationID);
                Employee employee = EmployeeDA.GetEmployeeByName(data.EmployeeName, org);
                
                if (employee != null)
                {
                    data.Authorized = true;
                    data.EmployeeID = employee.ID;
                }

                return data;
            }
            catch (Exception)
            {
                return null;
            }

            return new SalesAuth();
        }
    }
}