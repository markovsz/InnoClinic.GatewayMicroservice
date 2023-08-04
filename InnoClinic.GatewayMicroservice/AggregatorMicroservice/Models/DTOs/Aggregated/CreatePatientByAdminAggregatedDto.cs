using InnoClinic.SharedModels.DTOs.Documents.Incoming;

namespace AggregatorMicroservice.Models.DTOs.Aggregated;

public class CreatePatientByAdminAggregatedDto
{
    public DocumentIncomingDto Photo { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MiddleName { get; set; }
    public string Email { get; set; }
    public DateTime BirthDate { get; set; }
}
