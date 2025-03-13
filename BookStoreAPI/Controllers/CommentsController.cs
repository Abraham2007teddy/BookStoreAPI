using BookStoreAPI.Models;
using BookStoreAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class CommentsController : ControllerBase
{
    private readonly CommentService _commentService;
    private readonly BookService _bookService;

    public CommentsController(CommentService commentService, BookService bookService)
    {
        _commentService = commentService;
        _bookService = bookService;
    }

    [HttpGet("{bookId}")]
    public async Task<ActionResult<List<Comment>>> GetComments(string bookId)
    {
        var comments = await _commentService.GetCommentsByBookIdAsync(bookId);
        return Ok(comments);
    }

    [HttpPost]
    public async Task<ActionResult<Comment>> Post([FromBody] Comment comment)
    {
        var book = await _bookService.GetBookByIdAsync(comment.BookId);
        if (book == null)
        {
            return BadRequest("Invalid BookId. Book does not exist.");
        }

        await _commentService.CreateCommentAsync(comment);
        return CreatedAtAction(nameof(GetComments), new { bookId = comment.BookId }, comment);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] Comment updateData)
    {
        var existingComment = await _commentService.GetCommentByIdAsync(id);
        if (existingComment == null)
            return NotFound();

        await _commentService.UpdateCommentAsync(id, updateData);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var comment = await _commentService.GetCommentByIdAsync(id);
        if (comment == null)
            return NotFound();

        await _commentService.DeleteCommentAsync(id);
        return NoContent();
    }
}

