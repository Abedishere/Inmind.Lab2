using System;
using System.Collections.Generic;
using System.Globalization;
using myFirstProject.Models;

namespace myFirstProject.Services
{
    public class UserService : IUserService
    {
        
        private readonly List<User> _users;

        public UserService()
        {
            _users = new List<User>
            {
                new User { Id = 1, Name = "Alice", Email = "alice@example.com" },
                new User { Id = 2, Name = "Bob", Email = "bob@example.com" },
                new User { Id = 3, Name = "Charlie", Email = "charlie@example.com" }
            };
        }

        
        public List<User> GetAllUsers()
        {
            return _users;
        }

        
        public User GetUserById(long id)
        {
            for (int i = 0; i < _users.Count; i++)
            {
                if (_users[i].Id == id)
                {
                    return _users[i];
                }
            }
            throw new ArgumentException($"User with id {id} not found.");
        }

        
        public List<User> GetUsersByName(string name) 
        {
            var filtered = new List<User>();
            foreach (var user in _users)
            {
                if (user.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
                {
                    filtered.Add(user);
                }
            }
            return filtered;
        }

        
        public User UpdateUser(UpdateUserRequest request)
        {
            if (request.Id <= 0)
                throw new ArgumentException("Invalid user Id.");

            if (string.IsNullOrEmpty(request.NewName))
                throw new ArgumentException("New name cannot be empty.");

            if (string.IsNullOrEmpty(request.Email))
                throw new ArgumentException("Email cannot be empty.");

            for (int i = 0; i < _users.Count; i++)
            {
                if (_users[i].Id == request.Id)
                {
                    _users[i].Name = request.NewName;
                    _users[i].Email = request.Email;
                    return _users[i];
                }
            }
            throw new ArgumentException($"User with id {request.Id} not found.");
        }

       
        public void DeleteUser(long id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid user Id.");

            bool removed = false;
            for (int i = 0; i < _users.Count; i++)
            {
                if (_users[i].Id == id)
                {
                    _users.RemoveAt(i);
                    removed = true;
                    break;
                }
            }
            if (!removed)
            {
                throw new ArgumentException($"User with id {id} not found.");
            }
        }

        
        public string GetFormattedDate(string acceptedLanguage)
        {
            CultureInfo culture = null;

            try
            {
                culture = new CultureInfo(acceptedLanguage);
            }
            catch (CultureNotFoundException)
            {
                throw new ArgumentException($"The culture {acceptedLanguage} is not valid.");
            }

            
            var supportedCultures = new List<string> { "en-US", "es-ES", "fr-FR" };
            bool isSupported = false;
            for (int i = 0; i < supportedCultures.Count; i++)
            {
                if (string.Equals(supportedCultures[i], acceptedLanguage, StringComparison.OrdinalIgnoreCase))
                {
                    isSupported = true;
                    break;
                }
            }
            if (!isSupported)
                throw new ArgumentException($"The culture {acceptedLanguage} is not supported.");

            
            return DateTime.Now.ToString("D", culture);
        }
    }
}
