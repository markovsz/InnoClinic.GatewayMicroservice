namespace AggregatorMicroservice.Models.RequestParameters;

public class AppointmentParameters
{
    public string? DoctorFirstName { get; set; }
    public string? DoctorLastName { get; set; }
    public string? DoctorMiddleName { get; set; }
    public string? ServiceName { get; set; }
    public bool? IsApproved { get; set; }
    public Guid? OfficeId { get; set; }
    public DateTime? Date { get; set; }
}
