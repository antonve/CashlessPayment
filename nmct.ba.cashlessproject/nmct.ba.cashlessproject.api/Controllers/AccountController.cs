using nmct.ba.cashlessproject.api.Models;
using nmct.ba.cashlessproject.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using Microsoft.AspNet.Identity;

namespace nmct.ba.cashlessproject.api.Controllers
{
    [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        // PUT: api/Account/ChangePassword
        [Route("ChangePassword")]
        public HttpResponseMessage PutChangePassword([FromBody]ChangePasswordBindingModel model)
        {
            HttpResponseMessage br = new HttpResponseMessage() { StatusCode = HttpStatusCode.BadRequest };

            if (!ModelState.IsValid)
            {
                br.ReasonPhrase = "Passwords don't match.";
                return br;
            }

            ClaimsPrincipal p = RequestContext.Principal as ClaimsPrincipal;
            var data = User.Identity;
            int affected = AccountDA.UpdatePassword(Convert.ToInt32(p.Claims.FirstOrDefault(c => c.Type == "userid").Value), model, p.Claims);

            if (affected != 0)
            {
                return new HttpResponseMessage() {StatusCode = HttpStatusCode.OK};
            }

            br.ReasonPhrase = "Old password incorrect.";
            return br;
        }
    }
}
