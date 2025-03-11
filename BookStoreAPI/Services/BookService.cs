using BookStoreAPI.Models;
using MongoDB.Driver;


namespace BookStoreAPI.Services
{
    public class BookService
    {
        private readonly IMongoCollection<Book> _booksCollection;

        public BookService(IConfiguration config)
        {
            var mongoClient = new MongoClient(config["MongoDb:ConnectionString"]);
            var mongoDatabase = mongoClient.GetDatabase(config["MongoDb:DatabaseName"]);
            _booksCollection = mongoDatabase.GetCollection<Book>(config["MongoDb:CollectionName"]);
        }

        public async Task<List<Book>> GetBookAsync() => 
            await _booksCollection.Find(_ => true).ToListAsync();

        public async Task<Book?> GetBookByIdAsync(string id) =>
            await _booksCollection.Find(book => book.Id == id).FirstOrDefaultAsync();

        public async Task CreateBookAsync(Book book)
        {
            book.Id = null;
            await _booksCollection.InsertOneAsync(book);
        }
        public async Task UpdateBookAsync(string id, Book book) =>
            await _booksCollection.ReplaceOneAsync(b => b.Id == id, book);

        public async Task DeleteBookAsync(string id) =>
            await _booksCollection.DeleteOneAsync(book => book.Id == id);
    }
}
