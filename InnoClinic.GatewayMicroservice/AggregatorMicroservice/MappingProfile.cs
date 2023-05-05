using AggregatorMicroservice.Models.DTOs.Aggregated;
using AutoMapper;
using InnoClinic.SharedModels.DTOs.Appointments.Outgoing;
using InnoClinic.SharedModels.DTOs.Identity.Incoming;
using InnoClinic.SharedModels.DTOs.Offices.Outgoing;
using InnoClinic.SharedModels.DTOs.Profiles.Incoming;

namespace AggregatorMicroservice;

public class MappingProfile : Profile
{
	public MappingProfile()
	{
		CreateMap<OfficeAddressAggregatedDto, OfficeResponse>();
		CreateMap<CreateAccountAggregatedDto, SignUpIncomingDto>();
		CreateMap<CreatePatientAggregatedDto, PatientIncomingDto>();
		CreateMap<CreateDoctorAggregatedDto, DoctorIncomingDto>();
		CreateMap<CreateReceptionistAggregatedDto, ReceptionistIncomingDto>();
		CreateMap<AppointmentByReceptionistOutgoingDto, AppointmentByReceptionistAggregatedDto>();
		CreateMap<AppointmentScheduleByDoctorOutgoingDto, AppointmentScheduleByDoctorAggregatedDto>();
		CreateMap<ResultOutgoingDto, ResultAggregatedDto>();
		CreateMap<AppointmentForResultOutgoingDto, AppointmentForResultAggregatedDto>();
	}
}
