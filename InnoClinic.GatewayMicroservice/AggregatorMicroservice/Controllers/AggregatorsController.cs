using AggregatorMicroservice.Models.DTOs.Aggregated;
using AggregatorMicroservice.Models.Enums;
using AggregatorMicroservice.Services.Abstractions;
using InnoClinic.SharedModels.DTOs.Appointments.RequestParameters;
using Microsoft.AspNetCore.Mvc;

namespace AggregatorMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AggregatorsController : ControllerBase
    {
        private readonly IAggregatorsService _aggregatorsService;
        public AggregatorsController(IAggregatorsService aggregatorsService) 
        {
            _aggregatorsService = aggregatorsService;
        }

        [HttpGet("Appointments/list")]
        public async Task<IActionResult> GetAppointmentsByReceptionistAsync([FromQuery] AppointmentParameters parameters, string authParam)
        {
            var appointments = await _aggregatorsService.GetAppointmentsByReceptionistAsync(parameters, authParam);
            return Ok(appointments);
        }

        [HttpGet("Appointments/schedule")]
        public async Task<IActionResult> GetAppointmentsScheduleByDoctorAsync([FromQuery] ScheduleParameters parameters, string authParam)
        {
            var appointments = await _aggregatorsService.GetAppointmentsScheduleByDoctorAsync(parameters, authParam);
            return Ok(appointments);
        }

        [HttpGet("Results/{id}")]
        public async Task<IActionResult> GetResultByIdAsync(Guid id, string authParam)
        {
            var result = await _aggregatorsService.GetResultByIdAsync(id, authParam);
            return Ok(result);
        }

        [HttpGet("Doctors/doctor/{id}")]
        public async Task<IActionResult> GetDoctorProfileAsync(Guid id, string roleName, string authParam)
        {
            if (roleName.Equals(nameof(UserRole.Patient)))
            {
                var result = await _aggregatorsService.GetDoctorProfileByPatientAsync(id, authParam);
                return Ok(result);
            }
            else if (roleName.Equals(nameof(UserRole.Doctor)))
            {
                var result = await _aggregatorsService.GetDoctorProfileByDoctorAsync(id, authParam);
                return Ok(result);
            }
            else if (roleName.Equals(nameof(UserRole.Receptionist)))
            {
                var result = await _aggregatorsService.GetDoctorProfileByReceptionistAsync(id, authParam);
                return Ok(result);
            }
            return Forbid();
        }

        [HttpPost("signup")]
        public async Task<IActionResult> CreateAccountAsync(CreateAccountAggregatedDto aggregatedDto, string authParam)
        {
            await _aggregatorsService.CreateAccountAsync(aggregatedDto, authParam);
            return Ok();
        }

        [HttpPost("Patient")]
        public async Task<IActionResult> CreatePatientAsync(CreatePatientAggregatedDto aggregatedDto, string authParam)
        {
            var id = await _aggregatorsService.CreatePatientAsync(aggregatedDto, authParam);
            return Created("", id);
        }

        [HttpPost("Doctor")]
        public async Task<IActionResult> CreateDoctorAsync(CreateDoctorAggregatedDto aggregatedDto, string authParam)
        {
            var id = await _aggregatorsService.CreateDoctorAsync(aggregatedDto, authParam);
            return Created("", id);
        }

        [HttpPost("Receptionist")]
        public async Task<IActionResult> CreateReceptionistAsync(CreateReceptionistAggregatedDto aggregatedDto, string authParam)
        {
            var id = await _aggregatorsService.CreateReceptionistAsync(aggregatedDto, authParam);
            return Created("", id);
        }
    }
}
