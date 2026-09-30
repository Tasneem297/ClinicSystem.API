namespace ClinicSystem.Application.DTOs;

public record PatientDto(
    Guid Id,
    string FirstName,
    string LastName,
    DateTime DateOfBirth,
    string Gender,
    string Phone,
    string Email);
