﻿using AggregatorMicroservice.Models.DTOs.Aggregated;
using AggregatorMicroservice.Models.Parameters;
using InnoClinic.SharedModels.DTOs.Appointments.RequestParameters;
using InnoClinic.SharedModels.DTOs.Profiles.RequestParameters;

namespace AggregatorMicroservice.Services.Abstractions;

public interface IAggregatorsService
{
    Task CreateAccountAsync(CreateAccountAggregatedDto aggregatedDto);
    Task<Guid> CreatePatientAsync(CreatePatientAggregatedDto incomingDto, string authParam);
    Task<Guid> CreateDoctorAsync(CreateDoctorAggregatedDto incomingDto, string authParam);
    Task<Guid> CreateReceptionistAsync(CreateReceptionistAggregatedDto incomingDto, string authParam);
    Task<Guid> CreateOfficeAsync(CreateOfficeAggregatedDto incomingDto, string authParam);
    Task<Guid> CreateAppointmentAsync(CreateAppointmentAggregatedDto incomingDto, string authParam);
    Task<IEnumerable<AppointmentByReceptionistAggregatedDto>> GetAppointmentsByReceptionistAsync(AppointmentParameters parameters, string authParam);
    Task<IEnumerable<AppointmentScheduleByDoctorAggregatedDto>> GetAppointmentsScheduleByDoctorAsync(ScheduleParameters parameters, string authParam);
    Task<ResultAggregatedDto> GetResultByIdAsync(Guid resultId, string authParam);
    Task<DoctorProfileByPatientAggregatedDto> GetDoctorProfileByPatientAsync(Guid doctorId, string authParam);
    Task<DoctorProfileByDoctorAggregatedDto> GetDoctorProfileByDoctorAsync(Guid doctorId, string authParam);
    Task<DoctorProfileByDoctorAggregatedDto> GetDoctorProfileAsync(string authParam);
    Task<DoctorProfileByReceptionistAggregatedDto> GetDoctorProfileByReceptionistAsync(Guid doctorId, string authParam);
    Task<DoctorPaginationAggregatedDto> GetDoctorProfilesByPatientAsync(DoctorParameters parameters, string authParam);
    Task<PatientProfileAggregatedDto> GetPatientProfileByIdAsync(Guid doctorId, string authParam);
    Task<PatientProfileAggregatedDto> GetPatientProfileByAccountIdAsync(string authParam);
    Task<ReceptionistProfileAggregatedDto> GetReceptionistByIdAsync(Guid id, string authParam);
    Task<ReceptionistProfileAggregatedDto> GetReceptionistProfileAsync(string authParam);
    Task<IEnumerable<ReceptionistsListItemAggregatedDto>> GetReceptionistsAsync(string authParam);
    Task<IEnumerable<DateTime>> GetTimeSlotsAsync(TimeSlotAggregatedParameters parameters, string authParam);
    Task RescheduleAppointmentAsync(RescheduleAppointmentAggregatedDto incomingDto, Guid appointmentId, string authParam);
    Task UpdateDoctorAsync(Guid doctorId, UpdateDoctorAggregatedDto aggregatedDto, string authParam);
    Task UpdateReceptionistAsync(Guid receptionistId, UpdateReceptionistAggregatedDto aggregatedDto, string authParam);
    Task UpdatePatientAsync(Guid patientId, UpdatePatientAggregatedDto aggregatedDto, string authParam);
}
