using SisandAirlines.Application.DTOs;
using SisandAirlines.Domain.Entities;
using SisandAirlines.Domain.Interfaces;
using System.Security.Cryptography;
using SisandAirlines.Application.Utils;
using System.Text;

namespace SisandAirlines.Application.Services
{
    public class CustomerService
    {
        private readonly IUnitOfWork _uow;

        public CustomerService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CustomerDto> CreateCustomerAsync(CreateCustomerRequest request)
        {
            // Verifica duplicidade de e-mail
            var existing = await _uow.Customers.GetByEmailAsync(request.Email);
            if (existing != null)
                throw new InvalidOperationException("E-mail já cadastrado.");

            var entity = new Customer
            {
                FullName = request.FullName,
                Email = request.Email,
                Cpf = request.Cpf,
                PasswordHash = PasswordHasher.Hash(request.Password),
                BirthDate = request.BirthDate,
                CreatedAt = DateTime.UtcNow
            };

            await _uow.Customers.AddAsync(entity);
            await _uow.CommitAsync();

            return new CustomerDto
            {
                FullName = entity.FullName,
                Email = entity.Email,
                Cpf = request.Cpf,
                BirthDate = request.BirthDate,
                CreatedAt = entity.CreatedAt
            };
        }

        public async Task<IEnumerable<CustomerDto>> GetAllAsync()
        {
            var customers = await _uow.Customers.GetAllAsync();
            return customers.Select(c => new CustomerDto
            {
                Id = c.Id,
                FullName = c.FullName,
                Email = c.Email,
                CreatedAt = c.CreatedAt
            });
        }

    }
}
