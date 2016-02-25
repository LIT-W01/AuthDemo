using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AuthDemo.Data
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string PasswordHash { get; set; }
        public string Salt { get; set; }
    }

    public class UserManager
    {
        private string _connectionString;

        public UserManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public User AddUser(string userName, string password, string name)
        {
            //we should really check if username exists.....
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                User user = new User();
                user.Username = userName;
                user.Name = name;
                string salt = PasswordHelper.GenerateSalt();
                string passwordHash = PasswordHelper.HashPassword(password, salt);
                user.Salt = salt;
                user.PasswordHash = passwordHash;

                cmd.CommandText = "INSERT INTO Users (Name, Username, PasswordHash, Salt) VALUES " +
                                  "(@name, @username, @hash, @salt); SELECT @@Identity";
                cmd.Parameters.AddWithValue("@name", user.Name);
                cmd.Parameters.AddWithValue("@username", user.Username);
                cmd.Parameters.AddWithValue("@hash", user.PasswordHash);
                cmd.Parameters.AddWithValue("@salt", user.Salt);

                connection.Open();
                user.Id = (int)(decimal)cmd.ExecuteScalar();

                return user;
            }
        }

        public User GetUser(string username, string password)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Users WHERE Username = @username";
                cmd.Parameters.AddWithValue("@username", username);
                connection.Open();
                var reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    return null; //username not found....
                }

                User user = new User();
                user.Id = (int)reader["Id"];
                user.Name = (string)reader["Name"];
                user.PasswordHash = (string)reader["PasswordHash"];
                user.Salt = (string)reader["Salt"];
                user.Username = (string)reader["Username"];

                bool correctPassword = PasswordHelper.PasswordMatch(password, user.PasswordHash, user.Salt);
                return correctPassword ? user : null; //ternary operator
            }
        }

        public User GetUserByUsername(string username)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Users WHERE Username = @username";
                cmd.Parameters.AddWithValue("@username", username);
                connection.Open();
                var reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    return null; //username not found....
                }

                User user = new User();
                user.Id = (int)reader["Id"];
                user.Name = (string)reader["Name"];
                user.PasswordHash = (string)reader["PasswordHash"];
                user.Salt = (string)reader["Salt"];
                user.Username = (string)reader["Username"];
                return user;
            }
        }

        public static class PasswordHelper
        {
            public static string GenerateSalt()
            {
                RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
                byte[] bytes = new byte[15];
                provider.GetBytes(bytes);
                return Convert.ToBase64String(bytes);
            }

            public static string HashPassword(string password, string salt)
            {
                SHA256Managed managed = new SHA256Managed();
                byte[] passwordBytes = Encoding.ASCII.GetBytes(password);
                byte[] saltBytes = Encoding.ASCII.GetBytes(salt);
                byte[] combined = passwordBytes.Concat(saltBytes).ToArray();
                return Convert.ToBase64String(managed.ComputeHash(combined));
            }

            public static bool PasswordMatch(string input, string passwordHash, string salt)
            {
                string inputHash = HashPassword(input, salt);
                return inputHash == passwordHash;
            }
        }
    }
}
