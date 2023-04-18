namespace AggregatorMicroservice.Models.DTOs.Aggregated;

public class CreateReceptionistAggregatedDto
{
    public Guid PhotoId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MiddleName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string ReEnteredPassword { get; set; }
    public Guid OfficeId { get; set; }
}
