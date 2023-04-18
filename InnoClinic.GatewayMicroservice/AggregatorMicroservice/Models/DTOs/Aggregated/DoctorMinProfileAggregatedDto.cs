using AggregatorMicroservice.Models.DTOs.Outgoing;

namespace AggregatorMicroservice.Models.DTOs.Aggregated;

public class DoctorMinProfileAggregatedDto
{
    public Guid Id { get; set; }
    public Guid PhotoId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MiddleName { get; set; }
    public SpecializationMinOutgoingDto Spectialization { get; set; }
    public OfficeAddressAggregatedDto Office { get; set; }
    public string Status { get; set; }
    public DateTime DateOfBirth { get; set; }
    public int Experience { get; set; }
}
