using AutoMapper;
using Entity.DTOs.DepartmentDtos;
using Entity.Models;

namespace BusinessLogicLayer.Mappings.AutoMapper
{
    public class DepartmentProfile : Profile
    {
        public DepartmentProfile()
        {
            // Source -> Target
            CreateMap<Department, DepartmentDto>();
            CreateMap<DepartmentCreateDto, Department>();
            CreateMap<DepartmentUpdateDto, Department>();
        }
    }
} 