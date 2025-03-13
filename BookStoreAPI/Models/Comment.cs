using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace BookStoreAPI.Models
{
    public class Comment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        [BsonElement("bookId")]
        public string BookId { get; set; } = string.Empty;
        [BsonElement("user")]
        public string User { get; set; } = string.Empty;
        [BsonElement("text")]
        public string Text { get; set; } = string.Empty;
        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
