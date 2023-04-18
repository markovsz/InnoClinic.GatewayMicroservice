namespace AggregatorMicroservice.Models.DTOs.Outgoing;

public class ResultOutgoingDto
{
    public Guid Id { get; set; }
    public DateTime DateTime { get; set; }
    public AppointmentForResultOutgoingDto Appointment { get; set; }
    public string Complaints { get; set; }
    public string Conclusion { get; set; }
    public string Recomendations { get; set; }
}
