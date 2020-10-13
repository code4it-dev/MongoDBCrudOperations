using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDBOperations.Models;
using System;
using System.Linq;

namespace MongoDBOperations.Controllers
{
    public class BoardGameController : Controller
    {
        private readonly MongoClient client;
        private readonly IMongoDatabase db;
        private readonly IMongoCollection<Game> dbCollection;

        public BoardGameController()
        {
            client = new MongoClient("mongodb://mongoadmin:secret@localhost:27888/boardgamesdb?authSource=admin");
            db = client.GetDatabase("boardgamesdb");
            dbCollection = db.GetCollection<Game>("boardgames");

        }

        // GET: BoardGameController
        public ActionResult Index()
        {
            var filter = Builders<Game>.Filter;
            var allDocuments = dbCollection.Find<Game>(filter.Empty).ToList();

            return View(allDocuments);
        }

        // GET: BoardGameController/Details/5
        public ActionResult Details(string id)
        {

            var filter = Builders<Game>.Filter;
            var eqFilter = filter.Eq(x => x.Id, new ObjectId(id));
            var game = dbCollection.Find<Game>(eqFilter).FirstOrDefault();

            return View(game);

        }

        // GET: BoardGameController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BoardGameController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                var game = new Game
                {
                    Name = collection["Name"],
                    AverageMinutesDuration = Convert.ToInt32(collection["AverageMinutesDuration"]),
                    MinNumberOfPlayers = Convert.ToInt32(collection["MinNumberOfPlayers"]),
                    MaxNumberOfPlayers = Convert.ToInt32(collection["MaxNumberOfPlayers"])
                };

                dbCollection.InsertOne(game);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: BoardGameController/Edit/5
        public ActionResult Edit(string id)
        {

            var filter = Builders<Game>.Filter;
            var eqFilter = filter.Eq(x => x.Id, new ObjectId(id));
            var game = dbCollection.Find<Game>(eqFilter).FirstOrDefault();

            return View(game);
        }

        // POST: BoardGameController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, IFormCollection collection)
        {
            try
            {
                var filter = Builders<Game>.Filter;
                var eqFilter = filter.Eq(x => x.Id, new ObjectId(id));
                //var game = dbCollection.Find<Game>(eqFilter).FirstOrDefault();

                var newGame = new Game
                {
                    Name = collection["Name"],
                    AverageMinutesDuration = Convert.ToInt32(collection["AverageMinutesDuration"]),
                    MinNumberOfPlayers = Convert.ToInt32(collection["MinNumberOfPlayers"]),
                    MaxNumberOfPlayers = Convert.ToInt32(collection["MaxNumberOfPlayers"])
                };
                newGame.Id = new ObjectId(id); // Importante per evitare errore
                // A bulk write operation resulted in one or more errors.\r\n  After applying the update, the (immutable) field '_id' was found to have been altered to _id: ObjectId('000000000000000000000000')"}, 

                dbCollection.ReplaceOne(eqFilter, newGame);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return View();
            }
        }

        // GET: BoardGameController/Delete/5
        public ActionResult Delete(string id)
        {
            var filter = Builders<Game>.Filter;
            var eqFilter = filter.Eq(x => x.Id, new ObjectId(id));
            var game = dbCollection.Find<Game>(eqFilter).FirstOrDefault();

            return View(game);
        }

        // POST: BoardGameController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id, IFormCollection collection)
        {
            try
            {
                var filter = Builders<Game>.Filter;
                var eqFilter = filter.Eq(x => x.Id, new ObjectId(id));
                dbCollection.DeleteOne(eqFilter);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
