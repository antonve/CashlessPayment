using nmct.ba.cashlessproject.api.Models;
using nmct.ba.cashlessproject.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace nmct.ba.cashlessproject.api.Controllers
{
    public class ProductController : ApiController
    {
        // GET: api/product
        public List<Product> Get()
        {
            return ProductDA.GetProducts();
        }

        // GET: api/product/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/product
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/product/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/product/5
        public void Delete(int id)
        {
        }
    }
}
