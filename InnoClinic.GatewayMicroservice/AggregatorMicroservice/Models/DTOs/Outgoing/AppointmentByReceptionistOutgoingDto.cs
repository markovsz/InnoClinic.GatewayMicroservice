namespace AggregatorMicroservice.Models.DTOs.Outgoing;

public class AppointmentByReceptionistOutgoingDto
{
    public DateTime Date { get; set; }
    public TimeSpan Time { get; set; }
    public string DoctorFirstName { get; set; }
    public string DoctorLastName { get; set; }
    public string DoctorMiddleName { get; set; }
    public Guid PatientId { get; set; }
    public string ServiceName { get; set; }
}
