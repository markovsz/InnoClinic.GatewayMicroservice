namespace AggregatorMicroservice.Models.DTOs.Aggregated;

public class AppointmentScheduleByDoctorAggregatedDto
{
    public Guid Id { get; set; }
    public DateTime DateTime { get; set; }
    public Guid PatientId { get; set; }
    public string PatientFirstName { get; set; }
    public string PatientLastName { get; set; }
    public string PatientMiddleName { get; set; }
    public DateTime PatientBirthDate { get; set; }
    public string DoctorFirstName { get; set; }
    public string DoctorLastName { get; set; }
    public string DoctorMiddleName { get; set; }
    public string ServiceName { get; set; }
    public string SpecializationName { get; set; }
    public bool IsApproved { get; set; }
    public Guid? ResultId { get; set; }
}
