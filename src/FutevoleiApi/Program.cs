using FutevoleiApi.Configuration;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDb"));

var mongoSettings = builder.Configuration.GetSection("MongoDb").Get<MongoDbSettings>()
    ?? throw new InvalidOperationException("Seção MongoDb não encontrada na configuração.");

builder.Services.AddSingleton<IMongoClient>(_ =>
    new MongoClient(mongoSettings.ConnectionString));

builder.Services.AddScoped<IMongoDatabase>(sp =>
    sp.GetRequiredService<IMongoClient>().GetDatabase(mongoSettings.DatabaseName));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();