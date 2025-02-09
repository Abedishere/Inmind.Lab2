using System.Collections.Generic;
using myFirstProject.Models;

namespace myFirstProject.Services
{
    public interface IUserService
    {
        List<User> GetAllUsers();
        User GetUserById(long id);
        List<User> GetUsersByName(string name); 
        string GetFormattedDate(string language);
        User UpdateUser(UpdateUserRequest request); 
        void DeleteUser(long id); 
    }
}