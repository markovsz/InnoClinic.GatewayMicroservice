namespace AggregatorMicroservice.Models.DTOs.Aggregated;

public class AppointmentForResultAggregatedDto
{
    public string PatientFirstName { get; set; }
    public string PatientLastName { get; set; }
    public string PatientMiddleName { get; set; }
    public string DoctorFirstName { get; set; }
    public string DoctorLastName { get; set; }
    public string DoctorMiddleName { get; set; }
    public string DoctorSpecializationName { get; set; }
    public string ServiceName { get; set; }
}
