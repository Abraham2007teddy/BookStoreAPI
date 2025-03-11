using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using BookStoreAPI.Models;
using BCrypt.Net;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IMongoCollection<User> _users;

    public AuthController(IMongoDatabase database)
    {
        _users = database.GetCollection<User>("Users");
    }

    [HttpPost("signup")]
    public async Task<IActionResult> SignUp([FromBody] User user)
    {
        var existingUser = await _users.Find(u => u.Username == user.Username).FirstOrDefaultAsync();
        if (existingUser != null)
            return BadRequest(new { message = "User already exists" });

        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password); // Ensure correct reference
        await _users.InsertOneAsync(user);
        return Ok(new { message = "User registered successfully" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] User user)
    {
        var existingUser = await _users.Find(u => u.Username == user.Username).FirstOrDefaultAsync();
        if (existingUser == null || !BCrypt.Net.BCrypt.Verify(user.Password, existingUser.Password))
            return Unauthorized(new { message = "Invalid credentials" });

        HttpContext.Session.SetString("Username", existingUser.Username);
        return Ok(new { message = "User logged in successfully" });
    }

    [HttpGet("logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return Ok(new { message = "User logged out successfully" });
    }
}
