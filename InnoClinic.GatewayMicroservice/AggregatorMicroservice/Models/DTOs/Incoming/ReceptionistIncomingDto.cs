namespace AggregatorMicroservice.Models.DTOs.Incoming;

public class ReceptionistIncomingDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MiddleName { get; set; }
    public Guid OfficeId { get; set; }
}
