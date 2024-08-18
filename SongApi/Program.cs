using Microsoft.EntityFrameworkCore;
using MinimalSongApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Lägg till DbContext med SQLite som databas
// Här konfigureras tjänsterna för applikationen, inklusive anslutningen till SQLite-databasen via SongContext.
builder.Services.AddDbContext<SongContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Seed the database with initial data
// Här skapas en tjänstescope (service scope) för att kunna få åtkomst till SongContext och säkerställa att databasen skapas (om den inte redan finns).
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<SongContext>();
    context.Database.EnsureCreated(); // Skapar databasen om den inte redan existerar.
}

// Mappar en GET-förfrågan till "/songs" för att hämta alla låtar.
// När en GET-förfrågan görs till "/songs", hämtar denna metod alla låtar från databasen och returnerar dem som en lista.
app.MapGet("/songs", async (SongContext db) =>
    await db.Songs.ToListAsync());

// Mappar en GET-förfrågan till "/songs/{id}" för att hämta en specifik låt baserat på dess ID.
// När en GET-förfrågan görs till "/songs/{id}", försöker denna metod hämta låten med det angivna ID:t. Om den finns, returneras den, annars returneras en 404 Not Found.
app.MapGet("/songs/{id}", async (int id, SongContext db) =>
    await db.Songs.FindAsync(id) is Song song ? Results.Ok(song) : Results.NotFound());

// Mappar en POST-förfrågan till "/songs" för att skapa en ny låt.
// När en POST-förfrågan görs till "/songs", läggs den nya låten till i databasen och svaret innehåller en Created-status med den nya låtens information.
app.MapPost("/songs", async (Song song, SongContext db) =>
{
    db.Songs.Add(song); // Lägger till den nya låten i databasen.
    await db.SaveChangesAsync(); // Sparar ändringarna i databasen.
    return Results.Created($"/songs/{song.Id}", song); // Returnerar en 201 Created med information om den nyskapade låten.
});

// Mappar en PUT-förfrågan till "/songs/{id}" för att uppdatera en befintlig låt.
// När en PUT-förfrågan görs till "/songs/{id}", försöker denna metod uppdatera låten med det angivna ID:t. Om låten inte finns, returneras en 404 Not Found.
app.MapPut("/songs/{id}", async (int id, Song updatedSong, SongContext db) =>
{
    var song = await db.Songs.FindAsync(id); // Hittar låten med det specifika ID:t.

    if (song is null) return Results.NotFound(); // Om låten inte hittas, returneras en 404 Not Found.

    // Uppdaterar låtens egenskaper med de nya värdena.
    song.Artist = updatedSong.Artist;
    song.Title = updatedSong.Title;
    song.LengthInSeconds = updatedSong.LengthInSeconds;
    song.Category = updatedSong.Category;

    await db.SaveChangesAsync(); // Sparar ändringarna i databasen.

    return Results.NoContent(); // Returnerar en 204 No Content som indikerar att uppdateringen lyckades.
});

// Mappar en DELETE-förfrågan till "/songs/{id}" för att radera en låt.
// När en DELETE-förfrågan görs till "/songs/{id}", försöker denna metod radera låten med det angivna ID:t. Om låten inte finns, returneras en 404 Not Found.
app.MapDelete("/songs/{id}", async (int id, SongContext db) =>
{
    var song = await db.Songs.FindAsync(id); // Hittar låten med det specifika ID:t.

    if (song is null) return Results.NotFound(); // Om låten inte hittas, returneras en 404 Not Found.

    db.Songs.Remove(song); // Tar bort låten från databasen.
    await db.SaveChangesAsync(); // Sparar ändringarna i databasen.

    return Results.NoContent(); // Returnerar en 204 No Content som indikerar att raderingen lyckades.
});

app.Run(); // Startar applikationen.
