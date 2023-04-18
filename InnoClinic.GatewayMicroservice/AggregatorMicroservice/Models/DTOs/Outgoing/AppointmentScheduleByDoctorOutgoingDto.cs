namespace AggregatorMicroservice.Models.DTOs.Outgoing;

public class AppointmentScheduleByDoctorOutgoingDto
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan Time { get; set; }
    public Guid PatientId { get; set; }
    public string ServiceName { get; set; }
    public bool IsApproved { get; set; }
    public Guid? ResultId { get; set; }
}
