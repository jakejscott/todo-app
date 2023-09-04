using dotenv.net;

namespace TodoList.Api.UnitTests;

public class TestApplication : WebApplicationFactory<Program>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        // NOTE: Load environment variables from .env file
        DotEnv.Fluent()
            .WithTrimValues()
            .WithProbeForEnv(8)
            .Load();
        
        var connectionString = Env.GetString("PLANET_SCALE_CONNECTION_STRING");
        var version = new MySqlServerVersion(ServerVersion.AutoDetect(connectionString));
        
        builder.ConfigureServices(services =>
        {
            // We're going to use the factory from our tests
            services.AddDbContextFactory<TodoContext>();
            
            // We need to replace the configuration for the DbContext to use a different configured database
            services.AddDbContextOptions<TodoContext>(o => o.UseMySql(connectionString, version));
        });
        
        
        return base.CreateHost(builder);
    }
    
    public async Task<TodoContext> CreateTodoContext()
    {
        var db = await Services.GetRequiredService<IDbContextFactory<TodoContext>>().CreateDbContextAsync();

        await db.TodoItems.ExecuteDeleteAsync();
        
        return db;
    }
}