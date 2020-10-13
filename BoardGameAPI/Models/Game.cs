using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BoardGameAPI.Models
{
    public class Game
    {
        /** NOTE: if you want to try the native Id handling, uncomment the line below and comment the AnotherId field*/
        // public ObjectId Id { get; set; }

        [BsonId]
        public string AnotherId { get; set; }

        [BsonElement("title")]
        public string Name { get; set; }
        public int AverageMinutesDuration { get; set; }
        public int MinNumberOfPlayers { get; set; }
        public int MaxNumberOfPlayers { get; set; }
    }
}
