using BookStoreAPI.Models;
using BookStoreAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BookStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly BookService _bookService;
        private readonly IMongoCollection<User> _users;

        public BooksController(BookService bookService, IMongoDatabase database)
        {
            _bookService = bookService;
            _users = database.GetCollection<User>("Users");
        }

        private async Task<bool> IsUserAuthenticated(HttpContext httpContext)
        {
            var username = httpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username)) return false;

            var existingUser = await _users.Find(u => u.Username == username).FirstOrDefaultAsync();
            return existingUser != null;
        }

        [HttpGet]
        public async Task<ActionResult<List<Book>>> Get()
        {
            if (!await IsUserAuthenticated(HttpContext))
                return Unauthorized(new { message = "Please log in first" });

            var books = await _bookService.GetBookAsync();
            return Ok(books);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> Get(string id)
        {
            if (!await IsUserAuthenticated(HttpContext))
                return Unauthorized(new { message = "Please log in first" });

            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
                return NotFound();
            return Ok(book);
        }

        [HttpPost]
        public async Task<ActionResult<Book>> Post(Book book)
        {
            if (!await IsUserAuthenticated(HttpContext))
                return Unauthorized(new { message = "Please log in first" });

            await _bookService.CreateBookAsync(book);
            return CreatedAtAction(nameof(Get), new { id = book.Id }, book);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, Book updateBook)
        {
            if (!await IsUserAuthenticated(HttpContext))
                return Unauthorized(new { message = "Please log in first" });

            var existingBook = await _bookService.GetBookByIdAsync(id);
            if (existingBook == null)
                return NotFound();

            updateBook.Id = id;
            await _bookService.UpdateBookAsync(id, updateBook);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (!await IsUserAuthenticated(HttpContext))
                return Unauthorized(new { message = "Please log in first" });

            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
                return NotFound();

            await _bookService.DeleteBookAsync(id);
            return NoContent();
        }

        [HttpGet("check-session")]
        public async Task<IActionResult> CheckSession()
        {
            var username = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(username))
            {
                return Ok(new { message = "No active session", userExists = false });
            }

            var userExists = await _users.Find(u => u.Username == username).FirstOrDefaultAsync();

            return Ok(new { username, userExists = userExists != null });
        }

    }
}
