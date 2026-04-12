using Backend.Services;
using Dapper;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddSingleton<NpgsqlDataSource>(_ =>
    NpgsqlDataSource.Create(connectionString));

builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();
app.MapGet("/", () => Results.Redirect("/swagger"));
app.MapControllers();

await EnsureLibraryUsersTableAsync(app.Services);

app.Run();

static async Task EnsureLibraryUsersTableAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var dataSource = scope.ServiceProvider.GetRequiredService<NpgsqlDataSource>();
    await using var conn = await dataSource.OpenConnectionAsync();
    await conn.ExecuteAsync(
        """
        CREATE TABLE IF NOT EXISTS library_users (
            id SERIAL PRIMARY KEY,
            name CHARACTER VARYING(200) NOT NULL,
            email CHARACTER VARYING(320) NOT NULL UNIQUE
        );
        """);
}
