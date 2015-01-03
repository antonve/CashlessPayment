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
    [Authorize]
    public class EmployeeController : ApiController
    {
        // GET: api/Employee
        public List<Employee> Get()
        {
            ClaimsPrincipal p = RequestContext.Principal as ClaimsPrincipal;
            return EmployeeDA.GetEmployees(p.Claims);
        }

        // POST: api/Employee
        public int Post([FromBody]Employee prod)
        {
            ClaimsPrincipal p = RequestContext.Principal as ClaimsPrincipal;
            return EmployeeDA.SaveEmployee(prod, p.Claims);
        }

        // PUT: api/Employee/5
        public int Put(int id, [FromBody]Employee prod)
        {
            ClaimsPrincipal p = RequestContext.Principal as ClaimsPrincipal;
            return EmployeeDA.UpdateEmployee(prod, p.Claims);
        }

        // DELETE: api/Employee/5
        public int Delete(int id)
        {
            ClaimsPrincipal p = RequestContext.Principal as ClaimsPrincipal;
            return EmployeeDA.DeleteEmployee(id, p.Claims);
        }
    }
}
