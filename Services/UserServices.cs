using FlightBooking.Models;
using BCrypt.Net;

namespace FlightBooking.Services
{
    public static class UserServices
    {
        static List<User> Users { get; }
        static int nextId = 2;

        static UserServices()
        {
            Users = new List<User>
            {
                new User {
                    Id = 1,
                    Username = "John Doe",
                    Email = "john@flightbooking.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("password"),
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                }
            };
        }

        // Get all users
        public static List<User> GetAll() => Users;

        // Get user by ID
        public static User? Get(int id)
        {
            return Users.FirstOrDefault(u => u.Id == id);
        }

        // Get user by email
        public static User? FindByEmail(string email)
        {
            return Users.FirstOrDefault(u =>
                u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }

        // Register a new user
        public static void Add(User user)
        {
            // Validation - check if user already exists
            if (user == null)
                throw new ArgumentNullException(nameof(user));


            if (string.IsNullOrWhiteSpace(user.Username))
                throw new ArgumentException("Username is required");

            if (string.IsNullOrWhiteSpace(user.Email))
                throw new ArgumentException("Email is required");

            if (string.IsNullOrWhiteSpace(user.Password) || user.Password.Length < 6)
                throw new ArgumentException("Password must be at least 6 characters");

            // Check if email already exists
            if (EmailExists(user.Email))
                throw new InvalidOperationException("Email already registered");

            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            user.Id = nextId++;
            user.CreatedAt = DateTime.Now;
            user.UpdatedAt = DateTime.Now;
            Users.Add(user);
        }

        // Update an existing user
        public static void Update(User user)
        {
            // Check if user exists
            var index = Users.FindIndex(u => u.Id == user.Id);
            if (index == -1)
                throw new KeyNotFoundException($"User with ID {user.Id} not found");

            var existingUser = Users[index];

            // If password is provided, update it
            if (!string.IsNullOrWhiteSpace(user.Password) && user.Password != existingUser.Password)
            {
                if (user.Password.Length < 6)
                    throw new ArgumentException("Password must be at least 6 characters");
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            }

            // If password is not provided, keep the existing password
            else if (string.IsNullOrWhiteSpace(user.Password))
            {
                user.Password = existingUser.Password;
            }

            user.CreatedAt = existingUser.CreatedAt;
            user.UpdatedAt = DateTime.Now;
            Users[index] = user;
        }


        // Delete a user
        public static void Delete(int id)
        {
            var user = Get(id);
            if (user is null)
                throw new KeyNotFoundException($"User with ID {id} not found");
            Users.Remove(user);
        }

        // Authenticate a user  
        public static User? Authenticate(string email, string password)
        {
            // If email or password is null or empty, return null
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return null;

            // Find user by email
            var user = Users.FirstOrDefault(u =>
                u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

            if (user == null)
                return null;

            bool isValid = BCrypt.Net.BCrypt.Verify(password, user.Password);
            return isValid ? user : null;
        }


        // Change password
        public static bool ChangePassword(int userId, string currentPassword, string newPassword)
        {
            var user = Get(userId);
            if (user == null)
                return false;

            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
                throw new ArgumentException("New password must be at least 6 characters");

            if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.Password))
                return false;

            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.UpdatedAt = DateTime.Now;
            return true;
        }

        // Email exists
        public static bool EmailExists(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return Users.Any(u =>
                u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }

        // Get safe user
        public static object? GetSafeUser(int id)
        {
            var user = Get(id);
            if (user == null)
                return null;

            return new
            {
                user.Id,
                user.Username,
                user.Email,
                user.CreatedAt,
                user.UpdatedAt
            };
        }

        // Get all safe users
        public static List<object> GetAllSafe()
        {
            return Users.Select(u => new
            {
                u.Id,
                u.Username,
                u.Email,
                u.CreatedAt,
                u.UpdatedAt
            }).Cast<object>().ToList();
        }
    }
}