using FutevoleiApi.Configuration;
using MongoDB.Driver;
using FutevoleiApi.Repositories;
using FutevoleiApi.Services;

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

builder.Services.AddScoped<IQuadraRepository, QuadraRepository>();
builder.Services.AddScoped<IReservaRepository, ReservaRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IQuadraService, QuadraService>();
builder.Services.AddScoped<IReservaService, ReservaService>();

var app = builder.Build();
app.UseDefaultFiles();      // serve index.html na rota "/"
app.UseStaticFiles();       // serve wwwroot

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();    // se você adicionou o JWT
app.UseAuthorization();

app.MapControllers();

app.Run();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();