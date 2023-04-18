namespace AggregatorMicroservice.Models.DTOs.Aggregated;

public class AppointmentScheduleByDoctorAggregatedDto
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan Time { get; set; }
    public Guid PatientId { get; set; }
    public string PatientFirstName { get; set; }
    public string PatientLastName { get; set; }
    public string PatientMiddleName { get; set; }
    public string ServiceName { get; set; }
    public bool IsApproved { get; set; }
    public Guid? ResultId { get; set; }
}
