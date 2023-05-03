namespace AggregatorMicroservice.Models.DTOs.Incoming;

public class DoctorIncomingDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MiddleName { get; set; }
    public DateTime BirthDate { get; set; }
    public string AccountId { get; set; }
    public Guid SpectializationId { get; set; }
    public Guid OfficeId { get; set; }
    public int CareerStartYear { get; set; }
    public string Status { get; set; }
}
