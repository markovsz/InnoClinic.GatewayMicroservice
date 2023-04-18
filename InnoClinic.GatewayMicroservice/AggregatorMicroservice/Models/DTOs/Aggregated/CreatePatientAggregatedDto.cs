namespace AggregatorMicroservice.Models.DTOs.Aggregated;

public class CreatePatientAggregatedDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MiddleName { get; set; }
    public bool IsLinkedToAccount { get; set; }
    public DateTime BirthDate { get; set; }
}
