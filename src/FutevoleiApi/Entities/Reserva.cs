using FutevoleiApi.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FutevoleiApi.Entities;

public class Reserva
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("quadraId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string QuadraId { get; set; } = string.Empty;

    [BsonElement("responsavel")]
    public string Responsavel { get; set; } = string.Empty;

    [BsonElement("telefone")]
    public string Telefone { get; set; } = string.Empty;

    [BsonElement("inicio")]
    public DateTime Inicio { get; set; }

    [BsonElement("fim")]
    public DateTime Fim { get; set; }

    [BsonElement("valorTotal")]
    public decimal ValorTotal { get; set; }

    [BsonElement("status")]
    [BsonRepresentation(BsonType.String)]
    public StatusReserva Status { get; set; } = StatusReserva.Confirmada;

    [BsonElement("criadaEm")]
    public DateTime CriadaEm { get; set; } = DateTime.UtcNow;
}
