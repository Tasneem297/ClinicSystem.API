namespace ClinicSystem.Application.DTOs;

public record DoctorDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Specialization,
    string Phone,
    string Email);
