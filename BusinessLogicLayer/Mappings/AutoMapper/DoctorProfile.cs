using AutoMapper;
using Entity.DTOs.DoctorDtos;
using Entity.Models;

namespace BusinessLogicLayer.Mappings.AutoMapper
{
    public class DoctorProfile : Profile
    {
        public DoctorProfile()
        {
            // Source -> Target
            CreateMap<Doctor, DoctorDto>()
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department != null ? src.Department.Name : "N/A"));

            CreateMap<DoctorCreateDto, Doctor>();
            CreateMap<DoctorUpdateDto, Doctor>();
        }
    }
} 