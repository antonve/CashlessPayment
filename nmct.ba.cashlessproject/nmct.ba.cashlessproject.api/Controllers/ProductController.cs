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
    public class ProductController : ApiController
    {
        // GET: api/product
        public List<Product> Get()
        {
            ClaimsPrincipal p = RequestContext.Principal as ClaimsPrincipal;
            return ProductDA.GetProducts(p.Claims);
        }

        // GET: api/product/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/product
        public int Post([FromBody]Product p)
        {
            return ProductDA.SaveProduct(p);
        }

        // PUT: api/product/5
        public int Put(int id, [FromBody]Product p)
        {
            return ProductDA.UpdateProduct(p);
        }

        // DELETE: api/product/5
        public int Delete(int id)
        {
            return ProductDA.DeleteProduct(id);
        }
    }
}
