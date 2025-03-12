using MongoDB.Driver;
using BookStoreAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Read MongoDB settings from appsettings.json
var mongoSettings = builder.Configuration.GetSection("MongoDB");
var mongoClient = new MongoClient(mongoSettings["ConnectionString"]);
var database = mongoClient.GetDatabase(mongoSettings["DatabaseName"]);

// Register MongoDB collections as services
builder.Services.AddSingleton<IMongoDatabase>(database);
builder.Services.AddSingleton<IMongoCollection<User>>(database.GetCollection<User>("Users"));
builder.Services.AddSingleton<IMongoCollection<Book>>(database.GetCollection<Book>("Books"));


builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Register BookService
builder.Services.AddSingleton<BookStoreAPI.Services.BookService>();

var app = builder.Build();

app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSession();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
