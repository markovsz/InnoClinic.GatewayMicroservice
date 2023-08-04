using InnoClinic.SharedModels.DTOs.Documents.Incoming;

namespace AggregatorMicroservice.Models.DTOs.Aggregated;

public class UpdateReceptionistAggregatedDto
{
    public DocumentIncomingDto Photo { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MiddleName { get; set; }
    public Guid OfficeId { get; set; }
}
