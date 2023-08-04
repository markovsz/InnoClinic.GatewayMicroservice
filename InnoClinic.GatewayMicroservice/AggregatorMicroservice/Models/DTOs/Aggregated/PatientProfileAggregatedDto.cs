namespace AggregatorMicroservice.Models.DTOs.Aggregated;

public class PatientProfileAggregatedDto
{
    public Guid Id { get; set; }
    public string AccountId { get; set; }
    public string PhotoUrl { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MiddleName { get; set; }
    public DateTime BirthDate { get; set; }
    public string PhoneNumber { get; set; }
}
