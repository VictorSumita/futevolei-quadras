using FutevoleiApi.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FutevoleiApi.Entities;

public class Quadra
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("nome")]
    public string Nome { get; set; } = string.Empty;

    [BsonElement("tipoPiso")]
    [BsonRepresentation(BsonType.String)]
    public TipoPiso TipoPiso { get; set; }

    [BsonElement("coberta")]
    public bool Coberta { get; set; }

    [BsonElement("precoHora")]
    public decimal PrecoHora { get; set; }

    [BsonElement("ativa")]
    public bool Ativa { get; set; } = true;

    [BsonElement("criadaEm")]
    public DateTime CriadaEm { get; set; } = DateTime.UtcNow;
}
