using SisandAirlines.Application.DTOs;
using SisandAirlines.Domain.Interfaces;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using SisandAirlines.Application.Utils;


namespace SisandAirlines.Application.Services
{
    public class AuthService
    {
        private readonly IUnitOfWork _uow;
        private readonly IConfiguration _config;

        public AuthService(IUnitOfWork uow, IConfiguration config)
        {
            _uow = uow;
            _config = config;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var customer = await _uow.Customers.GetByEmailAsync(request.Email);
            if (customer == null)
                throw new UnauthorizedAccessException("E-mail não encontrado.");

            if (!PasswordHasher.Verify(request.Password, customer.PasswordHash))
                throw new UnauthorizedAccessException("Senha incorreta.");

            var token = GenerateJwtToken(customer);

            return new AuthResponse
            {
                Token = token,
                Email = customer.Email,
                FullName = customer.FullName,
                Expiration = DateTime.UtcNow.AddMinutes(120)
            };
        }
        
        private string GenerateJwtToken(Domain.Entities.Customer customer)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT_SECRET"] ?? "default_secret_key"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, customer.Email),
                new Claim("customerId", customer.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["JWT_ISSUER"],
                audience: _config["JWT_AUDIENCE"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(120),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
