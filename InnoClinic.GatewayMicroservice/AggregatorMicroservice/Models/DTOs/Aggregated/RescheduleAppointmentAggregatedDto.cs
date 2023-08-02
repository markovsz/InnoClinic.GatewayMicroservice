namespace AggregatorMicroservice.Models.DTOs.Aggregated;

public class RescheduleAppointmentAggregatedDto
{
    public Guid Id { get; set; }
    public DateTime DateTime { get; set; }
    public Guid DoctorId { get; set; }
    public Guid ServiceId { get; set; }
}
