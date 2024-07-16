﻿using System;
using System.Threading.Tasks;
using P2PLoan.Models;

namespace P2PLoan.Interfaces;

public interface IUserRepository
{
    Task<User?> FindByEmail(string email);
    Task<User?> GetSystemUser();
    void Add(User user);
    Task<bool> SaveChangesAsync();

}
