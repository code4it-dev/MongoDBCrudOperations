using BoardGameAPI.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoardGameAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoardGamesController : ControllerBase
    {
        private readonly MongoClient client;
        private readonly IMongoDatabase db;
        private readonly IMongoCollection<Game> dbCollection;

        public BoardGamesController()
        {
            client = new MongoClient("mongodb://mongoadmin:secret@localhost:27888/boardgamesdb?authSource=admin");
            db = client.GetDatabase("boardgamesdb");
            dbCollection = db.GetCollection<Game>("boardgames");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Game>>> GetAll()
        {
            FilterDefinitionBuilder<Game> filter = Builders<Game>.Filter;
            FilterDefinition<Game> emptyFilter = filter.Empty;
            IAsyncCursor<Game> allDocuments = await dbCollection.FindAsync<Game>(emptyFilter).ConfigureAwait(false);
            return Ok(allDocuments.ToList());
        }

        //[HttpGet]
        //[Route("{id}")]
        //public async Task<ActionResult> GetById(string id)
        //{
        //    var filter = Builders<Game>.Filter;
        //    var eqFilter = filter.Eq(x => x.Id, new ObjectId(id));
        //    var game = await dbCollection.FindAsync<Game>(eqFilter).ConfigureAwait(false);
        //    return Ok(game.FirstOrDefault());
        //}

        [HttpGet]
        [Route("byname/{name}")]
        public async Task<ActionResult> GetByName(string name)
        {
            var filter = Builders<Game>.Filter;
            var eqFilter = filter.Eq(x => x.Name, name);
            var game = await dbCollection.FindAsync<Game>(eqFilter).ConfigureAwait(false);
            return Ok(game.FirstOrDefault());
        }

        [HttpPost]
        public async Task<ActionResult> Add([FromBody] Game newGame)
        {
            newGame.AnotherId = DateTime.UtcNow.ToString("yyyy-MM-dd-hh-mm-ss-ffff");
            dbCollection.InsertOne(newGame);
            return Ok();
        }
    }
}
