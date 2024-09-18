using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using P2PLoan.Models;
using Microsoft.EntityFrameworkCore.Storage;
using P2PLoan.Constants;

namespace P2PLoan.Interfaces;

public interface IModuleRepository
{
    void Add(Module module);
    void AddRange(IEnumerable<Module> modules);
    Task<IEnumerable<Module>> GetAllAsync();
    Task<bool> SaveChangesAsync();

    Task<Module> GetModuleByIdentifierAsync(Modules identifier);
    Task<Module> GetModuleByIdAsync(Guid id);

    Task<IDbContextTransaction> BeginTransactionAsync();
}
