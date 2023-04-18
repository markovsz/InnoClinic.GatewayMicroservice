namespace AggregatorMicroservice.Models.DTOs.Outgoing;

public class AppointmentForResultOutgoingDto
{
    public Guid PatientId { get; set; }
    public string DoctorFirstName { get; set; }
    public string DoctorLastName { get; set; }
    public string DoctorMiddleName { get; set; }
    public Guid SpecializationId { get; set; }
    public string ServiceName { get; set; }
}
