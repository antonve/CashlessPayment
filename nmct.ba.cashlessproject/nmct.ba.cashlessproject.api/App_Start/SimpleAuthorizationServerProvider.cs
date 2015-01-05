using Microsoft.Owin.Security.OAuth;
using nmct.ba.cashlessproject.api.Models;
using nmct.ba.cashlessproject.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace nmct.ba.cashlessproject.api
{
    public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            string mode = context.UserName.Substring(0, 1);
            Organisation o = null;
            bool fail = true;
            string username = context.UserName.Substring(1);

            switch (mode)
            {
                case "m":
                    o = OrganisationDA.CheckCredentials(username, context.Password);
                    if (o != null)
                    {
                        fail = false;
                    }
                    break;
                case "s":
                    o = OrganisationDA.GetOrganisation(Int32.Parse(username));
                    Employee employee = EmployeeDA.GetEmployeeByName(context.Password, o);
                    if (o != null && employee != null)
                    {
                        fail = false;
                    }
                    break;
                case "r":
                    o = OrganisationDA.GetOrganisation(Int32.Parse(username));
                    Customer customer = CustomerDA.GetCustomerByName(context.Password, o);
                    if (o != null && customer != null)
                    {
                        fail = false;
                    }
                    break;
            }
            if (fail)
            {
                context.Rejected();
                return Task.FromResult(0);
            }

            var id = new ClaimsIdentity(context.Options.AuthenticationType);
            id.AddClaim(new Claim("userid", o.ID.ToString()));
            id.AddClaim(new Claim("dbname", o.DbName));
            id.AddClaim(new Claim("dblogin", o.DbLogin));
            id.AddClaim(new Claim("dbpass", o.DbPassword));

            context.Validated(id);
            return Task.FromResult(0);
        }
    }
}