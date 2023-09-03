namespace TodoList.Api.UnitTests;

public class TestApplication : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _sqliteConnection = new("Filename=:memory:");

    protected override IHost CreateHost(IHostBuilder builder)
    {
        _sqliteConnection.Open();
        
        builder.ConfigureServices(services =>
        {
            // We're going to use the factory from our tests
            services.AddDbContextFactory<TodoContext>();
            
            // We need to replace the configuration for the DbContext to use a different configured database
            services.AddDbContextOptions<TodoContext>(o => o.UseSqlite(_sqliteConnection));
        });
        
        
        return base.CreateHost(builder);
    }
    
    public TodoContext CreateTodoContext()
    {
        var db = Services.GetRequiredService<IDbContextFactory<TodoContext>>().CreateDbContext();
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
        return db;
    }

    protected override void Dispose(bool disposing)
    {
        _sqliteConnection?.Dispose();
        base.Dispose(disposing);
    }
}