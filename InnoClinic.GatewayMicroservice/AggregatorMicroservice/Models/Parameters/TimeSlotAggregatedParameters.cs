namespace AggregatorMicroservice.Models.Parameters;

public class TimeSlotAggregatedParameters
{
    public int Day { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public Guid DoctorId { get; set; }
    public Guid ServiceId { get; set; }
}
