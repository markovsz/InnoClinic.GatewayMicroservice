namespace AggregatorMicroservice.Models.DTOs.Outgoing;

public class ServiceCategoryOutgoingDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int TimeSlotSize { get; set; }
}
