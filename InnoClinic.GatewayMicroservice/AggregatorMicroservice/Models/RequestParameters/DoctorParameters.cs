namespace AggregatorMicroservice.Models.RequestParameters;

public class DoctorParameters
{
    public int Page { get; set; }
    public int Size { get; set; }
    public string? FirstNameSearch { get; set; }
    public string? LastNameSearch { get; set; }
    public string? MiddleNameSearch { get; set; }
    public Guid? SpecializationId { get; set; }
    public Guid? OfficeId { get; set; }
}
