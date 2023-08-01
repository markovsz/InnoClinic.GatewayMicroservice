namespace AggregatorMicroservice.Models.DTOs.Aggregated;

public class ReceptionistsListItemAggregatedDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MiddleName { get; set; }
    public OfficeAddressAggregatedDto Office { get; set; }
}
