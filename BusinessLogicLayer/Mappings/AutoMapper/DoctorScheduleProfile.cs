using AutoMapper;
using Entity.DTOs.DoctorScheduleDtos;
using Entity.Models;
using System;

namespace BusinessLogicLayer.Mappings.AutoMapper
{
    public class DoctorScheduleProfile : Profile
    {
        public DoctorScheduleProfile()
        {
            // Source -> Target
            CreateMap<DoctorSchedule, DoctorScheduleDto>()
                .ForMember(dest => dest.DoctorFullName, opt => opt.MapFrom(src => src.Doctor != null ? src.Doctor.FullName : "N/A"))
                .ForMember(dest => dest.DayOfWeek, opt => opt.MapFrom(src => (DayOfWeek)src.DayOfWeek));

            CreateMap<DoctorScheduleCreateDto, DoctorSchedule>();
            CreateMap<DoctorScheduleUpdateDto, DoctorSchedule>();
        }
    }
} 