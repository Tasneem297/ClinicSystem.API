using AutoMapper;
using ClinicSystem.Application.DTOs;
using ClinicSystem.Domain.Entities;

namespace ClinicSystem.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Appointment, AppointmentDto>()
            .ForCtorParam("PatientName", opt => opt.MapFrom(src => $"{src.Patient.FirstName} {src.Patient.LastName}"))
            .ForCtorParam("DoctorName", opt => opt.MapFrom(src => $"{src.Doctor.FirstName} {src.Doctor.LastName}"))
            .ForCtorParam("Status", opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<Patient, PatientDto>()
            .ForCtorParam("Gender", opt => opt.MapFrom(src => src.Gender.ToString()));

        CreateMap<Doctor, DoctorDto>();

        CreateMap<TimeSlot, TimeSlotDto>()
            .ForCtorParam("DoctorName", opt => opt.MapFrom(src => $"{src.Doctor.FirstName} {src.Doctor.LastName}"));

        CreateMap<DoctorSchedule, DoctorScheduleDto>()
            .ForCtorParam("DayOfWeek", opt => opt.MapFrom(src => src.DayOfWeek.ToString()));
    }
}
