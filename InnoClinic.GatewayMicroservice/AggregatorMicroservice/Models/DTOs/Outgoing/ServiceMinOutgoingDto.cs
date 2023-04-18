namespace AggregatorMicroservice.Models.DTOs.Outgoing;

public class ServiceMinOutgoingDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Decimal Price { get; set; }
    public ServiceCategoryOutgoingDto Category { get; set; }
    public string Status { get; set; }
}
