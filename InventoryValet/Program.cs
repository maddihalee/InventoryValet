using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using InventoryValet.Models;
using System.Reflection.Metadata;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// allows passing datetimes without time zone data 
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// allows our api endpoints to access the database through Entity Framework Core
builder.Services.AddNpgsql<InventoryVDbContext>(builder.Configuration["InventoryVDbConnectionString"]);

// Set the JSON serializer options
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins("http://localhost:3000",
                                "http://localhost:7191")
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowAnyOrigin();
        });
});

var app = builder.Build();

app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//Get all items
app.MapGet("/items", (InventoryVDbContext db) =>
{
    return db.Items.OrderBy(item => item.Id).Take(10).ToList();
});

//Get item by ID
app.MapGet("/items/{id}", (InventoryVDbContext db, int id) =>
{
    var item=db.Items.Where(i=>i.Id == id).FirstOrDefault();

    return item;
});

//Add item
app.MapPost("/items", (InventoryVDbContext db, Item item) =>
{
    db.Items.Add(item);
    db.SaveChanges();
    return Results.Created($"/item/{item.Id}", item);
});

//Delete a item
app.MapDelete("/items/{id}", (InventoryVDbContext db, int id) =>
{
    Item item = db.Items.SingleOrDefault(i => i.Id == id);
    if (item == null)
    {
        return Results.NotFound();
    }
    db.Items.Remove(item);
    db.SaveChanges();
    return Results.NoContent();
});

//Update a recipe
app.MapPut("/items/{id}", (InventoryVDbContext db, int id, Item item) =>
{
    Item itemToUpdate = db.Items.SingleOrDefault(item => item.Id == id);
    if (itemToUpdate == null)
    {
        return Results.NotFound();
    }
    itemToUpdate.Name = item.Name;
    itemToUpdate.Description = item.Description;
    itemToUpdate.Price = item.Price;
    itemToUpdate.Image = item.Image;
    //itemToUpdate.CategoryId = item.CategoryId;
    //itemToUpdate.Size = item.Size;

    db.SaveChanges();
    return Results.NoContent();
});

//Get item by CategoryId
//app.MapGet("/itemByCategory/{categoryId}", (InventoryVDbContext db, int cid) =>
//{
//    var itemCategory = db.Items.Where(i => i.CategoryId == cid).FirstOrDefault();

//    return itemCategory;
//});


//Get all categories
app.MapGet("/categories", (InventoryVDbContext db) =>
{
    return db.Category.ToList();
});

//View items by Category
app.MapGet("/items/category/{categoryId}", (InventoryVDbContext db, int categoryId) =>
{
    var category = db.Category.Find(categoryId);
    if (category == null)
    {
        return Results.NotFound();
    }
    var items = db.Items.Where(r => r.CategoryId == categoryId).ToList();
    return Results.Ok(items);
});

// Create User
app.MapPost("/register", (InventoryVDbContext db, User user) =>
{
    db.Users.Add(user);
    db.SaveChanges();
    return Results.Created($"/user/user.Id", user);
});

//Check if a user is in the database
app.MapGet("/checkuser/{uid}", (InventoryVDbContext db, string uid) =>
{
    var user = db.Users.Where(x => x.FirebaseId == uid).ToList();
    if (uid == null)
    {
        return Results.NotFound();
    }
    else
    {
        return Results.Ok(user);
    }
});

// Get All users
app.MapGet("/user", (InventoryVDbContext db) =>
{
    return db.Users.ToList();
});

//Get users by ID
app.MapGet("/user/{id}", (InventoryVDbContext db, int id) =>
{
    var user = db.Users.Where(u => u.Id == id);
    return user;
});

app.Run();

/*[
  {
    "id": 1,
    "name": "T-Shirt",
    "description": "A plain t-shirt",
    "price": 25,
    "image": "abc123"
  },
  {
    "id": 2,
    "name": "Dress",
    "description": "red dress",
    "price": 60,
    "image": "ABC"
  }
]*/