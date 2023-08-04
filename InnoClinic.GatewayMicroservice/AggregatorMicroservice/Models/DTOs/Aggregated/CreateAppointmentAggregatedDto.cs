namespace AggregatorMicroservice.Models.DTOs.Aggregated;

public class CreateAppointmentAggregatedDto
{
    public Guid PatientId { get; set; }
    public Guid SpecializationId { get; set; }
    public Guid DoctorId { get; set; }
    public Guid ServiceId { get; set; }
    public Guid OfficeId { get; set; }
    public DateTime DateTime { get; set; }
}
