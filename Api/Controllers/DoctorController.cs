using BusinessLogicLayer.Abstact;
using Entity.DTOs.DoctorDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorService _doctorService;

        public DoctorController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _doctorService.GetAllAsync();
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _doctorService.GetByIdAsync(id);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return NotFound(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(DoctorCreateDto createDto)
        {
            var result = await _doctorService.CreateAsync(createDto);
            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result);
            }
            return BadRequest(result);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(DoctorUpdateDto updateDto)
        {
            var result = await _doctorService.UpdateAsync(updateDto);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _doctorService.DeleteAsync(id);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
} 