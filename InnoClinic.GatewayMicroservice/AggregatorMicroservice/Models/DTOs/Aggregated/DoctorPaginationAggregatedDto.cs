namespace AggregatorMicroservice.Models.DTOs.Aggregated;

public class DoctorPaginationAggregatedDto
{
    public IEnumerable<DoctorMinProfileAggregatedDto> Entities { get; set; }
    public int PagesCount { get; set; }
}
