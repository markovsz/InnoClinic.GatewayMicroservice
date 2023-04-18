using AggregatorMicroservice.Models.DTOs.Aggregated;
using AggregatorMicroservice.Models.DTOs.Outgoing;
using AutoMapper;

namespace AggregatorMicroservice;

public class MappingProfile : Profile
{
	public MappingProfile()
	{
		CreateMap<AppointmentByReceptionistOutgoingDto, AppointmentByReceptionistAggregatedDto>();
		CreateMap<AppointmentScheduleByDoctorOutgoingDto, AppointmentScheduleByDoctorAggregatedDto>();
		CreateMap<ResultOutgoingDto, ResultAggregatedDto>();
		CreateMap<AppointmentForResultOutgoingDto, AppointmentForResultAggregatedDto>();
	}
}
