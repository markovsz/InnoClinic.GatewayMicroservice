using AggregatorMicroservice.Models.DTOs.Aggregated;
using AggregatorMicroservice.Models.Parameters;
using AutoMapper;
using InnoClinic.SharedModels.DTOs.Appointments.Incoming;
using InnoClinic.SharedModels.DTOs.Appointments.Outgoing;
using InnoClinic.SharedModels.DTOs.Appointments.RequestParameters;
using InnoClinic.SharedModels.DTOs.Identity.Incoming;
using InnoClinic.SharedModels.DTOs.Offices.Incoming.Commands;
using InnoClinic.SharedModels.DTOs.Offices.Outgoing;
using InnoClinic.SharedModels.DTOs.Profiles.Incoming;
using InnoClinic.SharedModels.DTOs.Profiles.Outgoing;
using InnoClinic.SharedModels.DTOs.Services.Outgoing;

namespace AggregatorMicroservice;

public class MappingProfile : Profile
{
	public MappingProfile()
	{
		CreateMap<CreateOfficeAggregatedDto, CreateOfficeModel>();
		CreateMap<OfficeAddressResponse, OfficeAddressAggregatedDto>();
		CreateMap<OfficeResponse, OfficeAddressAggregatedDto>();
		CreateMap<CreateAppointmentAggregatedDto, AppointmentIncomingDto>();
		CreateMap<CreateAccountAggregatedDto, SignUpIncomingDto>();
		CreateMap<CreatePatientAggregatedDto, PatientIncomingDto>();
		CreateMap<CreateDoctorAggregatedDto, DoctorIncomingDto>();
		CreateMap<DoctorOutgoingDto, DoctorMinProfileAggregatedDto>();
		CreateMap<CreateReceptionistAggregatedDto, ReceptionistIncomingDto>();
		CreateMap<AppointmentByReceptionistOutgoingDto, AppointmentByReceptionistAggregatedDto>();
		CreateMap<AppointmentScheduleByDoctorOutgoingDto, AppointmentScheduleByDoctorAggregatedDto>();
		CreateMap<ResultOutgoingDto, ResultAggregatedDto>();
		CreateMap<AppointmentForResultOutgoingDto, AppointmentForResultAggregatedDto>();
		CreateMap<SpecializationOutgoingDto, SpecializationMinOutgoingDto>();
		CreateMap<DoctorOutgoingDto, DoctorProfileByPatientAggregatedDto>();
		CreateMap<DoctorOutgoingDto, DoctorProfileByDoctorAggregatedDto>();
		CreateMap<DoctorOutgoingDto, DoctorProfileByReceptionistAggregatedDto>();
		CreateMap<ReceptionistOutgoingDto, ReceptionistProfileAggregatedDto>();
		CreateMap<ReceptionistOutgoingDto, ReceptionistsListItemAggregatedDto>();
		CreateMap<UpdateReceptionistAggregatedDto, UpdateReceptionistIncomingDto>();
		CreateMap<UpdatePatientAggregatedDto, UpdatePatientIncomingDto>();
		CreateMap<UpdateDoctorAggregatedDto, UpdateDoctorIncomingDto>();
		CreateMap<TimeSlotAggregatedParameters, TimeSlotParameters>();
		CreateMap<PatientOutgoingDto, PatientProfileAggregatedDto>();
		CreateMap<RescheduleAppointmentAggregatedDto, RescheduleAppointmentIncomingDto>();
	}
}
