using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Server.Models;
using Server.Data;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CardController : ControllerBase
    {
        public IRepository<CardDto> Repository { get; }

        public CardController(IRepository<CardDto> repository)
        {
            Repository = repository;
        }

        // GET: api/<CardController>
        [HttpGet]
        public IEnumerable<CardDto> Get()
        {
            return Repository.GetAll();
        }

        // GET api/<CardController>/5
        [HttpGet("{id}")]
        public CardDto Get(int id)
        {
            return Repository.Get(id);
        }

        // POST api/<CardController>
        [HttpPost]
        public void Post([FromBody] CardDto value)
        {
            if(!Repository.Insert(value))
            {
                Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
            }
        }

        // PUT api/<CardController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] CardDto value)
        {
            value.Id = id;
            if (!Repository.Update(value))
            {
                Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
            }
        }

        // DELETE api/<CardController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            if (!Repository.Delete(id))
            {
                Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
            }
        }
    }
}
