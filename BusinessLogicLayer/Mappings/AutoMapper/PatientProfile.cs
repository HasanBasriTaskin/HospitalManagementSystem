using AutoMapper;
using Entity.DTOs.PatientDtos;
using Entity.Models;
using System;

namespace BusinessLogicLayer.Mappings.AutoMapper
{
    public class PatientProfile : Profile
    {
        public PatientProfile()
        {
            // Source -> Target
            CreateMap<Patient, PatientDto>()
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender.ToString()))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName)) // Entity has this computed property
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.Age));       // Entity has this computed property

            CreateMap<PatientCreateDto, Patient>();
            CreateMap<PatientUpdateDto, Patient>();
        }
    }
} 