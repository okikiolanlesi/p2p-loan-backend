using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using P2PLoan.Data;
using P2PLoan.Interfaces;
using P2PLoan.Models;

namespace P2PLoan.Repositories;

public class UserRepository : IUserRepository
{
    private readonly P2PLoanDbContext dbContext;
    private readonly IConstants constants;

    public UserRepository(P2PLoanDbContext dbContext, IConstants constants)
    {
        this.dbContext = dbContext;
        this.constants = constants;
    }
    public async Task<User?> FindByEmail(string email)
    {
        return await dbContext.Users.FirstOrDefaultAsync(x => x.Email == email);
    }
    public async Task<User?> GetSystemUser()
    {
        return await dbContext.Users.FirstOrDefaultAsync(x => x.Email == constants.SYSTEM_USER_EMAIL);
    }

    public void Add(User user)
    {
        dbContext.Users.Add(user);
    }


    public async Task<bool> SaveChangesAsync()
    {
        return await dbContext.SaveChangesAsync() > 0;
    }
}
