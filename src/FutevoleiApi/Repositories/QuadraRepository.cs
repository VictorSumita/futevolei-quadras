using FutevoleiApi.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace FutevoleiApi.Repositories;

public class QuadraRepository : IQuadraRepository
{
    private readonly IMongoCollection<Quadra> _collection;

    public QuadraRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<Quadra>("quadras");
    }

    public async Task<List<Quadra>> ObterTodasAsync() =>
        await _collection.Find(_ => true).ToListAsync();

    public async Task<Quadra?> ObterPorIdAsync(string id)
    {
        if (!ObjectId.TryParse(id, out _))
            return null;

        return await _collection.Find(q => q.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Quadra> CriarAsync(Quadra quadra)
    {
        await _collection.InsertOneAsync(quadra);
        return quadra;
    }

    public async Task<bool> AtualizarAsync(string id, Quadra quadra)
    {
        if (!ObjectId.TryParse(id, out _))
            return false;

        quadra.Id = id;
        var resultado = await _collection.ReplaceOneAsync(q => q.Id == id, quadra);
        return resultado.MatchedCount > 0;
    }

    public async Task<bool> RemoverAsync(string id)
    {
        if (!ObjectId.TryParse(id, out _))
            return false;

        var resultado = await _collection.DeleteOneAsync(q => q.Id == id);
        return resultado.DeletedCount > 0;
    }
}
