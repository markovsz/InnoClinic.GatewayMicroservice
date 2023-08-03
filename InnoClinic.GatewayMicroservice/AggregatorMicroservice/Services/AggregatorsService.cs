using AggregatorMicroservice.Models.DTOs;
using AggregatorMicroservice.Models.DTOs.Aggregated;
using AggregatorMicroservice.Models.Parameters;
using AggregatorMicroservice.Services.Abstractions;
using AutoMapper;
using InnoClinic.SharedModels.DTOs.Appointments.Incoming;
using InnoClinic.SharedModels.DTOs.Appointments.Outgoing;
using InnoClinic.SharedModels.DTOs.Appointments.RequestParameters;
using InnoClinic.SharedModels.DTOs.Documents.Incoming;
using InnoClinic.SharedModels.DTOs.Documents.Outgoing;
using InnoClinic.SharedModels.DTOs.Identity.Incoming;
using InnoClinic.SharedModels.DTOs.Identity.Outgoing;
using InnoClinic.SharedModels.DTOs.Offices.Incoming.Commands;
using InnoClinic.SharedModels.DTOs.Offices.Incoming.Queries;
using InnoClinic.SharedModels.DTOs.Offices.Outgoing;
using InnoClinic.SharedModels.DTOs.Profiles.Incoming;
using InnoClinic.SharedModels.DTOs.Profiles.Outgoing;
using InnoClinic.SharedModels.DTOs.Profiles.RequestParameters;
using InnoClinic.SharedModels.DTOs.Services.Outgoing;
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
        var specializatiosUrl = _configuration.GetSection("ApiUrls").GetSection("ServicesUrl").Value;

        var fullAppointmentsUrl = appointmentsUrl + "/api/Appointments/schedule?" + ToRequestParams(parameters);
        var fullProfilesUrl = profilesUrl + $"/api/Patients/ids";
        var fullSpecializationsUrl = specializatiosUrl + $"/api/Specializations/ids";

        var appointmentsContent = await _crudClient.GetAsync<IEnumerable<AppointmentScheduleByDoctorOutgoingDto>>(fullAppointmentsUrl, authParam);
        var patientIds = appointmentsContent
            .Select(e => e.PatientId)
            .Distinct()
            .ToList();
        var patientsContent = await _crudClient.PostAsync<IEnumerable<Guid>, IEnumerable<PatientOutgoingDto>>(fullProfilesUrl, patientIds, authParam);
        
        var specializationIds = appointmentsContent
            .Select(e => e.SpecializationId)
            .Distinct()
            .ToList();
        var specializationsContent = await _crudClient.PostAsync<IEnumerable<Guid>, IEnumerable<SpecializationMinOutgoingDto>>(fullSpecializationsUrl, specializationIds, authParam);

        var aggregated = new List<AppointmentScheduleByDoctorAggregatedDto>();
        foreach (var appointment in appointmentsContent)
        {
            var patient = patientsContent
                .Where(e => e.Id.Equals(appointment.PatientId))
                .FirstOrDefault();
            var specialization = specializationsContent
                .Where(e => e.Id.Equals(appointment.SpecializationId))
                .FirstOrDefault();

            var agregatedAppointment = _mapper.Map<AppointmentScheduleByDoctorAggregatedDto>(appointment);
            agregatedAppointment.PatientFirstName = patient.FirstName;
            agregatedAppointment.PatientLastName = patient.LastName;
            agregatedAppointment.PatientMiddleName = patient.MiddleName;
            agregatedAppointment.SpecializationName = specialization.Name;
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

    public async Task<PatientProfileAggregatedDto> GetPatientProfileByAccountIdAsync(string authParam)
    {
        var identityUrl = _configuration.GetSection("ApiUrls").GetSection("IdentityServerUrl").Value;
        var profilesUrl = _configuration.GetSection("ApiUrls").GetSection("ProfilesUrl").Value;

        var fullPatientUrl = profilesUrl + $"/api/Patients/profile";
        var patientContent = await _crudClient.GetAsync<PatientOutgoingDto>(fullPatientUrl, authParam);

        var fullAccountUrl = identityUrl + $"/api/Auth/account";
        var accountContent = await _crudClient.GetAsync<AccountOutgoingDto>(fullAccountUrl, authParam);

        var aggregated = _mapper.Map<PatientProfileAggregatedDto>(patientContent);
        aggregated.PhotoUrl = accountContent.PhotoUrl;
        return aggregated;
    }

    public async Task<PatientProfileAggregatedDto> GetPatientProfileByIdAsync(Guid patientId, string authParam)
    {
        var identityUrl = _configuration.GetSection("ApiUrls").GetSection("IdentityServerUrl").Value;
        var profilesUrl = _configuration.GetSection("ApiUrls").GetSection("ProfilesUrl").Value;

        var fullPatientUrl = profilesUrl + $"/api/Patients/patient/{patientId}";
        var patientContent = await _crudClient.GetAsync<PatientOutgoingDto>(fullPatientUrl, authParam);

        var fullAccountUrl = identityUrl + $"/api/Auth/account/{patientContent.AccountId}/info";
        var accountContent = await _crudClient.GetAsync<AccountOutgoingDto>(fullAccountUrl, authParam);

        var aggregated = _mapper.Map<PatientProfileAggregatedDto>(patientContent);
        aggregated.PhotoUrl = accountContent.PhotoUrl;
        return aggregated;
    }

    public async Task<DoctorProfileByPatientAggregatedDto> GetDoctorProfileByPatientAsync(Guid doctorId, string authParam)
    {
        var identityUrl = _configuration.GetSection("ApiUrls").GetSection("IdentityServerUrl").Value;
        var officesUrl = _configuration.GetSection("ApiUrls").GetSection("OfficesUrl").Value;
        var profilesUrl = _configuration.GetSection("ApiUrls").GetSection("ProfilesUrl").Value;
        var servicesUrl = _configuration.GetSection("ApiUrls").GetSection("ServicesUrl").Value;

        var fullDoctorUrl = profilesUrl + $"/api/Doctors/doctor/{doctorId}";
        var doctorContent = await _crudClient.GetAsync<DoctorOutgoingDto>(fullDoctorUrl, authParam);

        var fullAccountUrl = identityUrl + $"/api/Auth/account/{doctorContent.AccountId}/info";
        var accountContent = await _crudClient.GetAsync<AccountOutgoingDto>(fullAccountUrl, authParam);

        var fullMinSpecializationUrl = servicesUrl + $"/api/Specializations/specialization/{doctorContent.SpecializationId}/min";
        var fullSpecializationUrl = servicesUrl + $"/api/Specializations/specialization/{doctorContent.SpecializationId}";
        var fullOfficesUrl = officesUrl + $"/api/Offices/office/{doctorContent.OfficeId}";
        var officeContent = await _crudClient.GetAsync<OfficeResponse>(fullOfficesUrl, authParam);
        var fullSpecializationsContent = await _crudClient.GetAsync<SpecializationOutgoingDto>(fullSpecializationUrl, authParam);
        var specializationContent = _mapper.Map<SpecializationMinOutgoingDto>(fullSpecializationsContent);
        var servicesContent = fullSpecializationsContent.Services;

        var aggregated = _mapper.Map<DoctorProfileByPatientAggregatedDto>(doctorContent);
        aggregated.PhotoUrl = accountContent.PhotoUrl;
        aggregated.Specialization = specializationContent;
        aggregated.Office = _mapper.Map<OfficeAddressAggregatedDto>(officeContent);
        aggregated.Services = servicesContent;
        return aggregated;
    }

    public async Task<DoctorProfileByDoctorAggregatedDto> GetDoctorProfileByDoctorAsync(Guid doctorId, string authParam)
    {
        var identityUrl = _configuration.GetSection("ApiUrls").GetSection("IdentityServerUrl").Value;
        var officesUrl = _configuration.GetSection("ApiUrls").GetSection("OfficesUrl").Value;
        var profilesUrl = _configuration.GetSection("ApiUrls").GetSection("ProfilesUrl").Value;
        var servicesUrl = _configuration.GetSection("ApiUrls").GetSection("ServicesUrl").Value;

        var fullDoctorUrl = profilesUrl + $"/api/Doctors/doctor/{doctorId}";
        var doctorContent = await _crudClient.GetAsync<DoctorOutgoingDto>(fullDoctorUrl, authParam);

        var fullAccountUrl = identityUrl + $"/api/Auth/account/{doctorContent.AccountId}/info";
        var accountContent = await _crudClient.GetAsync<AccountOutgoingDto>(fullAccountUrl, authParam);

        var fullSpecializationsUrl = servicesUrl + $"/api/Specializations/specialization/{doctorContent.SpecializationId}/min";
        var fullOfficesUrl = officesUrl + $"/api/Offices/office/{doctorContent.OfficeId}";
        var specializationContent = await _crudClient.GetAsync<SpecializationMinOutgoingDto>(fullSpecializationsUrl, authParam);
        var officeContent = await _crudClient.GetAsync<OfficeResponse>(fullOfficesUrl, authParam);

        var aggregated = _mapper.Map<DoctorProfileByDoctorAggregatedDto>(doctorContent);
        aggregated.PhotoUrl = accountContent.PhotoUrl;
        aggregated.Specialization = specializationContent;
        aggregated.Office = _mapper.Map<OfficeAddressAggregatedDto>(officeContent);

        return aggregated;
    }

    public async Task<DoctorProfileByDoctorAggregatedDto> GetDoctorProfileAsync(string authParam)
    {
        var identityUrl = _configuration.GetSection("ApiUrls").GetSection("IdentityServerUrl").Value;
        var officesUrl = _configuration.GetSection("ApiUrls").GetSection("OfficesUrl").Value;
        var profilesUrl = _configuration.GetSection("ApiUrls").GetSection("ProfilesUrl").Value;
        var servicesUrl = _configuration.GetSection("ApiUrls").GetSection("ServicesUrl").Value;

        var fullAccountUrl = identityUrl + $"/api/Auth/account";
        var accountContent = await _crudClient.GetAsync<AccountOutgoingDto>(fullAccountUrl, authParam);

        var fullDoctorUrl = profilesUrl + $"/api/Doctors/profile";
        var doctorContent = await _crudClient.GetAsync<DoctorOutgoingDto>(fullDoctorUrl, authParam);

        var fullSpecializationsUrl = servicesUrl + $"/api/Specializations/specialization/{doctorContent.SpecializationId}/min";
        var fullOfficesUrl = officesUrl + $"/api/Offices/office/{doctorContent.OfficeId}";
        var specializationContent = await _crudClient.GetAsync<SpecializationMinOutgoingDto>(fullSpecializationsUrl, authParam);
        var officeContent = await _crudClient.GetAsync<OfficeResponse>(fullOfficesUrl, authParam);

        var aggregated = _mapper.Map<DoctorProfileByDoctorAggregatedDto>(doctorContent);
        aggregated.Specialization = specializationContent;
        aggregated.PhotoUrl = accountContent.PhotoUrl;
        aggregated.Office = _mapper.Map<OfficeAddressAggregatedDto>(officeContent);

        return aggregated;
    }

    public async Task<DoctorProfileByReceptionistAggregatedDto> GetDoctorProfileByReceptionistAsync(Guid doctorId, string authParam)
    {
        var identityUrl = _configuration.GetSection("ApiUrls").GetSection("IdentityServerUrl").Value;
        var officesUrl = _configuration.GetSection("ApiUrls").GetSection("OfficesUrl").Value;
        var profilesUrl = _configuration.GetSection("ApiUrls").GetSection("ProfilesUrl").Value;
        var servicesUrl = _configuration.GetSection("ApiUrls").GetSection("ServicesUrl").Value;

        var fullDoctorUrl = profilesUrl + $"/api/Doctors/doctor/{doctorId}";
        var doctorContent = await _crudClient.GetAsync<DoctorOutgoingDto>(fullDoctorUrl, authParam);

        var fullAccountUrl = identityUrl + $"/api/Auth/account/{doctorContent.AccountId}/info";
        var accountContent = await _crudClient.GetAsync<AccountOutgoingDto>(fullAccountUrl, authParam);

        var fullSpecializationsUrl = servicesUrl + $"/api/Specializations/specialization/{doctorContent.SpecializationId}/min";
        var fullOfficesUrl = officesUrl + $"/api/Offices/office/{doctorContent.OfficeId}";
        var specializationContent = await _crudClient.GetAsync<SpecializationMinOutgoingDto>(fullSpecializationsUrl, authParam);
        var officeContent = await _crudClient.GetAsync<OfficeResponse>(fullOfficesUrl, authParam);

        var aggregated = _mapper.Map<DoctorProfileByReceptionistAggregatedDto>(doctorContent);
        aggregated.PhotoUrl = accountContent.PhotoUrl;
        aggregated.Specialization = specializationContent;
        aggregated.Office = _mapper.Map<OfficeAddressAggregatedDto>(officeContent);

        return aggregated;
    }

    public async Task<DoctorPaginationAggregatedDto> GetDoctorProfilesByPatientAsync(DoctorParameters parameters, string authParam)
    {
        var photosUrl = _configuration.GetSection("ApiUrls").GetSection("IdentityServerUrl").Value;
        var officesUrl = _configuration.GetSection("ApiUrls").GetSection("OfficesUrl").Value;
        var profilesUrl = _configuration.GetSection("ApiUrls").GetSection("ProfilesUrl").Value;
        var servicesUrl = _configuration.GetSection("ApiUrls").GetSection("ServicesUrl").Value;

        var fullDoctorUrl = profilesUrl + $"/api/Doctors/list?" + ToRequestParams(parameters);
        var doctorsContent = await _crudClient.GetAsync<DoctorsPaginationOutgoingDto>(fullDoctorUrl, authParam);

        var specializationIds = doctorsContent.Entities
            .Select(e => e.SpecializationId)
            .Distinct()
            .ToList();
        var fullSpecializationsUrl = servicesUrl + $"/api/Specializations/ids";
        var specializationsContent = await _crudClient.PostAsync<IEnumerable<Guid>, IEnumerable<SpecializationMinOutgoingDto>>(fullSpecializationsUrl, specializationIds, authParam);

        var officesIds = doctorsContent.Entities
            .Select(e => e.OfficeId)
            .Distinct()
            .ToList();
        var officesDto = new GetOfficesByIdsModel(officesIds);
        var fullOfficesUrl = officesUrl + $"/api/Offices/ids";
        var officesContent = await _crudClient.PostAsync<GetOfficesByIdsModel, OfficesResponse>(fullOfficesUrl, officesDto, authParam);
        var mappedOfficesContent = _mapper.Map<IEnumerable<OfficeAddressAggregatedDto>>(officesContent.Offices);

        var aggregatedDoctors = new List<DoctorMinProfileAggregatedDto>();
        foreach (var doctor in doctorsContent.Entities)
        {
            var mappedDoctor = _mapper.Map<DoctorMinProfileAggregatedDto>(doctor);
            mappedDoctor.Office = mappedOfficesContent
                .Where(e => e.Id.Equals(doctor.OfficeId))
                .FirstOrDefault();
            mappedDoctor.Specialization = specializationsContent
                .Where(e => e.Id.Equals(doctor.SpecializationId))
                .FirstOrDefault();
            aggregatedDoctors.Add(mappedDoctor);
        }
        var aggregated = new DoctorPaginationAggregatedDto();
        aggregated.Entities = aggregatedDoctors;
        aggregated.PagesCount = doctorsContent.PagesCount;
        return aggregated;
    }

    public async Task CreateAccountAsync(CreateAccountAggregatedDto incomingDto)
    {
        var identityServerUrl = _configuration.GetSection("ApiUrls").GetSection("IdentityServerUrl").Value;
        var documentsUrl = _configuration.GetSection("ApiUrls").GetSection("DocumentsUrl").Value;

        var fullPhotosUrl = documentsUrl + $"/api/Documents/Photos";
        var photoDto = incomingDto.Photo;
        var photoCreatedDto = await _crudClient.PostAsync<DocumentIncomingDto, DocumentCreatedOutgoingDto>(fullPhotosUrl, photoDto);

        var fullSignUpUrl = identityServerUrl + $"/api/Auth/patient/signup";
        var signUpDto = _mapper.Map<SignUpIncomingDto>(incomingDto);
        signUpDto.PhotoUrl = photoCreatedDto.FilePath;

        await _crudClient.PostAsync<SignUpIncomingDto>(fullSignUpUrl, signUpDto);
    }

    public async Task<Guid> CreatePatientAsync(CreatePatientAggregatedDto incomingDto, string authParam)
    {
        var profilesUrl = _configuration.GetSection("ApiUrls").GetSection("ProfilesUrl").Value;
        var identityServerUrl = _configuration.GetSection("ApiUrls").GetSection("IdentityServerUrl").Value;
        var documentsUrl = _configuration.GetSection("ApiUrls").GetSection("DocumentsUrl").Value;

        var fullPhotosUrl = documentsUrl + $"/api/Documents/Photos";
        var photoDto = incomingDto.Photo;
        var photoCreatedDto = await _crudClient.PostAsync<DocumentIncomingDto, DocumentCreatedOutgoingDto>(fullPhotosUrl, photoDto, authParam);

        var fullSignUpUrl = identityServerUrl + $"/api/Auth/admin/patient/signup";
        var signUpDto = new SignUpWithoutPasswordIncomingDto()
        {
            Email = incomingDto.Email,
            PhotoUrl = photoCreatedDto.FilePath
        };
        var signUpResult = await _crudClient.PostAsync<SignUpWithoutPasswordIncomingDto, SignUpOutgoingDto>(fullSignUpUrl, signUpDto, authParam);

        var fullPatientUrl = profilesUrl + $"/api/Patients";
        var patientDto = _mapper.Map<PatientIncomingDto>(incomingDto);
        patientDto.AccountId = signUpResult.AccountId;
        patientDto.IsLinkedToAccount = true;
        var patientId = await _crudClient.PostAsync<PatientIncomingDto, Guid>(fullPatientUrl, patientDto, authParam);

        return patientId;
    }

    public async Task<Guid> CreateDoctorAsync(CreateDoctorAggregatedDto incomingDto, string authParam)
    {
        var profilesUrl = _configuration.GetSection("ApiUrls").GetSection("ProfilesUrl").Value;
        var identityServerUrl = _configuration.GetSection("ApiUrls").GetSection("IdentityServerUrl").Value;
        var documentsUrl = _configuration.GetSection("ApiUrls").GetSection("DocumentsUrl").Value;

        var fullPhotosUrl = documentsUrl + $"/api/Documents/Photos";
        var photoDto = incomingDto.Photo;
        var photoCreatedDto = await _crudClient.PostAsync<DocumentIncomingDto, DocumentCreatedOutgoingDto>(fullPhotosUrl, photoDto, authParam);

        var fullSignUpUrl = identityServerUrl + $"/api/Auth/admin/doctor/signup";
        var signUpDto = new SignUpWithoutPasswordIncomingDto()
        {
            Email = incomingDto.Email,
            PhotoUrl = photoCreatedDto.FilePath
        };
        var signUpResult = await _crudClient.PostAsync<SignUpWithoutPasswordIncomingDto, SignUpOutgoingDto>(fullSignUpUrl, signUpDto, authParam);

        var fullDoctorUrl = profilesUrl + $"/api/Doctors";
        var doctorDto = _mapper.Map<DoctorIncomingDto>(incomingDto);
        doctorDto.AccountId = signUpResult.AccountId;
        var doctorId = await _crudClient.PostAsync<DoctorIncomingDto, Guid>(fullDoctorUrl, doctorDto, authParam);

        return doctorId;
    }

    public async Task<Guid> CreateReceptionistAsync(CreateReceptionistAggregatedDto incomingDto, string authParam)
    {
        var profilesUrl = _configuration.GetSection("ApiUrls").GetSection("ProfilesUrl").Value;
        var identityServerUrl = _configuration.GetSection("ApiUrls").GetSection("IdentityServerUrl").Value;
        var documentsUrl = _configuration.GetSection("ApiUrls").GetSection("DocumentsUrl").Value;

        var fullPhotosUrl = documentsUrl + $"/api/Documents/Photos";
        var photoDto = incomingDto.Photo;
        var photoCreatedDto = await _crudClient.PostAsync<DocumentIncomingDto, DocumentCreatedOutgoingDto>(fullPhotosUrl, photoDto, authParam);

        var fullSignUpUrl = identityServerUrl + $"/api/Auth/admin/receptionist/signup";
        var signUpDto = new SignUpWithoutPasswordIncomingDto()
        {
            Email = incomingDto.Email,
            PhotoUrl = photoCreatedDto.FilePath
        };
        var signUpResult = await _crudClient.PostAsync<SignUpWithoutPasswordIncomingDto, SignUpOutgoingDto>(fullSignUpUrl, signUpDto, authParam);


        var fullReceptionistUrl = profilesUrl + $"/api/Receptionists";
        var receptionistDto = _mapper.Map<ReceptionistIncomingDto>(incomingDto);
        receptionistDto.AccountId = signUpResult.AccountId;
        var receptionistId = await _crudClient.PostAsync<ReceptionistIncomingDto, Guid>(fullReceptionistUrl, receptionistDto, authParam);

        return receptionistId;
    }

    public async Task<Guid> CreateOfficeAsync(CreateOfficeAggregatedDto incomingDto, string authParam)
    {
        var officesUrl = _configuration.GetSection("ApiUrls").GetSection("OfficesUrl").Value;
        var documentsUrl = _configuration.GetSection("ApiUrls").GetSection("DocumentsUrl").Value;

        var fullPhotosUrl = documentsUrl + $"/api/Documents/Photos";
        var photoDto = incomingDto.Photo;
        var photoCreatedDto = await _crudClient.PostAsync<DocumentIncomingDto, DocumentCreatedOutgoingDto>(fullPhotosUrl, photoDto, authParam);

        var fullOfficesUrl = officesUrl + $"/api/Offices";
        var office = _mapper.Map<CreateOfficeModel>(incomingDto);
        office.PhotoUrl = photoCreatedDto.FilePath;
        var officeId = await _crudClient.PostAsync<CreateOfficeModel, Guid>(fullOfficesUrl, office, authParam);

        return officeId;
    }

    public async Task<Guid> CreateAppointmentAsync(CreateAppointmentAggregatedDto incomingDto, string authParam)
    {
        var servicesUrl = _configuration.GetSection("ApiUrls").GetSection("ServicesUrl").Value;
        var doctorsUrl = _configuration.GetSection("ApiUrls").GetSection("ProfilesUrl").Value;
        var appointmentsUrl = _configuration.GetSection("ApiUrls").GetSection("AppointmentsUrl").Value;
        var officesUrl = _configuration.GetSection("ApiUrls").GetSection("OfficesUrl").Value;

        var appointment = _mapper.Map<AppointmentIncomingDto>(incomingDto);

        var fullServicesUrl = servicesUrl + $"/api/Services/service/{incomingDto.ServiceId}/min";
        var service = await _crudClient.GetAsync<ServiceMinOutgoingDto>(fullServicesUrl, authParam);
        if (service is null)
            throw new Exception("service doesnt exist");

        var fullDoctorsUrl = doctorsUrl + $"/api/Doctors/doctor/{incomingDto.DoctorId}";
        var doctor = await _crudClient.GetAsync<DoctorOutgoingDto>(fullDoctorsUrl, authParam);

        appointment.ServiceName = service.Name;
        appointment.DoctorFirstName = doctor.FirstName;
        appointment.DoctorLastName = doctor.LastName;
        appointment.DoctorMiddleName = doctor.MiddleName;



        var fullOfficesUrl = officesUrl + $"/api/Offices/office/{appointment.OfficeId}";
        var officeContent = await _crudClient.GetAsync<OfficeResponse>(fullOfficesUrl, authParam);
        if (officeContent is null)
            throw new Exception("office doesnt exist");

        var fullSpecializationsUrl = servicesUrl + $"/api/Specializations/specialization/{doctor.SpecializationId}/min";
        var specializationContent = await _crudClient.GetAsync<SpecializationMinOutgoingDto>(fullSpecializationsUrl, authParam);
        if (specializationContent is null)
            throw new Exception("specialization doesnt exist");



        var timeSlotDto = new CheckTimeSlotDto();
        timeSlotDto.DateTime = incomingDto.DateTime;
        timeSlotDto.DoctorId = incomingDto.DoctorId;
        timeSlotDto.ServiceId = incomingDto.ServiceId;
        bool isValidTimeSlot = await CheckTimeSlotAsync(timeSlotDto, authParam);

        if (!isValidTimeSlot)
            throw new Exception();
        var fullAppointmentsUrl = appointmentsUrl + $"/api/Appointments";
        var appointmentId = await _crudClient.PostAsync<AppointmentIncomingDto, Guid>(fullAppointmentsUrl, appointment, authParam);

        return appointmentId;
    }

    public async Task<ReceptionistProfileAggregatedDto> GetReceptionistProfileAsync(string authParam)
    {
        var identityUrl = _configuration.GetSection("ApiUrls").GetSection("IdentityServerUrl").Value;
        var profilesUrl = _configuration.GetSection("ApiUrls").GetSection("ProfilesUrl").Value;

        var fullReceptionistUrl = profilesUrl + $"/api/Receptionists/profile";
        var receptionistContent = await _crudClient.GetAsync<ReceptionistOutgoingDto>(fullReceptionistUrl, authParam);

        var fullAccountUrl = identityUrl + $"/api/Auth/account";
        var accountContent = await _crudClient.GetAsync<AccountOutgoingDto>(fullAccountUrl, authParam);

        var aggregated = _mapper.Map<ReceptionistProfileAggregatedDto>(receptionistContent);
        aggregated.PhotoUrl = accountContent.PhotoUrl;
        return aggregated;
    }

    public async Task<ReceptionistProfileAggregatedDto> GetReceptionistByIdAsync(Guid id, string authParam)
    {
        var identityUrl = _configuration.GetSection("ApiUrls").GetSection("IdentityServerUrl").Value;
        var profilesUrl = _configuration.GetSection("ApiUrls").GetSection("ProfilesUrl").Value;

        var fullReceptionistUrl = profilesUrl + $"/api/Receptionists/receptionist/{id}";
        var receptionistContent = await _crudClient.GetAsync<ReceptionistOutgoingDto>(fullReceptionistUrl, authParam);

        var fullAccountUrl = identityUrl + $"/api/Auth/account/{receptionistContent.AccountId}/info";
        var accountContent = await _crudClient.GetAsync<AccountOutgoingDto>(fullAccountUrl, authParam);

        var aggregated = _mapper.Map<ReceptionistProfileAggregatedDto>(receptionistContent);
        aggregated.PhotoUrl = accountContent.PhotoUrl;
        return aggregated;
    }

    public async Task<IEnumerable<ReceptionistsListItemAggregatedDto>> GetReceptionistsAsync(string authParam)
    {
        var profilesUrl = _configuration.GetSection("ApiUrls").GetSection("ProfilesUrl").Value;
        var officesUrl = _configuration.GetSection("ApiUrls").GetSection("OfficesUrl").Value;

        var fullReceptionistUrl = profilesUrl + $"/api/Receptionists/list";
        var receptionistsContent = await _crudClient.GetAsync<IEnumerable<ReceptionistOutgoingDto>>(fullReceptionistUrl, authParam);

        var officesIds = receptionistsContent
            .Select(e => e.OfficeId)
            .Distinct()
            .ToList();
        var officesDto = new GetOfficesByIdsModel(officesIds);
        var fullOfficesUrl = officesUrl + $"/api/Offices/ids";
        var officesContent = await _crudClient.PostAsync<GetOfficesByIdsModel, OfficesResponse>(fullOfficesUrl, officesDto, authParam);
        var mappedOfficesContent = _mapper.Map<IEnumerable<OfficeAddressAggregatedDto>>(officesContent.Offices);

        var aggregatedReceptionists = new List<ReceptionistsListItemAggregatedDto>();
        foreach (var receptionist in receptionistsContent)
        {
            var mappedDoctor = _mapper.Map<ReceptionistsListItemAggregatedDto>(receptionist);
            mappedDoctor.Office = mappedOfficesContent
                .Where(e => e.Id.Equals(receptionist.OfficeId))
                .FirstOrDefault();
            aggregatedReceptionists.Add(mappedDoctor);
        }
        return aggregatedReceptionists;
    }

    public async Task RescheduleAppointmentAsync(RescheduleAppointmentAggregatedDto incomingDto, Guid appointmentId, string authParam)
    {
        var appointmentsUrl = _configuration.GetSection("ApiUrls").GetSection("AppointmentsUrl").Value;
        var doctorsUrl = _configuration.GetSection("ApiUrls").GetSection("ProfilesUrl").Value;

        var timeSlotDto = new CheckTimeSlotDto();
        timeSlotDto.DateTime = incomingDto.DateTime;
        timeSlotDto.DoctorId = incomingDto.DoctorId;
        timeSlotDto.ServiceId = incomingDto.ServiceId;
        bool isValidTimeSlot = await CheckTimeSlotAsync(timeSlotDto, authParam);

        if (!isValidTimeSlot)
            throw new Exception("invalid time slot");

        var fullDoctorsUrl = doctorsUrl + $"/api/Doctors/doctor/{timeSlotDto.DoctorId}";
        var doctor = await _crudClient.GetAsync<DoctorOutgoingDto>(fullDoctorsUrl, authParam);

        var fullAppointmentUrl = appointmentsUrl + $"/api/Appointments/appointment/{appointmentId}/reschedule";
        var rescheduleAppointmentDto = _mapper.Map<RescheduleAppointmentIncomingDto>(incomingDto);
        rescheduleAppointmentDto.DoctorFirstName = doctor.FirstName;
        rescheduleAppointmentDto.DoctorLastName = doctor.LastName;
        rescheduleAppointmentDto.DoctorMiddleName = doctor.MiddleName;
        await _crudClient.PutAsync<RescheduleAppointmentIncomingDto>(fullAppointmentUrl, rescheduleAppointmentDto, authParam);
    }

    public async Task<IEnumerable<DateTime>> GetTimeSlotsAsync(TimeSlotAggregatedParameters parameters, string authParam)
    {
        var servicesUrl = _configuration.GetSection("ApiUrls").GetSection("ServicesUrl").Value;
        var appointmentsUrl = _configuration.GetSection("ApiUrls").GetSection("AppointmentsUrl").Value;

        var fullServiceUrl = servicesUrl + $"/api/Services/service/{parameters.ServiceId}/min";
        var serviceContent = await _crudClient.GetAsync<ServiceMinOutgoingDto>(fullServiceUrl, authParam);
        

        var mappedParams = _mapper.Map<TimeSlotParameters>(parameters);

        var fullAppointmentUrl = appointmentsUrl + $"/api/Appointments/orderedTimeSlots?" + ToRequestParams(parameters);
        var orderedTimeSlots = await _crudClient.GetAsync<List<TimeSlotAppointmentOutgoingDto>>(fullAppointmentUrl, authParam);

        var serviceIds = orderedTimeSlots
            .Select(e => e.ServiceId)
            .Distinct()
            .ToList();

        var fullServicesUrl = servicesUrl + $"/api/Services/ids";
        var servicesContent = await _crudClient.PostAsync<IEnumerable<Guid>, IEnumerable<ServiceMinOutgoingDto>>(fullServicesUrl, serviceIds, authParam);

        var timeSlotSizes = new List<int>();
        foreach (var orderedTimeSlot in orderedTimeSlots)
        {
            var timeSlotSize = servicesContent
                .Where(e => e.Id == orderedTimeSlot.ServiceId)
                .Select(e => e.Category.TimeSlotSize)
                .FirstOrDefault();

            timeSlotSizes.Add(timeSlotSize);
        }

        var minTimeSlotSize = 10;
        var categoryTimeSlotSize = serviceContent.Category.TimeSlotSize;

        var minWorkingTime = new DateTime(parameters.Year, parameters.Month, parameters.Day, 8, 0, 0);
        var maxWorkingTime = new DateTime(parameters.Year, parameters.Month, parameters.Day, 18, 0, 0);


        var timeSlots = new List<DateTime>();
        int orderedTimeSlotIndex = 0;
        var minWorkingTimeInMinutes = minWorkingTime.Hour * 60 + minWorkingTime.Minute;
        var maxWorkingTimeInMinutes = maxWorkingTime.Hour * 60 + maxWorkingTime.Minute;
        for (int timeSlot = minWorkingTimeInMinutes; timeSlot < maxWorkingTimeInMinutes; timeSlot += minTimeSlotSize)
        {
            if(orderedTimeSlotIndex < orderedTimeSlots.Count)
            {
                var orderedTimeSlotInMinutes = orderedTimeSlots[orderedTimeSlotIndex].DateTime.Hour * 60 + orderedTimeSlots[orderedTimeSlotIndex].DateTime.Minute;
                var endOfTimeSlot = timeSlot + categoryTimeSlotSize;
                if (endOfTimeSlot <= orderedTimeSlotInMinutes && endOfTimeSlot <= maxWorkingTimeInMinutes || (orderedTimeSlotIndex + 1) == orderedTimeSlots.Count)
                {
                    timeSlots.Add(new DateTime(parameters.Year, parameters.Month, parameters.Day, timeSlot / 60, timeSlot % 60, 0));
                }
                else
                {
                    timeSlot = orderedTimeSlotInMinutes + timeSlotSizes[orderedTimeSlotIndex];
                    timeSlot -= minTimeSlotSize;
                    if ((orderedTimeSlotIndex + 1) < orderedTimeSlots.Count)
                        ++orderedTimeSlotIndex;
                }
            }
            else 
            {
                timeSlots.Add(new DateTime(parameters.Year, parameters.Month, parameters.Day, timeSlot / 60, timeSlot % 60, 0));
            }
        }

        return timeSlots;
    }

    public async Task<bool> CheckTimeSlotAsync(CheckTimeSlotDto timeSlotDto, string authParam)
    {
        var servicesUrl = _configuration.GetSection("ApiUrls").GetSection("ServicesUrl").Value;
        var doctorsUrl = _configuration.GetSection("ApiUrls").GetSection("ProfilesUrl").Value;
        var appointmentsUrl = _configuration.GetSection("ApiUrls").GetSection("AppointmentsUrl").Value;
        var officesUrl = _configuration.GetSection("ApiUrls").GetSection("OfficesUrl").Value;

        var fullServicesUrl = servicesUrl + $"/api/Services/service/{timeSlotDto.ServiceId}/min";
        var service = await _crudClient.GetAsync<ServiceMinOutgoingDto>(fullServicesUrl, authParam);
        if (service is null)
            throw new Exception("service doesn't exist");

        var fullDoctorsUrl = doctorsUrl + $"/api/Doctors/doctor/{timeSlotDto.DoctorId}";
        var doctor = await _crudClient.GetAsync<DoctorOutgoingDto>(fullDoctorsUrl, authParam);

        var parameters = new TimeSlotParameters();
        parameters.Day = timeSlotDto.DateTime.Day;
        parameters.Month = timeSlotDto.DateTime.Month;
        parameters.Year = timeSlotDto.DateTime.Year;
        parameters.DoctorId = timeSlotDto.DoctorId;

        var fullAppointmentUrl = appointmentsUrl + $"/api/Appointments/orderedTimeSlots?" + ToRequestParams(parameters);
        var orderedTimeSlots = await _crudClient.GetAsync<List<TimeSlotAppointmentOutgoingDto>>(fullAppointmentUrl, authParam);

        var serviceIds = orderedTimeSlots
            .Select(e => e.ServiceId)
            .Distinct()
            .ToList();

        var fullServicesIdsUrl = servicesUrl + $"/api/Services/ids";
        var servicesContent = await _crudClient.PostAsync<IEnumerable<Guid>, IEnumerable<ServiceMinOutgoingDto>>(fullServicesIdsUrl, serviceIds, authParam);

        var timeSlotSizes = new List<int>();
        foreach (var orderedTimeSlot in orderedTimeSlots)
        {
            var timeSlotSize = servicesContent
                .Where(e => e.Id == orderedTimeSlot.ServiceId)
                .Select(e => e.Category.TimeSlotSize)
                .FirstOrDefault();

            timeSlotSizes.Add(timeSlotSize);
        }

        var minTimeSlotSize = 10;
        var categoryTimeSlotSize = service.Category.TimeSlotSize;

        var minWorkingTime = new DateTime(parameters.Year, parameters.Month, parameters.Day, 8, 0, 0);
        var maxWorkingTime = new DateTime(parameters.Year, parameters.Month, parameters.Day, 18, 0, 0);

        int orderedTimeSlotIndex = 0;
        var minWorkingTimeInMinutes = minWorkingTime.Hour * 60 + minWorkingTime.Minute;
        var maxWorkingTimeInMinutes = maxWorkingTime.Hour * 60 + maxWorkingTime.Minute;

        var timeSlotBegin = timeSlotDto.DateTime.Hour * 60 + timeSlotDto.DateTime.Minute;
        var timeSlotEnd = timeSlotBegin + categoryTimeSlotSize;
        bool isValidTimeSlot = true;
        foreach (var orderedTimeSlot in orderedTimeSlots)
        {
            var orderedTimeSlotBegin = orderedTimeSlot.DateTime.Hour * 60 + orderedTimeSlot.DateTime.Minute;
            var orderedTimeSlotEnd = orderedTimeSlotBegin + timeSlotSizes[orderedTimeSlotIndex];
            if (timeSlotEnd > orderedTimeSlotBegin && orderedTimeSlotEnd > timeSlotBegin) isValidTimeSlot = false;
            ++orderedTimeSlotIndex;
        }
        return isValidTimeSlot;
    }

    public async Task UpdateDoctorAsync(Guid doctorId, UpdateDoctorAggregatedDto aggregatedDto, string authParam)
    {
        var identityUrl = _configuration.GetSection("ApiUrls").GetSection("IdentityServerUrl").Value;
        var profilesUrl = _configuration.GetSection("ApiUrls").GetSection("ProfilesUrl").Value;
        var documentsUrl = _configuration.GetSection("ApiUrls").GetSection("DocumentsUrl").Value;

        var fullPhotosUrl = documentsUrl + $"/api/Documents/Photos";
        var photoDto = aggregatedDto.Photo;
        var photoCreatedDto = await _crudClient.PostAsync<DocumentIncomingDto, DocumentCreatedOutgoingDto>(fullPhotosUrl, photoDto, authParam);

        var fullAccountUrl = identityUrl + $"/api/Auth/photo";
        await _crudClient.PostAsync<string>(fullAccountUrl, photoCreatedDto.FilePath, authParam);

        var fullDoctorUrl = profilesUrl + $"/api/Doctors/doctor/{doctorId}";
        var updateDto = _mapper.Map<UpdateDoctorIncomingDto>(aggregatedDto);
        await _crudClient.PutAsync<UpdateDoctorIncomingDto>(fullDoctorUrl, updateDto, authParam);
    }

    public async Task UpdateReceptionistAsync(Guid receptionistId, UpdateReceptionistAggregatedDto aggregatedDto, string authParam)
    {
        var identityUrl = _configuration.GetSection("ApiUrls").GetSection("IdentityServerUrl").Value;
        var profilesUrl = _configuration.GetSection("ApiUrls").GetSection("ProfilesUrl").Value;
        var documentsUrl = _configuration.GetSection("ApiUrls").GetSection("DocumentsUrl").Value;

        var fullPhotosUrl = documentsUrl + $"/api/Documents/Photos";
        var photoDto = aggregatedDto.Photo;
        var photoCreatedDto = await _crudClient.PostAsync<DocumentIncomingDto, DocumentCreatedOutgoingDto>(fullPhotosUrl, photoDto, authParam);

        var fullReceptionistUrl = profilesUrl + $"/api/Receptionists/receptionist/{receptionistId}";
        var receptionistContent = await _crudClient.GetAsync<ReceptionistOutgoingDto>(fullReceptionistUrl, authParam);

        var fullAccountUrl = identityUrl + $"/api/Auth/admin/account/{receptionistContent.AccountId}/photo";
        await _crudClient.PostAsync<string>(fullAccountUrl, photoCreatedDto.FilePath, authParam);

        var fullUpdateReceptionistUrl = profilesUrl + $"/api/Receptionists/receptionist/{receptionistId}";
        var updateDto = _mapper.Map<UpdateReceptionistIncomingDto>(aggregatedDto);
        await _crudClient.PutAsync<UpdateReceptionistIncomingDto>(fullUpdateReceptionistUrl, updateDto, authParam);
    }

    public async Task UpdatePatientAsync(Guid patientId, UpdatePatientAggregatedDto aggregatedDto, string authParam)
    {
        var identityUrl = _configuration.GetSection("ApiUrls").GetSection("IdentityServerUrl").Value;
        var profilesUrl = _configuration.GetSection("ApiUrls").GetSection("ProfilesUrl").Value;
        var documentsUrl = _configuration.GetSection("ApiUrls").GetSection("DocumentsUrl").Value;

        var fullPhotosUrl = documentsUrl + $"/api/Documents/Photos";
        var photoDto = aggregatedDto.Photo;
        var photoCreatedDto = await _crudClient.PostAsync<DocumentIncomingDto, DocumentCreatedOutgoingDto>(fullPhotosUrl, photoDto, authParam);

        var fullAccountUrl = identityUrl + $"/api/Auth/admin/account/{aggregatedDto}/photo";
        await _crudClient.PostAsync<string>(fullAccountUrl, photoCreatedDto.FilePath, authParam);

        var fullPatientUrl = profilesUrl + $"/api/Patients/patient/{patientId}";
        var updateDto = _mapper.Map<UpdatePatientIncomingDto>(aggregatedDto);
        await _crudClient.PutAsync<UpdatePatientIncomingDto>(fullPatientUrl, updateDto, authParam);
    }


    public async Task UpdateOfficeAsync(Guid officeId, UpdateOfficeAggregatedDto incomingDto, string authParam)
    {
        var officesUrl = _configuration.GetSection("ApiUrls").GetSection("OfficesUrl").Value;
        var documentsUrl = _configuration.GetSection("ApiUrls").GetSection("DocumentsUrl").Value;

        var fullPhotosUrl = documentsUrl + $"/api/Documents/Photos";
        var photoDto = incomingDto.Photo;
        var photoCreatedDto = await _crudClient.PostAsync<DocumentIncomingDto, DocumentCreatedOutgoingDto>(fullPhotosUrl, photoDto, authParam);

        var fullOfficesUrl = officesUrl + $"/api/Offices/{officeId}";
        var office = _mapper.Map<UpdateOfficeModel>(incomingDto);
        office.PhotoUrl = photoCreatedDto.FilePath;
        await _crudClient.PutAsync<UpdateOfficeModel>(fullOfficesUrl, office, authParam);
    }


    private string ToRequestParams<TParam>(TParam param)
    {
        var paramsAsJson = JsonConvert.SerializeObject(param);
        var paramsDictionary = JsonConvert.DeserializeObject<IDictionary<string, string>>(paramsAsJson);
        var paramsList = paramsDictionary.Select(x => HttpUtility.UrlEncode(x.Key) + "=" + HttpUtility.UrlEncode(x.Value));
        return string.Join("&", paramsList);
    }
}
