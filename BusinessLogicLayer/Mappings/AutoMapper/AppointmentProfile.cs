using AutoMapper;
using Entity.DTOs.AppointmentDtos;
using Entity.Models;

namespace BusinessLogicLayer.Mappings.AutoMapper
{
    public class AppointmentProfile : Profile
    {
        public AppointmentProfile()
        {
            CreateMap<Appointment, AppointmentDto>()
                .ForMember(dest => dest.PatientFullName, opt => opt.MapFrom(src => src.Patient != null ? src.Patient.FullName : "N/A"))
                .ForMember(dest => dest.DoctorFullName, opt => opt.MapFrom(src => src.Doctor != null ? src.Doctor.FullName : "N/A"))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<AppointmentCreateDto, Appointment>();
        }
    }
} 