using AggregatorMicroservice.Models.DTOs.Aggregated;
using AutoMapper;
using InnoClinic.SharedModels.DTOs.Appointments.Outgoing;
using InnoClinic.SharedModels.DTOs.Identity.Incoming;
using InnoClinic.SharedModels.DTOs.Offices.Incoming.Commands;
using InnoClinic.SharedModels.DTOs.Offices.Outgoing;
using InnoClinic.SharedModels.DTOs.Profiles.Incoming;
using InnoClinic.SharedModels.DTOs.Profiles.Outgoing;

namespace AggregatorMicroservice;

public class MappingProfile : Profile
{
	public MappingProfile()
	{
		CreateMap<CreateOfficeAggregatedDto, CreateOfficeModel>();
		CreateMap<OfficeAddressResponse, OfficeAddressAggregatedDto>();
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
		CreateMap<PatientOutgoingDto, PatientProfileAggregatedDto>();
	}
}
