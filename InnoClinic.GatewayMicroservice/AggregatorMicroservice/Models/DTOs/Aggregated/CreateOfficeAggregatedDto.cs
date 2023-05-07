using InnoClinic.SharedModels.DTOs.Documents.Incoming;
using System;

namespace AggregatorMicroservice.Models.DTOs.Aggregated;

public class CreateOfficeAggregatedDto
{
    public DocumentIncomingDto Photo { get; set; }
    public string City { get; set; }
    public string Street { get; set; }
    public string HouseNumber { get; set; }
    public string OfficeNumber { get; set; }
    public string RegistryPhoneNumber { get; set; }
    public string Status { get; set; }

}
