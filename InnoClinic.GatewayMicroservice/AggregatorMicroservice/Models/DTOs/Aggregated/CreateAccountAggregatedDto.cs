using InnoClinic.SharedModels.DTOs.Documents.Incoming;

namespace AggregatorMicroservice.Models.DTOs.Aggregated;

public class CreateAccountAggregatedDto
{
    public DocumentIncomingDto Photo { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string ReEnteredPassword { get; set; }
}
