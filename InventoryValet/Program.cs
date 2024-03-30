using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using InventoryValet.Models;

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
    return db.Items.ToList();
});

//Get item by ID
app.MapGet("/item/{id}", (InventoryVDbContext db, int id) =>
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