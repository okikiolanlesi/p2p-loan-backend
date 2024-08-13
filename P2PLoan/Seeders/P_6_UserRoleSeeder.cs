using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using P2PLoan.Data;
using P2PLoan.Interfaces;
using P2PLoan.Models;

public class  P_6_UserRoleSeeder : ISeeder
{
    private readonly IUserRoleRepository userRoleRepository;
    private readonly IUserRepository userRepository;
    private readonly IRoleRepository roleRepository;

    public P_6_UserRoleSeeder(IUserRoleRepository userRoleRepository, IUserRepository userRepository, IRoleRepository roleRepository)
    {
        this.userRoleRepository = userRoleRepository;
        this.userRepository = userRepository;
        this.roleRepository = roleRepository;
    }

    public async Task up()
    {
        var systemUser = await userRepository.GetSystemUser();
       // Check if there are already UserRoles in the database
        var existingUserRoles = await userRoleRepository.GetAll();
        if (existingUserRoles.Any())
        {
            return; // DB has been seeded
        }

        // Define some sample UserRole data
        var userRoles = new List<UserRole>
        {
            new UserRole
            {
                Id = Guid.NewGuid(),
                UserId = systemUser.Id, 
                RoleId = Guid.Parse("role-id-1"), 
                CreatedBy = systemUser,
                ModifiedBy = systemUser
            }
        };

            // Add the UserRole entities to the repository
            userRoleRepository.AddRange(userRoles);

            // Save the changes to the database
            await userRoleRepository.SaveChangesAsync();
    }

    public string Description()
    {
        throw new NotImplementedException();
    }

    public Task down()
    {
        throw new NotImplementedException();
    }

    
}
