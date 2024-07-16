using System;
using System.Threading.Tasks;
using P2PLoan.Interfaces;
using P2PLoan.Models;

namespace P2PLoan.Seeders;

public class P_1_UserSeeder : ISeeder
{
    private readonly IUserRepository userRepository;
    private readonly IConstants constants;

    public P_1_UserSeeder(IUserRepository userRepository, IConstants constants)
    {
        this.userRepository = userRepository;
        this.constants = constants;
    }
    public async Task up()
    {
        var systemUser = new User
        {
            FirstName = constants.SYSTEM_USER_FIRST_NAME,
            LastName = constants.SYSTEM_USER_LAST_NAME,
            Email = constants.SYSTEM_USER_EMAIL
        };

        userRepository.Add(systemUser);

        await userRepository.SaveChangesAsync();

    }
    public async Task down()
    {
        throw new NotImplementedException();
    }

    public string Description()
    {
        return "";
    }
}
