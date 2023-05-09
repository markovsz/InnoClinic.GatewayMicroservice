namespace AggregatorMicroservice.Models.DTOs.Aggregated;

public class ResultAggregatedDto
{
    public Guid Id { get; set; }
    public DateTime DateTime { get; set; }
    public AppointmentForResultAggregatedDto Appointment {get; set; }
    public string Complaints { get; set; }
    public string Conclusion { get; set; }
    public string Recomendations { get; set; }
}
