using BookStoreAPI.Models;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace BookStoreAPI.Services
{
    public class BookService
    {
        private readonly IMongoCollection<Book> _booksCollection;
        private readonly IMongoCollection<Comment> _commentsCollection;
        private readonly GridFSBucket _gridFS;

        public BookService(IConfiguration config)
        {
            var mongoClient = new MongoClient(config["MongoDb:ConnectionString"]);
            var mongoDatabase = mongoClient.GetDatabase(config["MongoDb:DatabaseName"]);
            _booksCollection = mongoDatabase.GetCollection<Book>(config["MongoDb:CollectionName"]);
            _commentsCollection = mongoDatabase.GetCollection<Comment>(config["MongoDb:CommentsCollectionName"]);
            _gridFS = new GridFSBucket(mongoDatabase);
        }

        public async Task<List<Book>> GetBookAsync() => 
            await _booksCollection.Find(_ => true).ToListAsync();

        public async Task<Book?> GetBookByIdAsync(string id)
        {
            var book = await _booksCollection.Find(b => b.Id == id).FirstOrDefaultAsync();
            if (book != null)
            {
                book.Comments = await _commentsCollection.Find(c => c.BookId == id).ToListAsync();
            }
            return book;
        }

        public async Task CreateBookAsync(Book book)
        {
            book.Id = null;
            await _booksCollection.InsertOneAsync(book);
        }
        public async Task UpdateBookAsync(string id, Book book) =>
            await _booksCollection.ReplaceOneAsync(b => b.Id == id, book);

        public async Task DeleteBookAsync(string id)
        {
            await _booksCollection.DeleteOneAsync(book => book.Id == id);
            await _commentsCollection.DeleteManyAsync(comment => comment.BookId == id);
        }
    }
}
