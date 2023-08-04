using InnoClinic.SharedModels.DTOs.Documents.Incoming;
using InnoClinic.SharedModels.DTOs.Services.Outgoing;

namespace AggregatorMicroservice.Models.DTOs.Aggregated;

public class UpdatePatientAggregatedDto
{
    public DocumentIncomingDto Photo { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MiddleName { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime BirthDate { get; set; }
}
