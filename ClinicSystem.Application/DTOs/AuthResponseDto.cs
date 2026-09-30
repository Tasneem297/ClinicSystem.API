namespace ClinicSystem.Application.DTOs;

public record AuthResponseDto(
    string Token,
    DateTime ExpiresAt,
    string UserId,
    string Email,
    string Role,
    Guid? ProfileId = null);
