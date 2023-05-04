using AggregatorMicroservice.Models.DTOs.Aggregated;
using AggregatorMicroservice.Models.DTOs.Incoming;
using AggregatorMicroservice.Models.DTOs.Outgoing;
using AggregatorMicroservice.Models.RequestParameters;
using AggregatorMicroservice.Services.Abstractions;
using AutoMapper;
using Newtonsoft.Json;
using System.Web;

namespace AggregatorMicroservice.Services;

public class AggregatorsService : IAggregatorsService
{
    private readonly IHttpCrudClient _crudClient;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;

    public AggregatorsService(IHttpCrudClient crudClient, IMapper mapper, IConfiguration configuration)
    {
        _crudClient = crudClient;
        _mapper = mapper;
        _configuration = configuration;
    }

    public async Task<IEnumerable<AppointmentByReceptionistAggregatedDto>> GetAppointmentsByReceptionistAsync(AppointmentParameters parameters, string authParam)
    {
        var appointmentsUrl = _configuration.GetSection("ApiUrls").GetSection("AppointmentsUrl").Value;
        var profilesUrl = _configuration.GetSection("ApiUrls").GetSection("ProfilesUrl").Value;

        var fullAppointmentsUrl = appointmentsUrl + "/api/Appointments/list" + "?" + ToRequestParams(parameters);
        var fullProfilesUrl = profilesUrl + $"/api/Patients/ids";

        var appointmentsContent = await _crudClient.GetAsync<IEnumerable<AppointmentByReceptionistOutgoingDto>>(fullAppointmentsUrl, authParam);
        var patientIds = appointmentsContent
            .Select(e => e.PatientId)
            .Distinct()
            .ToList();
        var patientsContent = await _crudClient.PostAsync<IEnumerable<Guid>, IEnumerable<PatientOutgoingDto>>(fullProfilesUrl, patientIds, authParam);
        var aggregated = new List<AppointmentByReceptionistAggregatedDto>();
        foreach (var appointment in appointmentsContent)
        {
            var patient = patientsContent
                .Where(e => e.Id.Equals(appointment.PatientId))
                .FirstOrDefault();
            var agregatedAppointment = _mapper.Map<AppointmentByReceptionistAggregatedDto>(appointment);
            agregatedAppointment.PatientFirstName = patient.FirstName;
            agregatedAppointment.PatientLastName = patient.LastName;
            agregatedAppointment.PatientMiddleName = patient.MiddleName;
            agregatedAppointment.PatientPhoneNumber = patient.PhoneNumber;
            aggregated.Add(agregatedAppointment);
        }
        return aggregated;
    }

    public async Task<IEnumerable<AppointmentScheduleByDoctorAggregatedDto>> GetAppointmentsScheduleByDoctorAsync(ScheduleParameters parameters, string authParam)
    {
        var appointmentsUrl = _configuration.GetSection("ApiUrls").GetSection("AppointmentsUrl").Value;
        var profilesUrl = _configuration.GetSection("ApiUrls").GetSection("ProfilesUrl").Value;

        var fullAppointmentsUrl = appointmentsUrl + "/api/Appointments/schedule?" + ToRequestParams(parameters);
        var fullProfilesUrl = profilesUrl + $"/api/Patients/ids";

        var appointmentsContent = await _crudClient.GetAsync<IEnumerable<AppointmentScheduleByDoctorOutgoingDto>>(fullAppointmentsUrl, authParam);
        var patientIds = appointmentsContent
            .Select(e => e.PatientId)
            .Distinct()
            .ToList();
        var patientsContent = await _crudClient.PostAsync<IEnumerable<Guid>, IEnumerable<PatientOutgoingDto>>(fullProfilesUrl, patientIds, authParam);
        var aggregated = new List<AppointmentScheduleByDoctorAggregatedDto>();
        foreach (var appointment in appointmentsContent)
        {
            var patient = patientsContent
                .Where(e => e.Id.Equals(appointment.PatientId))
                .FirstOrDefault();
            var agregatedAppointment = _mapper.Map<AppointmentScheduleByDoctorAggregatedDto>(appointment);
            agregatedAppointment.PatientFirstName = patient.FirstName;
            agregatedAppointment.PatientLastName = patient.LastName;
            agregatedAppointment.PatientMiddleName = patient.MiddleName;
            aggregated.Add(agregatedAppointment);
        }
        return aggregated;
    }

    public async Task<ResultAggregatedDto> GetResultByIdAsync(Guid resultId, string authParam)
    {
        var appointmentsUrl = _configuration.GetSection("ApiUrls").GetSection("AppointmentsUrl").Value;
        var profilesUrl = _configuration.GetSection("ApiUrls").GetSection("ProfilesUrl").Value;
        var servicesUrl = _configuration.GetSection("ApiUrls").GetSection("ServicesUrl").Value;

        var fullAppointmentUrl = appointmentsUrl + $"/api/Results/{resultId}";
        var resultContent = await _crudClient.GetAsync<ResultOutgoingDto>(fullAppointmentUrl, authParam);

        var fullSpecializationsUrl = servicesUrl + $"/api/Specializations/specialization/{resultContent.Appointment.SpecializationId}/min";
        var fullProfilesUrl = profilesUrl + $"/api/Patients/patient/{resultContent.Appointment.PatientId}";
        var specializationContent = await _crudClient.GetAsync<SpecializationMinOutgoingDto>(fullSpecializationsUrl, authParam);
        var patientContent = await _crudClient.GetAsync<PatientOutgoingDto>(fullProfilesUrl, authParam);

        var aggregated = _mapper.Map<ResultAggregatedDto>(resultContent);
        aggregated.Appointment.PatientFirstName = patientContent.FirstName;
        aggregated.Appointment.PatientLastName = patientContent.LastName;
        aggregated.Appointment.PatientMiddleName = patientContent.MiddleName;
        aggregated.Appointment.DoctorSpecializationName = specializationContent.Name;

        return aggregated;
    }

    public async Task<DoctorProfileByPatientAggregatedDto> GetDoctorProfileByPatientAsync(Guid doctorId, string authParam)
    {
        var officesUrl = _configuration.GetSection("ApiUrls").GetSection("AppointmentsUrl").Value;
        var profilesUrl = _configuration.GetSection("ApiUrls").GetSection("ProfilesUrl").Value;
        var servicesUrl = _configuration.GetSection("ApiUrls").GetSection("ServicesUrl").Value;

        var fullDoctorUrl = profilesUrl + $"/api/Doctors/doctor/{doctorId}";
        var doctorContent = await _crudClient.GetAsync<DoctorOutgoingDto>(fullDoctorUrl, authParam);

        var fullSpecializationsUrl = servicesUrl + $"/api/Specializations/specialization/{doctorContent.SpecializationId}/min";
        var fullOfficesUrl = officesUrl + $"/api/Offices/office/{doctorContent.OfficeId}";
        var specializationContent = await _crudClient.GetAsync<SpecializationMinOutgoingDto>(fullSpecializationsUrl, authParam);
        var officeContent = await _crudClient.GetAsync<OfficeOutgoingDto>(fullOfficesUrl, authParam);
        var servicesContent = await _crudClient.GetAsync<IEnumerable<ServiceMinOutgoingDto>>(fullOfficesUrl, authParam);

        var aggregated = _mapper.Map<DoctorProfileByPatientAggregatedDto>(doctorContent);
        aggregated.Spectialization = specializationContent;
        aggregated.Office = _mapper.Map<OfficeAddressAggregatedDto>(officeContent);
        aggregated.Services = servicesContent;
        return aggregated;
    }

    public async Task<DoctorProfileByDoctorAggregatedDto> GetDoctorProfileByDoctorAsync(Guid doctorId, string authParam)
    {
        var officesUrl = _configuration.GetSection("ApiUrls").GetSection("AppointmentsUrl").Value;
        var profilesUrl = _configuration.GetSection("ApiUrls").GetSection("ProfilesUrl").Value;
        var servicesUrl = _configuration.GetSection("ApiUrls").GetSection("ServicesUrl").Value;

        var fullDoctorUrl = profilesUrl + $"/api/Doctors/doctor/{doctorId}";
        var doctorContent = await _crudClient.GetAsync<DoctorOutgoingDto>(fullDoctorUrl, authParam);

        var fullSpecializationsUrl = servicesUrl + $"/api/Specializations/specialization/{doctorContent.SpecializationId}/min";
        var fullOfficesUrl = officesUrl + $"/api/Offices/office/{doctorContent.OfficeId}";
        var specializationContent = await _crudClient.GetAsync<SpecializationMinOutgoingDto>(fullSpecializationsUrl, authParam);
        var officeContent = await _crudClient.GetAsync<OfficeOutgoingDto>(fullOfficesUrl, authParam);

        var aggregated = _mapper.Map<DoctorProfileByDoctorAggregatedDto>(doctorContent);
        aggregated.Spectialization = specializationContent;
        aggregated.Office = _mapper.Map<OfficeAddressAggregatedDto>(officeContent);

        return aggregated;
    }

    public async Task<DoctorProfileByReceptionistAggregatedDto> GetDoctorProfileByReceptionistAsync(Guid doctorId, string authParam)
    {
        var officesUrl = _configuration.GetSection("ApiUrls").GetSection("AppointmentsUrl").Value;
        var profilesUrl = _configuration.GetSection("ApiUrls").GetSection("ProfilesUrl").Value;
        var servicesUrl = _configuration.GetSection("ApiUrls").GetSection("ServicesUrl").Value;

        var fullDoctorUrl = profilesUrl + $"/api/Doctors/doctor/{doctorId}";
        var doctorContent = await _crudClient.GetAsync<DoctorOutgoingDto>(fullDoctorUrl, authParam);

        var fullSpecializationsUrl = servicesUrl + $"/api/Specializations/specialization/{doctorContent.SpecializationId}/min";
        var fullOfficesUrl = officesUrl + $"/api/Offices/office/{doctorContent.OfficeId}";
        var specializationContent = await _crudClient.GetAsync<SpecializationMinOutgoingDto>(fullSpecializationsUrl, authParam);
        var officeContent = await _crudClient.GetAsync<OfficeOutgoingDto>(fullOfficesUrl, authParam);

        var aggregated = _mapper.Map<DoctorProfileByReceptionistAggregatedDto>(doctorContent);
        aggregated.Spectialization = specializationContent;
        aggregated.Office = _mapper.Map<OfficeAddressAggregatedDto>(officeContent);

        return aggregated;
    }

    public async Task<IEnumerable<DoctorMinProfileAggregatedDto>> GetDoctorProfilesByPatientAsync(DoctorParameters parameters, string authParam)
    {
        var officesUrl = _configuration.GetSection("ApiUrls").GetSection("AppointmentsUrl").Value;
        var profilesUrl = _configuration.GetSection("ApiUrls").GetSection("ProfilesUrl").Value;
        var servicesUrl = _configuration.GetSection("ApiUrls").GetSection("ServicesUrl").Value;

        var fullDoctorUrl = profilesUrl + $"/api/Doctors?" + ToRequestParams(parameters);
        var doctorsContent = await _crudClient.GetAsync<IEnumerable<DoctorOutgoingDto>>(fullDoctorUrl, authParam);

        var specializationIds = doctorsContent
            .Select(e => e.SpecializationId)
            .Distinct()
            .ToList();
        var fullSpecializationsUrl = servicesUrl + $"/api/Specializations/ids";
        var specializationsContent = await _crudClient.PostAsync<IEnumerable<Guid>, IEnumerable<SpecializationMinOutgoingDto>>(fullSpecializationsUrl, specializationIds, authParam);

        var officesIds = doctorsContent
            .Select(e => e.OfficeId)
            .Distinct()
            .ToList();
        var fullOfficesUrl = officesUrl + $"/api/Offices/ids";
        var officesContent = await _crudClient.PostAsync<IEnumerable<Guid>, IEnumerable<OfficeOutgoingDto>>(fullOfficesUrl, officesIds, authParam);
        var mappedOfficesContent = _mapper.Map<IEnumerable<OfficeAddressAggregatedDto>>(officesContent);

        var aggregated = new List<DoctorMinProfileAggregatedDto>();
        foreach (var doctor in doctorsContent)
        {
            var mappedDoctor = _mapper.Map<DoctorMinProfileAggregatedDto>(doctor);
            mappedDoctor.Office = mappedOfficesContent
                .Where(e => e.Id.Equals(doctor.OfficeId))
                .FirstOrDefault();
            mappedDoctor.Spectialization = specializationsContent
                .Where(e => e.Id.Equals(doctor.SpecializationId))
                .FirstOrDefault();
        }
        return aggregated;
    }

    public async Task CreateAccountAsync(CreateAccountAggregatedDto incomingDto, string authParam)
    {
        var identityServerUrl = _configuration.GetSection("ApiUrls").GetSection("IdentityServerUrl").Value;
        var documentsUrl = _configuration.GetSection("ApiUrls").GetSection("DocumentsUrl").Value;

        var fullPhotosUrl = documentsUrl + $"/api/Documents/Photos";
        var photoDto = _mapper.Map<DocumentIncomingDto>(incomingDto);
        var photoUrl = await _crudClient.PostAsync<DocumentIncomingDto, string>(fullPhotosUrl, photoDto, authParam);

        var fullSignUpUrl = identityServerUrl + $"/api/Auth/patient/signup";
        var signUpDto = _mapper.Map<SignUpIncomingDto>(incomingDto);
        signUpDto.PhotoUrl = photoUrl;

        await _crudClient.PostAsync<SignUpIncomingDto>(fullSignUpUrl, signUpDto, authParam);
    }

    public async Task<Guid> CreatePatientAsync(CreatePatientAggregatedDto incomingDto, string authParam)
    {
        var profilesUrl = _configuration.GetSection("ApiUrls").GetSection("ProfilesUrl").Value;

        var fullPatientUrl = profilesUrl + $"/api/Patients";
        var patientDto = _mapper.Map<PatientIncomingDto>(incomingDto);
        var patientId = await _crudClient.PostAsync<PatientIncomingDto, Guid>(fullPatientUrl, patientDto, authParam);

        return patientId;
    }

    public async Task<Guid> CreateDoctorAsync(CreateDoctorAggregatedDto incomingDto, string authParam)
    {
        var profilesUrl = _configuration.GetSection("ApiUrls").GetSection("ProfilesUrl").Value;

        var fullDoctorUrl = profilesUrl + $"/api/Doctors";
        var doctorDto = _mapper.Map<DoctorIncomingDto>(incomingDto);
        var doctorId = await _crudClient.PostAsync<DoctorIncomingDto, Guid>(fullDoctorUrl, doctorDto, authParam);

        return doctorId;
    }

    public async Task<Guid> CreateReceptionistAsync(CreateReceptionistAggregatedDto incomingDto, string authParam)
    {
        var profilesUrl = _configuration.GetSection("ApiUrls").GetSection("ProfilesUrl").Value;

        var fullReceptionistUrl = profilesUrl + $"/api/Receptionists";
        var receptionistDto = _mapper.Map<ReceptionistIncomingDto>(incomingDto);
        var receptionistId = await _crudClient.PostAsync<ReceptionistIncomingDto, Guid>(fullReceptionistUrl, receptionistDto, authParam);

        return receptionistId;
    }


    private string ToRequestParams<TParam>(TParam param)
    {
        var paramsAsJson = JsonConvert.SerializeObject(param);
        var paramsDictionary = JsonConvert.DeserializeObject<IDictionary<string, string>>(paramsAsJson);
        var paramsList = paramsDictionary.Select(x => HttpUtility.UrlEncode(x.Key) + "=" + HttpUtility.UrlEncode(x.Value));
        return string.Join("&", paramsList);
    }
}
