using BookStoreAPI.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookStoreAPI.Services
{
    public class CommentService
    {
        private readonly IMongoCollection<Comment> _commentsCollection;
        private readonly IMongoCollection<Book> _booksCollection;

        public CommentService(IConfiguration config)
        {
            var mongoClient = new MongoClient(config["MongoDb:ConnectionString"]);
            var mongoDatabase = mongoClient.GetDatabase(config["MongoDb:DatabaseName"]);
            _commentsCollection = mongoDatabase.GetCollection<Comment>("Comments");
            _booksCollection = mongoDatabase.GetCollection<Book>(config["MongoDb:CollectionName"]);
        }

        public async Task<List<Comment>> GetCommentsByBookIdAsync(string bookId)
        {
            return await _commentsCollection.Find(c => c.BookId == bookId).ToListAsync();
        }

        public async Task<Book?> GetBookWithCommentsAsync(string bookId)
        {
            var book = await _booksCollection.Find(b => b.Id == bookId).FirstOrDefaultAsync();
            if (book != null)
            {
                book.Comments = await _commentsCollection.Find(c => c.BookId == bookId).ToListAsync();
            }
            return book;
        }

        public async Task<Comment?> GetCommentByIdAsync(string id)
        {
            return await _commentsCollection.Find(c => c.Id == id).FirstOrDefaultAsync();
        }

        public async Task<bool> CreateCommentAsync(Comment comment)
        {
            var book = await _booksCollection.Find(b => b.Id == comment.BookId).FirstOrDefaultAsync();
            if (book == null)
            {
                return false;
            }

            await _commentsCollection.InsertOneAsync(comment);
            return true; 
        }


        public async Task<bool> UpdateCommentAsync(string id, Comment comment)
        {
            var result = await _commentsCollection.ReplaceOneAsync(c => c.Id == id, comment);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteCommentAsync(string id)
        {
            var result = await _commentsCollection.DeleteOneAsync(c => c.Id == id);
            return result.DeletedCount > 0;
        }

    }
}
