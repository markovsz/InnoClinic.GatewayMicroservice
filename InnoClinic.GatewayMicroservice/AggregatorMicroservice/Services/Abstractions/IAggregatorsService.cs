using AggregatorMicroservice.Models.DTOs.Aggregated;
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
    Task<IEnumerable<AppointmentByReceptionistAggregatedDto>> GetAppointmentsByReceptionistAsync(AppointmentParameters parameters, string authParam);
    Task<IEnumerable<AppointmentScheduleByDoctorAggregatedDto>> GetAppointmentsScheduleByDoctorAsync(ScheduleParameters parameters, string authParam);
    Task<ResultAggregatedDto> GetResultByIdAsync(Guid resultId, string authParam);
    Task<DoctorProfileByPatientAggregatedDto> GetDoctorProfileByPatientAsync(Guid doctorId, string authParam);
    Task<DoctorProfileByDoctorAggregatedDto> GetDoctorProfileByDoctorAsync(Guid doctorId, string authParam);
    Task<DoctorProfileByReceptionistAggregatedDto> GetDoctorProfileByReceptionistAsync(Guid doctorId, string authParam);
    Task<IEnumerable<DoctorMinProfileAggregatedDto>> GetDoctorProfilesByPatientAsync(DoctorParameters parameters, string authParam);
}
