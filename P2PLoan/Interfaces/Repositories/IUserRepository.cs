using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using P2PLoan.Models;

namespace P2PLoan.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid userId);
    void MarkAsModified(User user);
    void Remove(User user);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetSystemUser();
    void Add(User user);
    Task<bool> SaveChangesAsync();
    Task<IDbContextTransaction> BeginTransactionAsync();

}
