using InnoClinic.SharedModels.DTOs.Documents.Incoming;

namespace AggregatorMicroservice.Models.DTOs.Aggregated;

public class CreateDoctorAggregatedDto
{
    public DocumentIncomingDto Photo { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MiddleName { get; set; }
    public DateTime BirthDate { get; set; }
    public string Email { get; set; }
    public Guid SpecializationId { get; set; }
    public Guid OfficeId { get; set; }
    public int CareerStartYear { get; set; }
    public string Status { get; set; }
}
