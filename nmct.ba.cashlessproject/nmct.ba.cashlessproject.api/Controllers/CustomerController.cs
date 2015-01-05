using nmct.ba.cashlessproject.api.Models;
using nmct.ba.cashlessproject.model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace nmct.ba.cashlessproject.api.Controllers
{
    [Authorize]
    public class CustomerController : ApiController
    {
        // GET: api/Customer
        public List<Customer> Get()
        {
            ClaimsPrincipal p = RequestContext.Principal as ClaimsPrincipal;
            return CustomerDA.GetCustomers(p.Claims);
        }

        // POST: api/Customer/GetByName/:orgid
        [AllowAnonymous]
        [HttpPost]
        public Customer GetByName(int org, [FromBody]string name)
        {
            ConnectionStringSettings cs = OrganisationDA.GetCSById(org);

            if (cs == null)
            {
                return null;
            }

            return CustomerDA.GetCustomerByName(cs, name);
        }

        // PUT: api/Customer/5
        public int Put(int id, [FromBody]Customer cust)
        {
            ClaimsPrincipal p = RequestContext.Principal as ClaimsPrincipal;
            return CustomerDA.UpdateCustomer(cust, p.Claims);
        }
    }
}