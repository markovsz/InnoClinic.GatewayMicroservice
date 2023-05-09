namespace AggregatorMicroservice.Models.DTOs.Aggregated;

public class AppointmentByReceptionistAggregatedDto
{
    public DateTime Date { get; set; }
    public TimeSpan Time { get; set; }
    public string DoctorFirstName { get; set; }
    public string DoctorLastName { get; set; }
    public string DoctorMiddleName { get; set; }
    public string PatientFirstName { get; set; }
    public string PatientLastName { get; set; }
    public string PatientMiddleName { get; set; }
    public string PatientPhoneNumber { get; set; }
    public string ServiceName { get; set; }
}
