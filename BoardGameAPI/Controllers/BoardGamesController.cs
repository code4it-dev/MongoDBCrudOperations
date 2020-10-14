using BoardGameAPI.Models;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult> GetById(string id)
        {
            var filter = Builders<Game>.Filter;
            var eqFilter = filter.Eq(x => x.AnotherId, id);

            /*NOTE: If you prefer the native ObjectId, here's how you can query Mongo using that object type*/
            // var eqFilter = filter.Eq(x => x.Id, new ObjectId(id));

            var game = await dbCollection.FindAsync<Game>(eqFilter).ConfigureAwait(false);
            return Ok(game.FirstOrDefault());
        }

        [HttpGet]
        [Route("byplayers/{players}")]
        public async Task<ActionResult> GetByName(int players)
        {
            var filter = Builders<Game>.Filter;
            var minNumberFilter = filter.Lte(x => x.MinNumberOfPlayers, players);
            var maxNumberFilter = filter.Gte(x => x.MaxNumberOfPlayers, players);

            var finalFilter = filter.And(minNumberFilter, maxNumberFilter);

            var game = await dbCollection.FindAsync<Game>(finalFilter).ConfigureAwait(false);
            return Ok(game.ToList());
        }

        [HttpPost]
        public async Task<ActionResult> Add([FromBody] Game newGame)
        {
            newGame.AnotherId = DateTime.UtcNow.ToString("yyyy-MM-dd-hh-mm-ss-ffff");
            dbCollection.InsertOne(newGame);
            return Ok();
        }

        [HttpPatch]
        public async Task<ActionResult> Update([FromQuery] string gameId, [FromQuery] string newName)
        {
            FilterDefinitionBuilder<Game> eqfilter = Builders<Game>.Filter;
            FilterDefinition<Game> eqFilterDefinition = eqfilter.Eq(x => x.AnotherId, gameId);


            UpdateDefinitionBuilder<Game> updateFilter = Builders<Game>.Update;
            UpdateDefinition<Game> updateFilterDefinition = updateFilter.Set(x => x.Name, newName);

            UpdateResult updateResult = await dbCollection.UpdateOneAsync(eqFilterDefinition, updateFilterDefinition).ConfigureAwait(false);

            if (updateResult.ModifiedCount > 0)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpDelete]
        public async Task<ActionResult> Delete([FromQuery] string gameId)
        {
            FilterDefinitionBuilder<Game> filter = Builders<Game>.Filter;
            FilterDefinition<Game> eqFilter = filter.Eq(x => x.AnotherId, gameId);
            DeleteResult res = await dbCollection.DeleteOneAsync(eqFilter).ConfigureAwait(false);

            if (res.DeletedCount > 0)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

    }
}
