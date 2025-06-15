using BusinessLogicLayer.Abstact;
using Entity.DTOs.AppointmentDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _appointmentService.GetByIdAsync(id);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return NotFound(result);
        }

        [HttpGet("doctor/{doctorId}/date/{date}")]
        public async Task<IActionResult> GetByDoctorId(int doctorId, DateTime date)
        {
            var result = await _appointmentService.GetAppointmentsByDoctorIdAsync(doctorId, date);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetByPatientId(int patientId)
        {
            var result = await _appointmentService.GetAppointmentsByPatientIdAsync(patientId);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("available-slots/doctor/{doctorId}/date/{date}")]
        public async Task<IActionResult> GetAvailableSlots(int doctorId, DateTime date)
        {
            var result = await _appointmentService.GetAvailableAppointmentSlotsAsync(doctorId, date);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost]
        [Authorize] // All authenticated users can create appointments
        public async Task<IActionResult> Create(AppointmentCreateDto createDto)
        {
            var result = await _appointmentService.CreateAsync(createDto);
            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result);
            }
            return BadRequest(result);
        }

        [HttpPut("{id}/cancel")]
        [Authorize] // All authenticated users can cancel their appointments
        public async Task<IActionResult> Cancel(int id)
        {
            var result = await _appointmentService.CancelAsync(id);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
} 