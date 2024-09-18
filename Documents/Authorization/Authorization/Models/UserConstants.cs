using System.Data;

namespace Authorization.Models
{
    public class UserConstants
    {
        public static List<UserModel> Users = new List<UserModel>()
        {
                new UserModel(){Email = "john.doe@example.com", Password = "password", FirstName = "John", LastName = "Doe", Role = "Supervisor"},
                new UserModel(){Email = "Akan.Victor@example.com", Password = "password", FirstName = "Akan", LastName = "Victor", Role = "Designer"},
                new UserModel(){Email = "Oyedepo.Bukola@example.com", Password = "password", FirstName = "Bukola", LastName = "Oyedepo", Role = "Developer"},
        };
    }
}
