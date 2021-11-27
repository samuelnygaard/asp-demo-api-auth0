using Data;
using Data.Models;
using Data.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    /// <summary>
    /// MovieController test
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class MovieController : ControllerBase
    {
        private readonly ILogger<MovieController> log;
        private readonly MovieRepository db;

        public MovieController(ILogger<MovieController> logger, MovieRepository db)
        {
            this.log = logger;
            this.db = db;
        }

        // GET: api/<MovieController>
        [HttpGet]
        public async Task<IEnumerable<Movie>> Get()
        {
            log.LogInformation("GetAll");
            return await db.GetAll();
        }

        // GET api/<MovieController>/5
        [HttpGet("{id}")]
        public async Task<Movie?> Get(int id)
        {
            log.LogInformation("Get");
            return await db.Get(id);
        }

        // POST api/<MovieController>
        [HttpPost]
        public async Task Post([FromBody] Movie data)
        {
            log.LogInformation("Post");
            await db.Add(data);
        }

        // PUT api/<MovieController>/5
        [HttpPut("{id}")]
        public async Task<Movie> Put(int id, [FromBody] Movie data)
        {
            log.LogInformation("Put");

            if (id == 0)
            {
                throw new BadHttpRequestException("Error");
            }

            return await db.Update(data);
        }

        // DELETE api/<MovieController>/5
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            log.LogInformation("Delete");
            await db.Delete(id);
        }
    }
}
