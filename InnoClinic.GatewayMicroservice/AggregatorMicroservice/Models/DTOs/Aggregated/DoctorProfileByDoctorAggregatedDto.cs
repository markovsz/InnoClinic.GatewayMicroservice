﻿using InnoClinic.SharedModels.DTOs.Services.Outgoing;

namespace AggregatorMicroservice.Models.DTOs.Aggregated;

public class DoctorProfileByDoctorAggregatedDto
{
    public Guid Id { get; set; }
    public string AccountId { get; set; }
    public string PhotoUrl { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MiddleName { get; set; }
    public DateTime BirthDate { get; set; }
    public SpecializationMinOutgoingDto Specialization { get; set; }
    public OfficeAddressAggregatedDto Office { get; set; }
    public int CareerStartYear { get; set; }
    public string Status { get; set; }
}
