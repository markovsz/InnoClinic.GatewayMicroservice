using AggregatorMicroservice.FilterAttributes;
using AggregatorMicroservice.Models.DTOs.Aggregated;
using AggregatorMicroservice.Models.Enums;
using AggregatorMicroservice.Services.Abstractions;
using InnoClinic.SharedModels.DTOs.Appointments.RequestParameters;
using InnoClinic.SharedModels.DTOs.Profiles.RequestParameters;
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
        public async Task<IActionResult> GetAppointmentsByReceptionistAsync([FromQuery] AppointmentParameters parameters, string? authParam)
        {
            var appointments = await _aggregatorsService.GetAppointmentsByReceptionistAsync(parameters, authParam);
            return Ok(appointments);
        }

        [HttpGet("Appointments/schedule")]
        public async Task<IActionResult> GetAppointmentsScheduleByDoctorAsync([FromQuery] ScheduleParameters parameters, string? authParam)
        {
            var appointments = await _aggregatorsService.GetAppointmentsScheduleByDoctorAsync(parameters, authParam);
            return Ok(appointments);
        }

        [HttpGet("Results/{id}")]
        public async Task<IActionResult> GetResultByIdAsync(Guid id, string? authParam)
        {
            var result = await _aggregatorsService.GetResultByIdAsync(id, authParam);
            return Ok(result);
        }

        [ServiceFilter(typeof(ExtractJwtTokenAttribute))]
        [ServiceFilter(typeof(ExtractRoleAttribute))]
        [HttpGet("Doctors/doctor/{id}")]
        public async Task<IActionResult> GetDoctorProfileByIdAsync(Guid id, string? roleName, string? authParam)
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
        
        [ServiceFilter(typeof(ExtractJwtTokenAttribute))]
        [HttpGet("Patients/patient/{id}")]
        public async Task<IActionResult> GetPatientProfileByIdAsync(Guid id, string? authParam)
        {
            var result = await _aggregatorsService.GetPatientProfileByIdAsync(id, authParam);
            return Ok(result);
        }

        [ServiceFilter(typeof(ExtractJwtTokenAttribute))]
        [HttpGet("Doctors/profile")]
        public async Task<IActionResult> GetDoctorProfileAsync(string? authParam)
        {
            var result = await _aggregatorsService.GetDoctorProfileAsync(authParam);
            return Ok(result);
        }

        [ServiceFilter(typeof(ExtractJwtTokenAttribute))]
        [HttpGet("Patients/profile")]
        public async Task<IActionResult> GetPatientProfileAsync(string? authParam)
        {
            var result = await _aggregatorsService.GetPatientProfileByAccountIdAsync(authParam);
            return Ok(result);
        }

        [ServiceFilter(typeof(ExtractJwtTokenAttribute))]
        [HttpGet("Doctors")]
        public async Task<IActionResult> GetDoctorProfilesAsync([FromQuery] DoctorParameters parameters, string? authParam)
        {
            var doctors = await _aggregatorsService.GetDoctorProfilesByPatientAsync(parameters, authParam);
            return Ok(doctors);
        }

        [HttpPost("signup")]
        public async Task<IActionResult> CreateAccountAsync(CreateAccountAggregatedDto aggregatedDto)
        {
            await _aggregatorsService.CreateAccountAsync(aggregatedDto);
            return Ok();
        }

        [HttpPost("Patient")]
        public async Task<IActionResult> CreatePatientAsync(CreatePatientAggregatedDto aggregatedDto, string? authParam)
        {
            var id = await _aggregatorsService.CreatePatientAsync(aggregatedDto, authParam);
            return Created("", id);
        }

        [HttpPost("Doctor")]
        public async Task<IActionResult> CreateDoctorAsync(CreateDoctorAggregatedDto aggregatedDto, string? authParam)
        {
            var id = await _aggregatorsService.CreateDoctorAsync(aggregatedDto, authParam);
            return Created("", id);
        }

        [HttpPost("Receptionist")]
        public async Task<IActionResult> CreateReceptionistAsync(CreateReceptionistAggregatedDto aggregatedDto, string? authParam)
        {
            var id = await _aggregatorsService.CreateReceptionistAsync(aggregatedDto, authParam);
            return Created("", id);
        }

        [HttpPost("Offices")]
        public async Task<IActionResult> CreateOfficeAsync(CreateOfficeAggregatedDto aggregatedDto, string? authParam)
        {
            var id = await _aggregatorsService.CreateOfficeAsync(aggregatedDto, authParam);
            return Created("", id);
        }

        [ServiceFilter(typeof(ExtractJwtTokenAttribute))]
        [HttpPost("Appointments")]
        public async Task<IActionResult> CreateAppointmentAsync(CreateAppointmentAggregatedDto aggregatedDto, string? authParam)
        {
            var id = await _aggregatorsService.CreateAppointmentAsync(aggregatedDto, authParam);
            return Created("", id);
        }

        [ServiceFilter(typeof(ExtractJwtTokenAttribute))]
        [HttpGet("Receptionists/profile")]
        public async Task<IActionResult> GetReceptionistProfileAsync(string? authParam)
        {
            var receptionist = await _aggregatorsService.GetReceptionistProfileAsync(authParam);
            return Ok(receptionist);
        }

        [ServiceFilter(typeof(ExtractJwtTokenAttribute))]
        [HttpGet("Receptionists/receptionist/{id}")]
        public async Task<IActionResult> GetReceptionistByIdAsync(Guid id, string? authParam)
        {
            var receptionist = await _aggregatorsService.GetReceptionistByIdAsync(id, authParam);
            return Ok(receptionist);
        }
        [HttpPut("Doctors/doctor/{doctorId}")]
        public async Task<IActionResult> UpdateDoctorAsync([FromBody] UpdateDoctorAggregatedDto updateDto, Guid doctorId, string? authParam)
        {
            await _aggregatorsService.UpdateDoctorAsync(doctorId, updateDto, authParam);
            return NoContent();
        }

        [ServiceFilter(typeof(ExtractJwtTokenAttribute))]
        [HttpPut("Receptionists/receptionist/{receptionistId}")]
        public async Task<IActionResult> UpdateDoctorAsync([FromBody] UpdateReceptionistAggregatedDto updateDto, Guid receptionistId, string? authParam)
        {
            await _aggregatorsService.UpdateReceptionistAsync(receptionistId, updateDto, authParam);
            return NoContent();
        }

        [ServiceFilter(typeof(ExtractJwtTokenAttribute))]
        [HttpPut("Patients/patient/{patientId}")]
        public async Task<IActionResult> UpdatePatientAsync([FromBody] UpdatePatientAggregatedDto updateDto, Guid patientId, string? authParam)
        {
            await _aggregatorsService.UpdatePatientAsync(patientId, updateDto, authParam);
            return NoContent();
        }
        }
    }
}
