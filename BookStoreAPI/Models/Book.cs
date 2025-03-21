﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace BookStoreAPI.Models
{
    public class Book
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        [BsonElement("title")]
        public string Title { get; set; } = string.Empty;
        [BsonElement("author")]
        public string Author { get; set; } = string.Empty;
        [BsonElement("price")]
        public decimal Price { get; set; }
        [BsonElement("image")]
        public string? ImageBase64 { get; set; }
        [BsonElement("pdf")]
        public string? PdfBase64 { get; set; }
        //Comments for book
        [BsonElement("comments")]
        public List<Comment> Comments { get; set; } = new List<Comment>();
    }
}
