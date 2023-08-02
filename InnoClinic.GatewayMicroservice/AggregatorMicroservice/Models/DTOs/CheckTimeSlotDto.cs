namespace AggregatorMicroservice.Models.DTOs;

public class CheckTimeSlotDto
{
    public DateTime DateTime { get; set; }
    public Guid DoctorId { get; set; }
    public Guid ServiceId { get; set; }
}
