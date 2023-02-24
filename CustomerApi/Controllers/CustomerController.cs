using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomerApi.Data;
using CustomerApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CustomerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IRepository _repo;

        public CustomerController(IRepository repo)
        {
            _repo = repo;
        }

        // GET: api/Customer/5
        [HttpGet("{id}", Name = "Get")]
        public Customer Get(int id)
        {
            return _repo.Get(id);
        }

        // POST: api/Customer
        [HttpPost]
        public Customer Post([FromBody] Customer value)
        {
            return _repo.Add(value);
        }

        // PUT: api/Customer
        [HttpPut]
        public Customer Put([FromBody] Customer value)
        {
            return _repo.Update(value);
        }

        // DELETE: api/Customer/5
        [HttpDelete("{id}")]
        public bool Delete(int id)
        {
            return _repo.Delete(id);
        }
    }
}
