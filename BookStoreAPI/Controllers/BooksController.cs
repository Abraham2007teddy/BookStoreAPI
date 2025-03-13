using BookStoreAPI.Models;
using BookStoreAPI.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

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


        [HttpGet]
        public async Task<ActionResult<List<Book>>> Get()
        {
            var books = await _bookService.GetBookAsync();
            return Ok(books);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> Get(string id)
        {

            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
                return NotFound();
            return Ok(book);
        }
        [HttpGet("{id}/image")]
        public async Task<IActionResult> GetBookImage(string id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null || string.IsNullOrEmpty(book.ImageBase64))
            {
                return NotFound("Image not found");
            }

            byte[] imageBytes = Convert.FromBase64String(book.ImageBase64);
            return File(imageBytes, "image/jpeg");
        }


        [HttpPost]
        public async Task<ActionResult<Book>> Post([FromForm] Book book, IFormFile? imageFile, IFormFile? pdfFile)
        {
            if (imageFile != null)
            {
                using var memoryStream = new MemoryStream();
                await imageFile.CopyToAsync(memoryStream);
                book.ImageBase64 = Convert.ToBase64String(memoryStream.ToArray());
            }

            if (pdfFile != null)
            {
                using var pdfMemoryStream = new MemoryStream();
                await pdfFile.CopyToAsync(pdfMemoryStream);
                book.PdfBase64 = Convert.ToBase64String(pdfMemoryStream.ToArray());
            }

            await _bookService.CreateBookAsync(book);
            return CreatedAtAction(nameof(Get), new { id = book.Id }, book);
        }

        

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, Book updateBook)
        {

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

            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
                return NotFound();

            await _bookService.DeleteBookAsync(id);
            return NoContent();
        }

    }
}
