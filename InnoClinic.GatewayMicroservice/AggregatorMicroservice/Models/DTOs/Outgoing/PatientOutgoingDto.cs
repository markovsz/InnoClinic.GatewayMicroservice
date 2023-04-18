namespace AggregatorMicroservice.Models.DTOs.Outgoing;

public class PatientOutgoingDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MiddleName { get; set; }
    public string PhoneNumber { get; set; }
    public bool IsLinkedToAccount { get; set; }
    public DateTime BirthDate { get; set; }
    public string AccountId { get; set; }
}
