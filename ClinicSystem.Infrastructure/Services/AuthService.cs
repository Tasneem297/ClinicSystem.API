using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ClinicSystem.Application.Common.Exceptions;
using ClinicSystem.Application.DTOs;
using ClinicSystem.Application.Interfaces;
using ClinicSystem.Domain.Entities;
using ClinicSystem.Domain.Enums;
using ClinicSystem.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ClinicSystem.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly IUnitOfWork _unitOfWork;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration,
        IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthResponseDto> RegisterPatientAsync(string email, string password,
        string firstName, string lastName, string phone, DateTime dateOfBirth,
        Gender gender, CancellationToken cancellationToken = default)
    {
        var existingUser = await _userManager.FindByEmailAsync(email);
        if (existingUser is not null)
            throw new BadRequestException("A user with this email already exists.");

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
            throw new BadRequestException(string.Join("; ", result.Errors.Select(e => e.Description)));

        await EnsureRoleExistsAsync("Patient");
        await _userManager.AddToRoleAsync(user, "Patient");

        var patient = new Patient
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Phone = phone,
            DateOfBirth = dateOfBirth,
            Gender = gender,
            UserId = user.Id
        };

        await _unitOfWork.Patients.AddAsync(patient, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var token = await GenerateJwtTokenAsync(user);
        var expiration = DateTime.UtcNow.AddMinutes(
            double.Parse(_configuration["Jwt:ExpirationInMinutes"] ?? "60"));

        return new AuthResponseDto(token, expiration, user.Id, email, "Patient", patient.Id);
    }

    public async Task<AuthResponseDto> RegisterDoctorAsync(string email, string password,
        string firstName, string lastName, string phone, string specialization,
        CancellationToken cancellationToken = default)
    {
        var existingUser = await _userManager.FindByEmailAsync(email);
        if (existingUser is not null)
            throw new BadRequestException("A user with this email already exists.");

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
            throw new BadRequestException(string.Join("; ", result.Errors.Select(e => e.Description)));

        await EnsureRoleExistsAsync("Doctor");
        await _userManager.AddToRoleAsync(user, "Doctor");

        var doctor = new Doctor
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Phone = phone,
            Specialization = specialization,
            UserId = user.Id
        };

        await _unitOfWork.Doctors.AddAsync(doctor, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var token = await GenerateJwtTokenAsync(user);
        var expiration = DateTime.UtcNow.AddMinutes(
            double.Parse(_configuration["Jwt:ExpirationInMinutes"] ?? "60"));

        return new AuthResponseDto(token, expiration, user.Id, email, "Doctor", doctor.Id);
    }

    public async Task<AuthResponseDto> LoginAsync(string email, string password,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email)
            ?? throw new BadRequestException("Invalid email or password.");

        var isValidPassword = await _userManager.CheckPasswordAsync(user, password);
        if (!isValidPassword)
            throw new BadRequestException("Invalid email or password.");

        var roles = await _userManager.GetRolesAsync(user);
        var token = await GenerateJwtTokenAsync(user);
        var expiration = DateTime.UtcNow.AddMinutes(
            double.Parse(_configuration["Jwt:ExpirationInMinutes"] ?? "60"));

        Guid? profileId = null;
        var role = roles.FirstOrDefault() ?? "User";
        if (role.Equals("Doctor", StringComparison.OrdinalIgnoreCase))
        {
            var doctor = await _unitOfWork.Doctors.GetByUserIdAsync(user.Id, cancellationToken);
            profileId = doctor?.Id;
        }
        else if (role.Equals("Patient", StringComparison.OrdinalIgnoreCase))
        {
            var patient = await _unitOfWork.Patients.GetByUserIdAsync(user.Id, cancellationToken);
            profileId = patient?.Id;
        }

        return new AuthResponseDto(token, expiration, user.Id, email, role, profileId);
    }

    private async Task<string> GenerateJwtTokenAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("JWT Key is not configured.")));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                double.Parse(_configuration["Jwt:ExpirationInMinutes"] ?? "60")),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task EnsureRoleExistsAsync(string roleName)
    {
        if (!await _roleManager.RoleExistsAsync(roleName))
            await _roleManager.CreateAsync(new IdentityRole(roleName));
    }
}
