namespace AggregatorMicroservice.Models.DTOs.Aggregated;

public class OfficeAddressAggregatedDto
{
    public Guid Id { get; set; }
    public string City { get; set; }
    public string Street { get; set; }
    public string HouseNumber { get; set; }
    public string OfficeNumber { get; set; }
}
