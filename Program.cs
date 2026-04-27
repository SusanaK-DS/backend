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
builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<IArticleService, ArticleService>();
builder.Services.AddScoped<IAuthService, AuthService>();

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

await EnsureTablesAsync(app.Services);

app.Run();

static async Task EnsureTablesAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var dataSource = scope.ServiceProvider.GetRequiredService<NpgsqlDataSource>();
    await using var connection = await dataSource.OpenConnectionAsync();

    await connection.ExecuteAsync(
        """
        CREATE TABLE IF NOT EXISTS books (
            id SERIAL PRIMARY KEY,
            title CHARACTER VARYING(500) NOT NULL,
            author CHARACTER VARYING(200) NOT NULL
        );
        """);

    await connection.ExecuteAsync(
        """
        CREATE TABLE IF NOT EXISTS library_users (
            id SERIAL PRIMARY KEY,
            name CHARACTER VARYING(200) NOT NULL,
            email CHARACTER VARYING(320) NOT NULL UNIQUE
        );
        """);

    await connection.ExecuteAsync(
        """
        CREATE TABLE IF NOT EXISTS author (
            id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
            name VARCHAR(100),
            email VARCHAR(100),
            birthdate DATE
        );
        """);
        
    await connection.ExecuteAsync(
        """
        CREATE TABLE IF NOT EXISTS article (
            id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
            title VARCHAR(100),
            author VARCHAR(100),
            publishedDate DATE
        );
        """);

    await connection.ExecuteAsync(
        """
        DO $$
        BEGIN
            IF EXISTS (
                SELECT 1
                FROM information_schema.columns
                WHERE table_name = 'article' AND column_name = 'publishedate'
            ) AND NOT EXISTS (
                SELECT 1
                FROM information_schema.columns
                WHERE table_name = 'article' AND column_name = 'publisheddate'
            ) THEN
                ALTER TABLE article RENAME COLUMN publishedate TO publisheddate;
            END IF;

            IF NOT EXISTS (
                SELECT 1
                FROM information_schema.columns
                WHERE table_name = 'article' AND column_name = 'publisheddate'
            ) THEN
                ALTER TABLE article ADD COLUMN publisheddate DATE;
            END IF;
        END $$;
        """);
}
