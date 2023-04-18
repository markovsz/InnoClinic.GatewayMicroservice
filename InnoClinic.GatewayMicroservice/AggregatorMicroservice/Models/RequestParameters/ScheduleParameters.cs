namespace AggregatorMicroservice.Models.RequestParameters;

public class ScheduleParameters
{
    public Guid PatientId { get; set; }
    public int? Day { get; set; }
    public int? Month { get; set; }
    public int? Year { get; set; }
}
