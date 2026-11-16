using FutevoleiApi.Entities;
using FutevoleiApi.Enums;
using MongoDB.Bson;
using MongoDB.Driver;

namespace FutevoleiApi.Repositories;

public class ReservaRepository : IReservaRepository
{
    private readonly IMongoCollection<Reserva> _collection;

    public ReservaRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<Reserva>("reservas");
    }

    public async Task<List<Reserva>> ObterTodasAsync() =>
        await _collection.Find(_ => true)
            .SortByDescending(r => r.Inicio)
            .ToListAsync();

    public async Task<List<Reserva>> ObterPorQuadraAsync(string quadraId) =>
        await _collection.Find(r => r.QuadraId == quadraId)
            .SortBy(r => r.Inicio)
            .ToListAsync();

    public async Task<Reserva?> ObterPorIdAsync(string id)
    {
        if (!ObjectId.TryParse(id, out _))
            return null;

        return await _collection.Find(r => r.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Reserva> CriarAsync(Reserva reserva)
    {
        await _collection.InsertOneAsync(reserva);
        return reserva;
    }

    public async Task<bool> AtualizarAsync(string id, Reserva reserva)
    {
        if (!ObjectId.TryParse(id, out _))
            return false;

        reserva.Id = id;
        var resultado = await _collection.ReplaceOneAsync(r => r.Id == id, reserva);
        return resultado.MatchedCount > 0;
    }

    public async Task<bool> RemoverAsync(string id)
    {
        if (!ObjectId.TryParse(id, out _))
            return false;

        var resultado = await _collection.DeleteOneAsync(r => r.Id == id);
        return resultado.DeletedCount > 0;
    }

    public async Task<bool> ExisteConflitoAsync(string quadraId, DateTime inicio, DateTime fim, string? ignorarReservaId = null)
    {
        var filtros = new List<FilterDefinition<Reserva>>
        {
            Builders<Reserva>.Filter.Eq(r => r.QuadraId, quadraId),
            Builders<Reserva>.Filter.Ne(r => r.Status, StatusReserva.Cancelada),
            Builders<Reserva>.Filter.Lt(r => r.Inicio, fim),
            Builders<Reserva>.Filter.Gt(r => r.Fim, inicio)
        };

        if (!string.IsNullOrEmpty(ignorarReservaId))
            filtros.Add(Builders<Reserva>.Filter.Ne(r => r.Id, ignorarReservaId));

        var filtro = Builders<Reserva>.Filter.And(filtros);
        return await _collection.Find(filtro).AnyAsync();
    }
}
