﻿using AggregatorMicroservice.Models.DTOs.Aggregated;
using AggregatorMicroservice.Services.Abstractions;
using AutoMapper;
using InnoClinic.SharedModels.DTOs.Appointments.Outgoing;
using InnoClinic.SharedModels.DTOs.Documents.Incoming;
using InnoClinic.SharedModels.DTOs.Identity.Incoming;
using InnoClinic.SharedModels.DTOs.Profiles.Incoming;
using InnoClinic.SharedModels.DTOs.Profiles.Outgoing;
using InnoClinic.SharedModels.DTOs.Services.Outgoing;
using InnoClinic.SharedModels.DTOs.Offices.Outgoing;
using Newtonsoft.Json;
using System.Web;
using InnoClinic.SharedModels.DTOs.Appointments.RequestParameters;
using InnoClinic.SharedModels.DTOs.Profiles.RequestParameters;
using InnoClinic.SharedModels.DTOs.Documents.Outgoing;
using InnoClinic.SharedModels.DTOs.Identity.Outgoing;
using System.Threading.Tasks;
using System;
using InnoClinic.SharedModels.DTOs.Offices.Incoming.Commands;
using InnoClinic.SharedModels.DTOs.Offices.Incoming.Queries;

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
        var officeContent = await _crudClient.GetAsync<OfficeResponse>(fullOfficesUrl, authParam);
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
        var officeContent = await _crudClient.GetAsync<OfficeResponse>(fullOfficesUrl, authParam);

        var aggregated = _mapper.Map<DoctorProfileByDoctorAggregatedDto>(doctorContent);
        aggregated.Spectialization = specializationContent;
        aggregated.Office = _mapper.Map<OfficeAddressAggregatedDto>(officeContent);

        return aggregated;
    }

    public async Task<DoctorProfileByDoctorAggregatedDto> GetDoctorProfileAsync(string authParam)
    {
        var officesUrl = _configuration.GetSection("ApiUrls").GetSection("AppointmentsUrl").Value;
        var profilesUrl = _configuration.GetSection("ApiUrls").GetSection("ProfilesUrl").Value;
        var servicesUrl = _configuration.GetSection("ApiUrls").GetSection("ServicesUrl").Value;

        var fullDoctorUrl = profilesUrl + $"/api/Doctors/profile";
        var doctorContent = await _crudClient.GetAsync<DoctorOutgoingDto>(fullDoctorUrl, authParam);

        var fullSpecializationsUrl = servicesUrl + $"/api/Specializations/specialization/{doctorContent.SpecializationId}/min";
        var fullOfficesUrl = officesUrl + $"/api/Offices/office/{doctorContent.OfficeId}";
        var specializationContent = await _crudClient.GetAsync<SpecializationMinOutgoingDto>(fullSpecializationsUrl, authParam);
        var officeContent = await _crudClient.GetAsync<OfficeResponse>(fullOfficesUrl, authParam);

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
        var officeContent = await _crudClient.GetAsync<OfficeResponse>(fullOfficesUrl, authParam);

        var aggregated = _mapper.Map<DoctorProfileByReceptionistAggregatedDto>(doctorContent);
        aggregated.Spectialization = specializationContent;
        aggregated.Office = _mapper.Map<OfficeAddressAggregatedDto>(officeContent);

        return aggregated;
    }

    public async Task<IEnumerable<DoctorMinProfileAggregatedDto>> GetDoctorProfilesByPatientAsync(DoctorParameters parameters, string authParam)
    {
        var officesUrl = _configuration.GetSection("ApiUrls").GetSection("OfficesUrl").Value;
        var profilesUrl = _configuration.GetSection("ApiUrls").GetSection("ProfilesUrl").Value;
        var servicesUrl = _configuration.GetSection("ApiUrls").GetSection("ServicesUrl").Value;

        var fullDoctorUrl = profilesUrl + $"/api/Doctors?" + ToRequestParams(parameters);
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

        var aggregated = new List<DoctorMinProfileAggregatedDto>();
        foreach (var doctor in doctorsContent.Entities)
        {
            var mappedDoctor = _mapper.Map<DoctorMinProfileAggregatedDto>(doctor);
            mappedDoctor.Office = mappedOfficesContent
                .Where(e => e.Id.Equals(doctor.OfficeId))
                .FirstOrDefault();
            mappedDoctor.Spectialization = specializationsContent
                .Where(e => e.Id.Equals(doctor.SpecializationId))
                .FirstOrDefault();
            aggregated.Add(mappedDoctor);
        }
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

        var fullSignUpUrl = identityServerUrl + $"/api/Auth/admin/doctor/signup";
        var signUpDto = new SignUpWithoutPasswordIncomingDto()
        {
            Email = incomingDto.Email,
            PhotoUrl = photoCreatedDto.FilePath
        };
        var signUpResult = await _crudClient.PostAsync<SignUpWithoutPasswordIncomingDto, SignUpOutgoingDto>(fullSignUpUrl, signUpDto, authParam);

        var fullPatientUrl = profilesUrl + $"/api/Patients";
        var patientDto = _mapper.Map<PatientIncomingDto>(incomingDto);
        patientDto.AccountId = signUpResult.AccountId;
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


    private string ToRequestParams<TParam>(TParam param)
    {
        var paramsAsJson = JsonConvert.SerializeObject(param);
        var paramsDictionary = JsonConvert.DeserializeObject<IDictionary<string, string>>(paramsAsJson);
        var paramsList = paramsDictionary.Select(x => HttpUtility.UrlEncode(x.Key) + "=" + HttpUtility.UrlEncode(x.Value));
        return string.Join("&", paramsList);
    }
}
