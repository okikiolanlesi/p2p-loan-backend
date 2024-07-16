using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using P2PLoan.Models;

namespace P2PLoan.Interfaces;

public interface IModuleRepository
{
    void Add(Module module);

    void AddRange(IEnumerable<Module> modules);
    Task<bool> SaveChangesAsync();
}
