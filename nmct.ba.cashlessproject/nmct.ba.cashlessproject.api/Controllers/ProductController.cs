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
    public class ProductController : ApiController
    {
        // GET: api/product
        public List<Product> Get()
        {
            ClaimsPrincipal p = RequestContext.Principal as ClaimsPrincipal;
            return ProductDA.GetProducts(p.Claims);
        }

        // POST: api/product
        public int Post([FromBody]Product prod)
        {
            ClaimsPrincipal p = RequestContext.Principal as ClaimsPrincipal;
            return ProductDA.SaveProduct(prod, p.Claims);
        }

        // PUT: api/product/5
        public int Put(int id, [FromBody]Product prod)
        {
            ClaimsPrincipal p = RequestContext.Principal as ClaimsPrincipal;
            return ProductDA.UpdateProduct(prod, p.Claims);
        }

        // DELETE: api/product/5
        public int Delete(int id)
        {
            ClaimsPrincipal p = RequestContext.Principal as ClaimsPrincipal;
            return ProductDA.DeleteProduct(id, p.Claims);
        }
    }
}
